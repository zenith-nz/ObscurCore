﻿#region License

//  	Copyright 2013-2014 Matthew Ducker
//  	
//  	Licensed under the Apache License, Version 2.0 (the "License");
//  	you may not use this file except in compliance with the License.
//  	
//  	You may obtain a copy of the License at
//  		
//  		http://www.apache.org/licenses/LICENSE-2.0
//  	
//  	Unless required by applicable law or agreed to in writing, software
//  	distributed under the License is distributed on an "AS IS" BASIS,
//  	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  	See the License for the specific language governing permissions and 
//  	limitations under the License.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Obscur.Core;
using Obscur.Core.Cryptography.Ciphers;
using Obscur.Core.DTO;

namespace ObscurCore.Tests.Cryptography.Ciphers
{
    internal abstract class CipherTestBase : IOTestBase
    {
        private static Random Rng = new Random();

        public CipherTestBase()
        {
            DiscreteVectorTests = new List<DiscreteVectorTestCase>();
            SegmentedVectorTests = new List<SegmentedVectorTestCase>();
        }

        // Vector testing resources

        public List<DiscreteVectorTestCase> DiscreteVectorTests { get; private set; }

        public List<SegmentedVectorTestCase> SegmentedVectorTests { get; private set; }

        protected static byte[] CreateRandomByteArray(int lengthBits)
        {
            var key = new byte[lengthBits / 8];
            Rng.NextBytes(key);
            return key;
        }

        [Test]
        public void Correctness()
        {
            Assume.That(DiscreteVectorTests != null && SegmentedVectorTests != null && DiscreteVectorTests.Count + SegmentedVectorTests.Count > 0, "No tests to run.");

            var sb = new StringBuilder(DiscreteVectorTests.Count + " discrete vector tests ran successfully:\n\n");
            for (int i = 0; i < DiscreteVectorTests.Count; i++) {
                RunDiscreteVectorTest(i, DiscreteVectorTests[i]);
                sb.AppendLine("> " + DiscreteVectorTests[i].Name);
            }
            sb.AppendLine();
            sb.AppendLine(SegmentedVectorTests.Count + " segmented vectors:\n");
            for (int i = 0; i < SegmentedVectorTests.Count; i++) {
                RunSegmentedVectorTest(i, SegmentedVectorTests[i]);
                sb.AppendLine("> " + SegmentedVectorTests[i].Name);
            }
            Assert.Pass(sb.ToString());
        }

        protected abstract CipherConfiguration GetCipherConfiguration(CipherTestCase testCase);

        protected void RunDiscreteVectorTest(int number, DiscreteVectorTestCase testCase)
        {
            CipherConfiguration config = GetCipherConfiguration(testCase);
            byte[] plaintext = testCase.Plaintext;

            byte[] ciphertext;
            using (var msCiphertext = new MemoryStream()) {
                using (var cs = new CipherStream(msCiphertext, true, config, testCase.Key, false)) {
                    cs.Write(plaintext, 0, plaintext.Length);
                }
                ciphertext = msCiphertext.ToArray();
            }

            Assert.IsTrue(testCase.Ciphertext.SequenceEqualShortCircuiting(ciphertext),
                "Test #{0} (\"{1}\") failed!", number, testCase.Name);
        }

        protected void RunSegmentedVectorTest(int number, SegmentedVectorTestCase testCase)
        {
            CipherConfiguration config = GetCipherConfiguration(testCase);
            var plaintext = new byte[testCase.IV.Length];
            TestVectorSegment lastSegment = testCase.Segments.Last();
            int requiredCiphertextLength = lastSegment.Offset + lastSegment.Length;
            var msCiphertext = new MemoryStream();

            using (var cs = new CipherStream(msCiphertext, true, config, testCase.Key, false)) {
                while (cs.BytesOut < requiredCiphertextLength) {
                    cs.Write(plaintext, 0, plaintext.Length);
                }
            }

            // Now we analyse the ciphertext segment-wise
            foreach (var segment in testCase.Segments) {
                msCiphertext.Seek(segment.Offset, SeekOrigin.Begin);
                var segmentCiphertext = new byte[segment.Length];
                msCiphertext.Read(segmentCiphertext, 0, segment.Length);
                byte[] referenceCiphertext = segment.Ciphertext;
                // Validate the segment
                Assert.IsTrue(referenceCiphertext.SequenceEqualShortCircuiting(segmentCiphertext),
                    "Segmented vector test #{0} (\"{1}\") failed at segment {2}!",
                    number, testCase.Name, segment.Name);
            }
        }

        // Performance testing resources (not called in this base class, but called from derived classes)

        protected void RunPerformanceTest(CipherConfiguration config, byte[] overrideKey = null)
        {
            MemoryStream msInputPlaintext = LargeBinaryFile;
            byte[] key = overrideKey ?? CreateRandomByteArray(config.KeySizeBits);

            var msCiphertext = new MemoryStream((int) (msInputPlaintext.Length * 1.1));
            var sw = new Stopwatch();

            // TEST STARTS HERE

            using (var cs = new CipherStream(msCiphertext, true, config, key, false)) {
                sw.Start();
                msInputPlaintext.CopyTo(cs, GetBufferSize());
            }
            sw.Stop();
            TimeSpan encryptionElapsed = sw.Elapsed;

            var msOutputPlaintext = new MemoryStream((int) msInputPlaintext.Length);
            msCiphertext.Seek(0, SeekOrigin.Begin);

            sw.Reset();
            using (var cs = new CipherStream(msCiphertext, false, config, key, false)) {
                sw.Start();
                cs.CopyTo(msOutputPlaintext, GetBufferSize());
            }
            sw.Stop();
            TimeSpan decryptionElapsed = sw.Elapsed;

            // TEST ENDS HERE

            // TEST OUTPUT PLAINTEXT VALIDITY

            msInputPlaintext.Seek(0, SeekOrigin.Begin);
            msOutputPlaintext.Seek(0, SeekOrigin.Begin);
            int failurePosition;
            Assert.IsTrue(StreamsContentMatches(msInputPlaintext, msOutputPlaintext, (int) msInputPlaintext.Length, out failurePosition),
                "Input and output plaintext does not match. First failure observed at position # " + failurePosition);

            // OUTPUT SUCCESS STATISTICS

            double encSpeed = ((double) msInputPlaintext.Length / 1048576) / encryptionElapsed.TotalSeconds,
                   decSpeed =
                       ((double) msInputPlaintext.Length / 1048576) / decryptionElapsed.TotalSeconds;
            Assert.Pass("{0:N0} ms ({1:N2} MB/s) : {2:N0} ms ({3:N2} MB/s)",
                encryptionElapsed.TotalMilliseconds, encSpeed, decryptionElapsed.TotalMilliseconds, decSpeed);
        }

        protected static bool StreamsContentMatches(System.IO.Stream a, System.IO.Stream b, int length, out int failurePosition)
        {
            for (int i = 0; i < length; i++) {
                if (a.ReadByte() == b.ReadByte()) {
                    continue;
                }
                failurePosition = i;
                return false;
            }
            failurePosition = -1;
            return true;
        }

        #region Nested type: CipherTestCase

        public abstract class CipherTestCase
        {
            public CipherTestCase(string name, string key, string iv, string extra = null)
            {
                this.Name = name;
                this.Key = key.HexToBinary();
                this.IV = iv.HexToBinary();
                this.Extra = extra;
            }

            public string Name { get; private set; }
            public byte[] Key { get; private set; }
            public byte[] IV { get; private set; }

            /// <summary>
            ///     Extra configuration for test case where required.
            /// </summary>
            public string Extra { get; private set; }
        }

        #endregion

        #region Nested type: DiscreteVectorTestCase

        public class DiscreteVectorTestCase : CipherTestCase
        {
            public DiscreteVectorTestCase(string name, string key, string iv, string plaintext,
                                          string ciphertext, string extra = null) : base(name, key, iv, extra)
            {
                this.Plaintext = plaintext.HexToBinary();
                this.Ciphertext = ciphertext.HexToBinary();
            }

            public byte[] Plaintext { get; private set; }
            public byte[] Ciphertext { get; private set; }
        }

        #endregion

        #region Nested type: SegmentedVectorTestCase

        public class SegmentedVectorTestCase : CipherTestCase
        {
            public SegmentedVectorTestCase(string name, string key, string iv,
                                           ICollection<TestVectorSegment> segments, string extra = null) : base(name, key, iv, extra)
            {
                Segments = new List<TestVectorSegment>(segments);
            }

            public List<TestVectorSegment> Segments { get; private set; }
        }

        #endregion

        #region Nested type: TestVectorSegment

        public class TestVectorSegment
        {
            public TestVectorSegment(string name, int offset, string ciphertext, string extra = null)
            {
                this.Name = name;
                this.Offset = offset;
                this.Ciphertext = ciphertext.HexToBinary();
                this.Extra = extra;
            }

            public string Name { get; private set; }
            public int Offset { get; private set; }
            public byte[] Ciphertext { get; private set; }

            public int Length
            {
                get { return Ciphertext.Length; }
            }

            /// <summary>
            ///     Extra configuration data where required.
            /// </summary>
            public string Extra { get; private set; }
        }

        #endregion
    }
}

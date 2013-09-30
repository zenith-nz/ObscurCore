﻿//
//  Copyright 2013  Matthew Ducker
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using ObscurCore.Cryptography;
using ObscurCore.DTO;

namespace ObscurCore.Tests.Cryptography
{
    public abstract class CryptoTestBase : IOTestBase
    {
        protected byte[] Key { get; private set; }

        protected static byte[] CreateRandomKey (int lengthBits) {
            var key = new byte[lengthBits / 8];
            var rng = new Random();
            rng.NextBytes(key);
            return key;
        }

        protected void SetRandomFixtureKey(int lengthBits) { Key = CreateRandomKey(lengthBits); }

        protected void RunEqualityTest (SymmetricCipherConfiguration config, byte[] overrideKey = null) {
            TimeSpan enc, dec;

            config.Key = Key;

            byte[] keyBackup = null;
            var backupRequired = false;
            try {
                if (overrideKey != null) {
                    keyBackup = new byte[config.Key.Length];
                    Array.Copy(config.Key, keyBackup, config.Key.Length);
                    config.Key = overrideKey;
                    config.KeySize = overrideKey.Length * 8;
                    backupRequired = true;
                }
                Assert.IsTrue(OutputNonMalformed(LargeBinaryFile, config, out enc, out dec));
            }
            finally {
                if(backupRequired) config.Key = keyBackup;
            }
            double encSpeed = ((double) LargeBinaryFile.Length / 1048576) / enc.TotalSeconds, decSpeed = ((double) LargeBinaryFile.Length / 1048576) / dec.TotalSeconds;
            Assert.Pass("{0:N0} ms ({1:N2} MB/s) : {2:N0} ms ({3:N2} MB/s)", enc.TotalMilliseconds, encSpeed, dec.TotalMilliseconds, decSpeed);
        }

        protected bool OutputNonMalformed (MemoryStream input, ISymmetricCipherConfiguration config, out TimeSpan encryptTime, out TimeSpan decryptTime) {
            var crypted = new MemoryStream();

            var sw = new Stopwatch();
            sw.Start();
            using (var cs = new SymmetricCryptoStream(crypted, true, config, null, true)) {
                input.CopyTo(cs, GetBufferSize());
            }
            sw.Stop();
            encryptTime = sw.Elapsed;

            var decrypted = new MemoryStream();
            crypted.Seek(0, SeekOrigin.Begin);

            sw.Reset();
            sw.Start();
            using (var cs = new SymmetricCryptoStream(crypted, false, config, null, true)) {
                cs.CopyTo(decrypted, GetBufferSize());
            }
            sw.Stop();
            decryptTime = sw.Elapsed;

            return decrypted.ToArray().SequenceEqual(input.ToArray());
        }
    }

    public abstract class BlockCipherTestBase : CryptoTestBase
    {
        protected SymmetricBlockCiphers _blockCipher;
        protected int _defaultBlockSize, _defaultKeySize;

        protected BlockCipherTestBase(SymmetricBlockCiphers cipher, int blockSize, int keySize) 
        { 
            _blockCipher = cipher;
            _defaultBlockSize = blockSize;
            _defaultKeySize = keySize;
			SetRandomFixtureKey(_defaultKeySize);
        }

        #region Paddingless modes of operation
        [Test]
        public virtual void CTR () {
            // Using default block & key size
            var config = new BlockCipherConfiguration(_blockCipher, BlockCipherModes.CTR,
                                                      BlockCipherPaddings.None, _defaultBlockSize, _defaultKeySize);
            RunEqualityTest(config);
        }

        [Test]
        public virtual void CFB () {
            // Using default block & key size
            var config = new BlockCipherConfiguration(_blockCipher, BlockCipherModes.CFB,
                                                      BlockCipherPaddings.None, _defaultBlockSize, _defaultKeySize);
            RunEqualityTest(config);
        }

        [Test]
        public virtual void OFB () {
            // Using default block & key size
            var config = new BlockCipherConfiguration(_blockCipher, BlockCipherModes.OFB,
                                                      BlockCipherPaddings.None, _defaultBlockSize, _defaultKeySize);
            RunEqualityTest(config);
        }
        #endregion

        [Test]
        public virtual void CTS () {
            // Using default block & key size
            var config = new BlockCipherConfiguration(_blockCipher, BlockCipherModes.CTS_CBC,
                                                      BlockCipherPaddings.None, _defaultBlockSize, _defaultKeySize);
            RunEqualityTest(config);
        }

        #region CBC with padding modes
        [Test]
        public virtual void CBC_ISO10126D2 () {
            // Using default block & key size
            var config = new BlockCipherConfiguration(_blockCipher, BlockCipherModes.CBC,
                                                      BlockCipherPaddings.ISO10126D2, _defaultBlockSize, _defaultKeySize);
            RunEqualityTest(config);
        }

        [Test]
        public virtual void CBC_ISO7816D4 () {
            // Using default block & key size
            var config = new BlockCipherConfiguration(_blockCipher, BlockCipherModes.CBC,
                                                      BlockCipherPaddings.ISO7816D4, _defaultBlockSize, _defaultKeySize);
            RunEqualityTest(config);
        }

        [Test]
        public virtual void CBC_PKCS7 () {
            // Using default block & key size
            var config = new BlockCipherConfiguration(_blockCipher, BlockCipherModes.CBC,
                                                      BlockCipherPaddings.PKCS7, _defaultBlockSize, _defaultKeySize);
            RunEqualityTest(config);
        }

        [Test]
        public virtual void CBC_TBC () {
            // Using default block & key size
            var config = new BlockCipherConfiguration(_blockCipher, BlockCipherModes.CBC,
                                                      BlockCipherPaddings.TBC, _defaultBlockSize, _defaultKeySize);
            RunEqualityTest(config);
        }

        [Test]
        public virtual void CBC_X923 () {
            // Using default block & key size
            var config = new BlockCipherConfiguration(_blockCipher, BlockCipherModes.CBC,
                                                      BlockCipherPaddings.X923, _defaultBlockSize, _defaultKeySize);
            RunEqualityTest(config);
        }
        #endregion

        #region Authenticated (AEAD) modes
        [Test]
        public virtual void GCM () {
            // Using default block & key size
            var config = new AEADCipherConfiguration(_blockCipher, AEADBlockCipherModes.GCM, BlockCipherPaddings.None, 
                _defaultKeySize, _defaultBlockSize);
            RunEqualityTest(config);
        }

        [Test]
        public virtual void EAX () {
            // Using default block & key size
            var config = new AEADCipherConfiguration(_blockCipher, AEADBlockCipherModes.EAX, BlockCipherPaddings.None, 
                _defaultKeySize, _defaultBlockSize);
            RunEqualityTest(config);
        }
        #endregion
    }
}

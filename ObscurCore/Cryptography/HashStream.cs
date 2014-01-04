//
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
using System.IO;
using ObscurCore.Cryptography.Authentication;
using ObscurCore.DTO;

namespace ObscurCore.Cryptography
{
	public sealed class HashStream : DecoratingStream
	{
		private IDigest _digest;
		private readonly byte[] _output;
		private byte[] _buffer;
		private const int BufferSize = 4096;

		/// <summary>
		/// The output/digest of the internal hash function. Zeroed if function is not finished.
		/// </summary>
		public byte[] Hash { get { return _output; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="ObscurCore.Cryptography.HashStream"/> class.
		/// </summary>
		/// <param name="binding">Stream to write to or read from.</param>
		/// <param name="writing">If set to <c>true</c>, writing; otherwise, reading.</param>
		/// <param name="function">Hash/digest function to instantiate.</param>
		/// <param name="output">Byte array where the finished hash will be output to.</param>
		/// <param name="closeOnDispose">If set to <c>true</c>, bound stream will be closed on dispose/close.</param>
		public HashStream (Stream binding, bool writing, HashFunction function, out byte[] output, bool closeOnDispose = true) 
			: base(binding, writing, closeOnDispose)
		{
			_digest = Source.CreateHashPrimitive (function);
            _output = new byte[_digest.DigestSize];
			output = _output;
		}

		public HashStream(Stream binding, bool writing, IVerificationFunctionConfiguration config, 
			bool closeOnDispose = true) : base(binding, writing, closeOnDispose) 
		{
			if(config.FunctionType.ToEnum<VerificationFunctionType>() != VerificationFunctionType.Mac) {
				throw new ConfigurationInvalidException ("Configuration specifies function type other than MAC.");
			}

			_digest = Source.CreateHashPrimitive (config.FunctionName.ToEnum<HashFunction>());
			_output = new byte[_digest.DigestSize];
		}


		public override void Write (byte[] buffer, int offset, int count) {
			base.Write(buffer, offset, count);
			_digest.BlockUpdate(buffer, offset, count);
		}

		public override void WriteByte (byte b) {
			base.WriteByte (b);
			_digest.Update(b);
		} 

		public override int ReadByte () {
			var readByte = base.ReadByte();
			if (readByte >= 0) {
				_digest.Update((byte)readByte);
			}
			return readByte;
		}

		public override int Read (byte[] buffer, int offset, int count) {
			int readBytes = base.Read(buffer, offset, count);
			if (readBytes > 0) {
				_digest.BlockUpdate(buffer, offset, readBytes);
			}
			return readBytes;
		}

		public override long WriteExactlyFrom (Stream source, long length) {
			CheckIfCanDecorate ();
			if(source == null) {
				throw new ArgumentNullException ("source");
			}

			if(_buffer == null) {
				_buffer = new byte[BufferSize];
			}
			int iterIn = 0;
			long totalIn = 0;
			while(length > 0) {
				iterIn = source.Read (_buffer, 0, (int) Math.Min (BufferSize, length));
				if(iterIn == 0) {
					throw new EndOfStreamException ();
				}
				totalIn += iterIn;
				_digest.BlockUpdate(_buffer, 0, iterIn);
				Binding.Write (_buffer, 0, iterIn);
			}

			return totalIn;
		}

		public override long ReadExactlyTo (Stream destination, long length, bool finishing = false) {
			CheckIfCanDecorate ();
			if(destination == null) {
				throw new ArgumentNullException ("destination");
			}

			if(_buffer == null) {
				_buffer = new byte[BufferSize];
			}
			int iterIn = 0;
			long totalIn = 0;
			while(length > 0) {
				iterIn = DecoratorBinding.Read (_buffer, 0, (int) Math.Min (BufferSize, length));
				if(iterIn == 0) {
					throw new EndOfStreamException ();
				}
				totalIn += iterIn;
				_digest.BlockUpdate(_buffer, 0, iterIn);
				destination.Write (_buffer, 0, iterIn);
			}
			if(finishing) {
				Finish ();
			}

			return totalIn;
		}

		protected override void Finish () {
			CheckIfCanDecorate ();
			_digest.DoFinal (_output, 0);
			base.Finish ();
		}

		protected override void Reset (bool finish = false) {
			base.Reset (finish);
			_digest.Reset ();
			Array.Clear (_output, 0, _output.Length);
		}
	}
}


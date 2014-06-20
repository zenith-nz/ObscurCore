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

namespace ObscurCore
{
    /// <summary>
    ///     Base class for ObscurCore's decorating streams to inherit from.
    /// </summary>
    public abstract class DecoratingStream : Stream
    {
        /// <summary>
        ///     Default amount of data a buffer associated with this stream must store to avoid I/O errors.
        /// </summary>
        private const int DefaultBufferRequirement = 1024; // 1 KB

        private readonly bool _closeOnDispose;

        protected bool Disposed;
        protected bool Finished;
        protected Stream StreamBinding;

        /// <summary>
        ///     Set this field in the constructor of a derived class to indicate how much data the base stream
        ///     must have access to mid-operation to avoid I/O errors. Depends on behaviour of derived class logic.
        /// </summary>
        private int? _bufferRequirement;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObscurCore.DecoratingStream" /> class.
        /// </summary>
        /// <param name="binding">Stream to bind decoration functionality to.</param>
        /// <param name="writing">If set to <c>true</c>, stream is used for writing-only, as opposed to reading-only.</param>
        /// <param name="closeOnDispose">If set to <c>true</c>, when stream is closed, bound stream will also be closed.</param>
        protected DecoratingStream(Stream binding, bool writing, bool closeOnDispose)
        {
            StreamBinding = binding;
            Writing = writing;
            _closeOnDispose = closeOnDispose;
        }

        /// <summary>
        ///     Stream that decorator writes to or reads from.
        /// </summary>
        /// <value>Stream StreamBinding.</value>
        public Stream Binding
        {
            get { return StreamBinding; }
        }

        /// <summary>
        ///     Whether the stream that decorator writes/reads to/from is also a <see cref="ObscurCore.DecoratingStream" />.
        /// </summary>
        /// <value><c>true</c> if StreamBinding is decorator; otherwise, <c>false</c>.</value>
        public bool BindingIsDecorator
        {
            get { return Binding is DecoratingStream; }
        }

        /// <summary>
        ///     What I/O mode of the decorator is active.
        /// </summary>
        /// <value><c>true</c> if writing, <c>false</c> if reading.</value>
        public bool Writing { get; private set; }

        public long BytesIn { get; protected set; }
        public long BytesOut { get; protected set; }

        /// <summary>
        ///     How many bytes must be kept in reserve to avoid I/O errors.
        ///     When writing, this amount reflects capacity that must be free/empty to accomodate a write.
        ///     When reading, it reflects data that must be available to accomodate a read.
        /// </summary>
        /// <remarks>
        ///     Clearly, this cannot apply for the ends of streams;
        ///     this being violated is the means of end-of-stream detection.
        /// </remarks>
        public int BufferSizeRequirement
        {
            get { return GetBufferRequirement(0) ?? DefaultBufferRequirement; }
            protected set { _bufferRequirement = value; }
        }

        public override bool CanRead
        {
            get { return Binding.CanRead; }
        }

        public override bool CanWrite
        {
            get { return Binding.CanWrite; }
        }

        public override bool CanSeek
        {
            get { return StreamBinding.CanSeek; }
        }

        public override long Length
        {
            get { return StreamBinding.Length; }
        }

        public override long Position
        {
            get { return Binding.Position; }
            set
            {
                if (!CanSeek) {
                    throw new NotSupportedException();
                }
                //StreamBinding.Position = value;
                StreamBinding.Seek(value, SeekOrigin.Begin);
            }
        }

        /// <summary>
        ///     Determine the maximum of the minimum size buffers required for
        ///     reliable I/O in a sequence of bound streams.
        /// </summary>
        /// <returns>The buffer requirement.</returns>
        /// <param name="maxFound">Maximum of the minimum sizes found thus far in recursive call.</param>
        protected int? GetBufferRequirement(int maxFound)
        {
            var dc = Binding as DecoratingStream;
            if (dc != null) {
                int? bindingRequirement = dc.GetBufferRequirement(maxFound);
                if (bindingRequirement.HasValue) {
                    return Math.Max(maxFound, bindingRequirement.Value);
                }
            }
            return _bufferRequirement;
        }

        /// <summary>
        ///     Check if disposed or finished (throw exception if either).
        /// </summary>
        protected void CheckIfCanDecorate()
        {
            if (Disposed) {
                throw new ObjectDisposedException(GetType().Name);
            }
            if (Finished) {
                throw new InvalidOperationException();
            }
        }

        public override void WriteByte(byte b)
        {
            CheckIfCanDecorate();
            Binding.WriteByte(b);
            BytesIn++;
            BytesOut++;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            CheckIfCanDecorate();
            Binding.Write(buffer, offset, count);
            BytesIn += count;
            BytesOut += count;
        }

        /// <summary>
        ///     Write exact quantity of bytes (after decoration) to the destination.
        /// </summary>
        /// <returns>The quantity of bytes taken from the source stream to fulfil the request.</returns>
        /// <param name="source">Source.</param>
        /// <param name="length">Length.</param>
        public abstract long WriteExactlyFrom(Stream source, long length);

        public override int ReadByte()
        {
            if (Disposed) {
                throw new ObjectDisposedException(GetType().Name);
            }
            if (Finished) {
                return -1;
            }
            BytesIn++;
            BytesOut++;
            return Binding.ReadByte();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            CheckIfCanDecorate();
            int readBytes = Binding.Read(buffer, offset, count);
            BytesIn += readBytes;
            BytesOut += readBytes;
            return readBytes;
        }

        /// <summary>
        ///     Read an exact amount of bytes from the stream StreamBinding and write them
        ///     (after decoration) to the destination.
        /// </summary>
        /// <returns>The quantity of bytes written to the destination stream.</returns>
        /// <param name="destination">Stream to write output to.</param>
        /// <param name="length">Quantity of bytes to read.</param>
        /// <param name="finishing">
        ///     If used in derived class, and set to <c>true</c>, causes special behaviour if this is the last read.
        /// </param>
        public abstract long ReadExactlyTo(Stream destination, long length, bool finishing = false);

        public override long Seek(long offset, SeekOrigin origin)
        {
            return StreamBinding.Seek(offset, origin);
        }

        public override void SetLength(long length)
        {
            StreamBinding.SetLength(length);
        }

        public override void Flush()
        {
            StreamBinding.Flush();
        }

        /// <summary>
        ///     Changes the stream that is written to or read from from this decorating stream.
        ///     Writing/Reading mode is not reassignable without object reconstruction.
        /// </summary>
        /// <param name="newBinding">The stream that the decorator will be bound to after method completion.</param>
        /// <param name="reset">Whether to reset the rest of the decorator state in addition to the stream StreamBinding.</param>
        /// <param name="finish">
        ///     Whether to finalise the existing decoration operation before resetting. Only applicable if
        ///     resetting.
        /// </param>
        public void ReassignBinding(Stream newBinding, bool reset = true, bool finish = false)
        {
            if (newBinding == null || newBinding == Null) {
                throw new ArgumentNullException("newBinding", "Stream is null, cannot reassign.");
            }
            if (reset) {
                Reset(finish);
            }
            StreamBinding = newBinding;
            Finished = false;
        }

        protected virtual void Reset(bool finish = false)
        {
            if (finish) {
                Finish();
            }
            BytesIn = 0;
            BytesOut = 0;
            Finished = false;
        }

        /// <summary>
        ///     Finish the decoration operation (whatever that may constitute in a derived implementation).
        ///     Could be done before a close or reset.
        /// </summary>
        protected virtual void Finish()
        {
            if (Finished) {
                return;
            }
            Finished = true;
        }

        public override void Close()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected override void Dispose(bool disposing)
        {
            try {
                if (Disposed == false) {
                    if (disposing) {
                        // dispose managed resources
                        Finish();
                        if (StreamBinding != null && _closeOnDispose) {
                            StreamBinding.Close();
                        }
                        StreamBinding = null;
                    }
                }
                Disposed = true;
            }
            finally {
                if (StreamBinding != null) {
                    StreamBinding = null;
                    base.Dispose(disposing);
                }
            }
        }
    }
}

//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.IO;
using System.Security.Cryptography;

namespace Microsoft.Samples.Discovery
{

    class HashStream : Stream
    {
        bool hashNeedsReset;
        long length;

        public HashStream(HashAlgorithm hash)
        {
            Utility.IfNullThrowNullArgumentException(hash, "hash");
            this.Hash = hash;
        }

        public HashAlgorithm Hash { get; private set; }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get { return this.length; }
        }

        public override long Position
        {
            get
            {
                return this.length;
            }

            set
            {
                throw new NotSupportedException("Can't modify position, because it is is read-only");
            }
        }

        public override void Flush()
        {
        }

        public void FlushHash()
        {
            this.Hash.TransformFinalBlock(new byte[0], 0, 0);
        }

        public byte[] FlushHashAndGetValue()
        {
            this.FlushHash();
            return this.Hash.Hash;
        }

        public void Reset()
        {
            if (this.hashNeedsReset)
            {
                this.Hash.Initialize();
                this.hashNeedsReset = false;
            }

            this.length = 0L;
        }

        public void Reset(HashAlgorithm hash)
        {
            this.Hash = hash;
            this.hashNeedsReset = false;
            this.length = 0L;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.Hash.TransformBlock(buffer, offset, count, buffer, offset);
            this.length += count;
            this.hashNeedsReset = true;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("Read is not a supported operation on the HashStream");
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("Seek is not a supported operation on the HashStream");
        }

        public override void SetLength(long length)
        {
            throw new NotSupportedException("SetLength is not a supported operation on the HashStream");
        }
    }
}

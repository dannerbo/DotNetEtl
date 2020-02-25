using System;
using System.IO;

namespace DotNetEtl
{
	public abstract class BinaryStreamReader : DataReader
	{
		private bool disposeStream;

		public BinaryStreamReader(Stream stream, IRecordMapper recordMapper = null)
			: base(recordMapper)
		{
			this.Stream = stream;
		}

		public BinaryStreamReader(IStreamFactory streamFactory, IRecordMapper recordMapper = null)
			: base(recordMapper)
		{
			this.StreamFactory = streamFactory;
			this.disposeStream = true;
		}

		protected Stream Stream { get; private set; }
		protected IStreamFactory StreamFactory { get; private set; }
		protected BinaryReader InternalBinaryReader { get; private set; }

		public override void Open()
		{
			this.ThrowIfBinaryReaderAlreadyCreated();
			this.CreateStreamIfNeeded();

			this.InternalBinaryReader = this.CreateInternalBinaryReader();
		}

		public override void Close()
		{
			if (this.InternalBinaryReader != null)
			{
				this.InternalBinaryReader.Dispose();
				this.InternalBinaryReader = null;
			}

			if (this.disposeStream && this.Stream != null)
			{
				this.Stream.Dispose();
				this.Stream = null;
			}
		}

		protected virtual BinaryReader CreateInternalBinaryReader() => new BinaryReader(this.Stream);

		private void ThrowIfBinaryReaderAlreadyCreated()
		{
			if (this.InternalBinaryReader != null)
			{
				throw new InvalidOperationException("Stream is already open.");
			}
		}

		private void CreateStreamIfNeeded()
		{
			if (this.Stream == null)
			{
				this.Stream = this.StreamFactory.Create();
			}
		}
	}
}

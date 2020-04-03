using System;
using System.IO;

namespace DotNetEtl
{
	public abstract class StreamReader : DataReader
	{
		private bool disposeStream;

		public StreamReader(Stream stream, IRecordMapper recordMapper = null)
			: base(recordMapper)
		{
			this.Stream = stream;
		}

		public StreamReader(IStreamFactory streamFactory, IRecordMapper recordMapper = null)
			: base(recordMapper)
		{
			this.StreamFactory = streamFactory;
			this.disposeStream = true;
		}

		protected Stream Stream { get; private set; }
		protected IStreamFactory StreamFactory { get; private set; }
		protected System.IO.StreamReader InternalStreamReader { get; private set; }

		public override void Open()
		{
			this.ThrowIfStreamReaderAlreadyCreated();
			this.CreateStreamIfNeeded();

			this.InternalStreamReader = this.CreateInternalStreamReader();
		}

		public override void Close()
		{
			if (this.InternalStreamReader != null)
			{
				this.InternalStreamReader.Dispose();
				this.InternalStreamReader = null;
			}

			if (this.disposeStream && this.Stream != null)
			{
				this.Stream.Dispose();
				this.Stream = null;
			}
		}

		protected virtual System.IO.StreamReader CreateInternalStreamReader() => new System.IO.StreamReader(this.Stream);

		private void ThrowIfStreamReaderAlreadyCreated()
		{
			if (this.InternalStreamReader != null)
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

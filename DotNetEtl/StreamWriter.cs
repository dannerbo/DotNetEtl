using System;
using System.IO;

namespace DotNetEtl
{
	public abstract class StreamWriter : DataWriter
	{
		private bool disposeStream;

		public StreamWriter(Stream stream, IRecordFormatter recordFormatter = null)
			: base(recordFormatter)
		{
			this.Stream = stream;
		}

		public StreamWriter(IStreamFactory streamFactory, IRecordFormatter recordFormatter = null)
			: base(recordFormatter)
		{
			this.StreamFactory = streamFactory;
			this.disposeStream = true;
		}

		protected Stream Stream { get; private set; }
		protected IStreamFactory StreamFactory { get; private set; }
		protected System.IO.StreamWriter InternalStreamWriter { get; private set; }

		public override void Open()
		{
			this.ThrowIfStreamWriterAlreadyCreated();
			this.CreateStreamIfNeeded();

			this.InternalStreamWriter = this.CreateInternalStreamWriter();
		}

		public override void Close()
		{
			if (this.InternalStreamWriter != null)
			{
				this.InternalStreamWriter.Dispose();
				this.InternalStreamWriter = null;
			}

			if (this.disposeStream && this.Stream != null)
			{
				this.Stream.Dispose();
				this.Stream = null;
			}
		}

		public override void Commit()
		{
			this.InternalStreamWriter.Flush();
		}

		public override void Rollback()
		{
		}

		protected virtual System.IO.StreamWriter CreateInternalStreamWriter() => new System.IO.StreamWriter(this.Stream);

		private void ThrowIfStreamWriterAlreadyCreated()
		{
			if (this.InternalStreamWriter != null)
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

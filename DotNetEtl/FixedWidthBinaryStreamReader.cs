using System;
using System.IO;

namespace DotNetEtl
{
	public class FixedWidthBinaryStreamReader : BinaryStreamReader
	{
		public FixedWidthBinaryStreamReader(Stream stream, IRecordMapper recordMapper = null)
			: base(stream, recordMapper)
		{
		}

		public FixedWidthBinaryStreamReader(IStreamFactory streamFactory, IRecordMapper recordMapper = null)
			: base(streamFactory, recordMapper)
		{
		}

		public int RecordSize { get; set; }
		public int HeaderSize { get; set; }
		public int FooterSize { get; set; }

		protected override object ReadRecordInternal()
		{
			if (this.HeaderSize > 0 && this.InternalBinaryReader.BaseStream.Position == 0)
			{
				this.ReadHeader();
			}

			if (this.FooterSize > 0
				&& (this.InternalBinaryReader.BaseStream.Position >= this.InternalBinaryReader.BaseStream.Length - this.FooterSize))
			{
				this.ReadFooter();

				return null;
			}

			if (this.InternalBinaryReader.BaseStream.Position >= this.InternalBinaryReader.BaseStream.Length)
			{
				return null;
			}

			var buffer = new byte[this.RecordSize];
			var bytesRead = this.ReadFromStream(buffer, this.RecordSize);

			if (bytesRead != this.RecordSize)
			{
				throw new InvalidOperationException($"Read {bytesRead} bytes but expected {this.RecordSize}.");
			}

			return buffer;
		}

		protected virtual void ReadHeader()
		{
			var buffer = new byte[this.HeaderSize];
			var bytesRead = this.ReadFromStream(buffer, this.HeaderSize);

			if (bytesRead != this.HeaderSize)
			{
				throw new InvalidOperationException($"Read {bytesRead} bytes but expected {this.HeaderSize} for header.");
			}
		}

		protected virtual void ReadFooter()
		{
			var buffer = new byte[this.FooterSize];
			var bytesRead = this.ReadFromStream(buffer, this.FooterSize);

			if (bytesRead != this.FooterSize)
			{
				throw new InvalidOperationException($"Read {bytesRead} bytes but expected {this.FooterSize} for footer.");
			}
		}

		protected virtual int ReadFromStream(byte[] buffer, int length)
		{
			return this.InternalBinaryReader.Read(buffer, 0, length);
		}
	}
}

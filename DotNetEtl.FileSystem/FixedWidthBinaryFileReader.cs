using System;
using System.IO;

namespace DotNetEtl.FileSystem
{
	public class FixedWidthBinaryFileReader : FileReader
	{
		public FixedWidthBinaryFileReader(string filePath, IRecordMapper recordMapper = null, FileShare fileShare = FileShare.None)
			: base(filePath, recordMapper, fileShare)
		{
		}

		public int RecordSize { get; set; }
		public int? HeaderSize { get; set; }
		public int? FooterSize { get; set; }
		protected BinaryReader StreamReader { get; private set; }

		public override void Open()
		{
			base.Open();

			this.StreamReader = new BinaryReader(this.FileStream);
		}

		public override void Close()
		{
			if (this.StreamReader != null)
			{
				this.StreamReader.Dispose();
				this.StreamReader = null;
			}

			base.Close();
		}

		protected override object ReadRecordInternal()
		{
			if (this.HeaderSize.HasValue && this.StreamReader.BaseStream.Position == 0)
			{
				this.ReadHeader();
			}

			if (this.FooterSize.HasValue
				&& (this.StreamReader.BaseStream.Position >= this.StreamReader.BaseStream.Length - this.FooterSize.Value))
			{
				this.ReadFooter();

				return null;
			}

			if (this.StreamReader.BaseStream.Position >= this.StreamReader.BaseStream.Length)
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
			var buffer = new byte[this.HeaderSize.Value];
			var bytesRead = this.ReadFromStream(buffer, this.HeaderSize.Value);

			if (bytesRead != this.HeaderSize.Value)
			{
				throw new InvalidOperationException($"Read {bytesRead} bytes but expected {this.HeaderSize.Value} for header.");
			}
		}

		protected virtual void ReadFooter()
		{
			var buffer = new byte[this.FooterSize.Value];
			var bytesRead = this.ReadFromStream(buffer, this.FooterSize.Value);

			if (bytesRead != this.FooterSize.Value)
			{
				throw new InvalidOperationException($"Read {bytesRead} bytes but expected {this.FooterSize.Value} for footer.");
			}
		}

		protected virtual int ReadFromStream(byte[] buffer, int length)
		{
			return this.StreamReader.Read(buffer, 0, length);
		}
	}
}

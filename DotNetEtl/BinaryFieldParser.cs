using System;
using System.IO;
using System.Reflection;

namespace DotNetEtl
{
	public class BinaryFieldParser : IFieldParser
	{
		public bool TryParse(PropertyInfo property, object fieldValue, out object parsedFieldValue, out string failureMessage)
		{
			parsedFieldValue = null;
			failureMessage = null;

			var parseFieldAttribute = property.GetCustomAttribute<ParseFieldAttribute>(true);

			if (parseFieldAttribute != null)
			{
				return parseFieldAttribute.TryParse(property, fieldValue, out parsedFieldValue, out failureMessage);
			}

			var propertyType = property.PropertyType;
			var isNullable = !propertyType.IsValueType;

			if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				propertyType = Nullable.GetUnderlyingType(propertyType);
				isNullable = true;
			}

			var bytes = (byte[])fieldValue;

			if (isNullable && BinaryFieldParser.IsNull(bytes))
			{
				return false;
			}

			try
			{
				using (var memoryStream = new MemoryStream(bytes))
				using (var binaryReader = new BinaryReader(memoryStream))
				{
					if (propertyType == typeof(bool))
					{
						parsedFieldValue = binaryReader.ReadBoolean();
					}
					else if (propertyType == typeof(byte))
					{
						parsedFieldValue = binaryReader.ReadByte();
					}
					else if (propertyType == typeof(byte[]))
					{
						parsedFieldValue = binaryReader.ReadBytes(bytes.Length);
					}
					else if (propertyType == typeof(char))
					{
						parsedFieldValue = binaryReader.ReadChar();
					}
					else if (propertyType == typeof(char[]))
					{
						parsedFieldValue = binaryReader.ReadChars(bytes.Length);
					}
					else if (propertyType == typeof(decimal))
					{
						parsedFieldValue = binaryReader.ReadDecimal();
					}
					else if (propertyType == typeof(double))
					{
						parsedFieldValue = binaryReader.ReadDouble();
					}
					else if (propertyType == typeof(short))
					{
						parsedFieldValue = binaryReader.ReadInt16();
					}
					else if (propertyType == typeof(int))
					{
						parsedFieldValue = binaryReader.ReadInt32();
					}
					else if (propertyType == typeof(long))
					{
						parsedFieldValue = binaryReader.ReadInt64();
					}
					else if (propertyType == typeof(sbyte))
					{
						parsedFieldValue = binaryReader.ReadSByte();
					}
					else if (propertyType == typeof(float))
					{
						parsedFieldValue = binaryReader.ReadSingle();
					}
					else if (propertyType == typeof(string))
					{
						var stringValue = new string(binaryReader.ReadChars(bytes.Length));
						var nullTerminatorPosition = stringValue.IndexOf('\0');

						parsedFieldValue = nullTerminatorPosition > -1
							? stringValue.Remove(nullTerminatorPosition)
							: stringValue;
					}
					else if (propertyType == typeof(ushort))
					{
						parsedFieldValue = binaryReader.ReadUInt16();
					}
					else if (propertyType == typeof(uint))
					{
						parsedFieldValue = binaryReader.ReadUInt32();
					}
					else if (propertyType == typeof(ulong))
					{
						parsedFieldValue = binaryReader.ReadUInt64();
					}
					else if (propertyType == typeof(DateTime))
					{
						parsedFieldValue = DateTime.FromBinary(binaryReader.ReadInt64());
					}

					if (parsedFieldValue != null)
					{
						return true;
					}
				}
			}
			catch
			{
				parsedFieldValue = null;
				failureMessage = "Field is invalid.";

				return false;
			}

			throw new InvalidOperationException($"Cannot convert binary data to {property.PropertyType}.");
		}

		private static bool IsNull(byte[] bytes)
		{
			for (int i = 0; i < bytes.Length; i++)
			{
				if (bytes[i] != 0x0)
				{
					return false;
				}
			}

			return true;
		}
	}
}

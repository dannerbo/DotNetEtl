using System;
using System.Collections.Generic;

namespace DotNetEtl
{
	public static class TypeHelper
	{
		private static HashSet<Type> NumericTypes = new HashSet<Type>(new Type[]
{
			typeof(byte),
			typeof(sbyte),
			typeof(short),
			typeof(ushort),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
			typeof(float),
			typeof(double),
			typeof(decimal)
		});

		public static bool IsNumeric(Type type)
		{
			type = Nullable.GetUnderlyingType(type) ?? type;

			return TypeHelper.NumericTypes.Contains(type);
		}
	}
}

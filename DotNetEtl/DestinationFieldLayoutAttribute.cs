﻿using System;

namespace DotNetEtl
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class DestinationFieldLayoutAttribute : Attribute
	{
		public DestinationFieldLayoutAttribute(int startIndex, int length)
		{
			this.StartIndex = startIndex;
			this.Length = length;
		}

		public int StartIndex { get; private set; }
		public int Length { get; private set; }
	}
}

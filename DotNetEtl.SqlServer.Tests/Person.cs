using System;
using System.Collections.Generic;
using System.Data;
using Bogus;

namespace DotNetEtl.SqlServer.Tests
{
	public class Person : IEquatable<Person>
	{
		public static readonly Faker<Person> Faker = new Faker<Person>()
			.RuleFor(x => x.FirstName, p => p.Name.FirstName())
			.RuleFor(x => x.LastName, p => p.Name.LastName())
			.RuleFor(x => x.MiddleInitial, p => p.Random.String2(1).ToUpper())
			.RuleFor(x => x.Age, p => p.Random.Int(1, 100))
			.RuleFor(x => x.DateOfBirth, p => p.Date.Past(100, DateTime.Today).Date)
			.RuleFor(x => x.Gender, p => p.PickRandom(Gender.Male, Gender.Female));

		[SqlParameter("FirstName", SqlDbType.VarChar, 50)]
		[TableValuedParameterField(0, "FirstName", SqlDbType.VarChar, 50)]
		[SourceFieldName("FirstName")]
		public string FirstName { get; set; }

		[SqlParameter("LastName", SqlDbType.VarChar, 50)]
		[TableValuedParameterField(1, "LastName", SqlDbType.VarChar, 50)]
		[SourceFieldName("LastName")]
		public string LastName { get; set; }

		[SqlParameter("MiddleInitial", SqlDbType.Char, 1)]
		[TableValuedParameterField(2, "MiddleInitial", SqlDbType.Char, 1)]
		[SourceFieldName("MiddleInitial")]
		public string MiddleInitial { get; set; }

		[SqlParameter("Age", SqlDbType.TinyInt)]
		[TableValuedParameterField(3, "Age", SqlDbType.TinyInt)]
		[SourceFieldName("Age")]
		public int Age { get; set; }

		[SqlParameter("DateOfBirth", SqlDbType.Date)]
		[TableValuedParameterField(4, "DateOfBirth", SqlDbType.Date)]
		[SourceFieldName("DateOfBirth")]
		public DateTime DateOfBirth { get; set; }

		[SqlParameter("Gender", SqlDbType.TinyInt)]
		[TableValuedParameterField(5, "Gender", SqlDbType.TinyInt)]
		[SourceFieldName("Gender")]
		public Gender Gender { get; set; }

		public bool Equals(Person other)
		{
			return this.GetHashCode().Equals(other.GetHashCode());
		}

		public override bool Equals(object obj)
		{
			var otherPerson = obj as Person;

			return otherPerson != null
				? this.Equals(otherPerson)
				: false;
		}

		public override int GetHashCode()
		{
			var hashCode = -915476735;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.FirstName);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.LastName);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.MiddleInitial);
			hashCode = hashCode * -1521134295 + this.Age.GetHashCode();
			hashCode = hashCode * -1521134295 + this.DateOfBirth.GetHashCode();
			hashCode = hashCode * -1521134295 + this.Gender.GetHashCode();
			return hashCode;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace DotNetEtl
{
	public class DynamicRecordFactory : IRecordFactory
	{
		protected static Random random = new Random();
		private Dictionary<int, Type> recordTypes = new Dictionary<int, Type>();

		public DynamicRecordFactory(IDynamicRecordFieldProvider fieldProvider, IRecordTypeProvider recordTypeProvider)
		{
			this.FieldProvider = fieldProvider;
			this.RecordTypeProvider = recordTypeProvider;
		}

		public DynamicRecordFactory(IDynamicRecordFieldProvider fieldProvider)
			: this(fieldProvider, null)
		{
		}

		protected IDynamicRecordFieldProvider FieldProvider { get; private set; }
		protected IRecordTypeProvider RecordTypeProvider { get; private set; }

		public virtual object Create(object source)
		{
			var recordType = this.RecordTypeProvider?.GetRecordType(source);
			var fields = recordType != null
				? this.FieldProvider.GetFields(recordType)
				: this.FieldProvider.GetFields();

			if (fields == null || fields.Count() == 0)
			{
				throw new InvalidOperationException($"{nameof(IDynamicRecordFieldProvider)} did not return any fields.");
			}

			var fieldsHash = this.CreateFieldsHash(fields);
			
			if (!this.recordTypes.TryGetValue(fieldsHash, out Type dotNetRecordType))
			{
				dotNetRecordType = this.CreateRecordType(fields);

				this.recordTypes.Add(fieldsHash, dotNetRecordType);
			}

			var record = Activator.CreateInstance(dotNetRecordType);

			return record;
		}
		
		protected virtual int CreateFieldsHash(IEnumerable<IDynamicRecordField> fields)
		{
			var sb = new StringBuilder();

			foreach (var field in fields.OrderBy(x => x.Name))
			{
				sb.Append(field.Name).Append(",").Append(field.DotNetDataType.FullName).Append("|");
			}

			var hash = sb.ToString().GetHashCode();

			return hash;
		}

		protected virtual Type CreateRecordType(IEnumerable<IDynamicRecordField> fields)
		{
			var typeBuilder = GetTypeBuilder();
			var constructor = typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
			
			foreach (var field in fields)
			{
				this.CreateProperty(typeBuilder, field);
			}

			var recordType = typeBuilder.CreateType();

			return recordType;
		}

		protected virtual TypeBuilder GetTypeBuilder()
		{
			var typeName = this.GetTypeName();
			var assemblyName = new AssemblyName(typeName);
			var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule(String.Concat(nameof(DotNetEtl), ".", typeName));
			var typeBuilder = moduleBuilder.DefineType(
				typeName,
				TypeAttributes.Public
					| TypeAttributes.Class
					| TypeAttributes.AutoClass
					| TypeAttributes.AnsiClass
					| TypeAttributes.BeforeFieldInit
					| TypeAttributes.AutoLayout,
				null);

			return typeBuilder;
		}

		protected virtual void CreateProperty(TypeBuilder typeBuilder, IDynamicRecordField field)
		{
			var fieldBuilder = typeBuilder.DefineField("_" + field.Name, field.DotNetDataType, FieldAttributes.Private);
			var propertyBuilder = typeBuilder.DefineProperty(field.Name, PropertyAttributes.HasDefault, field.DotNetDataType, null);
			var getMethodBuilder = typeBuilder.DefineMethod(
				"get_" + field.Name,
				MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
				field.DotNetDataType,
				Type.EmptyTypes);
			var getIl = getMethodBuilder.GetILGenerator();

			getIl.Emit(OpCodes.Ldarg_0);
			getIl.Emit(OpCodes.Ldfld, fieldBuilder);
			getIl.Emit(OpCodes.Ret);

			var setMethodBuilder = typeBuilder.DefineMethod(
				"set_" + field.Name,
				MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
				null,
				new[] { field.DotNetDataType });

			var setIl = setMethodBuilder.GetILGenerator();
			var modifyProperty = setIl.DefineLabel();
			var exitSet = setIl.DefineLabel();

			setIl.MarkLabel(modifyProperty);
			setIl.Emit(OpCodes.Ldarg_0);
			setIl.Emit(OpCodes.Ldarg_1);
			setIl.Emit(OpCodes.Stfld, fieldBuilder);
			setIl.Emit(OpCodes.Nop);
			setIl.MarkLabel(exitSet);
			setIl.Emit(OpCodes.Ret);

			propertyBuilder.SetGetMethod(getMethodBuilder);
			propertyBuilder.SetSetMethod(setMethodBuilder);

			var attributes = field.GetCustomAttributes();

			if (attributes != null)
			{
				this.SetCustomAttributes(propertyBuilder, attributes);
			}
		}

		protected virtual string GetTypeName()
		{
			return "DynamicRecord_" + DynamicRecordFactory.GenerateRandomText(8);
		}

		protected virtual void SetCustomAttributes(PropertyBuilder property, IEnumerable<CustomAttributeBuilder> attributes)
		{
			foreach (var attribute in attributes)
			{
				property.SetCustomAttribute(attribute);
			}
		}
		
		protected static string GenerateRandomText(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new String(Enumerable.Repeat(chars, length).Select(s => s[DynamicRecordFactory.random.Next(s.Length)]).ToArray());
		}
	}
}

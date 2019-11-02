using System;
using System.Collections.Generic;
using System.Reflection;

namespace DotNetEtl
{
	public static class ReflectionCache
	{
		private static Dictionary<Type, PropertyInfo[]> GetPropertiesCache = new Dictionary<Type, PropertyInfo[]>();
		private static Dictionary<MemberInfo, Dictionary<Type, Attribute>> GetCustomAttributeCache = new Dictionary<MemberInfo, Dictionary<Type, Attribute>>();
		private static Dictionary<MemberInfo, Dictionary<Type, IEnumerable<Attribute>>> GetCustomAttributesCache = new Dictionary<MemberInfo, Dictionary<Type, IEnumerable<Attribute>>>();

		public static bool IsCachingEnabled { get; set; } = true;

		public static PropertyInfo[] GetCachedProperties(this Type type)
		{
			if (ReflectionCache.IsCachingEnabled)
			{
				lock (ReflectionCache.GetPropertiesCache)
				{
					if (!ReflectionCache.GetPropertiesCache.TryGetValue(type, out var properties))
					{
						properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

						ReflectionCache.GetPropertiesCache.Add(type, properties);
					}
				}

				return ReflectionCache.GetPropertiesCache[type];
			}
			else
			{
				return type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			}
		}

		public static T GetCachedCustomAttribute<T>(this MemberInfo memberInfo)
			where T : Attribute
		{
			if (ReflectionCache.IsCachingEnabled)
			{
				lock (ReflectionCache.GetCustomAttributeCache)
				{
					if (!ReflectionCache.GetCustomAttributeCache.TryGetValue(memberInfo, out var customAttributesLookup))
					{
						customAttributesLookup = new Dictionary<Type, Attribute>();
						
						ReflectionCache.GetCustomAttributeCache.Add(memberInfo, customAttributesLookup);
					}

					if (!customAttributesLookup.TryGetValue(typeof(T), out var attribute))
					{
						attribute = memberInfo.GetCustomAttribute<T>(true);

						customAttributesLookup.Add(typeof(T), attribute);
					}
				}

				return (T)ReflectionCache.GetCustomAttributeCache[memberInfo][typeof(T)];
			}
			else
			{
				return memberInfo.GetCustomAttribute<T>(true);
			}
		}

		public static IEnumerable<T> GetCachedCustomAttributes<T>(this MemberInfo memberInfo)
			where T : Attribute
		{
			if (ReflectionCache.IsCachingEnabled)
			{
				lock (ReflectionCache.GetCustomAttributesCache)
				{
					if (!ReflectionCache.GetCustomAttributesCache.TryGetValue(memberInfo, out var customAttributesLookup))
					{
						customAttributesLookup = new Dictionary<Type, IEnumerable<Attribute>>();

						ReflectionCache.GetCustomAttributesCache.Add(memberInfo, customAttributesLookup);
					}

					if (!customAttributesLookup.TryGetValue(typeof(T), out var attributes))
					{
						attributes = memberInfo.GetCustomAttributes<T>(true);

						customAttributesLookup.Add(typeof(T), attributes);
					}
				}

				return (IEnumerable<T>)ReflectionCache.GetCustomAttributesCache[memberInfo][typeof(T)];
			}
			else
			{
				return memberInfo.GetCustomAttributes<T>(true);
			}
		}
	}
}

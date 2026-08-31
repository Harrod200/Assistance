using System;
using System.Collections.Generic;
using System.Reflection;

namespace FullSerializer.Internal
{
	// Token: 0x02000484 RID: 1156
	public static class fsPortableReflection
	{
		// Token: 0x060018AD RID: 6317 RVA: 0x0007FCB6 File Offset: 0x0007DEB6
		public static bool HasAttribute<TAttribute>(MemberInfo element)
		{
			return fsPortableReflection.HasAttribute(element, typeof(TAttribute));
		}

		// Token: 0x060018AE RID: 6318 RVA: 0x0007FCC8 File Offset: 0x0007DEC8
		public static bool HasAttribute<TAttribute>(MemberInfo element, bool shouldCache)
		{
			return fsPortableReflection.HasAttribute(element, typeof(TAttribute), shouldCache);
		}

		// Token: 0x060018AF RID: 6319 RVA: 0x0007FCDB File Offset: 0x0007DEDB
		public static bool HasAttribute(MemberInfo element, Type attributeType)
		{
			return fsPortableReflection.HasAttribute(element, attributeType, true);
		}

		// Token: 0x060018B0 RID: 6320 RVA: 0x0007FCE5 File Offset: 0x0007DEE5
		public static bool HasAttribute(MemberInfo element, Type attributeType, bool shouldCache)
		{
			return element.IsDefined(attributeType, true);
		}

		// Token: 0x060018B1 RID: 6321 RVA: 0x0007FCF0 File Offset: 0x0007DEF0
		public static Attribute GetAttribute(MemberInfo element, Type attributeType, bool shouldCache)
		{
			fsPortableReflection.AttributeQuery attributeQuery = new fsPortableReflection.AttributeQuery
			{
				MemberInfo = element,
				AttributeType = attributeType
			};
			Attribute attribute;
			if (!fsPortableReflection._cachedAttributeQueries.TryGetValue(attributeQuery, out attribute))
			{
				object[] customAttributes = element.GetCustomAttributes(attributeType, true);
				if (customAttributes.Length != 0)
				{
					attribute = (Attribute)customAttributes[0];
				}
				if (shouldCache)
				{
					fsPortableReflection._cachedAttributeQueries[attributeQuery] = attribute;
				}
			}
			return attribute;
		}

		// Token: 0x060018B2 RID: 6322 RVA: 0x0007FD4C File Offset: 0x0007DF4C
		public static TAttribute GetAttribute<TAttribute>(MemberInfo element, bool shouldCache) where TAttribute : Attribute
		{
			return (TAttribute)((object)fsPortableReflection.GetAttribute(element, typeof(TAttribute), shouldCache));
		}

		// Token: 0x060018B3 RID: 6323 RVA: 0x0007FD64 File Offset: 0x0007DF64
		public static TAttribute GetAttribute<TAttribute>(MemberInfo element) where TAttribute : Attribute
		{
			return fsPortableReflection.GetAttribute<TAttribute>(element, true);
		}

		// Token: 0x060018B4 RID: 6324 RVA: 0x0007FD70 File Offset: 0x0007DF70
		public static PropertyInfo GetDeclaredProperty(this Type type, string propertyName)
		{
			PropertyInfo[] declaredProperties = type.GetDeclaredProperties();
			for (int i = 0; i < declaredProperties.Length; i++)
			{
				if (declaredProperties[i].Name == propertyName)
				{
					return declaredProperties[i];
				}
			}
			return null;
		}

		// Token: 0x060018B5 RID: 6325 RVA: 0x0007FDA8 File Offset: 0x0007DFA8
		public static MethodInfo GetDeclaredMethod(this Type type, string methodName)
		{
			MethodInfo[] declaredMethods = type.GetDeclaredMethods();
			for (int i = 0; i < declaredMethods.Length; i++)
			{
				if (declaredMethods[i].Name == methodName)
				{
					return declaredMethods[i];
				}
			}
			return null;
		}

		// Token: 0x060018B6 RID: 6326 RVA: 0x0007FDE0 File Offset: 0x0007DFE0
		public static ConstructorInfo GetDeclaredConstructor(this Type type, Type[] parameters)
		{
			foreach (ConstructorInfo constructorInfo in type.GetDeclaredConstructors())
			{
				ParameterInfo[] parameters2 = constructorInfo.GetParameters();
				if (parameters.Length == parameters2.Length)
				{
					for (int j = 0; j < parameters2.Length; j++)
					{
						parameters2[j].ParameterType != parameters[j];
					}
					return constructorInfo;
				}
			}
			return null;
		}

		// Token: 0x060018B7 RID: 6327 RVA: 0x0007FE3D File Offset: 0x0007E03D
		public static ConstructorInfo[] GetDeclaredConstructors(this Type type)
		{
			return type.GetConstructors(fsPortableReflection.DeclaredFlags);
		}

		// Token: 0x060018B8 RID: 6328 RVA: 0x0007FE4C File Offset: 0x0007E04C
		public static MemberInfo[] GetFlattenedMember(this Type type, string memberName)
		{
			List<MemberInfo> list = new List<MemberInfo>();
			while (type != null)
			{
				MemberInfo[] declaredMembers = type.GetDeclaredMembers();
				for (int i = 0; i < declaredMembers.Length; i++)
				{
					if (declaredMembers[i].Name == memberName)
					{
						list.Add(declaredMembers[i]);
					}
				}
				type = type.Resolve().BaseType;
			}
			return list.ToArray();
		}

		// Token: 0x060018B9 RID: 6329 RVA: 0x0007FEAC File Offset: 0x0007E0AC
		public static MethodInfo GetFlattenedMethod(this Type type, string methodName)
		{
			while (type != null)
			{
				MethodInfo[] declaredMethods = type.GetDeclaredMethods();
				for (int i = 0; i < declaredMethods.Length; i++)
				{
					if (declaredMethods[i].Name == methodName)
					{
						return declaredMethods[i];
					}
				}
				type = type.Resolve().BaseType;
			}
			return null;
		}

		// Token: 0x060018BA RID: 6330 RVA: 0x0007FEFB File Offset: 0x0007E0FB
		public static IEnumerable<MethodInfo> GetFlattenedMethods(this Type type, string methodName)
		{
			while (type != null)
			{
				MethodInfo[] methods = type.GetDeclaredMethods();
				int num;
				for (int i = 0; i < methods.Length; i = num)
				{
					if (methods[i].Name == methodName)
					{
						yield return methods[i];
					}
					num = i + 1;
				}
				type = type.Resolve().BaseType;
				methods = null;
			}
			yield break;
		}

		// Token: 0x060018BB RID: 6331 RVA: 0x0007FF14 File Offset: 0x0007E114
		public static PropertyInfo GetFlattenedProperty(this Type type, string propertyName)
		{
			while (type != null)
			{
				PropertyInfo[] declaredProperties = type.GetDeclaredProperties();
				for (int i = 0; i < declaredProperties.Length; i++)
				{
					if (declaredProperties[i].Name == propertyName)
					{
						return declaredProperties[i];
					}
				}
				type = type.Resolve().BaseType;
			}
			return null;
		}

		// Token: 0x060018BC RID: 6332 RVA: 0x0007FF64 File Offset: 0x0007E164
		public static MemberInfo GetDeclaredMember(this Type type, string memberName)
		{
			MemberInfo[] declaredMembers = type.GetDeclaredMembers();
			for (int i = 0; i < declaredMembers.Length; i++)
			{
				if (declaredMembers[i].Name == memberName)
				{
					return declaredMembers[i];
				}
			}
			return null;
		}

		// Token: 0x060018BD RID: 6333 RVA: 0x0007FF9B File Offset: 0x0007E19B
		public static MethodInfo[] GetDeclaredMethods(this Type type)
		{
			return type.GetMethods(fsPortableReflection.DeclaredFlags);
		}

		// Token: 0x060018BE RID: 6334 RVA: 0x0007FFA8 File Offset: 0x0007E1A8
		public static PropertyInfo[] GetDeclaredProperties(this Type type)
		{
			return type.GetProperties(fsPortableReflection.DeclaredFlags);
		}

		// Token: 0x060018BF RID: 6335 RVA: 0x0007FFB5 File Offset: 0x0007E1B5
		public static FieldInfo[] GetDeclaredFields(this Type type)
		{
			return type.GetFields(fsPortableReflection.DeclaredFlags);
		}

		// Token: 0x060018C0 RID: 6336 RVA: 0x0007FFC2 File Offset: 0x0007E1C2
		public static MemberInfo[] GetDeclaredMembers(this Type type)
		{
			return type.GetMembers(fsPortableReflection.DeclaredFlags);
		}

		// Token: 0x060018C1 RID: 6337 RVA: 0x0007FFCF File Offset: 0x0007E1CF
		public static MemberInfo AsMemberInfo(Type type)
		{
			return type;
		}

		// Token: 0x060018C2 RID: 6338 RVA: 0x0007FFD2 File Offset: 0x0007E1D2
		public static bool IsType(MemberInfo member)
		{
			return member is Type;
		}

		// Token: 0x060018C3 RID: 6339 RVA: 0x0007FFDD File Offset: 0x0007E1DD
		public static Type AsType(MemberInfo member)
		{
			return (Type)member;
		}

		// Token: 0x060018C4 RID: 6340 RVA: 0x0007FFE5 File Offset: 0x0007E1E5
		public static Type Resolve(this Type type)
		{
			return type;
		}

		// Token: 0x0400161D RID: 5661
		public static Type[] EmptyTypes = new Type[0];

		// Token: 0x0400161E RID: 5662
		private static IDictionary<fsPortableReflection.AttributeQuery, Attribute> _cachedAttributeQueries = new Dictionary<fsPortableReflection.AttributeQuery, Attribute>(new fsPortableReflection.AttributeQueryComparator());

		// Token: 0x0400161F RID: 5663
		private static BindingFlags DeclaredFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		// Token: 0x02000C58 RID: 3160
		private struct AttributeQuery
		{
			// Token: 0x04004E10 RID: 19984
			public MemberInfo MemberInfo;

			// Token: 0x04004E11 RID: 19985
			public Type AttributeType;
		}

		// Token: 0x02000C59 RID: 3161
		private class AttributeQueryComparator : IEqualityComparer<fsPortableReflection.AttributeQuery>
		{
			// Token: 0x06006C64 RID: 27748 RVA: 0x0030689B File Offset: 0x00304A9B
			public bool Equals(fsPortableReflection.AttributeQuery x, fsPortableReflection.AttributeQuery y)
			{
				return x.MemberInfo == y.MemberInfo && x.AttributeType == y.AttributeType;
			}

			// Token: 0x06006C65 RID: 27749 RVA: 0x003068C3 File Offset: 0x00304AC3
			public int GetHashCode(fsPortableReflection.AttributeQuery obj)
			{
				return obj.MemberInfo.GetHashCode() + 17 * obj.AttributeType.GetHashCode();
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace FullSerializer
{
	// Token: 0x0200045D RID: 1117
	public static class fsTypeExtensions
	{
		// Token: 0x0600178D RID: 6029 RVA: 0x0007A6E7 File Offset: 0x000788E7
		public static string CSharpName(this Type type)
		{
			return type.CSharpName(false);
		}

		// Token: 0x0600178E RID: 6030 RVA: 0x0007A6F0 File Offset: 0x000788F0
		public static string CSharpName(this Type type, bool includeNamespace, bool ensureSafeDeclarationName)
		{
			string text = type.CSharpName(includeNamespace);
			if (ensureSafeDeclarationName)
			{
				text = text.Replace('>', '_').Replace('<', '_').Replace('.', '_')
					.Replace(',', '_');
			}
			return text;
		}

		// Token: 0x0600178F RID: 6031 RVA: 0x0007A730 File Offset: 0x00078930
		public static string CSharpName(this Type type, bool includeNamespace)
		{
			if (type == typeof(void))
			{
				return "void";
			}
			if (type == typeof(int))
			{
				return "int";
			}
			if (type == typeof(float))
			{
				return "float";
			}
			if (type == typeof(bool))
			{
				return "bool";
			}
			if (type == typeof(double))
			{
				return "double";
			}
			if (type == typeof(string))
			{
				return "string";
			}
			if (type.IsGenericParameter)
			{
				return type.ToString();
			}
			string text = "";
			IEnumerable<Type> enumerable = type.GetGenericArguments();
			if (type.IsNested)
			{
				text = text + type.DeclaringType.CSharpName() + ".";
				if (type.DeclaringType.GetGenericArguments().Length != 0)
				{
					enumerable = enumerable.Skip<Type>(type.DeclaringType.GetGenericArguments().Length);
				}
			}
			if (!enumerable.Any<Type>())
			{
				text += type.Name;
			}
			else
			{
				int num = type.Name.IndexOf('`');
				if (num > 0)
				{
					text += type.Name.Substring(0, num);
				}
				text = text + "<" + string.Join(",", enumerable.Select<Type, string>((Type t) => t.CSharpName(includeNamespace)).ToArray<string>()) + ">";
			}
			if (includeNamespace && type.Namespace != null)
			{
				text = type.Namespace + "." + text;
			}
			return text;
		}
	}
}

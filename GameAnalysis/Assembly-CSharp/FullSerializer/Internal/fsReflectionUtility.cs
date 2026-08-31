using System;

namespace FullSerializer.Internal
{
	// Token: 0x02000488 RID: 1160
	public static class fsReflectionUtility
	{
		// Token: 0x060018E7 RID: 6375 RVA: 0x00080698 File Offset: 0x0007E898
		public static Type GetInterface(Type type, Type interfaceType)
		{
			if (interfaceType.Resolve().IsGenericType && !interfaceType.Resolve().IsGenericTypeDefinition)
			{
				throw new ArgumentException("GetInterface requires that if the interface type is generic, then it must be the generic type definition, not a specific generic type instantiation");
			}
			while (type != null)
			{
				foreach (Type type2 in type.GetInterfaces())
				{
					if (type2.Resolve().IsGenericType)
					{
						if (interfaceType == type2.GetGenericTypeDefinition())
						{
							return type2;
						}
					}
					else if (interfaceType == type2)
					{
						return type2;
					}
				}
				type = type.Resolve().BaseType;
			}
			return null;
		}
	}
}

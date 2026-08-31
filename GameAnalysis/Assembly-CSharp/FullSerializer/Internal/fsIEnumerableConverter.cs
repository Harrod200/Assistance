using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace FullSerializer.Internal
{
	// Token: 0x0200047A RID: 1146
	public class fsIEnumerableConverter : fsConverter
	{
		// Token: 0x0600186D RID: 6253 RVA: 0x0007ECF3 File Offset: 0x0007CEF3
		public override bool CanProcess(Type type)
		{
			return typeof(IEnumerable).IsAssignableFrom(type) && fsIEnumerableConverter.GetAddMethod(type) != null;
		}

		// Token: 0x0600186E RID: 6254 RVA: 0x0007ED15 File Offset: 0x0007CF15
		public override object CreateInstance(fsData data, Type storageType)
		{
			return fsMetaType.Get(this.Serializer.Config, storageType).CreateInstance();
		}

		// Token: 0x0600186F RID: 6255 RVA: 0x0007ED30 File Offset: 0x0007CF30
		public override fsResult TrySerialize(object instance_, out fsData serialized, Type storageType)
		{
			IEnumerable enumerable = (IEnumerable)instance_;
			fsResult success = fsResult.Success;
			Type elementType = fsIEnumerableConverter.GetElementType(storageType);
			serialized = fsData.CreateList(fsIEnumerableConverter.HintSize(enumerable));
			List<fsData> asList = serialized.AsList;
			foreach (object obj in enumerable)
			{
				fsData fsData;
				fsResult fsResult = this.Serializer.TrySerialize(elementType, obj, out fsData);
				success.AddMessages(fsResult);
				if (!fsResult.Failed)
				{
					asList.Add(fsData);
				}
			}
			if (this.IsStack(enumerable.GetType()))
			{
				asList.Reverse();
			}
			return success;
		}

		// Token: 0x06001870 RID: 6256 RVA: 0x0007EDEC File Offset: 0x0007CFEC
		private bool IsStack(Type type)
		{
			return type.Resolve().IsGenericType && type.Resolve().GetGenericTypeDefinition() == typeof(Stack<>);
		}

		// Token: 0x06001871 RID: 6257 RVA: 0x0007EE18 File Offset: 0x0007D018
		public override fsResult TryDeserialize(fsData data, ref object instance_, Type storageType)
		{
			IEnumerable enumerable = (IEnumerable)instance_;
			fsResult fsResult = fsResult.Success;
			fsResult fsResult2;
			fsResult = (fsResult2 = fsResult + base.CheckType(data, fsDataType.Array));
			if (fsResult2.Failed)
			{
				return fsResult;
			}
			Type elementType = fsIEnumerableConverter.GetElementType(storageType);
			MethodInfo addMethod = fsIEnumerableConverter.GetAddMethod(storageType);
			MethodInfo flattenedMethod = storageType.GetFlattenedMethod("get_Item");
			MethodInfo flattenedMethod2 = storageType.GetFlattenedMethod("set_Item");
			if (flattenedMethod2 == null)
			{
				fsIEnumerableConverter.TryClear(storageType, enumerable);
			}
			int num = fsIEnumerableConverter.TryGetExistingSize(storageType, enumerable);
			List<fsData> asList = data.AsList;
			for (int i = 0; i < asList.Count; i++)
			{
				fsData fsData = asList[i];
				object obj = null;
				if (flattenedMethod != null && i < num)
				{
					obj = flattenedMethod.Invoke(enumerable, new object[] { i });
				}
				fsResult fsResult3 = this.Serializer.TryDeserialize(fsData, elementType, ref obj);
				fsResult.AddMessages(fsResult3);
				if (!fsResult3.Failed)
				{
					if (flattenedMethod2 != null && i < num)
					{
						flattenedMethod2.Invoke(enumerable, new object[] { i, obj });
					}
					else
					{
						addMethod.Invoke(enumerable, new object[] { obj });
					}
				}
			}
			return fsResult;
		}

		// Token: 0x06001872 RID: 6258 RVA: 0x0007EF56 File Offset: 0x0007D156
		private static int HintSize(IEnumerable collection)
		{
			if (collection is ICollection)
			{
				return ((ICollection)collection).Count;
			}
			return 0;
		}

		// Token: 0x06001873 RID: 6259 RVA: 0x0007EF70 File Offset: 0x0007D170
		private static Type GetElementType(Type objectType)
		{
			if (objectType.HasElementType)
			{
				return objectType.GetElementType();
			}
			Type @interface = fsReflectionUtility.GetInterface(objectType, typeof(IEnumerable<>));
			if (@interface != null)
			{
				return @interface.GetGenericArguments()[0];
			}
			return typeof(object);
		}

		// Token: 0x06001874 RID: 6260 RVA: 0x0007EFBC File Offset: 0x0007D1BC
		private static void TryClear(Type type, object instance)
		{
			MethodInfo flattenedMethod = type.GetFlattenedMethod("Clear");
			if (flattenedMethod != null)
			{
				flattenedMethod.Invoke(instance, null);
			}
		}

		// Token: 0x06001875 RID: 6261 RVA: 0x0007EFE8 File Offset: 0x0007D1E8
		private static int TryGetExistingSize(Type type, object instance)
		{
			PropertyInfo flattenedProperty = type.GetFlattenedProperty("Count");
			if (flattenedProperty != null)
			{
				return (int)flattenedProperty.GetGetMethod().Invoke(instance, null);
			}
			return 0;
		}

		// Token: 0x06001876 RID: 6262 RVA: 0x0007F020 File Offset: 0x0007D220
		private static MethodInfo GetAddMethod(Type type)
		{
			Type @interface = fsReflectionUtility.GetInterface(type, typeof(ICollection<>));
			if (@interface != null)
			{
				MethodInfo declaredMethod = @interface.GetDeclaredMethod("Add");
				if (declaredMethod != null)
				{
					return declaredMethod;
				}
			}
			MethodInfo methodInfo;
			if ((methodInfo = type.GetFlattenedMethod("Add")) == null)
			{
				methodInfo = type.GetFlattenedMethod("Push") ?? type.GetFlattenedMethod("Enqueue");
			}
			return methodInfo;
		}
	}
}

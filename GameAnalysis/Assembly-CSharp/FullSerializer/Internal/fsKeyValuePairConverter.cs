using System;
using System.Collections.Generic;
using System.Reflection;

namespace FullSerializer.Internal
{
	// Token: 0x0200047B RID: 1147
	public class fsKeyValuePairConverter : fsConverter
	{
		// Token: 0x06001878 RID: 6264 RVA: 0x0007F08F File Offset: 0x0007D28F
		public override bool CanProcess(Type type)
		{
			return type.Resolve().IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<, >);
		}

		// Token: 0x06001879 RID: 6265 RVA: 0x0007F0B5 File Offset: 0x0007D2B5
		public override bool RequestCycleSupport(Type storageType)
		{
			return false;
		}

		// Token: 0x0600187A RID: 6266 RVA: 0x0007F0B8 File Offset: 0x0007D2B8
		public override bool RequestInheritanceSupport(Type storageType)
		{
			return false;
		}

		// Token: 0x0600187B RID: 6267 RVA: 0x0007F0BC File Offset: 0x0007D2BC
		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			fsResult fsResult = fsResult.Success;
			fsData fsData;
			fsResult fsResult2;
			fsResult = (fsResult2 = fsResult + base.CheckKey(data, "Key", out fsData));
			if (fsResult2.Failed)
			{
				return fsResult;
			}
			fsData fsData2;
			fsResult = (fsResult2 = fsResult + base.CheckKey(data, "Value", out fsData2));
			if (fsResult2.Failed)
			{
				return fsResult;
			}
			Type[] genericArguments = storageType.GetGenericArguments();
			Type type = genericArguments[0];
			Type type2 = genericArguments[1];
			object obj = null;
			object obj2 = null;
			fsResult.AddMessages(this.Serializer.TryDeserialize(fsData, type, ref obj));
			fsResult.AddMessages(this.Serializer.TryDeserialize(fsData2, type2, ref obj2));
			instance = Activator.CreateInstance(storageType, new object[] { obj, obj2 });
			return fsResult;
		}

		// Token: 0x0600187C RID: 6268 RVA: 0x0007F170 File Offset: 0x0007D370
		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			PropertyInfo declaredProperty = storageType.GetDeclaredProperty("Key");
			PropertyInfo declaredProperty2 = storageType.GetDeclaredProperty("Value");
			object value = declaredProperty.GetValue(instance, null);
			object value2 = declaredProperty2.GetValue(instance, null);
			Type[] genericArguments = storageType.GetGenericArguments();
			Type type = genericArguments[0];
			Type type2 = genericArguments[1];
			fsResult success = fsResult.Success;
			fsData fsData;
			success.AddMessages(this.Serializer.TrySerialize(type, value, out fsData));
			fsData fsData2;
			success.AddMessages(this.Serializer.TrySerialize(type2, value2, out fsData2));
			serialized = fsData.CreateDictionary();
			if (fsData != null)
			{
				serialized.AsDictionary["Key"] = fsData;
			}
			if (fsData2 != null)
			{
				serialized.AsDictionary["Value"] = fsData2;
			}
			return success;
		}
	}
}

using System;
using UnityEngine;
using UnityEngine.Events;

namespace FullSerializer.Internal.Converters
{
	// Token: 0x0200048C RID: 1164
	public class UnityEvent_Converter : fsConverter
	{
		// Token: 0x060018F9 RID: 6393 RVA: 0x00080A98 File Offset: 0x0007EC98
		public override bool CanProcess(Type type)
		{
			return typeof(UnityEvent).Resolve().IsAssignableFrom(type) && !type.IsGenericType;
		}

		// Token: 0x060018FA RID: 6394 RVA: 0x00080ABC File Offset: 0x0007ECBC
		public override bool RequestCycleSupport(Type storageType)
		{
			return false;
		}

		// Token: 0x060018FB RID: 6395 RVA: 0x00080AC0 File Offset: 0x0007ECC0
		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			Type type = (Type)instance;
			fsResult success = fsResult.Success;
			instance = JsonUtility.FromJson(fsJsonPrinter.CompressedJson(data), type);
			return success;
		}

		// Token: 0x060018FC RID: 6396 RVA: 0x00080AE8 File Offset: 0x0007ECE8
		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			fsResult success = fsResult.Success;
			serialized = fsJsonParser.Parse(JsonUtility.ToJson(instance));
			return success;
		}
	}
}

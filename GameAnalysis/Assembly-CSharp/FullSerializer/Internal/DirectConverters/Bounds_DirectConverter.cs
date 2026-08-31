using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullSerializer.Internal.DirectConverters
{
	// Token: 0x0200048E RID: 1166
	public class Bounds_DirectConverter : fsDirectConverter<Bounds>
	{
		// Token: 0x06001902 RID: 6402 RVA: 0x00080BED File Offset: 0x0007EDED
		protected override fsResult DoSerialize(Bounds model, Dictionary<string, fsData> serialized)
		{
			return fsResult.Success + base.SerializeMember<Vector3>(serialized, null, "center", model.center) + base.SerializeMember<Vector3>(serialized, null, "size", model.size);
		}

		// Token: 0x06001903 RID: 6403 RVA: 0x00080C28 File Offset: 0x0007EE28
		protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Bounds model)
		{
			fsResult success = fsResult.Success;
			Vector3 center = model.center;
			fsResult fsResult = success + base.DeserializeMember<Vector3>(data, null, "center", out center);
			model.center = center;
			Vector3 size = model.size;
			fsResult fsResult2 = fsResult + base.DeserializeMember<Vector3>(data, null, "size", out size);
			model.size = size;
			return fsResult2;
		}

		// Token: 0x06001904 RID: 6404 RVA: 0x00080C80 File Offset: 0x0007EE80
		public override object CreateInstance(fsData data, Type storageType)
		{
			return default(Bounds);
		}
	}
}

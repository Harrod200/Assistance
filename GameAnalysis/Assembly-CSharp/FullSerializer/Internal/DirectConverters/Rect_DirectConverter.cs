using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullSerializer.Internal.DirectConverters
{
	// Token: 0x02000495 RID: 1173
	public class Rect_DirectConverter : fsDirectConverter<Rect>
	{
		// Token: 0x0600191E RID: 6430 RVA: 0x00081778 File Offset: 0x0007F978
		protected override fsResult DoSerialize(Rect model, Dictionary<string, fsData> serialized)
		{
			return fsResult.Success + base.SerializeMember<float>(serialized, null, "xMin", model.xMin) + base.SerializeMember<float>(serialized, null, "yMin", model.yMin) + base.SerializeMember<float>(serialized, null, "xMax", model.xMax) + base.SerializeMember<float>(serialized, null, "yMax", model.yMax);
		}

		// Token: 0x0600191F RID: 6431 RVA: 0x000817F0 File Offset: 0x0007F9F0
		protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Rect model)
		{
			fsResult success = fsResult.Success;
			float xMin = model.xMin;
			fsResult fsResult = success + base.DeserializeMember<float>(data, null, "xMin", out xMin);
			model.xMin = xMin;
			float yMin = model.yMin;
			fsResult fsResult2 = fsResult + base.DeserializeMember<float>(data, null, "yMin", out yMin);
			model.yMin = yMin;
			float xMax = model.xMax;
			fsResult fsResult3 = fsResult2 + base.DeserializeMember<float>(data, null, "xMax", out xMax);
			model.xMax = xMax;
			float yMax = model.yMax;
			fsResult fsResult4 = fsResult3 + base.DeserializeMember<float>(data, null, "yMax", out yMax);
			model.yMax = yMax;
			return fsResult4;
		}

		// Token: 0x06001920 RID: 6432 RVA: 0x0008188C File Offset: 0x0007FA8C
		public override object CreateInstance(fsData data, Type storageType)
		{
			return default(Rect);
		}
	}
}

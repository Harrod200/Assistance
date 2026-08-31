using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullSerializer.Internal.DirectConverters
{
	// Token: 0x02000494 RID: 1172
	public class RectOffset_DirectConverter : fsDirectConverter<RectOffset>
	{
		// Token: 0x0600191A RID: 6426 RVA: 0x00081650 File Offset: 0x0007F850
		protected override fsResult DoSerialize(RectOffset model, Dictionary<string, fsData> serialized)
		{
			return fsResult.Success + base.SerializeMember<int>(serialized, null, "bottom", model.bottom) + base.SerializeMember<int>(serialized, null, "left", model.left) + base.SerializeMember<int>(serialized, null, "right", model.right) + base.SerializeMember<int>(serialized, null, "top", model.top);
		}

		// Token: 0x0600191B RID: 6427 RVA: 0x000816C4 File Offset: 0x0007F8C4
		protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref RectOffset model)
		{
			fsResult success = fsResult.Success;
			int bottom = model.bottom;
			fsResult fsResult = success + base.DeserializeMember<int>(data, null, "bottom", out bottom);
			model.bottom = bottom;
			int left = model.left;
			fsResult fsResult2 = fsResult + base.DeserializeMember<int>(data, null, "left", out left);
			model.left = left;
			int right = model.right;
			fsResult fsResult3 = fsResult2 + base.DeserializeMember<int>(data, null, "right", out right);
			model.right = right;
			int top = model.top;
			fsResult fsResult4 = fsResult3 + base.DeserializeMember<int>(data, null, "top", out top);
			model.top = top;
			return fsResult4;
		}

		// Token: 0x0600191C RID: 6428 RVA: 0x00081766 File Offset: 0x0007F966
		public override object CreateInstance(fsData data, Type storageType)
		{
			return new RectOffset();
		}
	}
}

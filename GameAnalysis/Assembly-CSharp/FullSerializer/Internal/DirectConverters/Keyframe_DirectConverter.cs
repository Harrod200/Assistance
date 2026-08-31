using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullSerializer.Internal.DirectConverters
{
	// Token: 0x02000492 RID: 1170
	public class Keyframe_DirectConverter : fsDirectConverter<Keyframe>
	{
		// Token: 0x06001912 RID: 6418 RVA: 0x00081464 File Offset: 0x0007F664
		protected override fsResult DoSerialize(Keyframe model, Dictionary<string, fsData> serialized)
		{
			return fsResult.Success + base.SerializeMember<float>(serialized, null, "time", model.time) + base.SerializeMember<float>(serialized, null, "value", model.value) + base.SerializeMember<int>(serialized, null, "tangentMode", model.tangentMode) + base.SerializeMember<float>(serialized, null, "inTangent", model.inTangent) + base.SerializeMember<float>(serialized, null, "outTangent", model.outTangent);
		}

		// Token: 0x06001913 RID: 6419 RVA: 0x000814F4 File Offset: 0x0007F6F4
		protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Keyframe model)
		{
			fsResult success = fsResult.Success;
			float time = model.time;
			fsResult fsResult = success + base.DeserializeMember<float>(data, null, "time", out time);
			model.time = time;
			float value = model.value;
			fsResult fsResult2 = fsResult + base.DeserializeMember<float>(data, null, "value", out value);
			model.value = value;
			int tangentMode = model.tangentMode;
			fsResult fsResult3 = fsResult2 + base.DeserializeMember<int>(data, null, "tangentMode", out tangentMode);
			model.tangentMode = tangentMode;
			float inTangent = model.inTangent;
			fsResult fsResult4 = fsResult3 + base.DeserializeMember<float>(data, null, "inTangent", out inTangent);
			model.inTangent = inTangent;
			float outTangent = model.outTangent;
			fsResult fsResult5 = fsResult4 + base.DeserializeMember<float>(data, null, "outTangent", out outTangent);
			model.outTangent = outTangent;
			return fsResult5;
		}

		// Token: 0x06001914 RID: 6420 RVA: 0x000815B4 File Offset: 0x0007F7B4
		public override object CreateInstance(fsData data, Type storageType)
		{
			return default(Keyframe);
		}
	}
}

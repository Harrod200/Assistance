using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullSerializer.Internal.DirectConverters
{
	// Token: 0x0200048D RID: 1165
	public class AnimationCurve_DirectConverter : fsDirectConverter<AnimationCurve>
	{
		// Token: 0x060018FE RID: 6398 RVA: 0x00080B04 File Offset: 0x0007ED04
		protected override fsResult DoSerialize(AnimationCurve model, Dictionary<string, fsData> serialized)
		{
			return fsResult.Success + base.SerializeMember<Keyframe[]>(serialized, null, "keys", model.keys) + base.SerializeMember<WrapMode>(serialized, null, "preWrapMode", model.preWrapMode) + base.SerializeMember<WrapMode>(serialized, null, "postWrapMode", model.postWrapMode);
		}

		// Token: 0x060018FF RID: 6399 RVA: 0x00080B60 File Offset: 0x0007ED60
		protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref AnimationCurve model)
		{
			fsResult success = fsResult.Success;
			Keyframe[] keys = model.keys;
			fsResult fsResult = success + base.DeserializeMember<Keyframe[]>(data, null, "keys", out keys);
			model.keys = keys;
			WrapMode preWrapMode = model.preWrapMode;
			fsResult fsResult2 = fsResult + base.DeserializeMember<WrapMode>(data, null, "preWrapMode", out preWrapMode);
			model.preWrapMode = preWrapMode;
			WrapMode postWrapMode = model.postWrapMode;
			fsResult fsResult3 = fsResult2 + base.DeserializeMember<WrapMode>(data, null, "postWrapMode", out postWrapMode);
			model.postWrapMode = postWrapMode;
			return fsResult3;
		}

		// Token: 0x06001900 RID: 6400 RVA: 0x00080BDE File Offset: 0x0007EDDE
		public override object CreateInstance(fsData data, Type storageType)
		{
			return new AnimationCurve();
		}
	}
}

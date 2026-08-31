using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullSerializer.Internal.DirectConverters
{
	// Token: 0x02000491 RID: 1169
	public class Gradient_DirectConverter : fsDirectConverter<Gradient>
	{
		// Token: 0x0600190E RID: 6414 RVA: 0x000813C1 File Offset: 0x0007F5C1
		protected override fsResult DoSerialize(Gradient model, Dictionary<string, fsData> serialized)
		{
			return fsResult.Success + base.SerializeMember<GradientAlphaKey[]>(serialized, null, "alphaKeys", model.alphaKeys) + base.SerializeMember<GradientColorKey[]>(serialized, null, "colorKeys", model.colorKeys);
		}

		// Token: 0x0600190F RID: 6415 RVA: 0x000813F8 File Offset: 0x0007F5F8
		protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Gradient model)
		{
			fsResult success = fsResult.Success;
			GradientAlphaKey[] alphaKeys = model.alphaKeys;
			fsResult fsResult = success + base.DeserializeMember<GradientAlphaKey[]>(data, null, "alphaKeys", out alphaKeys);
			model.alphaKeys = alphaKeys;
			GradientColorKey[] colorKeys = model.colorKeys;
			fsResult fsResult2 = fsResult + base.DeserializeMember<GradientColorKey[]>(data, null, "colorKeys", out colorKeys);
			model.colorKeys = colorKeys;
			return fsResult2;
		}

		// Token: 0x06001910 RID: 6416 RVA: 0x00081452 File Offset: 0x0007F652
		public override object CreateInstance(fsData data, Type storageType)
		{
			return new Gradient();
		}
	}
}

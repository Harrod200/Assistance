using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullSerializer.Internal.DirectConverters
{
	// Token: 0x0200048F RID: 1167
	public class GUIStyleState_DirectConverter : fsDirectConverter<GUIStyleState>
	{
		// Token: 0x06001906 RID: 6406 RVA: 0x00080CA3 File Offset: 0x0007EEA3
		protected override fsResult DoSerialize(GUIStyleState model, Dictionary<string, fsData> serialized)
		{
			return fsResult.Success + base.SerializeMember<Texture2D>(serialized, null, "background", model.background) + base.SerializeMember<Color>(serialized, null, "textColor", model.textColor);
		}

		// Token: 0x06001907 RID: 6407 RVA: 0x00080CDC File Offset: 0x0007EEDC
		protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref GUIStyleState model)
		{
			fsResult success = fsResult.Success;
			Texture2D background = model.background;
			fsResult fsResult = success + base.DeserializeMember<Texture2D>(data, null, "background", out background);
			model.background = background;
			Color textColor = model.textColor;
			fsResult fsResult2 = fsResult + base.DeserializeMember<Color>(data, null, "textColor", out textColor);
			model.textColor = textColor;
			return fsResult2;
		}

		// Token: 0x06001908 RID: 6408 RVA: 0x00080D36 File Offset: 0x0007EF36
		public override object CreateInstance(fsData data, Type storageType)
		{
			return new GUIStyleState();
		}
	}
}

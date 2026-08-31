using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullSerializer.Internal.DirectConverters
{
	// Token: 0x02000493 RID: 1171
	public class LayerMask_DirectConverter : fsDirectConverter<LayerMask>
	{
		// Token: 0x06001916 RID: 6422 RVA: 0x000815D7 File Offset: 0x0007F7D7
		protected override fsResult DoSerialize(LayerMask model, Dictionary<string, fsData> serialized)
		{
			return fsResult.Success + base.SerializeMember<int>(serialized, null, "value", model.value);
		}

		// Token: 0x06001917 RID: 6423 RVA: 0x000815F8 File Offset: 0x0007F7F8
		protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref LayerMask model)
		{
			fsResult success = fsResult.Success;
			int value = model.value;
			fsResult fsResult = success + base.DeserializeMember<int>(data, null, "value", out value);
			model.value = value;
			return fsResult;
		}

		// Token: 0x06001918 RID: 6424 RVA: 0x0008162C File Offset: 0x0007F82C
		public override object CreateInstance(fsData data, Type storageType)
		{
			return default(LayerMask);
		}
	}
}

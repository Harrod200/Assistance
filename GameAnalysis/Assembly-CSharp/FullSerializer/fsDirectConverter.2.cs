using System;
using System.Collections.Generic;

namespace FullSerializer
{
	// Token: 0x02000467 RID: 1127
	public abstract class fsDirectConverter<TModel> : fsDirectConverter
	{
		// Token: 0x17000357 RID: 855
		// (get) Token: 0x060017D7 RID: 6103 RVA: 0x0007B953 File Offset: 0x00079B53
		public override Type ModelType
		{
			get
			{
				return typeof(TModel);
			}
		}

		// Token: 0x060017D8 RID: 6104 RVA: 0x0007B960 File Offset: 0x00079B60
		public sealed override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			Dictionary<string, fsData> dictionary = new Dictionary<string, fsData>();
			fsResult fsResult = this.DoSerialize((TModel)((object)instance), dictionary);
			serialized = new fsData(dictionary);
			return fsResult;
		}

		// Token: 0x060017D9 RID: 6105 RVA: 0x0007B988 File Offset: 0x00079B88
		public sealed override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			fsResult fsResult = fsResult.Success;
			fsResult fsResult2;
			fsResult = (fsResult2 = fsResult + base.CheckType(data, fsDataType.Object));
			if (fsResult2.Failed)
			{
				return fsResult;
			}
			TModel tmodel = (TModel)((object)instance);
			fsResult += this.DoDeserialize(data.AsDictionary, ref tmodel);
			instance = tmodel;
			return fsResult;
		}

		// Token: 0x060017DA RID: 6106
		protected abstract fsResult DoSerialize(TModel model, Dictionary<string, fsData> serialized);

		// Token: 0x060017DB RID: 6107
		protected abstract fsResult DoDeserialize(Dictionary<string, fsData> data, ref TModel model);
	}
}

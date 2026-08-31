using System;
using System.Collections.Generic;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using Com.ForbiddenByte.OSA.DataHelpers;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000821 RID: 2081
	public class IntelScreenSpacebodyListAdapter : OSA<BaseParamsWithPrefab, IntelScreenSpacebodyListItemViewsHolder>
	{
		// Token: 0x17000E9D RID: 3741
		// (get) Token: 0x06004ADE RID: 19166 RVA: 0x001F4D40 File Offset: 0x001F2F40
		// (set) Token: 0x06004ADF RID: 19167 RVA: 0x001F4D48 File Offset: 0x001F2F48
		public SimpleDataHelper<IntelScreenSpacebodyListItemModel> Data { get; private set; }

		// Token: 0x06004AE0 RID: 19168 RVA: 0x001F4D51 File Offset: 0x001F2F51
		protected override void Start()
		{
			this.Data = new SimpleDataHelper<IntelScreenSpacebodyListItemModel>(this, true);
			base.Start();
		}

		// Token: 0x06004AE1 RID: 19169 RVA: 0x001F4D66 File Offset: 0x001F2F66
		protected override IntelScreenSpacebodyListItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			IntelScreenSpacebodyListItemViewsHolder intelScreenSpacebodyListItemViewsHolder = new IntelScreenSpacebodyListItemViewsHolder();
			intelScreenSpacebodyListItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return intelScreenSpacebodyListItemViewsHolder;
		}

		// Token: 0x06004AE2 RID: 19170 RVA: 0x001F4D8C File Offset: 0x001F2F8C
		protected override void UpdateViewsHolder(IntelScreenSpacebodyListItemViewsHolder newOrRecycled)
		{
			IntelScreenSpacebodyListItemModel intelScreenSpacebodyListItemModel = this.Data[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateFromModel(intelScreenSpacebodyListItemModel, this._Params);
		}

		// Token: 0x06004AE3 RID: 19171 RVA: 0x001F4DB8 File Offset: 0x001F2FB8
		public void SetItems(IList<IntelScreenSpacebodyListItemModel> items)
		{
			IList<IntelScreenSpacebodyListItemModel> list = new List<IntelScreenSpacebodyListItemModel>();
			foreach (IntelScreenSpacebodyListItemModel intelScreenSpacebodyListItemModel in items)
			{
				if (intelScreenSpacebodyListItemModel.IntelScreenSpacebodyListItemData.showInList)
				{
					list.Add(intelScreenSpacebodyListItemModel);
				}
			}
			this.Data.ResetItems(list, false);
		}
	}
}

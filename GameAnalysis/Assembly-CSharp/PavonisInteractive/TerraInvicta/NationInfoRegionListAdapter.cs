using System;
using System.Collections.Generic;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using Com.ForbiddenByte.OSA.DataHelpers;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000824 RID: 2084
	public class NationInfoRegionListAdapter : OSA<BaseParamsWithPrefab, NationInfoRegionListItemViewsHolder>
	{
		// Token: 0x17000E9E RID: 3742
		// (get) Token: 0x06004AE9 RID: 19177 RVA: 0x001F4E79 File Offset: 0x001F3079
		// (set) Token: 0x06004AEA RID: 19178 RVA: 0x001F4E81 File Offset: 0x001F3081
		public SimpleDataHelper<NationInfoRegionListItemModel> Data { get; private set; }

		// Token: 0x06004AEB RID: 19179 RVA: 0x001F4E8A File Offset: 0x001F308A
		protected override void Start()
		{
			this.Data = new SimpleDataHelper<NationInfoRegionListItemModel>(this, true);
			base.Start();
		}

		// Token: 0x06004AEC RID: 19180 RVA: 0x001F4E9F File Offset: 0x001F309F
		protected override NationInfoRegionListItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			NationInfoRegionListItemViewsHolder nationInfoRegionListItemViewsHolder = new NationInfoRegionListItemViewsHolder();
			nationInfoRegionListItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return nationInfoRegionListItemViewsHolder;
		}

		// Token: 0x06004AED RID: 19181 RVA: 0x001F4EC8 File Offset: 0x001F30C8
		protected override void UpdateViewsHolder(NationInfoRegionListItemViewsHolder newOrRecycled)
		{
			NationInfoRegionListItemModel nationInfoRegionListItemModel = this.Data[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateFromModel(nationInfoRegionListItemModel, this._Params);
		}

		// Token: 0x06004AEE RID: 19182 RVA: 0x001F4EF4 File Offset: 0x001F30F4
		public void SetItems(IList<NationInfoRegionListItemModel> items)
		{
			IList<NationInfoRegionListItemModel> list = new List<NationInfoRegionListItemModel>();
			foreach (NationInfoRegionListItemModel nationInfoRegionListItemModel in items)
			{
				if (nationInfoRegionListItemModel.regionListItemData.showInList)
				{
					list.Add(nationInfoRegionListItemModel);
				}
			}
			this.Data.ResetItems(list, false);
		}
	}
}

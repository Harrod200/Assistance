using System;
using System.Collections.Generic;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using Com.ForbiddenByte.OSA.DataHelpers;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008E5 RID: 2277
	public class NationScreenNationListAdapter : OSA<BaseParamsWithPrefab, NationScreenNationListItemViewsHolder>
	{
		// Token: 0x17000F1E RID: 3870
		// (get) Token: 0x060057A5 RID: 22437 RVA: 0x00284A0B File Offset: 0x00282C0B
		// (set) Token: 0x060057A6 RID: 22438 RVA: 0x00284A13 File Offset: 0x00282C13
		public SimpleDataHelper<NationScreenNationListItemModel> Data { get; private set; }

		// Token: 0x060057A7 RID: 22439 RVA: 0x00284A1C File Offset: 0x00282C1C
		protected override void Start()
		{
			this.Data = new SimpleDataHelper<NationScreenNationListItemModel>(this, true);
			base.Start();
		}

		// Token: 0x060057A8 RID: 22440 RVA: 0x00284A31 File Offset: 0x00282C31
		protected override NationScreenNationListItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			NationScreenNationListItemViewsHolder nationScreenNationListItemViewsHolder = new NationScreenNationListItemViewsHolder();
			nationScreenNationListItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return nationScreenNationListItemViewsHolder;
		}

		// Token: 0x060057A9 RID: 22441 RVA: 0x00284A58 File Offset: 0x00282C58
		protected override void UpdateViewsHolder(NationScreenNationListItemViewsHolder newOrRecycled)
		{
			NationScreenNationListItemModel nationScreenNationListItemModel = this.Data[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateFromModel(nationScreenNationListItemModel, this._Params);
		}

		// Token: 0x060057AA RID: 22442 RVA: 0x00284A84 File Offset: 0x00282C84
		public void SetItems(IList<NationScreenNationListItemModel> items)
		{
			IList<NationScreenNationListItemModel> list = new List<NationScreenNationListItemModel>();
			foreach (NationScreenNationListItemModel nationScreenNationListItemModel in items)
			{
				if (nationScreenNationListItemModel.NationScreenNationListItemData.showInList)
				{
					list.Add(nationScreenNationListItemModel);
				}
			}
			this.Data.ResetItems(list, false);
		}
	}
}

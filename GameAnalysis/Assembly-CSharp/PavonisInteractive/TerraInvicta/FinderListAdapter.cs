using System;
using System.Collections.Generic;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using Com.ForbiddenByte.OSA.DataHelpers;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000812 RID: 2066
	public class FinderListAdapter : OSA<BaseParamsWithPrefab, FinderListItemViewsHolder>
	{
		// Token: 0x17000E97 RID: 3735
		// (get) Token: 0x06004A9E RID: 19102 RVA: 0x001F45C4 File Offset: 0x001F27C4
		// (set) Token: 0x06004A9F RID: 19103 RVA: 0x001F45CC File Offset: 0x001F27CC
		public SimpleDataHelper<FinderListItemModel> Data { get; private set; }

		// Token: 0x06004AA0 RID: 19104 RVA: 0x001F45D5 File Offset: 0x001F27D5
		protected override void Start()
		{
			this.Data = new SimpleDataHelper<FinderListItemModel>(this, true);
			base.Start();
		}

		// Token: 0x06004AA1 RID: 19105 RVA: 0x001F45EA File Offset: 0x001F27EA
		protected override FinderListItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			FinderListItemViewsHolder finderListItemViewsHolder = new FinderListItemViewsHolder();
			finderListItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return finderListItemViewsHolder;
		}

		// Token: 0x06004AA2 RID: 19106 RVA: 0x001F4610 File Offset: 0x001F2810
		protected override void UpdateViewsHolder(FinderListItemViewsHolder newOrRecycled)
		{
			FinderListItemModel finderListItemModel = this.Data[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateFromModel(finderListItemModel, this._Params);
		}

		// Token: 0x06004AA3 RID: 19107 RVA: 0x001F463C File Offset: 0x001F283C
		public void SetItems(IList<FinderListItemModel> items)
		{
			IList<FinderListItemModel> list = new List<FinderListItemModel>();
			foreach (FinderListItemModel finderListItemModel in items)
			{
				if (finderListItemModel.finderListItemData.showInList)
				{
					list.Add(finderListItemModel);
				}
			}
			this.Data.ResetItems(list, false);
		}
	}
}

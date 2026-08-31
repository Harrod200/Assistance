using System;
using System.Collections.Generic;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using Com.ForbiddenByte.OSA.DataHelpers;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000818 RID: 2072
	public class GlobalSearchListAdapter : OSA<BaseParamsWithPrefab, GlobalSearchListItemViewsHolder>
	{
		// Token: 0x17000E9A RID: 3738
		// (get) Token: 0x06004ABD RID: 19133 RVA: 0x001F49FE File Offset: 0x001F2BFE
		// (set) Token: 0x06004ABE RID: 19134 RVA: 0x001F4A06 File Offset: 0x001F2C06
		public SimpleDataHelper<GlobalSearchListItemModel> Data { get; private set; }

		// Token: 0x06004ABF RID: 19135 RVA: 0x001F4A0F File Offset: 0x001F2C0F
		protected override void Start()
		{
			this.Data = new SimpleDataHelper<GlobalSearchListItemModel>(this, true);
			base.Start();
		}

		// Token: 0x06004AC0 RID: 19136 RVA: 0x001F4A24 File Offset: 0x001F2C24
		protected override GlobalSearchListItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			GlobalSearchListItemViewsHolder globalSearchListItemViewsHolder = new GlobalSearchListItemViewsHolder();
			globalSearchListItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return globalSearchListItemViewsHolder;
		}

		// Token: 0x06004AC1 RID: 19137 RVA: 0x001F4A4C File Offset: 0x001F2C4C
		protected override void UpdateViewsHolder(GlobalSearchListItemViewsHolder newOrRecycled)
		{
			GlobalSearchListItemModel globalSearchListItemModel = this.Data[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateFromModel(globalSearchListItemModel, this._Params);
		}

		// Token: 0x06004AC2 RID: 19138 RVA: 0x001F4A78 File Offset: 0x001F2C78
		public void SetItems(IList<GlobalSearchListItemModel> items)
		{
			this.Data.ResetItems(items, false);
		}
	}
}

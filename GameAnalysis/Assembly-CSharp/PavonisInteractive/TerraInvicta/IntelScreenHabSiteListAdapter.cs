using System;
using System.Collections.Generic;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using Com.ForbiddenByte.OSA.DataHelpers;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200081E RID: 2078
	public class IntelScreenHabSiteListAdapter : OSA<BaseParamsWithPrefab, IntelScreenHabSiteListItemViewsHolder>
	{
		// Token: 0x17000E9C RID: 3740
		// (get) Token: 0x06004AD3 RID: 19155 RVA: 0x001F4C0B File Offset: 0x001F2E0B
		// (set) Token: 0x06004AD4 RID: 19156 RVA: 0x001F4C13 File Offset: 0x001F2E13
		public SimpleDataHelper<IntelScreenHabSiteListItemModel> Data { get; private set; }

		// Token: 0x06004AD5 RID: 19157 RVA: 0x001F4C1C File Offset: 0x001F2E1C
		protected override void Start()
		{
			this.Data = new SimpleDataHelper<IntelScreenHabSiteListItemModel>(this, true);
			base.Start();
		}

		// Token: 0x06004AD6 RID: 19158 RVA: 0x001F4C31 File Offset: 0x001F2E31
		protected override IntelScreenHabSiteListItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			IntelScreenHabSiteListItemViewsHolder intelScreenHabSiteListItemViewsHolder = new IntelScreenHabSiteListItemViewsHolder();
			intelScreenHabSiteListItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return intelScreenHabSiteListItemViewsHolder;
		}

		// Token: 0x06004AD7 RID: 19159 RVA: 0x001F4C58 File Offset: 0x001F2E58
		protected override void UpdateViewsHolder(IntelScreenHabSiteListItemViewsHolder newOrRecycled)
		{
			IntelScreenHabSiteListItemModel intelScreenHabSiteListItemModel = this.Data[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateFromModel(intelScreenHabSiteListItemModel, this._Params);
		}

		// Token: 0x06004AD8 RID: 19160 RVA: 0x001F4C84 File Offset: 0x001F2E84
		public void SetItems(IList<IntelScreenHabSiteListItemModel> items)
		{
			IList<IntelScreenHabSiteListItemModel> list = new List<IntelScreenHabSiteListItemModel>();
			foreach (IntelScreenHabSiteListItemModel intelScreenHabSiteListItemModel in items)
			{
				if (intelScreenHabSiteListItemModel.IntelScreenHabSiteListItemData.showInList)
				{
					list.Add(intelScreenHabSiteListItemModel);
				}
			}
			this.Data.ResetItems(list, false);
		}
	}
}

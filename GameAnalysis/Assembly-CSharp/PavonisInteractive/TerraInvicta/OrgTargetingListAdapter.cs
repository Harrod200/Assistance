using System;
using System.Collections.Generic;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using Com.ForbiddenByte.OSA.DataHelpers;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008E8 RID: 2280
	public class OrgTargetingListAdapter : OSA<BaseParamsWithPrefab, OrgTargetingListItemViewsHolder>
	{
		// Token: 0x17000F1F RID: 3871
		// (get) Token: 0x060057B0 RID: 22448 RVA: 0x00284B30 File Offset: 0x00282D30
		// (set) Token: 0x060057B1 RID: 22449 RVA: 0x00284B38 File Offset: 0x00282D38
		public SimpleDataHelper<OrgTargetingListItemModel> Data { get; private set; }

		// Token: 0x060057B2 RID: 22450 RVA: 0x00284B41 File Offset: 0x00282D41
		protected override void Start()
		{
			this.Data = new SimpleDataHelper<OrgTargetingListItemModel>(this, true);
			base.Start();
		}

		// Token: 0x060057B3 RID: 22451 RVA: 0x00284B56 File Offset: 0x00282D56
		protected override OrgTargetingListItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			OrgTargetingListItemViewsHolder orgTargetingListItemViewsHolder = new OrgTargetingListItemViewsHolder();
			orgTargetingListItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return orgTargetingListItemViewsHolder;
		}

		// Token: 0x060057B4 RID: 22452 RVA: 0x00284B7C File Offset: 0x00282D7C
		protected override void UpdateViewsHolder(OrgTargetingListItemViewsHolder newOrRecycled)
		{
			OrgTargetingListItemModel orgTargetingListItemModel = this.Data[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateFromModel(orgTargetingListItemModel, this._Params);
		}

		// Token: 0x060057B5 RID: 22453 RVA: 0x00284BA8 File Offset: 0x00282DA8
		public void SetItems(IList<OrgTargetingListItemModel> items)
		{
			IList<OrgTargetingListItemModel> list = new List<OrgTargetingListItemModel>();
			foreach (OrgTargetingListItemModel orgTargetingListItemModel in items)
			{
				if (orgTargetingListItemModel.targetOrgListItemData.showInList)
				{
					list.Add(orgTargetingListItemModel);
				}
			}
			this.Data.ResetItems(items, false);
		}
	}
}

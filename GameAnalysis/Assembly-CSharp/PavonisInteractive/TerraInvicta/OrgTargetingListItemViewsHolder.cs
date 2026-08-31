using System;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008EA RID: 2282
	public class OrgTargetingListItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x060057B8 RID: 22456 RVA: 0x00284C20 File Offset: 0x00282E20
		public override void CollectViews()
		{
			base.CollectViews();
			this.targetOrgListItemController = this.root.GetComponent<TargetOrgListItemController>();
		}

		// Token: 0x060057B9 RID: 22457 RVA: 0x00284C39 File Offset: 0x00282E39
		public void UpdateFromModel(OrgTargetingListItemModel model, BaseParamsWithPrefab parameters)
		{
			this.targetOrgListItemController.SetListItem(model.targetOrgListItemData);
		}

		// Token: 0x04003F54 RID: 16212
		public TargetOrgListItemController targetOrgListItemController;
	}
}

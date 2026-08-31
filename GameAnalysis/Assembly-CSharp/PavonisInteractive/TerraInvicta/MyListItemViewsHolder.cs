using System;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008DF RID: 2271
	public class MyListItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x0600578F RID: 22415 RVA: 0x00284475 File Offset: 0x00282675
		public override void CollectViews()
		{
			base.CollectViews();
			this.targetListItem = this.root.GetComponent<TargetOrgListItemController>();
		}

		// Token: 0x06005790 RID: 22416 RVA: 0x0028448E File Offset: 0x0028268E
		public void UpdateFromModel(MyListItemModel model, BaseParamsWithPrefab parameters)
		{
		}

		// Token: 0x04003F45 RID: 16197
		public TargetOrgListItemController targetListItem;
	}
}

using System;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008E7 RID: 2279
	public class NationScreenNationListItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x060057AD RID: 22445 RVA: 0x00284AFC File Offset: 0x00282CFC
		public override void CollectViews()
		{
			base.CollectViews();
			this.nationScreenNationListItem = this.root.GetComponent<NationsScreenNationListItemController>();
		}

		// Token: 0x060057AE RID: 22446 RVA: 0x00284B15 File Offset: 0x00282D15
		public void UpdateFromModel(NationScreenNationListItemModel model, BaseParamsWithPrefab parameters)
		{
			this.nationScreenNationListItem.UpdateNationItem(model.NationScreenNationListItemData);
		}

		// Token: 0x04003F51 RID: 16209
		public NationsScreenNationListItemController nationScreenNationListItem;
	}
}

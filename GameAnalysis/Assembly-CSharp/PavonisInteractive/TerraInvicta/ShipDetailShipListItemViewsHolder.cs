using System;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200082C RID: 2092
	public class ShipDetailShipListItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x06004B07 RID: 19207 RVA: 0x001F515B File Offset: 0x001F335B
		public override void CollectViews()
		{
			base.CollectViews();
			this.ShipDetailShipListItem = this.root.GetComponent<ShipScreenShipListItemController>();
		}

		// Token: 0x06004B08 RID: 19208 RVA: 0x001F5174 File Offset: 0x001F3374
		public void UpdateFromModel(ShipDetailShipListItemModel model, BaseParamsWithPrefab parameters)
		{
			this.ShipDetailShipListItem.SetListItem(model.ShipDetailShipListItemData.shipState, model.ShipDetailShipListItemData.controller);
		}

		// Token: 0x04002BAE RID: 11182
		public ShipScreenShipListItemController ShipDetailShipListItem;
	}
}

using System;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000814 RID: 2068
	public class FinderListItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x06004AA6 RID: 19110 RVA: 0x001F46B4 File Offset: 0x001F28B4
		public override void CollectViews()
		{
			base.CollectViews();
			this.FinderListItemController = this.root.GetComponent<FinderListItemController>();
		}

		// Token: 0x06004AA7 RID: 19111 RVA: 0x001F46CD File Offset: 0x001F28CD
		public void UpdateFromModel(FinderListItemModel model, BaseParamsWithPrefab parameters)
		{
			if (GameControl.gameStartedUnloading)
			{
				return;
			}
			this.FinderListItemController.Initialize(model.finderListItemData.gameState, model.finderListItemData.controller);
			this.FinderListItemController.UpdateListItem(model.finderListItemData);
		}

		// Token: 0x04002B91 RID: 11153
		public FinderListItemController FinderListItemController;
	}
}

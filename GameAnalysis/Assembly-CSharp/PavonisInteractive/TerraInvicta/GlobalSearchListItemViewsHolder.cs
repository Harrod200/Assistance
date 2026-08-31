using System;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200081A RID: 2074
	public class GlobalSearchListItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x06004AC5 RID: 19141 RVA: 0x001F4A97 File Offset: 0x001F2C97
		public override void CollectViews()
		{
			base.CollectViews();
			this.globalSearchListItemController = this.root.GetComponent<GlobalSearchListItemController>();
		}

		// Token: 0x06004AC6 RID: 19142 RVA: 0x001F4AB0 File Offset: 0x001F2CB0
		public void UpdateFromModel(GlobalSearchListItemModel model, BaseParamsWithPrefab parameters)
		{
			this.globalSearchListItemController.UpdateListItem(model.globalSearchListItemData);
		}

		// Token: 0x04002B9C RID: 11164
		public GlobalSearchListItemController globalSearchListItemController;
	}
}

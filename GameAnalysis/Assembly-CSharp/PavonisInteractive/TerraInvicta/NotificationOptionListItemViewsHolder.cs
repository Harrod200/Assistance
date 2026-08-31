using System;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000829 RID: 2089
	public class NotificationOptionListItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x06004AFC RID: 19196 RVA: 0x001F5090 File Offset: 0x001F3290
		public override void CollectViews()
		{
			base.CollectViews();
			this.notificationOptionListItem = this.root.GetComponent<NotificationOptionListItem>();
		}

		// Token: 0x06004AFD RID: 19197 RVA: 0x001F50A9 File Offset: 0x001F32A9
		public void UpdateFromModel(NotificationOptionListItemModel model, BaseParamsWithPrefab parameters)
		{
			this.notificationOptionListItem.UpdateListItem(model.notificationOptionListItemData);
		}

		// Token: 0x04002BAB RID: 11179
		public NotificationOptionListItem notificationOptionListItem;
	}
}

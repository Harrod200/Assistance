using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000901 RID: 2305
	public class NotificationOptionListItem_Data
	{
		// Token: 0x0600583D RID: 22589 RVA: 0x002878D7 File Offset: 0x00285AD7
		public void SetNotificationOptionData(TINotificationTemplate template, NotificationsOptionsController controller, TINotificationTemplateOverride notificationOverride = null)
		{
			this.notificationTemplate = template;
			this.notificationTemplateOverride = notificationOverride;
			this.controller = controller;
			this.categoryHeader = Loc.T("");
		}

		// Token: 0x04003FD5 RID: 16341
		public bool showInList;

		// Token: 0x04003FD6 RID: 16342
		public bool isCollapsibleHeader;

		// Token: 0x04003FD7 RID: 16343
		public string categoryHeader;

		// Token: 0x04003FD8 RID: 16344
		public TINotificationTemplate notificationTemplate;

		// Token: 0x04003FD9 RID: 16345
		public TINotificationTemplateOverride notificationTemplateOverride;

		// Token: 0x04003FDA RID: 16346
		public NotificationsOptionsController controller;
	}
}

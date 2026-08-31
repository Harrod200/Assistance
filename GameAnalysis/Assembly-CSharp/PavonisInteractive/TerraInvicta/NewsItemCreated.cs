using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200062E RID: 1582
	public class NewsItemCreated : GameEvent
	{
		// Token: 0x06002853 RID: 10323 RVA: 0x000DA1EA File Offset: 0x000D83EA
		public NewsItemCreated(NotificationQueueItem newItem, NotificationSummaryItem newSummary)
		{
			this.newItem = newItem;
			this.newSummary = newSummary;
		}

		// Token: 0x04001E6C RID: 7788
		public NotificationQueueItem newItem;

		// Token: 0x04001E6D RID: 7789
		public NotificationSummaryItem newSummary;
	}
}

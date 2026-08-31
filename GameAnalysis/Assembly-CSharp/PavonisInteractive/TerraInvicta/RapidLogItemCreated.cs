using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200062F RID: 1583
	public class RapidLogItemCreated : GameEvent
	{
		// Token: 0x06002854 RID: 10324 RVA: 0x000DA200 File Offset: 0x000D8400
		public RapidLogItemCreated(NotificationSummaryItem newSummary)
		{
			this.newSummary = newSummary;
		}

		// Token: 0x04001E6E RID: 7790
		public NotificationSummaryItem newSummary;
	}
}

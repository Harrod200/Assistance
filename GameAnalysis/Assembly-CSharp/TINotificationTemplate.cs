using System;
using System.Collections.Generic;

// Token: 0x020002D5 RID: 725
public class TINotificationTemplate : TIDataTemplate
{
	// Token: 0x04000D92 RID: 3474
	public string alertHammerLoc;

	// Token: 0x04000D93 RID: 3475
	public bool allowAnyChanges;

	// Token: 0x04000D94 RID: 3476
	public bool allowAlertChanges;

	// Token: 0x04000D95 RID: 3477
	public NotificationAudience alertAudience;

	// Token: 0x04000D96 RID: 3478
	public bool firstAlertOverride;

	// Token: 0x04000D97 RID: 3479
	public NotificationAudience newsFeedAudience;

	// Token: 0x04000D98 RID: 3480
	public NotificationAudience timerAudience;

	// Token: 0x04000D99 RID: 3481
	public SummaryHandling summaryAudience;

	// Token: 0x04000D9A RID: 3482
	public StackingBehavior stacking;

	// Token: 0x04000D9B RID: 3483
	public List<string> unlockingObjectives;
}

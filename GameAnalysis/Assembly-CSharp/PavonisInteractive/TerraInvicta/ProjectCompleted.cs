using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000628 RID: 1576
	public class ProjectCompleted : GameEvent
	{
		// Token: 0x0600284D RID: 10317 RVA: 0x000DA16D File Offset: 0x000D836D
		public ProjectCompleted(TIFactionState council, TIProjectTemplate completedProjectTemplate)
		{
			this.council = council;
			this.completedProjectTemplate = completedProjectTemplate;
		}

		// Token: 0x04001E61 RID: 7777
		public TIFactionState council;

		// Token: 0x04001E62 RID: 7778
		public TIProjectTemplate completedProjectTemplate;
	}
}

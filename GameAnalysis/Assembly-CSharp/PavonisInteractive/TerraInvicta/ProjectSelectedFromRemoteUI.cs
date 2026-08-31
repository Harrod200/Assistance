using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000629 RID: 1577
	public class ProjectSelectedFromRemoteUI : GameEvent
	{
		// Token: 0x0600284E RID: 10318 RVA: 0x000DA183 File Offset: 0x000D8383
		public ProjectSelectedFromRemoteUI(TIFactionState council, TIProjectTemplate newProjectTemplate)
		{
			this.council = council;
			this.newProjectTemplate = newProjectTemplate;
		}

		// Token: 0x04001E63 RID: 7779
		public TIFactionState council;

		// Token: 0x04001E64 RID: 7780
		public TIProjectTemplate newProjectTemplate;
	}
}

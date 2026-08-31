using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200062A RID: 1578
	public class ProjectUIOptionsChanged : GameEvent
	{
		// Token: 0x0600284F RID: 10319 RVA: 0x000DA199 File Offset: 0x000D8399
		public ProjectUIOptionsChanged(TIFactionState council)
		{
			this.council = council;
		}

		// Token: 0x04001E65 RID: 7781
		public TIFactionState council;
	}
}

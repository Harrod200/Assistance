using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200062B RID: 1579
	public class TechCompleted : GameEvent
	{
		// Token: 0x06002850 RID: 10320 RVA: 0x000DA1A8 File Offset: 0x000D83A8
		public TechCompleted(TIFactionState winningCouncil, TITechTemplate completedTechTemplate)
		{
			this.winningCouncil = winningCouncil;
			this.completedTechTemplate = completedTechTemplate;
		}

		// Token: 0x04001E66 RID: 7782
		public TIFactionState winningCouncil;

		// Token: 0x04001E67 RID: 7783
		public TITechTemplate completedTechTemplate;
	}
}

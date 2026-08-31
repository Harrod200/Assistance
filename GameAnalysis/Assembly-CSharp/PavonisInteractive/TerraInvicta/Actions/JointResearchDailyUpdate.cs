using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000AA8 RID: 2728
	public class JointResearchDailyUpdate : SimulationAction
	{
		// Token: 0x060065AB RID: 26027 RVA: 0x002FDA81 File Offset: 0x002FBC81
		public override void Execute()
		{
			GameStateManager.GlobalResearch().CheckForCompletedTechs();
		}

		// Token: 0x040047F9 RID: 18425
		public TIGlobalResearchState researchState;
	}
}

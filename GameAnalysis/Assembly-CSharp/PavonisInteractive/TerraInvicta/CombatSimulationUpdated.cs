using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006AE RID: 1710
	public class CombatSimulationUpdated : GameEvent
	{
		// Token: 0x060028DA RID: 10458 RVA: 0x000DABAF File Offset: 0x000D8DAF
		public CombatSimulationUpdated(SimulatedCombat simulatedCombat, float progress)
		{
			this.simulatedCombat = simulatedCombat;
			this.progress = progress;
		}

		// Token: 0x04001F17 RID: 7959
		public SimulatedCombat simulatedCombat;

		// Token: 0x04001F18 RID: 7960
		public float progress;
	}
}

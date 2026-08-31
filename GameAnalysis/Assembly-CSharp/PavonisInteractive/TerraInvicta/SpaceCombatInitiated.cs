using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006B1 RID: 1713
	public class SpaceCombatInitiated : GameEvent
	{
		// Token: 0x060028DD RID: 10461 RVA: 0x000DABDC File Offset: 0x000D8DDC
		public SpaceCombatInitiated(TISpaceCombatState combat)
		{
			this.combat = combat;
		}

		// Token: 0x04001F1A RID: 7962
		public TISpaceCombatState combat;
	}
}

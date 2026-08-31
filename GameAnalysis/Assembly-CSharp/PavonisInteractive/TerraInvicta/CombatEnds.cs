using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006AF RID: 1711
	public class CombatEnds : GameEvent
	{
		// Token: 0x060028DB RID: 10459 RVA: 0x000DABC5 File Offset: 0x000D8DC5
		public CombatEnds(TISpaceCombatState combat)
		{
			this.combat = combat;
		}

		// Token: 0x04001F19 RID: 7961
		public TISpaceCombatState combat;
	}
}

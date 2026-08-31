using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006B2 RID: 1714
	public class HabEntersCombat : GameEvent
	{
		// Token: 0x060028DE RID: 10462 RVA: 0x000DABEB File Offset: 0x000D8DEB
		public HabEntersCombat(TISpaceCombatState combat, TIHabState hab)
		{
			this.combat = combat;
			this.hab = hab;
		}

		// Token: 0x04001F1B RID: 7963
		public TISpaceCombatState combat;

		// Token: 0x04001F1C RID: 7964
		public TIHabState hab;
	}
}

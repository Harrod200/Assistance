using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006AD RID: 1709
	public class CombatStarts : GameEvent
	{
		// Token: 0x060028D9 RID: 10457 RVA: 0x000DABA0 File Offset: 0x000D8DA0
		public CombatStarts(TISpaceCombatState combat)
		{
			this.combat = combat;
		}

		// Token: 0x04001F16 RID: 7958
		public TISpaceCombatState combat;
	}
}

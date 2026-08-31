using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006AB RID: 1707
	public class PrecombatComplete : GameEvent
	{
		// Token: 0x060028D7 RID: 10455 RVA: 0x000DAB82 File Offset: 0x000D8D82
		public PrecombatComplete(TISpaceCombatState combat)
		{
			this.combat = combat;
		}

		// Token: 0x04001F14 RID: 7956
		public TISpaceCombatState combat;
	}
}

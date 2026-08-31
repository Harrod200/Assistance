using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006BF RID: 1727
	public class CombatTargetedableStateSwap : GameEvent
	{
		// Token: 0x060028EB RID: 10475 RVA: 0x000DACED File Offset: 0x000D8EED
		public CombatTargetedableStateSwap(CombatTargetableState target)
		{
			this.target = target;
		}

		// Token: 0x04001F31 RID: 7985
		public CombatTargetableState target;
	}
}

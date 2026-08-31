using System;
using PavonisInteractive.TerraInvicta.Ship;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006C3 RID: 1731
	public class HabModuleDestroyedInCombat : GameEvent
	{
		// Token: 0x060028EF RID: 10479 RVA: 0x000DAD38 File Offset: 0x000D8F38
		public HabModuleDestroyedInCombat(TIHabModuleState habModule, DamageSource damageSource)
		{
			this.habModule = habModule;
			this.damageSource = damageSource;
		}

		// Token: 0x04001F37 RID: 7991
		public TIHabModuleState habModule;

		// Token: 0x04001F38 RID: 7992
		public DamageSource damageSource;
	}
}

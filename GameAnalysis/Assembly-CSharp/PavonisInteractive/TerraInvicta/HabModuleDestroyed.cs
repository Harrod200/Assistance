using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000652 RID: 1618
	public class HabModuleDestroyed : GameEvent
	{
		// Token: 0x06002879 RID: 10361 RVA: 0x000DA500 File Offset: 0x000D8700
		public HabModuleDestroyed(TIHabModuleState habModule, bool combat)
		{
			this.habModule = habModule;
			this.combat = combat;
		}

		// Token: 0x04001EAC RID: 7852
		public TIHabModuleState habModule;

		// Token: 0x04001EAD RID: 7853
		public bool combat;
	}
}

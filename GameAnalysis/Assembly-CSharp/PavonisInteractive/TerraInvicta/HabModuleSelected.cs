using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000676 RID: 1654
	public class HabModuleSelected : GameEvent
	{
		// Token: 0x0600289E RID: 10398 RVA: 0x000DA7FA File Offset: 0x000D89FA
		public HabModuleSelected(TIHabModuleState module)
		{
			this.module = module;
		}

		// Token: 0x04001ED7 RID: 7895
		public TIHabModuleState module;
	}
}

using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200064F RID: 1615
	public class HabModuleDecommissionStatusChange : GameEvent
	{
		// Token: 0x06002876 RID: 10358 RVA: 0x000DA4D3 File Offset: 0x000D86D3
		public HabModuleDecommissionStatusChange(TIHabModuleState habModule)
		{
			this.habModule = habModule;
		}

		// Token: 0x04001EA9 RID: 7849
		public TIHabModuleState habModule;
	}
}

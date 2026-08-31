using System;
using PavonisInteractive.TerraInvicta.Systems.Bootstrap;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005B9 RID: 1465
	public class StartupComplete : GameEvent
	{
		// Token: 0x060027DE RID: 10206 RVA: 0x000D99F3 File Offset: 0x000D7BF3
		public StartupComplete(IScenario scenario)
		{
			this.scenario = scenario;
		}

		// Token: 0x04001DD4 RID: 7636
		public IScenario scenario;
	}
}

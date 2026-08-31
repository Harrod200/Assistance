using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200065C RID: 1628
	public class EarthParticulateThresholdChanges : GameEvent
	{
		// Token: 0x06002883 RID: 10371 RVA: 0x000DA5A4 File Offset: 0x000D87A4
		public EarthParticulateThresholdChanges(int particulates)
		{
			this.particulates = particulates;
		}

		// Token: 0x04001EB8 RID: 7864
		public int particulates;
	}
}

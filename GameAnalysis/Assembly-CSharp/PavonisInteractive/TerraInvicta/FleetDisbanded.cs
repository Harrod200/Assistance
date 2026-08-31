using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000655 RID: 1621
	public class FleetDisbanded : GameEvent
	{
		// Token: 0x0600287C RID: 10364 RVA: 0x000DA534 File Offset: 0x000D8734
		public FleetDisbanded(TISpaceFleetState fleet)
		{
			this.fleet = fleet;
		}

		// Token: 0x04001EB0 RID: 7856
		public TISpaceFleetState fleet;
	}
}

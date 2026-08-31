using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200064B RID: 1611
	public class SectorDataUpdated : GameEvent
	{
		// Token: 0x06002870 RID: 10352 RVA: 0x000DA47D File Offset: 0x000D867D
		public SectorDataUpdated(TISectorState sector)
		{
			this.sector = sector;
		}

		// Token: 0x04001EA5 RID: 7845
		public TISectorState sector;
	}
}

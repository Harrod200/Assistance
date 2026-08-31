using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000611 RID: 1553
	public class AlienFacilityDamaged : GameEvent
	{
		// Token: 0x06002836 RID: 10294 RVA: 0x000D9FDC File Offset: 0x000D81DC
		public AlienFacilityDamaged(TIRegionAlienFacilityState facility)
		{
			this.facility = facility;
		}

		// Token: 0x04001E42 RID: 7746
		public TIRegionAlienFacilityState facility;
	}
}

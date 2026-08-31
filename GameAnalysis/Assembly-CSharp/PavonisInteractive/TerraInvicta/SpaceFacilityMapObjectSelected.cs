using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000670 RID: 1648
	public class SpaceFacilityMapObjectSelected : GameEvent
	{
		// Token: 0x06002898 RID: 10392 RVA: 0x000DA78B File Offset: 0x000D898B
		public SpaceFacilityMapObjectSelected(TIRegionSpaceFacilityState regionSpaceFacility)
		{
			this.regionSpaceFacility = regionSpaceFacility;
		}

		// Token: 0x04001ECE RID: 7886
		public TIRegionSpaceFacilityState regionSpaceFacility;
	}
}

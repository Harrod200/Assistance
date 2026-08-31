using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000616 RID: 1558
	public class RegionXenoformingIntelUpdate : GameEvent
	{
		// Token: 0x0600283B RID: 10299 RVA: 0x000DA027 File Offset: 0x000D8227
		public RegionXenoformingIntelUpdate(TIFactionState faction, TIRegionState region)
		{
			this.faction = faction;
			this.region = region;
		}

		// Token: 0x04001E47 RID: 7751
		public TIFactionState faction;

		// Token: 0x04001E48 RID: 7752
		public TIRegionState region;
	}
}

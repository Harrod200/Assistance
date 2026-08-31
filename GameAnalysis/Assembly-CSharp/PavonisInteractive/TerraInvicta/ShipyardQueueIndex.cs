using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007AD RID: 1965
	[Obsolete]
	public struct ShipyardQueueIndex
	{
		// Token: 0x060041E3 RID: 16867 RVA: 0x001AA6B4 File Offset: 0x001A88B4
		public ShipyardQueueIndex(TISectorState sector, int slot)
		{
			this.sector = sector;
			this.slot = slot;
			sector.faction.nShipyardQueues.Add(this.habModule, new List<ShipConstructionQueueItem>());
		}

		// Token: 0x17000C3F RID: 3135
		// (get) Token: 0x060041E4 RID: 16868 RVA: 0x001AA6DF File Offset: 0x001A88DF
		public TIHabModuleState habModule
		{
			get
			{
				return this.sector.habModules[this.slot];
			}
		}

		// Token: 0x040027B6 RID: 10166
		public TISectorState sector;

		// Token: 0x040027B7 RID: 10167
		public int slot;
	}
}

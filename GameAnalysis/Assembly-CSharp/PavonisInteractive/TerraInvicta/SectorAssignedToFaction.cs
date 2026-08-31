using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200064A RID: 1610
	public class SectorAssignedToFaction : GameEvent
	{
		// Token: 0x0600286F RID: 10351 RVA: 0x000DA46E File Offset: 0x000D866E
		public SectorAssignedToFaction(TISectorState sector)
		{
			this.sector = sector;
		}

		// Token: 0x04001EA4 RID: 7844
		public TISectorState sector;
	}
}

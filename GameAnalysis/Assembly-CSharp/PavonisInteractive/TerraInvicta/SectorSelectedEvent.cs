using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000667 RID: 1639
	public class SectorSelectedEvent : GameEvent
	{
		// Token: 0x0600288E RID: 10382 RVA: 0x000DA657 File Offset: 0x000D8857
		public SectorSelectedEvent(TISectorState sector)
		{
			this.sector = sector;
		}

		// Token: 0x04001EC5 RID: 7877
		public TISectorState sector;
	}
}

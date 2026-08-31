using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000648 RID: 1608
	public class ShipVisualizationDataDirty : GameEvent
	{
		// Token: 0x0600286D RID: 10349 RVA: 0x000DA450 File Offset: 0x000D8650
		public ShipVisualizationDataDirty(TISpaceShipState ship)
		{
			this.ship = ship;
		}

		// Token: 0x04001EA2 RID: 7842
		public TISpaceShipState ship;
	}
}

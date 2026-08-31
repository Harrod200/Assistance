using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006DA RID: 1754
	public class ShipAIControlChange : GameEvent
	{
		// Token: 0x06002906 RID: 10502 RVA: 0x000DAF3D File Offset: 0x000D913D
		public ShipAIControlChange(TISpaceShipState ship, bool AIInControl)
		{
			this.ship = ship;
			this.AIInControl = AIInControl;
		}

		// Token: 0x04001F66 RID: 8038
		public TISpaceShipState ship;

		// Token: 0x04001F67 RID: 8039
		public bool AIInControl;
	}
}

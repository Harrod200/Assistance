using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200066D RID: 1645
	public class LagrangePointSelectedEvent : GameEvent
	{
		// Token: 0x06002894 RID: 10388 RVA: 0x000DA6B1 File Offset: 0x000D88B1
		public LagrangePointSelectedEvent(TILagrangePointState lagrangePoint)
		{
			this.lagrangePoint = lagrangePoint;
		}

		// Token: 0x04001ECB RID: 7883
		public TILagrangePointState lagrangePoint;
	}
}

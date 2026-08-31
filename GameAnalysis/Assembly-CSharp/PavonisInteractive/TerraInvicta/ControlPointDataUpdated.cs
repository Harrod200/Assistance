using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005EC RID: 1516
	public class ControlPointDataUpdated : GameEvent
	{
		// Token: 0x06002811 RID: 10257 RVA: 0x000D9D29 File Offset: 0x000D7F29
		public ControlPointDataUpdated(TIControlPoint controlPoint)
		{
			this.controlPoint = controlPoint;
		}

		// Token: 0x04001E0F RID: 7695
		public TIControlPoint controlPoint;
	}
}

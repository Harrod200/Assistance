using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000602 RID: 1538
	public class NationGrowsNewControlPoint : GameEvent
	{
		// Token: 0x06002827 RID: 10279 RVA: 0x000D9EC1 File Offset: 0x000D80C1
		public NationGrowsNewControlPoint(TIControlPoint controlPoint)
		{
			this.controlPoint = controlPoint;
		}

		// Token: 0x04001E30 RID: 7728
		public TIControlPoint controlPoint;
	}
}

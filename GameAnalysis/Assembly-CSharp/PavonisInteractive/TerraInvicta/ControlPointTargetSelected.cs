using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000698 RID: 1688
	public class ControlPointTargetSelected : GameEvent
	{
		// Token: 0x060028C0 RID: 10432 RVA: 0x000DAA1C File Offset: 0x000D8C1C
		public ControlPointTargetSelected(TIControlPoint controlPoint)
		{
			this.controlPoint = controlPoint;
		}

		// Token: 0x04001EFE RID: 7934
		public TIControlPoint controlPoint;
	}
}

using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000601 RID: 1537
	public class NationControlPointOwnerChanged : GameEvent
	{
		// Token: 0x06002826 RID: 10278 RVA: 0x000D9EAB File Offset: 0x000D80AB
		public NationControlPointOwnerChanged(TINationState nation, TIControlPoint controlPoint)
		{
			this.nation = nation;
			this.controlPoint = controlPoint;
		}

		// Token: 0x04001E2E RID: 7726
		public TINationState nation;

		// Token: 0x04001E2F RID: 7727
		public TIControlPoint controlPoint;
	}
}

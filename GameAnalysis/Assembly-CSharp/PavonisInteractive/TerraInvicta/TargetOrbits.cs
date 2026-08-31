using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200067A RID: 1658
	public class TargetOrbits : GameEvent
	{
		// Token: 0x060028A2 RID: 10402 RVA: 0x000DA852 File Offset: 0x000D8A52
		public TargetOrbits(TIGameState targetingState, TISpaceObjectState barycenter = null)
		{
			this.targetingState = targetingState;
			this.barycenter = barycenter;
		}

		// Token: 0x04001EDF RID: 7903
		public TIGameState targetingState;

		// Token: 0x04001EE0 RID: 7904
		public TISpaceObjectState barycenter;
	}
}

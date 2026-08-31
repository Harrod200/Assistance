using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000783 RID: 1923
	public struct EventStateCooldownData
	{
		// Token: 0x06003C50 RID: 15440 RVA: 0x0016E4D6 File Offset: 0x0016C6D6
		public EventStateCooldownData(TIGameState coolingState, float cooldown_months)
		{
			this.coolingState = coolingState;
			this.cooldown_months = cooldown_months;
		}

		// Token: 0x04002658 RID: 9816
		public TIGameState coolingState;

		// Token: 0x04002659 RID: 9817
		public float cooldown_months;
	}
}

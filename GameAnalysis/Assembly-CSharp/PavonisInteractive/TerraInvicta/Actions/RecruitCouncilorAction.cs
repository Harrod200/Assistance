using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A7B RID: 2683
	public class RecruitCouncilorAction : PlayerAction
	{
		// Token: 0x0600654A RID: 25930 RVA: 0x002FC269 File Offset: 0x002FA469
		public RecruitCouncilorAction(TICouncilorState councilor, TIFactionState council)
		{
			this.councilorID = councilor.ID;
			this.councilID = council.ID;
		}

		// Token: 0x0600654B RID: 25931 RVA: 0x002FC28C File Offset: 0x002FA48C
		public override void Execute()
		{
			TIFactionState state = this.councilID.GetState<TIFactionState>(false);
			TICouncilorState state2 = this.councilorID.GetState<TICouncilorState>(false);
			state.AddAvailableCouncilor(state2, false);
		}

		// Token: 0x04004776 RID: 18294
		private GameStateID councilorID;

		// Token: 0x04004777 RID: 18295
		private GameStateID councilID;
	}
}

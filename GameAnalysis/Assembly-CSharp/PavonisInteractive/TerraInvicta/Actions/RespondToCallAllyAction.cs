using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A7F RID: 2687
	public class RespondToCallAllyAction : PlayerAction
	{
		// Token: 0x06006552 RID: 25938 RVA: 0x002FC3AC File Offset: 0x002FA5AC
		public RespondToCallAllyAction(TINationState ally, TINationState callingNation, TIWarState war, bool accept)
		{
			this.allyID = ally.ID;
			this.warID = war.ID;
			this.callingNationID = callingNation.ID;
			this.accept = accept;
		}

		// Token: 0x06006553 RID: 25939 RVA: 0x002FC3E0 File Offset: 0x002FA5E0
		public override void Execute()
		{
			TINationState state = this.allyID.GetState<TINationState>(false);
			TINationState state2 = this.callingNationID.GetState<TINationState>(false);
			TIWarState tiwarState;
			this.warID.TryGetState<TIWarState>(out tiwarState, false);
			if (tiwarState != null && state2.extant && state.extant)
			{
				if (this.accept)
				{
					state.JoinWar(state.executiveFaction, state2, tiwarState);
					return;
				}
				state.DeclineOffensiveWar(state2, tiwarState);
			}
		}

		// Token: 0x0400477F RID: 18303
		private GameStateID allyID;

		// Token: 0x04004780 RID: 18304
		private GameStateID warID;

		// Token: 0x04004781 RID: 18305
		private GameStateID callingNationID;

		// Token: 0x04004782 RID: 18306
		private bool accept;
	}
}

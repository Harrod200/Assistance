using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A80 RID: 2688
	public class RespondToPolicyProposalAction : PlayerAction
	{
		// Token: 0x06006554 RID: 25940 RVA: 0x002FC450 File Offset: 0x002FA650
		public RespondToPolicyProposalAction(TINationState respondingNation, TINationState proposingNation, TIGameState relatedGameState, TIPolicyOption policy, bool accept)
		{
			this.actorID = respondingNation.ID;
			this.proposerID = proposingNation.ID;
			if (relatedGameState != null)
			{
				this.relatedID = relatedGameState.ID;
			}
			else
			{
				this.relatedID = default(GameStateID);
			}
			this.policy = policy;
			this.accept = accept;
		}

		// Token: 0x06006555 RID: 25941 RVA: 0x002FC4B0 File Offset: 0x002FA6B0
		public override void Execute()
		{
			TINationState state = this.actorID.GetState<TINationState>(false);
			TINationState state2 = this.proposerID.GetState<TINationState>(false);
			TIGameState tigameState = null;
			this.relatedID.TryGetState<TIGameState>(out tigameState, true);
			if (this.accept)
			{
				this.policy.EnactPolicy(state2, this.policy.EnactAgainstRelatedState ? tigameState : state);
			}
			else
			{
				this.policy.DeclinePolicy(state2, state);
			}
			TIPromptQueueState.RemovePromptStatic(state, state2, tigameState, (this.policy as TIPolicyOptionWithConfirm).PromptName, 0);
		}

		// Token: 0x04004783 RID: 18307
		private GameStateID actorID;

		// Token: 0x04004784 RID: 18308
		private GameStateID proposerID;

		// Token: 0x04004785 RID: 18309
		private GameStateID relatedID;

		// Token: 0x04004786 RID: 18310
		private TIPolicyOption policy;

		// Token: 0x04004787 RID: 18311
		private bool accept;
	}
}

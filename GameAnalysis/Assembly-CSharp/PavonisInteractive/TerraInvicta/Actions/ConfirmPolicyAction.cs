using System;
using System.Linq;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A68 RID: 2664
	public class ConfirmPolicyAction : PlayerAction
	{
		// Token: 0x0600651E RID: 25886 RVA: 0x002FB3EC File Offset: 0x002F95EC
		public ConfirmPolicyAction(TINationState proposingNation, TIFactionState guidingFaction, TIGameState target, TICouncilorState triggeringCouncilor, TIPolicyOption policy)
		{
			this.proposerID = proposingNation.ID;
			this.targetID = ((target == null) ? default(GameStateID) : target.ID);
			this.councilorID = ((triggeringCouncilor == null) ? default(GameStateID) : triggeringCouncilor.ID);
			this.guidingFactionID = ((guidingFaction == null) ? default(GameStateID) : guidingFaction.ID);
			this.policy = policy;
		}

		// Token: 0x0600651F RID: 25887 RVA: 0x002FB478 File Offset: 0x002F9678
		public override void Execute()
		{
			TINationState enactingNation = this.proposerID.GetState<TINationState>(false);
			TIGameState tigameState;
			this.targetID.TryGetState<TIGameState>(out tigameState, true);
			this.policy.OnConfirm(enactingNation, tigameState);
			if (!this.policy.HandledAtFactionLevel())
			{
				TICouncilorState ticouncilorState;
				this.councilorID.TryGetState<TICouncilorState>(out ticouncilorState, false);
				TIFactionState tifactionState;
				this.guidingFactionID.TryGetState<TIFactionState>(out tifactionState, true);
				TIPromptQueueState.RemovePromptStatic(enactingNation, tifactionState, ticouncilorState, "PromptSelectPolicy", 0);
				if (tifactionState != null && tifactionState.plannedPolicies.Count > 0 && !(this.policy is CancelOption))
				{
					PolicyOptionWithTarget policyOptionWithTarget = tifactionState.plannedPolicies.FirstOrDefault<PolicyOptionWithTarget>((PolicyOptionWithTarget x) => x.actingNation == enactingNation && x.policy == this.policy);
					tifactionState.plannedPolicies.Remove(policyOptionWithTarget);
				}
			}
		}

		// Token: 0x04004744 RID: 18244
		private GameStateID targetID;

		// Token: 0x04004745 RID: 18245
		private GameStateID proposerID;

		// Token: 0x04004746 RID: 18246
		private GameStateID guidingFactionID;

		// Token: 0x04004747 RID: 18247
		private GameStateID councilorID;

		// Token: 0x04004748 RID: 18248
		private TIPolicyOption policy;
	}
}

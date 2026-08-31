using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A8B RID: 2699
	public class SelectTechAction : PlayerAction
	{
		// Token: 0x0600656A RID: 25962 RVA: 0x002FCD97 File Offset: 0x002FAF97
		public SelectTechAction(TIFactionState councilState, int slot, TITechTemplate techTemplate)
		{
			this.councilID = councilState.ID;
			this.slot = slot;
			this.techTemplate = techTemplate;
		}

		// Token: 0x0600656B RID: 25963 RVA: 0x002FCDBC File Offset: 0x002FAFBC
		public override void Execute()
		{
			TIGameState state = this.councilID.GetState<TIFactionState>(false);
			TIGlobalResearchState tiglobalResearchState = GameStateManager.FindGameState<TIGlobalResearchState>();
			tiglobalResearchState.AssignNewTechToSlot(this.techTemplate, this.slot);
			TIPromptQueueState.RemovePromptStatic(state, tiglobalResearchState, null, "PromptSelectTech", this.slot);
		}

		// Token: 0x040047AB RID: 18347
		private GameStateID councilID;

		// Token: 0x040047AC RID: 18348
		private int slot;

		// Token: 0x040047AD RID: 18349
		private TITechTemplate techTemplate;
	}
}

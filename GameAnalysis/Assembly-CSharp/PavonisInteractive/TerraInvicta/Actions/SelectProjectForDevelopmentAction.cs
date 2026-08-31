using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A8A RID: 2698
	public class SelectProjectForDevelopmentAction : PlayerAction
	{
		// Token: 0x06006568 RID: 25960 RVA: 0x002FCD34 File Offset: 0x002FAF34
		public SelectProjectForDevelopmentAction(TIFactionState faction, int slot, TIProjectTemplate projectTemplate)
		{
			this.factionID = faction.ID;
			this.slot = slot;
			this.projectTemplate = projectTemplate;
		}

		// Token: 0x06006569 RID: 25961 RVA: 0x002FCD58 File Offset: 0x002FAF58
		public override void Execute()
		{
			TIFactionState state = this.factionID.GetState<TIFactionState>(false);
			state.SetProjectInSlot(this.slot, this.projectTemplate);
			TIPromptQueueState.RemovePromptStatic(state, state, null, "PromptSelectProject", this.slot);
		}

		// Token: 0x040047A8 RID: 18344
		private GameStateID factionID;

		// Token: 0x040047A9 RID: 18345
		private int slot;

		// Token: 0x040047AA RID: 18346
		private TIProjectTemplate projectTemplate;
	}
}

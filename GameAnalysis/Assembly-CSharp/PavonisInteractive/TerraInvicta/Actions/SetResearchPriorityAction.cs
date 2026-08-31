using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A9A RID: 2714
	public class SetResearchPriorityAction : PlayerAction
	{
		// Token: 0x06006589 RID: 25993 RVA: 0x002FD343 File Offset: 0x002FB543
		public SetResearchPriorityAction(TIFactionState faction, int slot, int value)
		{
			this.factionID = faction.ID;
			this.slot = slot;
			this.value = value;
		}

		// Token: 0x0600658A RID: 25994 RVA: 0x002FD365 File Offset: 0x002FB565
		public override void Execute()
		{
			TIFactionState state = this.factionID.GetState<TIFactionState>(false);
			state.SetResearchPriority(this.slot, this.value);
			state.CompleteMilestone(CampaignMilestone.TutorialSetResearchPriority);
		}

		// Token: 0x040047D5 RID: 18389
		private GameStateID factionID;

		// Token: 0x040047D6 RID: 18390
		private int slot;

		// Token: 0x040047D7 RID: 18391
		private int value;
	}
}

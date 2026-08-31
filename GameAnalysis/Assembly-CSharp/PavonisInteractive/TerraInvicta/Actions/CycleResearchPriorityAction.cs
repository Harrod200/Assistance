using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A6A RID: 2666
	public class CycleResearchPriorityAction : PlayerAction
	{
		// Token: 0x06006522 RID: 25890 RVA: 0x002FB5CE File Offset: 0x002F97CE
		public CycleResearchPriorityAction(TIFactionState faction, int slot, bool decrement = false)
		{
			this.factionID = faction.ID;
			this.slot = slot;
			this.decrement = decrement;
		}

		// Token: 0x06006523 RID: 25891 RVA: 0x002FB5F0 File Offset: 0x002F97F0
		public override void Execute()
		{
			TIFactionState state = this.factionID.GetState<TIFactionState>(false);
			if (!this.decrement)
			{
				state.IncrementResearchPriority(this.slot);
			}
			else
			{
				state.DecrementResearchPriority(this.slot);
			}
			state.CompleteMilestone(CampaignMilestone.TutorialSetResearchPriority);
		}

		// Token: 0x0400474D RID: 18253
		private GameStateID factionID;

		// Token: 0x0400474E RID: 18254
		private int slot;

		// Token: 0x0400474F RID: 18255
		private bool decrement;
	}
}

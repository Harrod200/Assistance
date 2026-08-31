using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200062D RID: 1581
	public class MilestoneComplete : GameEvent
	{
		// Token: 0x06002852 RID: 10322 RVA: 0x000DA1D4 File Offset: 0x000D83D4
		public MilestoneComplete(CampaignMilestone milestone, TIFactionState faction)
		{
			this.milestone = milestone;
			this.faction = faction;
		}

		// Token: 0x04001E6A RID: 7786
		public CampaignMilestone milestone;

		// Token: 0x04001E6B RID: 7787
		public TIFactionState faction;
	}
}

using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000A4 RID: 164
public class TIFactionCondition_tbCompletedMilestone : TIFactionCondition
{
	// Token: 0x06000338 RID: 824 RVA: 0x00012426 File Offset: 0x00010626
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_faction != null && TICondition.PassesComparison<CampaignMilestone>(this.sign, this.strValue.ToEnum(CampaignMilestone.None), state.ref_faction.milestones);
	}
}

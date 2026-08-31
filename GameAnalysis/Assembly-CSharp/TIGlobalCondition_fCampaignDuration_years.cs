using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000CB RID: 203
public class TIGlobalCondition_fCampaignDuration_years : TIGlobalCondition
{
	// Token: 0x060003A5 RID: 933 RVA: 0x0001321B File Offset: 0x0001141B
	public override bool PassesCondition(TIGameState state)
	{
		return TICondition.PassesComparison(this.sign, TITimeState.CampaignDuration_years_Exact(), TIUtilities.GetFloatValue(this.strValue));
	}
}

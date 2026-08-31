using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000060 RID: 96
public class TINationCondition_bAnySpaceAssets : TINationCondition
{
	// Token: 0x06000276 RID: 630 RVA: 0x00010D6D File Offset: 0x0000EF6D
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x06000277 RID: 631 RVA: 0x00010D78 File Offset: 0x0000EF78
	public override bool PassesCondition(TIGameState state)
	{
		if (state.ref_nation != null)
		{
			ConditionSign sign = this.sign;
			bool flag;
			if (state.ref_nation.boostIncome_month_dekatons <= 0f && state.ref_nation.missionControl <= 0)
			{
				flag = state.ref_nation.regions.Any<TIRegionState>((TIRegionState x) => x.antiSpaceDefenses);
			}
			else
			{
				flag = true;
			}
			return TICondition.PassesComparison(sign, flag, TIUtilities.GetBoolValue(this.strValue));
		}
		return false;
	}
}

using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000FF RID: 255
public class TIHabCondition_iMineTier : TIHabCondition_Numeric
{
	// Token: 0x0600042F RID: 1071 RVA: 0x000147A4 File Offset: 0x000129A4
	public override bool PassesCondition(TIGameState state)
	{
		if (state.ref_hab != null)
		{
			ConditionSign sign = this.sign;
			TIHabModuleState mine = state.ref_hab.mine;
			int? num;
			if (mine == null)
			{
				num = null;
			}
			else
			{
				TIHabModuleTemplate moduleTemplate = mine.moduleTemplate;
				num = ((moduleTemplate != null) ? new int?(moduleTemplate.tier) : null);
			}
			int? num2 = num;
			return TICondition.PassesComparison(sign, num2.GetValueOrDefault(), TIUtilities.GetIntValue(this.strValue));
		}
		return false;
	}
}

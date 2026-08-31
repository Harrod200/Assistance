using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000100 RID: 256
public class TIHabCondition_bFactionOnlyMine : TIHabCondition
{
	// Token: 0x06000431 RID: 1073 RVA: 0x00014820 File Offset: 0x00012A20
	public override bool PassesCondition(TIGameState state)
	{
		if (state.ref_hab != null)
		{
			ConditionSign sign = this.sign;
			bool flag;
			if (state.ref_hab.HasMine)
			{
				flag = state.ref_faction.habs.Count<TIHabState>((TIHabState x) => x.HasMine) == 1;
			}
			else
			{
				flag = false;
			}
			return TICondition.PassesComparison(sign, flag, TIUtilities.GetBoolValue(this.strValue));
		}
		return false;
	}
}

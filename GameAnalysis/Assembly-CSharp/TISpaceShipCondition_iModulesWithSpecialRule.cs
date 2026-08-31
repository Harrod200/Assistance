using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200012C RID: 300
public class TISpaceShipCondition_iModulesWithSpecialRule : TISpaceShipCondition_Numeric
{
	// Token: 0x17000093 RID: 147
	// (get) Token: 0x06000495 RID: 1173 RVA: 0x000155C2 File Offset: 0x000137C2
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(2)
			{
				this.strIdx,
				base.GetNumericComparisonString(false)
			};
		}
	}

	// Token: 0x06000496 RID: 1174 RVA: 0x000155E4 File Offset: 0x000137E4
	public override bool PassesCondition(TIGameState state)
	{
		SpecialModuleRule specialModuleRule = this.strIdx.ToEnum(SpecialModuleRule.None);
		return state.ref_ship != null && TICondition.PassesComparison(this.sign, state.ref_ship.SpecialModuleRuleCount(specialModuleRule), TIUtilities.GetIntValue(this.strValue));
	}
}

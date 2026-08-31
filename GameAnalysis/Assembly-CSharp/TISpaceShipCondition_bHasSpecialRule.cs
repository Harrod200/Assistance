using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200012B RID: 299
public class TISpaceShipCondition_bHasSpecialRule : TISpaceShipCondition
{
	// Token: 0x06000491 RID: 1169 RVA: 0x0001554B File Offset: 0x0001374B
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x17000092 RID: 146
	// (get) Token: 0x06000492 RID: 1170 RVA: 0x00015553 File Offset: 0x00013753
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { this.strIdx };
		}
	}

	// Token: 0x06000493 RID: 1171 RVA: 0x00015568 File Offset: 0x00013768
	public override bool PassesCondition(TIGameState state)
	{
		SpecialModuleRule specialModuleRule = this.strIdx.ToEnum(SpecialModuleRule.None);
		return state.ref_ship != null && TICondition.PassesComparison(this.sign, state.ref_ship.SpecialModuleRules(false).Contains(specialModuleRule), TIUtilities.GetBoolValue(this.strValue));
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000115 RID: 277
public class TIHabCondition_bHasSpecialRule : TIHabCondition
{
	// Token: 0x0600045E RID: 1118 RVA: 0x00014F0A File Offset: 0x0001310A
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x1700008E RID: 142
	// (get) Token: 0x0600045F RID: 1119 RVA: 0x00014F14 File Offset: 0x00013114
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { Loc.T(new StringBuilder("HabModuleSpecialRule.").Append(this.strIdx.ToEnum(HabModuleSpecialRule.none).ToString()).ToString(), new object[] { "X", "X%", "X" }) };
		}
	}

	// Token: 0x06000460 RID: 1120 RVA: 0x00014F80 File Offset: 0x00013180
	public override bool PassesCondition(TIGameState state)
	{
		HabModuleSpecialRule habModuleSpecialRule = this.strIdx.ToEnum(HabModuleSpecialRule.none);
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.ActiveSpecialAbilities(state.ref_faction).Contains(habModuleSpecialRule), TIUtilities.GetBoolValue(this.strValue));
	}
}

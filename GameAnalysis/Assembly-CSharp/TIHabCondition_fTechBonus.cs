using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200010F RID: 271
public class TIHabCondition_fTechBonus : TIHabCondition_Numeric
{
	// Token: 0x1700008B RID: 139
	// (get) Token: 0x0600044F RID: 1103 RVA: 0x00014C34 File Offset: 0x00012E34
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(2)
			{
				TIGenericTechTemplate.GetTechCategoryString(this.strIdx.ToEnum(TechCategory.Materials)),
				base.GetNumericComparisonString(false)
			};
		}
	}

	// Token: 0x06000450 RID: 1104 RVA: 0x00014C60 File Offset: 0x00012E60
	public override bool PassesCondition(TIGameState state)
	{
		TechCategory techCategory = this.strIdx.ToEnum(TechCategory.Materials);
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.GetNetTechBonusByFaction(techCategory, state.ref_faction, false), TIUtilities.GetFloatValue(this.strValue));
	}
}

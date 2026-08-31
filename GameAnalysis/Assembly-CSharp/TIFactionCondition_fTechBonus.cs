using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200009D RID: 157
public class TIFactionCondition_fTechBonus : TIFactionCondition
{
	// Token: 0x1700005D RID: 93
	// (get) Token: 0x06000323 RID: 803 RVA: 0x0001216C File Offset: 0x0001036C
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

	// Token: 0x06000324 RID: 804 RVA: 0x00012198 File Offset: 0x00010398
	public override bool PassesCondition(TIGameState state)
	{
		TechCategory techCategory = this.strIdx.ToEnum(TechCategory.Materials);
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, state.ref_faction.SumCategoryModifiers(techCategory), TIUtilities.GetFloatValue(this.strValue));
	}
}

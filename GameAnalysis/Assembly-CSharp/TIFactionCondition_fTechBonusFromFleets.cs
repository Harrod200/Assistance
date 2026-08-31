using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200009E RID: 158
public class TIFactionCondition_fTechBonusFromFleets : TIFactionCondition
{
	// Token: 0x1700005E RID: 94
	// (get) Token: 0x06000326 RID: 806 RVA: 0x000121EC File Offset: 0x000103EC
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

	// Token: 0x06000327 RID: 807 RVA: 0x00012218 File Offset: 0x00010418
	public override bool PassesCondition(TIGameState state)
	{
		TechCategory techCategory = this.strIdx.ToEnum(TechCategory.Materials);
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, state.ref_faction.FleetsModifier(techCategory), TIUtilities.GetFloatValue(this.strValue));
	}
}

using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200007D RID: 125
public class TIRegionCondition_fMonthlyBoost : TIRegionCondition_Numeric_Symbol
{
	// Token: 0x1700004B RID: 75
	// (get) Token: 0x060002CB RID: 715 RVA: 0x00011643 File Offset: 0x0000F843
	public override string symbolResource
	{
		get
		{
			return TemplateManager.global.boostInlineSpritePath;
		}
	}

	// Token: 0x060002CC RID: 716 RVA: 0x0001164F File Offset: 0x0000F84F
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, state.ref_region.boostPerMonth_dekatons, TIUtilities.GetFloatValue(this.strValue));
	}
}

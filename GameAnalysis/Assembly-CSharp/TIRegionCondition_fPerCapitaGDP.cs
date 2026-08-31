using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200007F RID: 127
public class TIRegionCondition_fPerCapitaGDP : TIRegionCondition_Numeric_Symbol
{
	// Token: 0x1700004D RID: 77
	// (get) Token: 0x060002D1 RID: 721 RVA: 0x000116D1 File Offset: 0x0000F8D1
	public override string symbolResource
	{
		get
		{
			return TemplateManager.global.perCapitaGDPInlineSpritePath;
		}
	}

	// Token: 0x060002D2 RID: 722 RVA: 0x000116DD File Offset: 0x0000F8DD
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, state.ref_region.regionalPerCapitaGDP, (double)TIUtilities.GetFloatValue(this.strValue));
	}
}

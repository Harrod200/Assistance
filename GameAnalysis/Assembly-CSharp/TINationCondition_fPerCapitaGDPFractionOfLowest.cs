using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000044 RID: 68
public class TINationCondition_fPerCapitaGDPFractionOfLowest : TINationCondition_Numeric_Symbol
{
	// Token: 0x17000038 RID: 56
	// (get) Token: 0x06000234 RID: 564 RVA: 0x0001041C File Offset: 0x0000E61C
	public override string symbolResource
	{
		get
		{
			return TemplateManager.global.perCapitaGDPInlineSpritePath;
		}
	}

	// Token: 0x06000235 RID: 565 RVA: 0x00010428 File Offset: 0x0000E628
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.PerCapitaGDPFractionOfLowest(TIUtilities.GetIntValue(this.strIdx)), TIUtilities.GetFloatValue(this.strValue));
	}
}

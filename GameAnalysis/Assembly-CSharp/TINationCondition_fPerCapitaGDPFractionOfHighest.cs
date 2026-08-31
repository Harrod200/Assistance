using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000043 RID: 67
public class TINationCondition_fPerCapitaGDPFractionOfHighest : TINationCondition_Numeric_Symbol
{
	// Token: 0x17000037 RID: 55
	// (get) Token: 0x06000231 RID: 561 RVA: 0x000103CA File Offset: 0x0000E5CA
	public override string symbolResource
	{
		get
		{
			return TemplateManager.global.perCapitaGDPInlineSpritePath;
		}
	}

	// Token: 0x06000232 RID: 562 RVA: 0x000103D6 File Offset: 0x0000E5D6
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.PerCapitaGDPFractionOfHighest(TIUtilities.GetIntValue(this.strIdx)), TIUtilities.GetFloatValue(this.strValue));
	}
}

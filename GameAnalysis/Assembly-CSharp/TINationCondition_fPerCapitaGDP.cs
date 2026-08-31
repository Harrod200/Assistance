using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000040 RID: 64
public class TINationCondition_fPerCapitaGDP : TINationCondition_Numeric_Symbol
{
	// Token: 0x17000034 RID: 52
	// (get) Token: 0x06000228 RID: 552 RVA: 0x0001028F File Offset: 0x0000E48F
	public override string symbolResource
	{
		get
		{
			return TemplateManager.global.perCapitaGDPInlineSpritePath;
		}
	}

	// Token: 0x06000229 RID: 553 RVA: 0x0001029B File Offset: 0x0000E49B
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.perCapitaGDP, TIUtilities.GetFloatValue(this.strValue));
	}
}

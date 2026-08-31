using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200003F RID: 63
public class TINationCondition_fUnrest : TINationCondition_Numeric_Symbol
{
	// Token: 0x17000033 RID: 51
	// (get) Token: 0x06000225 RID: 549 RVA: 0x00010248 File Offset: 0x0000E448
	public override string symbolResource
	{
		get
		{
			return TemplateManager.global.unrestInlineSpritePath;
		}
	}

	// Token: 0x06000226 RID: 550 RVA: 0x00010254 File Offset: 0x0000E454
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.unrest, TIUtilities.GetFloatValue(this.strValue));
	}
}

using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200003A RID: 58
public class TINationCondition_fDemocracy : TINationCondition_Numeric_Symbol
{
	// Token: 0x1700002F RID: 47
	// (get) Token: 0x06000217 RID: 535 RVA: 0x000100F1 File Offset: 0x0000E2F1
	public override string symbolResource
	{
		get
		{
			return TemplateManager.global.democracyInlineSpritePath;
		}
	}

	// Token: 0x06000218 RID: 536 RVA: 0x000100FD File Offset: 0x0000E2FD
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.democracy, TIUtilities.GetFloatValue(this.strValue));
	}
}

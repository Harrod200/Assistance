using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200003C RID: 60
public class TINationCondition_fInequality : TINationCondition_Numeric_Symbol
{
	// Token: 0x17000031 RID: 49
	// (get) Token: 0x0600021D RID: 541 RVA: 0x0001017F File Offset: 0x0000E37F
	public override string symbolResource
	{
		get
		{
			return TemplateManager.global.inequalityInlineSpritePath;
		}
	}

	// Token: 0x0600021E RID: 542 RVA: 0x0001018B File Offset: 0x0000E38B
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.inequality, TIUtilities.GetFloatValue(this.strValue));
	}
}

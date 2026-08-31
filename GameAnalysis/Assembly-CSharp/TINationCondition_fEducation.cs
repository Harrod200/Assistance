using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200003B RID: 59
public class TINationCondition_fEducation : TINationCondition_Numeric_Symbol
{
	// Token: 0x17000030 RID: 48
	// (get) Token: 0x0600021A RID: 538 RVA: 0x00010138 File Offset: 0x0000E338
	public override string symbolResource
	{
		get
		{
			return TemplateManager.global.educationInlineSpritePath;
		}
	}

	// Token: 0x0600021B RID: 539 RVA: 0x00010144 File Offset: 0x0000E344
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.education, TIUtilities.GetFloatValue(this.strValue));
	}
}

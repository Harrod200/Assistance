using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200006B RID: 107
public class TINationCondition_fEducationGlobalMaxDifferential : TINationCondition_Numeric_Symbol
{
	// Token: 0x17000041 RID: 65
	// (get) Token: 0x06000296 RID: 662 RVA: 0x0001115C File Offset: 0x0000F35C
	public override string symbolResource
	{
		get
		{
			return TemplateManager.global.educationInlineSpritePath;
		}
	}

	// Token: 0x06000297 RID: 663 RVA: 0x00011168 File Offset: 0x0000F368
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, TIGlobalValuesState.GlobalValues.bestGlobalHumanEducation - state.ref_nation.education, TIUtilities.GetFloatValue(this.strValue));
	}
}

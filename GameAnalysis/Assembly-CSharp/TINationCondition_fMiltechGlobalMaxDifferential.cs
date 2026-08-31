using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200006A RID: 106
public class TINationCondition_fMiltechGlobalMaxDifferential : TINationCondition_Numeric_Symbol
{
	// Token: 0x17000040 RID: 64
	// (get) Token: 0x06000293 RID: 659 RVA: 0x0001110A File Offset: 0x0000F30A
	public override string symbolResource
	{
		get
		{
			return TemplateManager.global.miltechInlineSpritePath;
		}
	}

	// Token: 0x06000294 RID: 660 RVA: 0x00011116 File Offset: 0x0000F316
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, TIGlobalValuesState.GlobalValues.bestGlobalHumanMiltech - state.ref_nation.militaryTechLevel, TIUtilities.GetFloatValue(this.strValue));
	}
}

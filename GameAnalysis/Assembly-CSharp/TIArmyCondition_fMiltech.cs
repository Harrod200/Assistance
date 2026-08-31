using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000144 RID: 324
public class TIArmyCondition_fMiltech : TIArmyCondition_Numeric
{
	// Token: 0x17000098 RID: 152
	// (get) Token: 0x060004D6 RID: 1238 RVA: 0x00015C89 File Offset: 0x00013E89
	public override string symbolResource
	{
		get
		{
			return TemplateManager.global.miltechInlineSpritePath;
		}
	}

	// Token: 0x060004D7 RID: 1239 RVA: 0x00015C95 File Offset: 0x00013E95
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_army.techLevel, TIUtilities.GetFloatValue(this.strValue));
	}
}

using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200003D RID: 61
public class TINationCondition_fMiltech : TINationCondition_Numeric_Symbol
{
	// Token: 0x17000032 RID: 50
	// (get) Token: 0x06000220 RID: 544 RVA: 0x000101C6 File Offset: 0x0000E3C6
	public override string symbolResource
	{
		get
		{
			return TemplateManager.global.miltechInlineSpritePath;
		}
	}

	// Token: 0x06000221 RID: 545 RVA: 0x000101D2 File Offset: 0x0000E3D2
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.militaryTechLevel, TIUtilities.GetFloatValue(this.strValue));
	}
}

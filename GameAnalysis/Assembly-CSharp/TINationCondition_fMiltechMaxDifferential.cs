using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000069 RID: 105
public class TINationCondition_fMiltechMaxDifferential : TINationCondition_Numeric_Symbol
{
	// Token: 0x1700003F RID: 63
	// (get) Token: 0x06000290 RID: 656 RVA: 0x000110B7 File Offset: 0x0000F2B7
	public override string symbolResource
	{
		get
		{
			return TemplateManager.global.miltechInlineSpritePath;
		}
	}

	// Token: 0x06000291 RID: 657 RVA: 0x000110C3 File Offset: 0x0000F2C3
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.maxMilitaryTechLevel - state.ref_nation.militaryTechLevel, TIUtilities.GetFloatValue(this.strValue));
	}
}

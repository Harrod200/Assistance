using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000039 RID: 57
public class TINationCondition_fCohesion : TINationCondition_Numeric_Symbol
{
	// Token: 0x1700002E RID: 46
	// (get) Token: 0x06000214 RID: 532 RVA: 0x000100AA File Offset: 0x0000E2AA
	public override string symbolResource
	{
		get
		{
			return TemplateManager.global.cohesionInlineSpritePath;
		}
	}

	// Token: 0x06000215 RID: 533 RVA: 0x000100B6 File Offset: 0x0000E2B6
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.cohesion, TIUtilities.GetFloatValue(this.strValue));
	}
}

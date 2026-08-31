using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000116 RID: 278
public class TIHabCondition_bUnderConstruction : TIHabCondition
{
	// Token: 0x06000462 RID: 1122 RVA: 0x00014FDF File Offset: 0x000131DF
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x06000463 RID: 1123 RVA: 0x00014FE7 File Offset: 0x000131E7
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.CompletedModules().Count == 0, TIUtilities.GetBoolValue(this.strValue));
	}
}

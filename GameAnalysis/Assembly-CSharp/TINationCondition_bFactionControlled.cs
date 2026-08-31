using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000059 RID: 89
public class TINationCondition_bFactionControlled : TINationCondition
{
	// Token: 0x06000261 RID: 609 RVA: 0x00010ACA File Offset: 0x0000ECCA
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x06000262 RID: 610 RVA: 0x00010AD2 File Offset: 0x0000ECD2
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.executiveFaction != null, TIUtilities.GetBoolValue(this.strValue));
	}
}

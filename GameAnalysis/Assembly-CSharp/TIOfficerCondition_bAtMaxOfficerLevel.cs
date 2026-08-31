using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000138 RID: 312
public class TIOfficerCondition_bAtMaxOfficerLevel : TIOfficerCondition
{
	// Token: 0x060004B4 RID: 1204 RVA: 0x0001598A File Offset: 0x00013B8A
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x060004B5 RID: 1205 RVA: 0x00015992 File Offset: 0x00013B92
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_officer != null && TICondition.PassesComparison(this.sign, state.ref_officer.rank >= 3, TIUtilities.GetBoolValue(this.strValue));
	}
}

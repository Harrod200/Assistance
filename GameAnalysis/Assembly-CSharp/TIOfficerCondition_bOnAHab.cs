using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200013B RID: 315
public class TIOfficerCondition_bOnAHab : TIOfficerCondition
{
	// Token: 0x060004BD RID: 1213 RVA: 0x00015A64 File Offset: 0x00013C64
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x060004BE RID: 1214 RVA: 0x00015A6C File Offset: 0x00013C6C
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_officer != null && TICondition.PassesComparison(this.sign, TIGameState.Valid(state.ref_officer.hab), TIUtilities.GetBoolValue(this.strValue));
	}
}

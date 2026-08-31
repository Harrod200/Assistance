using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200010B RID: 267
public class TIHabCondition_bIrradiated : TIHabCondition
{
	// Token: 0x06000447 RID: 1095 RVA: 0x00014B3E File Offset: 0x00012D3E
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.irradiated, TIUtilities.GetBoolValue(this.strValue));
	}
}

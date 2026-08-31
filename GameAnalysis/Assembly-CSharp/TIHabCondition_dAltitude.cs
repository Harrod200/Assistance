using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200010C RID: 268
public class TIHabCondition_dAltitude : TIHabCondition_Numeric
{
	// Token: 0x06000449 RID: 1097 RVA: 0x00014B79 File Offset: 0x00012D79
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.altitude, TIUtilities.GetDoubleValue(this.strValue));
	}
}

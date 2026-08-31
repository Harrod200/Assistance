using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000F8 RID: 248
public class TIHabCondition_iCrewSize : TIHabCondition_Numeric
{
	// Token: 0x0600041D RID: 1053 RVA: 0x00014580 File Offset: 0x00012780
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.crew, TIUtilities.GetIntValue(this.strValue));
	}
}

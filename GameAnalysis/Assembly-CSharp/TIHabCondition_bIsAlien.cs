using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000F5 RID: 245
public class TIHabCondition_bIsAlien : TIHabCondition
{
	// Token: 0x06000417 RID: 1047 RVA: 0x000144CA File Offset: 0x000126CA
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.IsAlien(), TIUtilities.GetBoolValue(this.strValue));
	}
}

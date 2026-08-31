using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000FD RID: 253
public class TIHabCondition_bIsBase : TIHabCondition
{
	// Token: 0x0600042B RID: 1067 RVA: 0x0001472C File Offset: 0x0001292C
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.IsBase, TIUtilities.GetBoolValue(this.strValue));
	}
}

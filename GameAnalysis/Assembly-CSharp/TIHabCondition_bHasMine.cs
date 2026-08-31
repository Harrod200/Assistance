using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000FE RID: 254
public class TIHabCondition_bHasMine : TIHabCondition
{
	// Token: 0x0600042D RID: 1069 RVA: 0x00014767 File Offset: 0x00012967
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.HasMineFunctional, TIUtilities.GetBoolValue(this.strValue));
	}
}

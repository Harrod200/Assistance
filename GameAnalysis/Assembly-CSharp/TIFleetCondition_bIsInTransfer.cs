using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000125 RID: 293
public class TIFleetCondition_bIsInTransfer : TIFleetCondition
{
	// Token: 0x06000484 RID: 1156 RVA: 0x0001540C File Offset: 0x0001360C
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_fleet != null && TICondition.PassesComparison(this.sign, state.ref_fleet.transferAssigned, TIUtilities.GetBoolValue(this.strValue));
	}
}

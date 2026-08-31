using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000124 RID: 292
public class TIFleetCondition_bIsDocked : TIFleetCondition
{
	// Token: 0x06000482 RID: 1154 RVA: 0x000153D1 File Offset: 0x000135D1
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_fleet != null && TICondition.PassesComparison(this.sign, state.ref_fleet.dockedOrLanded, TIUtilities.GetBoolValue(this.strValue));
	}
}

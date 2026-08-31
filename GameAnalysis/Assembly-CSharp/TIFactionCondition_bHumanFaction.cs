using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000A6 RID: 166
public class TIFactionCondition_bHumanFaction : TIFactionCondition
{
	// Token: 0x0600033E RID: 830 RVA: 0x000124C7 File Offset: 0x000106C7
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, state.ref_faction.IsActiveHumanFaction, TIUtilities.GetBoolValue(this.strValue));
	}
}

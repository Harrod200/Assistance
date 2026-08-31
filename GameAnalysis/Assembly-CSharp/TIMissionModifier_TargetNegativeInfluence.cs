using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200024E RID: 590
public class TIMissionModifier_TargetNegativeInfluence : TIMissionModifier
{
	// Token: 0x060007AA RID: 1962 RVA: 0x00024640 File Offset: 0x00022840
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TIFactionState ref_faction = target.ref_faction;
		if (ref_faction != null && ref_faction.GetCurrentResourceAmount(FactionResource.Influence) <= 0f)
		{
			TIFactionState ref_faction2 = target.ref_faction;
			if (ref_faction2 != null && ref_faction2.GetYearlyIncome(FactionResource.Influence, false, false, false) <= 0f)
			{
				return 6f;
			}
		}
		return 0f;
	}
}

using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200024F RID: 591
public class TIMissionModifier_TargetBroke : TIMissionModifier
{
	// Token: 0x060007AC RID: 1964 RVA: 0x000246A4 File Offset: 0x000228A4
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TIFactionState ref_faction = target.ref_faction;
		if (ref_faction != null && ref_faction.GetCurrentResourceAmount(FactionResource.Money) <= 0f)
		{
			TIFactionState ref_faction2 = target.ref_faction;
			if (ref_faction2 != null && ref_faction2.GetYearlyIncome(FactionResource.Money, false, false, false) <= 0f)
			{
				return 6f;
			}
		}
		return 0f;
	}
}

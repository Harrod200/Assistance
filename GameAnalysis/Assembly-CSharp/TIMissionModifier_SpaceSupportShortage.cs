using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200023A RID: 570
public class TIMissionModifier_SpaceSupportShortage : TIMissionModifier
{
	// Token: 0x06000782 RID: 1922 RVA: 0x00023990 File Offset: 0x00021B90
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		float num = 0f;
		if (target.ref_spaceAsset != null)
		{
			if (target.ref_hab != null)
			{
				float num2 = target.ref_faction.DailyHabBoostShortage();
				if (num2 > 0f)
				{
					num -= num2;
				}
			}
			TIFactionState ref_faction = target.ref_faction;
			if (ref_faction != null && ref_faction.Insolvent)
			{
				num += target.ref_faction.GetMonthlyIncome(FactionResource.Money, false, false) / 10f;
			}
		}
		return num;
	}
}

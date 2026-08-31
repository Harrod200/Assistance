using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000219 RID: 537
public class TIMissionModifier_IntelonDefendingCouncilor : TIMissionModifier
{
	// Token: 0x06000734 RID: 1844 RVA: 0x00022AF0 File Offset: 0x00020CF0
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TICouncilorState ref_councilor = target.ref_councilor;
		if (ref_councilor != null)
		{
			if (attackingCouncilor.faction.HasIntelOnCouncilorSecrets(ref_councilor))
			{
				return 5f;
			}
			if (attackingCouncilor.faction.HasIntelOnCouncilorMission(ref_councilor))
			{
				return 3f;
			}
			if (attackingCouncilor.faction.HasIntelOnCouncilorDetails(ref_councilor))
			{
				return 1f;
			}
		}
		return 0f;
	}
}

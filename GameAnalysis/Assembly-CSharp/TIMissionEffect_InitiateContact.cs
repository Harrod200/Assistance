using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001EE RID: 494
public class TIMissionEffect_InitiateContact : TIMissionEffect
{
	// Token: 0x060006CD RID: 1741 RVA: 0x00020F08 File Offset: 0x0001F108
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		TICouncilorState councilor = mission.councilor;
		TICouncilorState ref_councilor = target.ref_councilor;
		TIFactionState faction = ref_councilor.faction;
		TIPromptQueueState.AddPromptStatic(councilor.faction, councilor, mission, "PromptFactionContactMakeOffer", 0);
		if (faction.IsAlienFaction && faction.WillingToTrade(councilor.faction))
		{
			councilor.faction.FixAssessedAlienHateToActualValue();
			if (councilor.faction.veryProAlien)
			{
				councilor.faction.CompleteMilestone(CampaignMilestone.AccessAlienTech);
				councilor.faction.CompleteMilestone(CampaignMilestone.AlienDiplomacy);
			}
		}
		if (!councilor.faction.permanentAlly(faction))
		{
			ref_councilor.knowsIveBeenSeenBy.AddUnique(councilor.faction);
		}
		return string.Empty;
	}
}

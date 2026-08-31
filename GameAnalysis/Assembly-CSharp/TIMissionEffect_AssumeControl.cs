using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001EC RID: 492
public class TIMissionEffect_AssumeControl : TIMissionEffect
{
	// Token: 0x060006C9 RID: 1737 RVA: 0x00020E1C File Offset: 0x0001F01C
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		TINationState ref_nation = target.ref_nation;
		TINationState tinationState = GameStateManager.AlienNation();
		int numControlPoints_unclamped = ref_nation.numControlPoints_unclamped;
		if (base.MissionSuccess(outcome))
		{
			if (tinationState.extant)
			{
				tinationState.AnnexNation(GameStateManager.AlienFaction(), ref_nation, false);
				TINotificationQueueState.LogAlienNationGrows(tinationState, ref_nation);
			}
			else
			{
				tinationState.AnnexNation(GameStateManager.AlienFaction(), ref_nation, true);
				TINotificationQueueState.LogAlienNationFounded(tinationState, ref_nation, true);
			}
			if (mission.councilor.faction.UnlockedExotics)
			{
				GameStateManager.AlienFaction().TransferResourceToFaction(10f * (float)numControlPoints_unclamped * (float)numControlPoints_unclamped, FactionResource.Exotics, mission.councilor.faction);
			}
			else
			{
				mission.councilor.faction.AddToCurrentResource(100f * (float)numControlPoints_unclamped * (float)numControlPoints_unclamped, FactionResource.Research, false, null);
			}
			return ref_nation.displayName;
		}
		return string.Empty;
	}
}

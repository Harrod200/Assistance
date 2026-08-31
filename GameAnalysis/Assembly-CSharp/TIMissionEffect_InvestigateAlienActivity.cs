using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001DE RID: 478
public class TIMissionEffect_InvestigateAlienActivity : TIMissionEffect
{
	// Token: 0x060006A8 RID: 1704 RVA: 0x0001FB67 File Offset: 0x0001DD67
	public override bool HasDelayedEffect()
	{
		return true;
	}

	// Token: 0x060006A9 RID: 1705 RVA: 0x0001FB6C File Offset: 0x0001DD6C
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		TICouncilorState councilor = mission.councilor;
		switch (outcome)
		{
		case TIMissionOutcome.Success:
			councilor.faction.alienInvestigations++;
			break;
		case TIMissionOutcome.CriticalSuccess:
			councilor.faction.alienInvestigations += 2;
			break;
		}
		return string.Empty;
	}

	// Token: 0x060006AA RID: 1706 RVA: 0x0001FBCC File Offset: 0x0001DDCC
	public override void ApplyDelayedEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success, string dataName = "")
	{
		TICouncilorState councilor = mission.councilor;
		switch (outcome)
		{
		case TIMissionOutcome.CriticalFailure:
		case TIMissionOutcome.Failure:
		case TIMissionOutcome.Aborted:
			break;
		case TIMissionOutcome.Success:
		case TIMissionOutcome.CriticalSuccess:
			if (target.isRegionAlienActivity)
			{
				target.ref_regionAlienActivity.RemoveActivity(councilor.faction);
				return;
			}
			councilor.faction.ExpireIntel(target, true);
			break;
		default:
			return;
		}
	}
}

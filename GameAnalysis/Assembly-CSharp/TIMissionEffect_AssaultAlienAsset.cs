using System;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001DF RID: 479
public class TIMissionEffect_AssaultAlienAsset : TIMissionEffect
{
	// Token: 0x060006AC RID: 1708 RVA: 0x0001FC2C File Offset: 0x0001DE2C
	public override bool HasDelayedEffect()
	{
		return true;
	}

	// Token: 0x060006AD RID: 1709 RVA: 0x0001FC30 File Offset: 0x0001DE30
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		TICouncilorState councilor = mission.councilor;
		string text = target.ref_regionAlienAsset.ResolveAssault(councilor, councilor.faction, outcome);
		if (outcome == TIMissionOutcome.CriticalFailure && (target.isRegionLandedUFO || target.isRegionAlienFacility) && mission.councilor.GetProtectors().Count == 0)
		{
			text = new StringBuilder(text).Append(" ").Append(Loc.T("TIMissionTemplate.CouncilorKilled.AssaultAlienAsset", new object[] { councilor.displayName })).ToString();
		}
		return text;
	}

	// Token: 0x060006AE RID: 1710 RVA: 0x0001FCB3 File Offset: 0x0001DEB3
	public override void ApplyDelayedEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success, string dataName = "")
	{
		if (outcome == TIMissionOutcome.CriticalFailure && (target.isRegionLandedUFO || target.isRegionAlienFacility) && mission.councilor.GetProtectors().Count == 0)
		{
			mission.councilor.KillCouncilorOnMission(mission);
		}
	}
}

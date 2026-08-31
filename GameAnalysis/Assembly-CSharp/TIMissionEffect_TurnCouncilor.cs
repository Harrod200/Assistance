using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001D5 RID: 469
public class TIMissionEffect_TurnCouncilor : TIMissionEffect
{
	// Token: 0x06000694 RID: 1684 RVA: 0x0001EAF0 File Offset: 0x0001CCF0
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		if (outcome == TIMissionOutcome.CriticalSuccess)
		{
			float num = target.ref_faction.GetDailyIncome(FactionResource.Research, false, false) * 30f;
			mission.councilor.faction.AddToCurrentResource(num, FactionResource.Research, false, "Turn Councilor");
			return num.ToString("N0");
		}
		return string.Empty;
	}

	// Token: 0x06000695 RID: 1685 RVA: 0x0001EB41 File Offset: 0x0001CD41
	public override bool HasDelayedEffect()
	{
		return true;
	}

	// Token: 0x06000696 RID: 1686 RVA: 0x0001EB44 File Offset: 0x0001CD44
	public override void ApplyDelayedEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success, string dataName = "")
	{
		if (base.MissionSuccess(outcome))
		{
			target.ref_councilor.TurnCouncilor(mission.councilor.faction);
		}
	}
}

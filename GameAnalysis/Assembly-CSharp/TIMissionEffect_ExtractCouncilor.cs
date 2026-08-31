using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001F4 RID: 500
public class TIMissionEffect_ExtractCouncilor : TIMissionEffect
{
	// Token: 0x060006DD RID: 1757 RVA: 0x00021918 File Offset: 0x0001FB18
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		TICouncilorState ref_councilor = target.ref_councilor;
		switch (outcome)
		{
		case TIMissionOutcome.CriticalFailure:
			mission.councilor.DetainCouncilor(ref_councilor.detainingFaction, 2f, 1f, true);
			break;
		case TIMissionOutcome.Success:
			ref_councilor.ReleaseCouncilor(false);
			break;
		case TIMissionOutcome.CriticalSuccess:
			mission.councilor.faction.GainIntel(ref_councilor.detainingFaction, 20f, null, false);
			ref_councilor.ReleaseCouncilor(false);
			break;
		}
		return string.Empty;
	}
}

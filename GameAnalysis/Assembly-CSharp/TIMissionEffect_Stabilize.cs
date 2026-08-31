using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001DC RID: 476
public class TIMissionEffect_Stabilize : TIMissionEffect
{
	// Token: 0x060006A4 RID: 1700 RVA: 0x0001F598 File Offset: 0x0001D798
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		TICouncilorState councilor = mission.councilor;
		TINationState ref_nation = target.ref_nation;
		if (base.MissionSuccess(outcome))
		{
			float num = ((ref_nation.unrest * 2f < ref_nation.unrestRestState) ? 0.5f : 1f) * ((outcome == TIMissionOutcome.CriticalSuccess) ? 2f : 1f) / TIMissionPhaseState.phasesPerMonth;
			ref_nation.StabilizeNation(councilor.faction, num, TINationState.UnrestChangeReason.UnrestReason_StabilizeMission);
			return TIUtilities.FormatSmallNumber(num, 7, 0, true, false);
		}
		if (outcome == TIMissionOutcome.CriticalFailure)
		{
			float num2 = -0.1f;
			ref_nation.AddToUnrest(-num2, TINationState.UnrestChangeReason.UnrestReason_StabilizeMissionFailure, 10f);
			return TIUtilities.FormatSmallNumber(-num2, 7, 0, true, false);
		}
		return string.Empty;
	}
}

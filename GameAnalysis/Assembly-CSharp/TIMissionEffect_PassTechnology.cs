using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001F2 RID: 498
public class TIMissionEffect_PassTechnology : TIMissionEffect
{
	// Token: 0x060006D9 RID: 1753 RVA: 0x000217E4 File Offset: 0x0001F9E4
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		float num = target.ref_faction.AddToCurrentResource(100f * TIGlobalValuesState.GetAlienProgressionModifiedDuration_IgnoreStartingProgression_years_exact() / TemplateManager.global.duration_scaling_divisor, FactionResource.Research, false, null);
		float num2 = 0f;
		if (target.ref_faction.UnlockedExotics && mission.ref_faction.GetCurrentResourceAmount(FactionResource.Exotics) * 2f > (float)AIEvaluators.AbundantValue(FactionResource.Exotics))
		{
			num2 = mission.ref_faction.TransferResourceToFaction(10f, FactionResource.Exotics, target.ref_faction);
		}
		if (num > 0f || num2 > 0f)
		{
			TINotificationQueueState.LogAliensPassTechnologyToMe(mission.councilor, target.ref_councilor, num, num2);
		}
		return string.Empty;
	}
}

using System;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x020001E7 RID: 487
public class TIMissionEffect_Abductions : TIMissionEffect
{
	// Token: 0x060006BF RID: 1727 RVA: 0x000207AC File Offset: 0x0001E9AC
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		TICouncilorState councilor = mission.councilor;
		TIRegionState ref_region = target.ref_region;
		switch (outcome)
		{
		case TIMissionOutcome.Success:
			ref_region.ConductAbductions(councilor.faction, Mathf.CeilToInt(2f / TIMissionPhaseState.phasesPerMonth));
			councilor.faction.AddToCurrentResource(TemplateManager.global.moneyGainFromAbductions_Success, FactionResource.Money, false, null);
			break;
		case TIMissionOutcome.CriticalSuccess:
			ref_region.ConductAbductions(councilor.faction, Mathf.CeilToInt(4f / TIMissionPhaseState.phasesPerMonth));
			councilor.faction.AddToCurrentResource(TemplateManager.global.moneyGainFromAbductions_CriticalSuccess, FactionResource.Money, false, null);
			break;
		}
		return string.Empty;
	}
}

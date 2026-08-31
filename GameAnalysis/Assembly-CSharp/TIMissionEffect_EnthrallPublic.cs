using System;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x020001E8 RID: 488
public class TIMissionEffect_EnthrallPublic : TIMissionEffect
{
	// Token: 0x060006C1 RID: 1729 RVA: 0x00020860 File Offset: 0x0001EA60
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		TICouncilorState councilor = mission.councilor;
		TIRegionState ref_region = target.ref_region;
		TINationState nation = ref_region.nation;
		TIFactionState faction = councilor.faction;
		float num = 0f;
		float num2 = ref_region.populationInMillions / nation.population_Millions;
		switch (outcome)
		{
		case TIMissionOutcome.Success:
			num = nation.PropagandaOnPop(faction.ideology, ((float)councilor.GetAttribute(CouncilorAttribute.Persuasion, true, true, true, false, false, false) + Mathf.Min((float)ref_region.abductions, TemplateManager.global.maxAbductionMissionImpact) * TemplateManager.global.GetAbductionMissionBonusDifficultyScaling()) * 4f * num2 / TIMissionPhaseState.phasesPerMonth, false);
			faction.AddToCurrentResource(TemplateManager.global.influenceGainFromEnthrallPublic, FactionResource.Influence, false, null);
			faction.AddToCurrentResource(TemplateManager.global.moneyGainFromEnthrallPublic_Success, FactionResource.Money, false, null);
			break;
		case TIMissionOutcome.CriticalSuccess:
			num = nation.PropagandaOnPop(faction.ideology, ((float)councilor.GetAttribute(CouncilorAttribute.Persuasion, true, true, true, false, false, false) + Mathf.Min((float)ref_region.abductions, TemplateManager.global.maxAbductionMissionImpact) * TemplateManager.global.GetAbductionMissionBonusDifficultyScaling()) * 8f * num2 / TIMissionPhaseState.phasesPerMonth, false);
			faction.AddToCurrentResource(2f * TemplateManager.global.influenceGainFromEnthrallPublic, FactionResource.Influence, false, null);
			faction.AddToCurrentResource(TemplateManager.global.moneyGainFromEnthrallPublic_CriticalSuccess, FactionResource.Money, false, null);
			break;
		}
		return num.ToPercent("P0");
	}
}

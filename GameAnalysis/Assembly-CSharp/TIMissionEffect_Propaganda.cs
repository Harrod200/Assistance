using System;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x020001D7 RID: 471
public class TIMissionEffect_Propaganda : TIMissionEffect
{
	// Token: 0x0600069A RID: 1690 RVA: 0x0001EC98 File Offset: 0x0001CE98
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		TICouncilorState councilor = mission.councilor;
		TINationState ref_nation = target.ref_nation;
		if (base.MissionSuccess(outcome))
		{
			float num = TemplateManager.global.basePropagandaStrength * ((outcome == TIMissionOutcome.CriticalSuccess) ? 2f : 1f);
			num += councilor.faction.PropagandaBonus;
			num += TIEffectsState.SumEffectsModifiers(Context.PublicCampaignStrength, mission.councilor.faction, num, null);
			num += TIUtilities.RandomRange(-3.5f, 3.5f);
			float num2 = ref_nation.PropagandaOnPop(councilor.faction.ideology, num, false);
			if (num2 < 0.005f)
			{
				return Loc.T("TIMissionEffect_Propaganda.Special2", new object[] { ref_nation.GetPublicOpinionOfFaction(councilor.faction).ToPercent("P0") });
			}
			return Loc.T("TIMissionEffect_Propaganda.Special", new object[]
			{
				num2.ToPercent("P0"),
				ref_nation.GetPublicOpinionOfFaction(councilor.faction).ToPercent("P0")
			});
		}
		else
		{
			if (outcome == TIMissionOutcome.CriticalFailure)
			{
				float num3 = ref_nation.PropagandaOnPop(councilor.faction.ideology, (float)Mathf.Min(-1, councilor.GetAttribute(CouncilorAttribute.Persuasion, true, true, true, false, false, false) - 11), false);
				return Loc.T("TIMissionEffect_Propaganda.Special", new object[]
				{
					num3.ToPercent("P0"),
					ref_nation.GetPublicOpinionOfFaction(councilor.faction).ToPercent("P0")
				});
			}
			return string.Empty;
		}
	}
}

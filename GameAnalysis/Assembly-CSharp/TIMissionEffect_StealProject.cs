using System;
using System.Text;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x020001E4 RID: 484
public class TIMissionEffect_StealProject : TIMissionEffect
{
	// Token: 0x060006B9 RID: 1721 RVA: 0x00020344 File Offset: 0x0001E544
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		if (base.MissionSuccess(outcome))
		{
			TIPromptQueueState.AddPromptStatic(mission.councilor.faction, mission.councilor, mission, "PromptStealProject", 0);
			float num = Mathf.Max(0f, target.ref_faction.GetDailyIncome(FactionResource.Research, false, false) - mission.councilor.faction.GetDailyIncome(FactionResource.Research, false, false));
			if (num > 0f)
			{
				if (outcome == TIMissionOutcome.CriticalSuccess)
				{
					num += 25f;
					num *= 2f;
				}
				num *= 10f;
				mission.councilor.faction.AddToCurrentResource(num, FactionResource.Research, false, "Steal Project");
				return Loc.T(new StringBuilder(base.GetType().Name).Append(".Steal").ToString(), new object[] { new StringBuilder(TemplateManager.global.researchInlineSpritePath).Append(num.ToString("N0")).ToString() });
			}
		}
		else if (outcome == TIMissionOutcome.CriticalFailure)
		{
			TIFactionState ref_faction = target.ref_faction;
			mission.councilor.DetainCouncilor(ref_faction, 2f, 1f, true);
			return Loc.T(new StringBuilder(base.GetType().Name).Append(".Special").ToString(), new object[] { ref_faction.displayNameCapitalized });
		}
		return string.Empty;
	}
}

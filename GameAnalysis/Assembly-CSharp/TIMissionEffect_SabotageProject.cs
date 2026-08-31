using System;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001E5 RID: 485
public class TIMissionEffect_SabotageProject : TIMissionEffect
{
	// Token: 0x060006BB RID: 1723 RVA: 0x000204A0 File Offset: 0x0001E6A0
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		if (base.MissionSuccess(outcome))
		{
			TIPromptQueueState.AddPromptStatic(mission.councilor.faction, mission.councilor, mission, "PromptSabotageProject", 0);
			if (outcome == TIMissionOutcome.CriticalSuccess)
			{
				float num = target.ref_faction.TransferResourceToFaction(50f, FactionResource.Research, mission.councilor.faction);
				return new StringBuilder(TemplateManager.global.researchInlineSpritePath).Append(num.ToString("N0")).ToString();
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

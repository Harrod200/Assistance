using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001E1 RID: 481
public class TIMissionEffect_GoToGround : TIMissionEffect
{
	// Token: 0x060006B2 RID: 1714 RVA: 0x00020088 File Offset: 0x0001E288
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		TIMissionEffect_GoToGround.ApplyEffect_Static(mission.councilor);
		if (mission.councilor.faction != null && mission.councilor.faction.isActivePlayer)
		{
			mission.councilor.faction.UnlockAchievement("councilorIntoGround");
		}
		return string.Empty;
	}

	// Token: 0x060006B3 RID: 1715 RVA: 0x000200E0 File Offset: 0x0001E2E0
	public static void ApplyEffect_Static(TICouncilorState councilor)
	{
		foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
		{
			if (tifactionState == councilor.agentForFaction)
			{
				councilor.knowsIveBeenSeenBy.Remove(tifactionState);
			}
			else if (tifactionState == councilor.faction)
			{
				tifactionState.SetIntel(councilor, TemplateManager.global.myCouncilorBaselineIntel, null, true);
			}
			else
			{
				tifactionState.GainIntel(councilor, -0.5f - 0.03f * (float)councilor.GetAttribute(CouncilorAttribute.Espionage, true, true, true, false, false, false), null, true);
				councilor.knowsIveBeenSeenBy.Remove(tifactionState);
			}
		}
		TIFactionState[] array = GameStateManager.AllFactions();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].GainIntel(councilor, -1E-45f, null, false);
		}
	}
}

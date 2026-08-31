using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001F5 RID: 501
public class TIMissionEffect_Win : TIMissionEffect
{
	// Token: 0x060006DF RID: 1759 RVA: 0x000219A4 File Offset: 0x0001FBA4
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		switch (mission.councilor.faction.victoryTemplate.victoryEffect)
		{
		case TIVictoryTemplate.VictoryEffectType.none:
		case TIVictoryTemplate.VictoryEffectType.EndGame:
			goto IL_011F;
		case TIVictoryTemplate.VictoryEffectType.HumanNationsToWinningFaction:
		{
			using (IEnumerator<TINationState> enumerator = GameStateManager.AllExtantHumanNations().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TINationState tinationState = enumerator.Current;
					foreach (TIControlPoint ticontrolPoint in tinationState.controlPoints)
					{
						if (ticontrolPoint.faction != mission.councilor.faction)
						{
							ticontrolPoint.nation.ChangeControlPointOwner(ticontrolPoint.positionInNation, ControlPointChangeCause.Victory, mission.councilor.faction);
						}
					}
				}
				goto IL_011F;
			}
			break;
		}
		case TIVictoryTemplate.VictoryEffectType.HumanNationsToAlienNation:
			break;
		default:
			goto IL_011F;
		}
		foreach (TINationState tinationState2 in GameStateManager.AllExtantHumanNations())
		{
			GameStateManager.AlienNation().AnnexNation(mission.councilor.faction, tinationState2, !GameStateManager.AlienNation().extant);
		}
		GameStateManager.AlienNation().AddToUnrest(-8f, TINationState.UnrestChangeReason.UnrestReason_AlienNationDominance, 10f);
		IL_011F:
		Log.Debug("Victory condition achieved: " + mission.councilor.faction.displayName, Array.Empty<object>());
		TINotificationQueueState.LogFactionWin(mission.councilor.faction);
		return string.Empty;
	}
}

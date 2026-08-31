using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001DB RID: 475
public class TIMissionEffect_IncreaseUnrest : TIMissionEffect
{
	// Token: 0x060006A2 RID: 1698 RVA: 0x0001F354 File Offset: 0x0001D554
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		TICouncilorState councilor = mission.councilor;
		TINationState ref_nation = target.ref_nation;
		if (base.MissionSuccess(outcome))
		{
			float num = ((ref_nation.unrest > 2f * ref_nation.unrestRestState) ? 0.5f : 1f) * ((outcome == TIMissionOutcome.CriticalSuccess) ? 2f : 1f) / TIMissionPhaseState.phasesPerMonth;
			List<TINationState> list = target.ref_region.SecessionCandidates();
			num = ref_nation.IncreaseUnrest(councilor.faction, num, target.ref_region.isCapital || list.Count == 0, TINationState.UnrestChangeReason.UnrestReason_UnrestMission);
			foreach (TINationState tinationState in list)
			{
				float num2 = ((outcome == TIMissionOutcome.CriticalSuccess) ? 10000f : 1000f);
				num2 += TIEffectsState.SumEffectsModifiers(Context.BreakawayChance, mission.councilor.faction, num2, null);
				if (tinationState.PostUnrestSecessionCheck(councilor.faction, num2 / TIMissionPhaseState.phasesPerMonth, false))
				{
					if (councilor.faction.isActivePlayer)
					{
						councilor.faction.UnlockAchievement("unrestBreakaway");
						break;
					}
					break;
				}
			}
			if (target.ref_region.isCapital && ref_nation.unrest > TINationState.minUnrestForRevolution && (outcome == TIMissionOutcome.CriticalSuccess || TIUtilities.RandomFloatValue() <= 0.1f))
			{
				ref_nation.Revolution();
			}
			return num.ToString("0.###");
		}
		if (outcome != TIMissionOutcome.CriticalFailure)
		{
			return string.Empty;
		}
		TIFactionState tifactionState = ref_nation.WeightedRandomFactionByControlPoints();
		if (tifactionState == mission.councilor.faction)
		{
			return string.Empty;
		}
		councilor.faction.CommitAtrocity(1, TIFactionState.AtrocityCause.IncreaseUnrestCritFailure, false, 0.333f);
		if (tifactionState == null)
		{
			float num3 = ref_nation.PropagandaOnPop(councilor.faction.ideology, -5f, false);
			return Loc.T("TIMissionEffect_IncreaseUnrest.Special1", new object[]
			{
				num3.ToPercent("P0"),
				councilor.faction.displayNameWithColor
			});
		}
		councilor.DetainCouncilor(tifactionState, 2f, 1f, true);
		return Loc.T("TIMissionEffect_IncreaseUnrest.Special2", new object[]
		{
			tifactionState.displayNameCapitalizedWithColor,
			councilor.faction.displayNameWithColor
		});
	}
}

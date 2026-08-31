using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200027A RID: 634
public class TIMissionTarget_VictoryMissionTarget : MissionTarget<TIGameState>
{
	// Token: 0x06000853 RID: 2131 RVA: 0x00026D60 File Offset: 0x00024F60
	public override TIFactionState GetRelevantFaction(TIGameState target)
	{
		return target.ref_faction;
	}

	// Token: 0x06000854 RID: 2132 RVA: 0x00026D68 File Offset: 0x00024F68
	public static bool IsVictoryTarget(TIObjectiveTemplate victoryObjective, TIGameState target)
	{
		switch (victoryObjective.targetMissionTarget)
		{
		case ObjectiveMissionTargetType.AlienHQ:
			return target.isHabModuleState && target.ref_habModule.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.AlienWormhole);
		case ObjectiveMissionTargetType.NewYorkRegion:
			return target.isRegionState && target.ref_region.mapRegionTemplateName == "map_NewYork";
		case ObjectiveMissionTargetType.EscapeLaunchSite:
			return target.isHabModuleState && target.ref_habModule.active && target.ref_habModule.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.InterstellarLaunchModule);
		case ObjectiveMissionTargetType.AppeaseSentinel:
			return target.isHabModuleState && target.ref_habModule.active && target.ref_habModule.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.SentinelModule);
		default:
			return false;
		}
	}

	// Token: 0x06000855 RID: 2133 RVA: 0x00026E3B File Offset: 0x0002503B
	private TIObjectiveTemplate GetVictoryObjective(TIFactionState faction)
	{
		return faction.GetObjectivesByType(ObjectiveType.Victory).FirstOrDefault<TIObjectiveTemplate>();
	}

	// Token: 0x06000856 RID: 2134 RVA: 0x00026E4C File Offset: 0x0002504C
	public override List<string> ValidateSingleTarget(TIMissionTemplate mission, TICouncilorState councilor, TIGameState target)
	{
		List<string> list = new List<string>();
		if (TIMissionTarget_VictoryMissionTarget.IsVictoryTarget(this.GetVictoryObjective(councilor.ref_faction), target))
		{
			using (List<TIMissionCondition>.Enumerator enumerator = mission.conditions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIMissionCondition timissionCondition = enumerator.Current;
					list.Add(timissionCondition.CanTarget(councilor, target));
				}
				return list;
			}
		}
		list.Add("_Fail");
		return list;
	}

	// Token: 0x06000857 RID: 2135 RVA: 0x00026ECC File Offset: 0x000250CC
	public override IEnumerable<TIGameState> GetAllPotentialTargets(TIFactionState faction)
	{
		switch (this.GetVictoryObjective(faction).targetMissionTarget)
		{
		case ObjectiveMissionTargetType.AlienHQ:
		case ObjectiveMissionTargetType.EscapeLaunchSite:
		case ObjectiveMissionTargetType.AppeaseSentinel:
			return faction.KnownHabs.SelectMany<TIHabState, TIHabModuleState>((TIHabState x) => x.OkayModules());
		case ObjectiveMissionTargetType.NewYorkRegion:
			return GameStateManager.AllRegions();
		default:
			return new List<TIGameState>();
		}
	}

	// Token: 0x06000858 RID: 2136 RVA: 0x00026F38 File Offset: 0x00025138
	public IEnumerable<TIGameState> GetVictoryTargets(TIFactionState faction)
	{
		TIObjectiveTemplate victoryObjective = this.GetVictoryObjective(faction);
		return from x in this.GetAllPotentialTargets(faction)
			where TIMissionTarget_VictoryMissionTarget.IsVictoryTarget(victoryObjective, x)
			select x;
	}

	// Token: 0x06000859 RID: 2137 RVA: 0x00026F70 File Offset: 0x00025170
	public override IList<TIGameState> GetValidTargets(TIMissionTemplate mission, TICouncilorState councilor)
	{
		List<TIGameState> list = new List<TIGameState>();
		switch (councilor.ref_faction.GetObjectivesByType(ObjectiveType.Victory).FirstOrDefault<TIObjectiveTemplate>().targetMissionTarget)
		{
		case ObjectiveMissionTargetType.AlienHQ:
		case ObjectiveMissionTargetType.EscapeLaunchSite:
		case ObjectiveMissionTargetType.AppeaseSentinel:
		{
			using (List<TIHabModuleState>.Enumerator enumerator = councilor.faction.KnownHabs.SelectMany<TIHabState, TIHabModuleState>((TIHabState x) => x.OkayModules()).ToList<TIHabModuleState>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIHabModuleState tihabModuleState = enumerator.Current;
					if (base.ValidTarget(this.ValidateSingleTarget(mission, councilor, tihabModuleState)))
					{
						list.Add(tihabModuleState);
					}
				}
				return list;
			}
			break;
		}
		case ObjectiveMissionTargetType.NewYorkRegion:
			break;
		default:
			return list;
		}
		foreach (TIRegionState tiregionState in GameStateManager.AllRegions())
		{
			if (base.ValidTarget(this.ValidateSingleTarget(mission, councilor, tiregionState)))
			{
				list.Add(tiregionState);
			}
		}
		return list;
	}
}

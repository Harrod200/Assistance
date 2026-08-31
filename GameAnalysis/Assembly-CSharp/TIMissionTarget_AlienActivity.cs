using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200026D RID: 621
public class TIMissionTarget_AlienActivity : MissionTarget<TIRegionAlienEntityState>
{
	// Token: 0x06000812 RID: 2066 RVA: 0x00025824 File Offset: 0x00023A24
	public override TIFactionState GetRelevantFaction(TIGameState target)
	{
		return target.ref_faction;
	}

	// Token: 0x06000813 RID: 2067 RVA: 0x0002582C File Offset: 0x00023A2C
	public override List<string> ValidateSingleTarget(TIMissionTemplate mission, TICouncilorState councilor, TIGameState target)
	{
		List<string> list = new List<string>();
		if (target.isRegionAlienActivity)
		{
			TIRegionAlienActivityState ref_regionAlienActivity = target.ref_regionAlienActivity;
			if (ref_regionAlienActivity.VisibleToFaction(councilor.faction))
			{
				using (List<TIMissionCondition>.Enumerator enumerator = mission.conditions.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIMissionCondition timissionCondition = enumerator.Current;
						list.Add(timissionCondition.CanTarget(councilor, ref_regionAlienActivity));
					}
					return list;
				}
			}
			list.Add("_Fail");
		}
		else if (target.isRegionUFOCrashdown)
		{
			TIRegionUFOCrashdownState ref_UFOCrashdown = target.ref_UFOCrashdown;
			if (ref_UFOCrashdown.VisibleToFaction(councilor.faction))
			{
				using (List<TIMissionCondition>.Enumerator enumerator = mission.conditions.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIMissionCondition timissionCondition2 = enumerator.Current;
						list.Add(timissionCondition2.CanTarget(councilor, ref_UFOCrashdown));
					}
					return list;
				}
			}
			list.Add("_Fail");
		}
		return list;
	}

	// Token: 0x06000814 RID: 2068 RVA: 0x00025930 File Offset: 0x00023B30
	public override IEnumerable<TIRegionAlienEntityState> GetAllPotentialTargets(TIFactionState faction = null)
	{
		return GameStateManager.AllRegions().SelectMany<TIRegionState, TIRegionAlienEntityState>((TIRegionState x) => x.alienActivities);
	}

	// Token: 0x06000815 RID: 2069 RVA: 0x0002595C File Offset: 0x00023B5C
	public override IList<TIRegionAlienEntityState> GetValidTargets(TIMissionTemplate mission, TICouncilorState councilor)
	{
		List<TIRegionAlienEntityState> list = new List<TIRegionAlienEntityState>();
		foreach (TIRegionAlienEntityState tiregionAlienEntityState in this.GetAllPotentialTargets(null))
		{
			if (base.ValidTarget(this.ValidateSingleTarget(mission, councilor, tiregionAlienEntityState)))
			{
				list.Add(tiregionAlienEntityState);
			}
		}
		return list;
	}
}

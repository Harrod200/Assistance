using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200026C RID: 620
public class TIMissionTarget_AlienAsset : MissionTarget<TIRegionAlienAssetState>
{
	// Token: 0x0600080D RID: 2061 RVA: 0x000255B4 File Offset: 0x000237B4
	public override TIFactionState GetRelevantFaction(TIGameState target)
	{
		return target.ref_faction;
	}

	// Token: 0x0600080E RID: 2062 RVA: 0x000255BC File Offset: 0x000237BC
	public override IEnumerable<TIRegionAlienAssetState> GetAllPotentialTargets(TIFactionState faction = null)
	{
		return GameStateManager.AllRegions().SelectMany<TIRegionState, TIRegionAlienAssetState>((TIRegionState x) => x.alienAssets);
	}

	// Token: 0x0600080F RID: 2063 RVA: 0x000255E8 File Offset: 0x000237E8
	public override List<string> ValidateSingleTarget(TIMissionTemplate mission, TICouncilorState councilor, TIGameState target)
	{
		List<string> list = new List<string>();
		if (target.isRegionAlienFacility && !councilor.faction.permanentAlly(GameStateManager.AlienFaction()))
		{
			TIRegionAlienFacilityState ref_alienFacility = target.ref_alienFacility;
			if (ref_alienFacility.built && ref_alienFacility.VisibleToFaction(councilor.faction))
			{
				using (List<TIMissionCondition>.Enumerator enumerator = mission.conditions.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIMissionCondition timissionCondition = enumerator.Current;
						list.Add(timissionCondition.CanTarget(councilor, ref_alienFacility));
					}
					return list;
				}
			}
			list.Add("_Fail");
		}
		else if (target.isRegionLandedUFO && !councilor.faction.permanentAlly(GameStateManager.AlienFaction()))
		{
			TIRegionUFOLandingState ref_UFOLanding = target.ref_UFOLanding;
			if (ref_UFOLanding.VisibleToFaction(councilor.faction))
			{
				using (List<TIMissionCondition>.Enumerator enumerator = mission.conditions.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIMissionCondition timissionCondition2 = enumerator.Current;
						list.Add(timissionCondition2.CanTarget(councilor, ref_UFOLanding));
					}
					return list;
				}
			}
			list.Add("_Fail");
		}
		else if (target.isRegionXenoformingState)
		{
			TIRegionXenoformingState ref_xenoforming = target.ref_xenoforming;
			if (ref_xenoforming.xenoformingLevel > 0f && ref_xenoforming.VisibleToFaction(councilor.faction))
			{
				using (List<TIMissionCondition>.Enumerator enumerator = mission.conditions.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIMissionCondition timissionCondition3 = enumerator.Current;
						list.Add(timissionCondition3.CanTarget(councilor, ref_xenoforming));
					}
					return list;
				}
			}
			list.Add("_Fail");
		}
		else
		{
			list.Add("_Fail");
		}
		return list;
	}

	// Token: 0x06000810 RID: 2064 RVA: 0x000257B4 File Offset: 0x000239B4
	public override IList<TIRegionAlienAssetState> GetValidTargets(TIMissionTemplate mission, TICouncilorState councilor)
	{
		List<TIRegionAlienAssetState> list = new List<TIRegionAlienAssetState>();
		foreach (TIRegionAlienAssetState tiregionAlienAssetState in this.GetAllPotentialTargets(null))
		{
			if (base.ValidTarget(this.ValidateSingleTarget(mission, councilor, tiregionAlienAssetState)))
			{
				list.Add(tiregionAlienAssetState);
			}
		}
		return list;
	}
}

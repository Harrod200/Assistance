using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000277 RID: 631
public class TIMissionTarget_RegionBase : MissionTarget<TIGameState>
{
	// Token: 0x06000844 RID: 2116 RVA: 0x0002690C File Offset: 0x00024B0C
	public override TIFactionState GetRelevantFaction(TIGameState target)
	{
		return target.ref_faction;
	}

	// Token: 0x06000845 RID: 2117 RVA: 0x00026914 File Offset: 0x00024B14
	public override List<string> ValidateSingleTarget(TIMissionTemplate mission, TICouncilorState councilor, TIGameState target)
	{
		List<string> list = new List<string>();
		if (target.isRegionState)
		{
			TIRegionState ref_region = target.ref_region;
			using (List<TIMissionCondition>.Enumerator enumerator = mission.conditions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIMissionCondition timissionCondition = enumerator.Current;
					list.Add(timissionCondition.CanTarget(councilor, ref_region));
				}
				return list;
			}
		}
		if (target.isHabState && target.ref_hab.IsBase)
		{
			TIHabState ref_hab = target.ref_hab;
			foreach (TIMissionCondition timissionCondition2 in mission.conditions)
			{
				list.Add(timissionCondition2.CanTarget(councilor, ref_hab));
			}
		}
		return list;
	}

	// Token: 0x06000846 RID: 2118 RVA: 0x000269F0 File Offset: 0x00024BF0
	public override IEnumerable<TIGameState> GetAllPotentialTargets(TIFactionState faction)
	{
		List<TIGameState> list = new List<TIGameState>();
		list.AddRange(faction.KnownBases);
		list.AddRange(GameStateManager.AllRegions());
		return list;
	}

	// Token: 0x06000847 RID: 2119 RVA: 0x00026A10 File Offset: 0x00024C10
	public override IList<TIGameState> GetValidTargets(TIMissionTemplate mission, TICouncilorState councilor)
	{
		List<TIGameState> list = new List<TIGameState>();
		foreach (TIGameState tigameState in this.GetAllPotentialTargets(councilor.faction))
		{
			if (base.ValidTarget(this.ValidateSingleTarget(mission, councilor, tigameState)))
			{
				list.Add(tigameState);
			}
		}
		return list;
	}
}

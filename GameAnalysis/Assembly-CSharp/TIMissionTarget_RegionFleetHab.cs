using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000274 RID: 628
public class TIMissionTarget_RegionFleetHab : MissionTarget<TIGameState>
{
	// Token: 0x06000835 RID: 2101 RVA: 0x000262E8 File Offset: 0x000244E8
	public override TIFactionState GetRelevantFaction(TIGameState target)
	{
		return target.ref_faction;
	}

	// Token: 0x06000836 RID: 2102 RVA: 0x000262F0 File Offset: 0x000244F0
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
		if (target.isHabState)
		{
			TIHabState ref_hab = target.ref_hab;
			using (List<TIMissionCondition>.Enumerator enumerator = mission.conditions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIMissionCondition timissionCondition2 = enumerator.Current;
					list.Add(timissionCondition2.CanTarget(councilor, ref_hab));
				}
				return list;
			}
		}
		if (target.isSpaceFleetState || target.isSpaceShipState)
		{
			TISpaceFleetState ref_fleet = target.ref_fleet;
			using (List<TIMissionCondition>.Enumerator enumerator = mission.conditions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIMissionCondition timissionCondition3 = enumerator.Current;
					list.Add(timissionCondition3.CanTarget(councilor, ref_fleet));
				}
				return list;
			}
		}
		list.Add("_Fail");
		return list;
	}

	// Token: 0x06000837 RID: 2103 RVA: 0x00026430 File Offset: 0x00024630
	public override IEnumerable<TIGameState> GetAllPotentialTargets(TIFactionState faction)
	{
		List<TIGameState> list = new List<TIGameState>();
		list.AddRange(faction.KnownHabs);
		list.AddRange(GameStateManager.AllRegions());
		list.AddRange(faction.KnownFleets);
		return list;
	}

	// Token: 0x06000838 RID: 2104 RVA: 0x0002645C File Offset: 0x0002465C
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

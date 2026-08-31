using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000275 RID: 629
public class TIMissionTarget_RegionCouncilorHab : MissionTarget<TIGameState>
{
	// Token: 0x0600083A RID: 2106 RVA: 0x000264D0 File Offset: 0x000246D0
	public override TIFactionState GetRelevantFaction(TIGameState target)
	{
		return target.ref_faction;
	}

	// Token: 0x0600083B RID: 2107 RVA: 0x000264D8 File Offset: 0x000246D8
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
		if (target.isCouncilorState)
		{
			TICouncilorState ref_councilor = target.ref_councilor;
			using (List<TIMissionCondition>.Enumerator enumerator = mission.conditions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIMissionCondition timissionCondition3 = enumerator.Current;
					list.Add(timissionCondition3.CanTarget(councilor, ref_councilor));
				}
				return list;
			}
		}
		list.Add("_Fail");
		return list;
	}

	// Token: 0x0600083C RID: 2108 RVA: 0x00026610 File Offset: 0x00024810
	public override IEnumerable<TIGameState> GetAllPotentialTargets(TIFactionState faction)
	{
		List<TIGameState> list = new List<TIGameState>();
		list.AddRange(faction.KnownHabs);
		list.AddRange(GameStateManager.AllRegions());
		list.AddRange(GameStateManager.AllFactions().SelectMany<TIFactionState, TICouncilorState>((TIFactionState x) => x.councilors));
		return list;
	}

	// Token: 0x0600083D RID: 2109 RVA: 0x00026668 File Offset: 0x00024868
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

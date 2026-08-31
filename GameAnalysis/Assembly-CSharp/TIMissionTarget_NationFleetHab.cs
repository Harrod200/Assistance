using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000272 RID: 626
public class TIMissionTarget_NationFleetHab : MissionTarget<TIGameState>
{
	// Token: 0x0600082B RID: 2091 RVA: 0x00025F94 File Offset: 0x00024194
	public override TIFactionState GetRelevantFaction(TIGameState target)
	{
		return target.ref_faction;
	}

	// Token: 0x0600082C RID: 2092 RVA: 0x00025F9C File Offset: 0x0002419C
	public override List<string> ValidateSingleTarget(TIMissionTemplate mission, TICouncilorState councilor, TIGameState target)
	{
		List<string> list = new List<string>();
		if (target.isNationState)
		{
			TINationState ref_nation = target.ref_nation;
			using (List<TIMissionCondition>.Enumerator enumerator = mission.conditions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIMissionCondition timissionCondition = enumerator.Current;
					list.Add(timissionCondition.CanTarget(councilor, ref_nation));
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
		if (target.isSpaceFleetState)
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

	// Token: 0x0600082D RID: 2093 RVA: 0x000260D4 File Offset: 0x000242D4
	public override IEnumerable<TIGameState> GetAllPotentialTargets(TIFactionState faction)
	{
		List<TIGameState> list = new List<TIGameState>();
		list.AddRange(faction.KnownHabs);
		list.AddRange(GameStateManager.AllExtantNations());
		list.AddRange(faction.KnownFleets);
		return list;
	}

	// Token: 0x0600082E RID: 2094 RVA: 0x00026100 File Offset: 0x00024300
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

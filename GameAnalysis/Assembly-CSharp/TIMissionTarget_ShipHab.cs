using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000278 RID: 632
public class TIMissionTarget_ShipHab : MissionTarget<TIGameState>
{
	// Token: 0x06000849 RID: 2121 RVA: 0x00026A84 File Offset: 0x00024C84
	public override TIFactionState GetRelevantFaction(TIGameState target)
	{
		return target.ref_faction;
	}

	// Token: 0x0600084A RID: 2122 RVA: 0x00026A8C File Offset: 0x00024C8C
	public override List<string> ValidateSingleTarget(TIMissionTemplate mission, TICouncilorState councilor, TIGameState target)
	{
		List<string> list = new List<string>();
		if (target.isHabState)
		{
			TIHabState ref_hab = target.ref_hab;
			using (List<TIMissionCondition>.Enumerator enumerator = mission.conditions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIMissionCondition timissionCondition = enumerator.Current;
					list.Add(timissionCondition.CanTarget(councilor, ref_hab));
				}
				return list;
			}
		}
		if (target.isSpaceShipState)
		{
			TISpaceShipState ref_ship = target.ref_ship;
			using (List<TIMissionCondition>.Enumerator enumerator = mission.conditions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIMissionCondition timissionCondition2 = enumerator.Current;
					list.Add(timissionCondition2.CanTarget(councilor, ref_ship));
				}
				return list;
			}
		}
		list.Add("_Fail");
		return list;
	}

	// Token: 0x0600084B RID: 2123 RVA: 0x00026B64 File Offset: 0x00024D64
	public override IEnumerable<TIGameState> GetAllPotentialTargets(TIFactionState faction)
	{
		List<TIGameState> list = new List<TIGameState>();
		list.AddRange(faction.KnownHabs);
		list.AddRange(faction.KnownFleets.SelectMany<TISpaceFleetState, TISpaceShipState>((TISpaceFleetState x) => x.ships));
		return list;
	}

	// Token: 0x0600084C RID: 2124 RVA: 0x00026BB4 File Offset: 0x00024DB4
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

using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000271 RID: 625
public class TIMissionTarget_SpaceAsset : MissionTarget<TISpaceAssetState>
{
	// Token: 0x06000826 RID: 2086 RVA: 0x00025E20 File Offset: 0x00024020
	public override TIFactionState GetRelevantFaction(TIGameState target)
	{
		return target.ref_faction;
	}

	// Token: 0x06000827 RID: 2087 RVA: 0x00025E28 File Offset: 0x00024028
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
		if (target.isSpaceFleetState)
		{
			TISpaceFleetState ref_fleet = target.ref_fleet;
			using (List<TIMissionCondition>.Enumerator enumerator = mission.conditions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIMissionCondition timissionCondition2 = enumerator.Current;
					list.Add(timissionCondition2.CanTarget(councilor, ref_fleet));
				}
				return list;
			}
		}
		list.Add("_Fail");
		return list;
	}

	// Token: 0x06000828 RID: 2088 RVA: 0x00025F00 File Offset: 0x00024100
	public override IEnumerable<TISpaceAssetState> GetAllPotentialTargets(TIFactionState faction)
	{
		List<TISpaceAssetState> list = new List<TISpaceAssetState>();
		list.AddRange(faction.KnownHabs);
		list.AddRange(faction.KnownFleets);
		return list;
	}

	// Token: 0x06000829 RID: 2089 RVA: 0x00025F20 File Offset: 0x00024120
	public override IList<TISpaceAssetState> GetValidTargets(TIMissionTemplate mission, TICouncilorState councilor)
	{
		List<TISpaceAssetState> list = new List<TISpaceAssetState>();
		foreach (TISpaceAssetState tispaceAssetState in this.GetAllPotentialTargets(councilor.faction))
		{
			if (base.ValidTarget(this.ValidateSingleTarget(mission, councilor, tispaceAssetState)))
			{
				list.Add(tispaceAssetState);
			}
		}
		return list;
	}
}

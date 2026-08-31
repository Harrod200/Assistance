using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000273 RID: 627
public class TIMissionTarget_NationHab : MissionTarget<TIGameState>
{
	// Token: 0x06000830 RID: 2096 RVA: 0x00026174 File Offset: 0x00024374
	public override TIFactionState GetRelevantFaction(TIGameState target)
	{
		return target.ref_faction;
	}

	// Token: 0x06000831 RID: 2097 RVA: 0x0002617C File Offset: 0x0002437C
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
		list.Add("_Fail");
		return list;
	}

	// Token: 0x06000832 RID: 2098 RVA: 0x00026254 File Offset: 0x00024454
	public override IEnumerable<TIGameState> GetAllPotentialTargets(TIFactionState faction)
	{
		List<TIGameState> list = new List<TIGameState>();
		list.AddRange(faction.KnownHabs);
		list.AddRange(GameStateManager.AllExtantNations());
		return list;
	}

	// Token: 0x06000833 RID: 2099 RVA: 0x00026274 File Offset: 0x00024474
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

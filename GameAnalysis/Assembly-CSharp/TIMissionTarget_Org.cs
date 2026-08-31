using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000270 RID: 624
public class TIMissionTarget_Org : MissionTarget<TIOrgState>
{
	// Token: 0x06000821 RID: 2081 RVA: 0x00025C34 File Offset: 0x00023E34
	public override TIFactionState GetRelevantFaction(TIGameState target)
	{
		return target.ref_faction;
	}

	// Token: 0x06000822 RID: 2082 RVA: 0x00025C3C File Offset: 0x00023E3C
	public override IEnumerable<TIOrgState> GetAllPotentialTargets(TIFactionState faction)
	{
		List<TIOrgState> list = new List<TIOrgState>();
		List<TIFactionState> list2 = GameStateManager.AllFactions().ToList<TIFactionState>();
		list2.Remove(faction);
		Func<TIFactionState, bool> <>9__1;
		Func<TIFactionState, bool> func;
		if ((func = <>9__1) == null)
		{
			func = (<>9__1 = (TIFactionState x) => !x.permanentAlly(faction));
		}
		foreach (TIFactionState tifactionState in list2.Where<TIFactionState>(func))
		{
			list.AddRange(tifactionState.unassignedOrgs);
			foreach (TICouncilorState ticouncilorState in tifactionState.councilors)
			{
				list.AddRange(ticouncilorState.orgs);
			}
		}
		return list.Where<TIOrgState>((TIOrgState x) => x.IsEligibleForFaction(faction));
	}

	// Token: 0x06000823 RID: 2083 RVA: 0x00025D38 File Offset: 0x00023F38
	public override IList<TIOrgState> GetValidTargets(TIMissionTemplate mission, TICouncilorState councilor)
	{
		List<TIOrgState> list = new List<TIOrgState>();
		foreach (TIOrgState tiorgState in this.GetAllPotentialTargets(councilor.faction))
		{
			if (base.ValidTarget(this.ValidateSingleTarget(mission, councilor, tiorgState)))
			{
				list.Add(tiorgState);
			}
		}
		return list;
	}

	// Token: 0x06000824 RID: 2084 RVA: 0x00025DA4 File Offset: 0x00023FA4
	public override List<string> ValidateSingleTarget(TIMissionTemplate mission, TICouncilorState councilor, TIGameState target)
	{
		List<string> list = new List<string>();
		if (target.isOrgState)
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
}

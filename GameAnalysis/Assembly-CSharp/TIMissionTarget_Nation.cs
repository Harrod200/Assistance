using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000268 RID: 616
public class TIMissionTarget_Nation : MissionTarget<TINationState>
{
	// Token: 0x060007F9 RID: 2041 RVA: 0x000251BB File Offset: 0x000233BB
	public override TIFactionState GetRelevantFaction(TIGameState target)
	{
		return target.ref_faction;
	}

	// Token: 0x060007FA RID: 2042 RVA: 0x000251C4 File Offset: 0x000233C4
	public override List<string> ValidateSingleTarget(TIMissionTemplate mission, TICouncilorState councilor, TIGameState target)
	{
		List<string> list = new List<string>();
		if (target.isNationState && target.ref_nation.extant)
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
		list.Add("_Fail");
		return list;
	}

	// Token: 0x060007FB RID: 2043 RVA: 0x0002524C File Offset: 0x0002344C
	public override IEnumerable<TINationState> GetAllPotentialTargets(TIFactionState faction = null)
	{
		return GameStateManager.AllExtantNations();
	}

	// Token: 0x060007FC RID: 2044 RVA: 0x00025254 File Offset: 0x00023454
	public override IList<TINationState> GetValidTargets(TIMissionTemplate mission, TICouncilorState councilor)
	{
		List<TINationState> list = new List<TINationState>();
		foreach (TINationState tinationState in this.GetAllPotentialTargets(null))
		{
			if (base.ValidTarget(this.ValidateSingleTarget(mission, councilor, tinationState)))
			{
				list.Add(tinationState);
			}
		}
		return list;
	}
}

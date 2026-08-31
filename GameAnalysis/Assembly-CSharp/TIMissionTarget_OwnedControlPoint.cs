using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200026E RID: 622
public class TIMissionTarget_OwnedControlPoint : MissionTarget<TIControlPoint>
{
	// Token: 0x06000817 RID: 2071 RVA: 0x000259CC File Offset: 0x00023BCC
	public override TIFactionState GetRelevantFaction(TIGameState target)
	{
		return target.ref_faction;
	}

	// Token: 0x06000818 RID: 2072 RVA: 0x000259D4 File Offset: 0x00023BD4
	public override List<string> ValidateSingleTarget(TIMissionTemplate mission, TICouncilorState councilor, TIGameState target)
	{
		TIControlPoint ref_controlPoint = target.ref_controlPoint;
		List<string> list = new List<string>();
		bool? flag;
		if (ref_controlPoint == null)
		{
			flag = null;
		}
		else
		{
			TINationState nation = ref_controlPoint.nation;
			flag = ((nation != null) ? new bool?(nation.extant) : null);
		}
		bool? flag2 = flag;
		if (flag2.GetValueOrDefault() && ref_controlPoint.EnemyFactionControlPoint(councilor.faction))
		{
			using (List<TIMissionCondition>.Enumerator enumerator = mission.conditions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIMissionCondition timissionCondition = enumerator.Current;
					list.Add(timissionCondition.CanTarget(councilor, ref_controlPoint));
				}
				return list;
			}
		}
		list.Add("_Fail");
		return list;
	}

	// Token: 0x06000819 RID: 2073 RVA: 0x00025A90 File Offset: 0x00023C90
	public override IEnumerable<TIControlPoint> GetAllPotentialTargets(TIFactionState faction = null)
	{
		return GameStateManager.IterateByClass<TIControlPoint>(false);
	}

	// Token: 0x0600081A RID: 2074 RVA: 0x00025A98 File Offset: 0x00023C98
	public override IList<TIControlPoint> GetValidTargets(TIMissionTemplate mission, TICouncilorState councilor)
	{
		List<TIControlPoint> list = new List<TIControlPoint>();
		foreach (TIControlPoint ticontrolPoint in this.GetAllPotentialTargets(null))
		{
			if (base.ValidTarget(this.ValidateSingleTarget(mission, councilor, ticontrolPoint)))
			{
				list.Add(ticontrolPoint);
			}
		}
		return list;
	}
}

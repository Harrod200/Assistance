using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000276 RID: 630
public class TIMissionTarget_EnemyProjectLocation : MissionTarget<TIGameState>
{
	// Token: 0x0600083F RID: 2111 RVA: 0x000266DC File Offset: 0x000248DC
	public override TIFactionState GetRelevantFaction(TIGameState target)
	{
		return target.ref_faction;
	}

	// Token: 0x06000840 RID: 2112 RVA: 0x000266E4 File Offset: 0x000248E4
	public override List<string> ValidateSingleTarget(TIMissionTemplate mission, TICouncilorState councilor, TIGameState target)
	{
		List<string> list = new List<string>();
		if (target.isHabState && target.ref_faction != councilor.faction)
		{
			TIHabState ref_hab = target.ref_hab;
			if (ref_hab.GetNetCurrentMonthlyIncome(ref_hab.ref_faction, FactionResource.Projects, true, false) > 0f)
			{
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
			list.Add("_Fail");
		}
		else if (target.isCouncilorState)
		{
			TICouncilorState ref_councilor = target.ref_councilor;
			if (ref_councilor.faction != null && ref_councilor.faction != councilor.faction && councilor.faction.GetIntel(ref_councilor) >= TemplateManager.global.intelToSeeCouncilorBasicData)
			{
				using (List<TIMissionCondition>.Enumerator enumerator = mission.conditions.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIMissionCondition timissionCondition2 = enumerator.Current;
						list.Add(timissionCondition2.CanTarget(councilor, ref_councilor));
					}
					return list;
				}
			}
			list.Add("_Fail");
		}
		else
		{
			list.Add("_Fail");
		}
		return list;
	}

	// Token: 0x06000841 RID: 2113 RVA: 0x00026848 File Offset: 0x00024A48
	public override IEnumerable<TIGameState> GetAllPotentialTargets(TIFactionState faction)
	{
		List<TIGameState> list = new List<TIGameState>();
		list.AddRange(faction.KnownHabs);
		list.AddRange(GameStateManager.AllFactions().SelectMany<TIFactionState, TICouncilorState>((TIFactionState x) => x.councilors));
		return list;
	}

	// Token: 0x06000842 RID: 2114 RVA: 0x00026898 File Offset: 0x00024A98
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

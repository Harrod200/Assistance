using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000279 RID: 633
public class TIMissionTarget_EnemyHabModule : MissionTarget<TIHabModuleState>
{
	// Token: 0x0600084E RID: 2126 RVA: 0x00026C28 File Offset: 0x00024E28
	public override TIFactionState GetRelevantFaction(TIGameState target)
	{
		return target.ref_faction;
	}

	// Token: 0x0600084F RID: 2127 RVA: 0x00026C30 File Offset: 0x00024E30
	public override List<string> ValidateSingleTarget(TIMissionTemplate mission, TICouncilorState councilor, TIGameState target)
	{
		List<string> list = new List<string>();
		if (target.isHabModuleState && target.ref_faction != councilor.faction)
		{
			TIHabModuleState ref_habModule = target.ref_habModule;
			using (List<TIMissionCondition>.Enumerator enumerator = mission.conditions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIMissionCondition timissionCondition = enumerator.Current;
					list.Add(timissionCondition.CanTarget(councilor, ref_habModule));
				}
				return list;
			}
		}
		list.Add("_Fail");
		return list;
	}

	// Token: 0x06000850 RID: 2128 RVA: 0x00026CC0 File Offset: 0x00024EC0
	public override IEnumerable<TIHabModuleState> GetAllPotentialTargets(TIFactionState faction)
	{
		return faction.KnownHabs.SelectMany<TIHabState, TIHabModuleState>((TIHabState x) => x.OkayModules());
	}

	// Token: 0x06000851 RID: 2129 RVA: 0x00026CEC File Offset: 0x00024EEC
	public override IList<TIHabModuleState> GetValidTargets(TIMissionTemplate mission, TICouncilorState councilor)
	{
		List<TIHabModuleState> list = new List<TIHabModuleState>();
		foreach (TIHabModuleState tihabModuleState in this.GetAllPotentialTargets(councilor.faction))
		{
			if (base.ValidTarget(this.ValidateSingleTarget(mission, councilor, tihabModuleState)))
			{
				list.Add(tihabModuleState);
			}
		}
		return list;
	}
}

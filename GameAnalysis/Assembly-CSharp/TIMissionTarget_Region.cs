using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000269 RID: 617
public class TIMissionTarget_Region : MissionTarget<TIRegionState>
{
	// Token: 0x060007FE RID: 2046 RVA: 0x000252C4 File Offset: 0x000234C4
	public override TIFactionState GetRelevantFaction(TIGameState target)
	{
		return target.ref_faction;
	}

	// Token: 0x060007FF RID: 2047 RVA: 0x000252CC File Offset: 0x000234CC
	public override List<string> ValidateSingleTarget(TIMissionTemplate mission, TICouncilorState councilor, TIGameState target)
	{
		TIRegionState ref_region = target.ref_region;
		List<string> list = new List<string>();
		foreach (TIMissionCondition timissionCondition in mission.conditions)
		{
			list.Add(timissionCondition.CanTarget(councilor, ref_region));
		}
		return list;
	}

	// Token: 0x06000800 RID: 2048 RVA: 0x00025334 File Offset: 0x00023534
	public override IEnumerable<TIRegionState> GetAllPotentialTargets(TIFactionState faction = null)
	{
		return GameStateManager.AllRegions();
	}

	// Token: 0x06000801 RID: 2049 RVA: 0x0002533C File Offset: 0x0002353C
	public override IList<TIRegionState> GetValidTargets(TIMissionTemplate mission, TICouncilorState councilor)
	{
		List<TIRegionState> list = new List<TIRegionState>();
		foreach (TIRegionState tiregionState in this.GetAllPotentialTargets(null))
		{
			if (base.ValidTarget(this.ValidateSingleTarget(mission, councilor, tiregionState)))
			{
				list.Add(tiregionState);
			}
		}
		return list;
	}
}

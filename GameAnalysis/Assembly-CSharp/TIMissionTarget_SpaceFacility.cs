using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200026B RID: 619
public class TIMissionTarget_SpaceFacility : MissionTarget<TIRegionSpaceFacilityState>
{
	// Token: 0x06000808 RID: 2056 RVA: 0x000254B7 File Offset: 0x000236B7
	public override TIFactionState GetRelevantFaction(TIGameState target)
	{
		return target.ref_faction;
	}

	// Token: 0x06000809 RID: 2057 RVA: 0x000254C0 File Offset: 0x000236C0
	public override List<string> ValidateSingleTarget(TIMissionTemplate mission, TICouncilorState councilor, TIGameState target)
	{
		TIRegionSpaceFacilityState ref_regionSpaceFacility = target.ref_regionSpaceFacility;
		List<string> list = new List<string>();
		if (ref_regionSpaceFacility.Extant())
		{
			using (List<TIMissionCondition>.Enumerator enumerator = mission.conditions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIMissionCondition timissionCondition = enumerator.Current;
					list.Add(timissionCondition.CanTarget(councilor, ref_regionSpaceFacility));
				}
				return list;
			}
		}
		list.Add("_Fail");
		return list;
	}

	// Token: 0x0600080A RID: 2058 RVA: 0x0002553C File Offset: 0x0002373C
	public override IEnumerable<TIRegionSpaceFacilityState> GetAllPotentialTargets(TIFactionState faction = null)
	{
		return GameStateManager.IterateByClass<TIRegionSpaceFacilityState>(true);
	}

	// Token: 0x0600080B RID: 2059 RVA: 0x00025544 File Offset: 0x00023744
	public override IList<TIRegionSpaceFacilityState> GetValidTargets(TIMissionTemplate mission, TICouncilorState councilor)
	{
		List<TIRegionSpaceFacilityState> list = new List<TIRegionSpaceFacilityState>();
		foreach (TIRegionSpaceFacilityState tiregionSpaceFacilityState in this.GetAllPotentialTargets(null))
		{
			if (base.ValidTarget(this.ValidateSingleTarget(mission, councilor, tiregionSpaceFacilityState)))
			{
				list.Add(tiregionSpaceFacilityState);
			}
		}
		return list;
	}
}

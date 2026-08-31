using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001D6 RID: 470
public class TIMissionEffect_DetectCouncilActivity : TIMissionEffect
{
	// Token: 0x06000698 RID: 1688 RVA: 0x0001EB70 File Offset: 0x0001CD70
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		TICouncilorState councilor = mission.councilor;
		if (target.isRegionState)
		{
			TIRegionState ref_region = target.ref_region;
			List<TIRegionState> list = new List<TIRegionState>();
			list.Add(ref_region);
			list.AddRange(ref_region.AdjacentRegions(false));
			using (List<TIRegionState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIRegionState tiregionState = enumerator.Current;
					councilor.faction.SetIntel(tiregionState, 1f, null, false);
					if (tiregionState.alienFacility.Extant() && !tiregionState.alienFacility.VisibleToFaction(councilor.faction))
					{
						councilor.faction.SetIntel(tiregionState.alienFacility, 1f, null, false);
					}
					if (tiregionState.xenoforming.Extant() && !tiregionState.xenoforming.VisibleToFaction(councilor.faction))
					{
						councilor.faction.SetIntel(tiregionState.xenoforming, 1f, mission.councilor, false);
					}
				}
				goto IL_00FC;
			}
		}
		councilor.faction.SetIntel(target, 1f, null, false);
		IL_00FC:
		return string.Empty;
	}
}

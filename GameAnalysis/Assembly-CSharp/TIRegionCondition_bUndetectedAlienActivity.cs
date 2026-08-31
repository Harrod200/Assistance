using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200008F RID: 143
public class TIRegionCondition_bUndetectedAlienActivity : TIRegionCondition
{
	// Token: 0x060002F4 RID: 756 RVA: 0x00011B1C File Offset: 0x0000FD1C
	public override bool PassesCondition(TIGameState state)
	{
		if (state.ref_region != null && state.ref_faction != null)
		{
			foreach (TICouncilorState ticouncilorState in GameStateManager.AlienFaction().councilors)
			{
				if (ticouncilorState.OnEarth && (ticouncilorState.ref_region == state.ref_region || ticouncilorState.ref_region.IsAdjacent(state.ref_region, false)))
				{
					TIMissionState timissionState = ticouncilorState.activeMission ?? ticouncilorState.completedMission;
					if (timissionState != null && state.ref_faction.CanDetectAlienMission(timissionState.missionTemplate) && !state.ref_region.alienActivity.VisibleToFaction(state.ref_faction))
					{
						return true;
					}
				}
			}
			return false;
		}
		return false;
	}
}

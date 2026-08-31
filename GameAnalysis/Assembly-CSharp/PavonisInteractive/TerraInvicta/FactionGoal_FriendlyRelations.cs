using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000737 RID: 1847
	public abstract class FactionGoal_FriendlyRelations : FactionGoal_Faction
	{
		// Token: 0x170006E2 RID: 1762
		// (get) Token: 0x06002E7F RID: 11903 RVA: 0x000FD469 File Offset: 0x000FB669
		// (set) Token: 0x06002E80 RID: 11904 RVA: 0x000FD471 File Offset: 0x000FB671
		public float yesterdaysHate { get; protected set; }

		// Token: 0x06002E81 RID: 11905 RVA: 0x000FD47C File Offset: 0x000FB67C
		public bool CheckAbortMissionForViolationOfPact(TICouncilorState councilor)
		{
			return councilor.HasMission && this.missionPayoffMultipliersAgainstTarget != null && councilor.activeMission.target.ref_faction == base.targetFaction && this.missionPayoffMultipliersAgainstTarget.ContainsKey(councilor.activeMission.missionTemplate.dataName) && this.missionPayoffMultipliersAgainstTarget[councilor.activeMission.missionTemplate.dataName] <= 0f;
		}
	}
}

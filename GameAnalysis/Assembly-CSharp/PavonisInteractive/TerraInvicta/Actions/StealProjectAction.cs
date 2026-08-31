using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A9E RID: 2718
	public class StealProjectAction : PlayerAction
	{
		// Token: 0x06006593 RID: 26003 RVA: 0x002FD551 File Offset: 0x002FB751
		public StealProjectAction(TIMissionState mission, TIProjectTemplate project)
		{
			this.missionID = mission.ID;
			this.project = project;
		}

		// Token: 0x06006594 RID: 26004 RVA: 0x002FD56C File Offset: 0x002FB76C
		public override void Execute()
		{
			TIMissionState state = this.missionID.GetState<TIMissionState>(false);
			TIFactionState faction = state.councilor.faction;
			TIPromptQueueState.RemovePromptStatic(faction, state.councilor, state, "PromptStealProject", 0);
			faction.AddAvailableProject(this.project.dataName);
			faction.AddSuspicionForMajorReversal(5f, null);
			TINotificationQueueState.LogMyTechStolen(faction, state.target.ref_faction, this.project, state.missionTemplate.hate[(int)state.missionOutcome]);
			if (faction.isActivePlayer)
			{
				faction.UnlockAchievement("stealTechnology");
			}
		}

		// Token: 0x040047E4 RID: 18404
		private GameStateID missionID;

		// Token: 0x040047E5 RID: 18405
		private TIProjectTemplate project;
	}
}

using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A81 RID: 2689
	public class SabotageProjectAction : PlayerAction
	{
		// Token: 0x06006556 RID: 25942 RVA: 0x002FC535 File Offset: 0x002FA735
		public SabotageProjectAction(TIMissionState mission, TIProjectTemplate project)
		{
			this.missionID = mission.ID;
			this.project = project;
		}

		// Token: 0x06006557 RID: 25943 RVA: 0x002FC550 File Offset: 0x002FA750
		public override void Execute()
		{
			TIMissionState state = this.missionID.GetState<TIMissionState>(false);
			TIPromptQueueState.RemovePromptStatic(state.councilor.faction, state.councilor, state, "PromptSabotageProject", 0);
			state.target.ref_faction.SufferProjectSabotage(this.project);
			TINotificationQueueState.LogMyTechSabotaged(state.councilor.faction, state.target.ref_faction, this.project, state.missionTemplate.hate[(int)state.missionOutcome]);
			if (state.councilor.faction.isActivePlayer)
			{
				state.councilor.faction.UnlockAchievement("sabotageResearch");
			}
		}

		// Token: 0x04004788 RID: 18312
		private GameStateID missionID;

		// Token: 0x04004789 RID: 18313
		private TIProjectTemplate project;
	}
}

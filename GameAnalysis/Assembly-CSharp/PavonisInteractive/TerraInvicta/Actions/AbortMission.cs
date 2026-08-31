using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A49 RID: 2633
	public class AbortMission : PlayerAction
	{
		// Token: 0x060064DE RID: 25822 RVA: 0x002FA208 File Offset: 0x002F8408
		public AbortMission(TICouncilorState councilor, bool postProcessAbort, TIMissionState.AbortReason reason, TIMissionState mission = null, string abortDetail = "")
		{
			this.councilorID = councilor.ID;
			if (mission != null)
			{
				this.missionID = mission.ID;
			}
			this.postProcessAbort = postProcessAbort;
			this.reason = reason;
			this.abortDetail = abortDetail;
		}

		// Token: 0x060064DF RID: 25823 RVA: 0x002FA254 File Offset: 0x002F8454
		public override void Execute()
		{
			TICouncilorState state = this.councilorID.GetState<TICouncilorState>(false);
			if (state.HasMission)
			{
				state.activeMission.ResolveMission(this.reason, this.abortDetail);
				return;
			}
			if (this.postProcessAbort)
			{
				TIMissionState state2 = this.missionID.GetState<TIMissionState>(false);
				state2.ref_faction.AddToCurrentResource(state2.resources, state2.missionTemplate.cost.resourceType, false, null);
			}
		}

		// Token: 0x040046EF RID: 18159
		private GameStateID councilorID;

		// Token: 0x040046F0 RID: 18160
		private GameStateID missionID;

		// Token: 0x040046F1 RID: 18161
		private bool postProcessAbort;

		// Token: 0x040046F2 RID: 18162
		private TIMissionState.AbortReason reason;

		// Token: 0x040046F3 RID: 18163
		private string abortDetail;
	}
}

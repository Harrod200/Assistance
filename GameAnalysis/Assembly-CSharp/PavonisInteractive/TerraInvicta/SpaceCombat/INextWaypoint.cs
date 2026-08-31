using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009ED RID: 2541
	public interface INextWaypoint : IPathDetail
	{
		// Token: 0x1700109D RID: 4253
		// (get) Token: 0x0600603B RID: 24635
		ITrajectory ValidTrajectorySequence { get; }

		// Token: 0x1700109E RID: 4254
		// (get) Token: 0x0600603C RID: 24636
		bool IsRecursivelyLocked { get; }

		// Token: 0x0600603D RID: 24637
		bool IsRecursiveStartChangeViable(IWaypoint changeProposal);

		// Token: 0x0600603E RID: 24638
		void ResetLocksRecursive();

		// Token: 0x0600603F RID: 24639
		void SetPreviousWaypoint(IPreviousWaypoint previousWaypoint);

		// Token: 0x06006040 RID: 24640
		void RecalculateTrajectoryPathRecursive();

		// Token: 0x06006041 RID: 24641
		void ResumePreviousTargetPosition(Vector3 targetDisplacement);

		// Token: 0x06006042 RID: 24642
		void CacheWaypointOrientationRecursively();

		// Token: 0x06006043 RID: 24643
		void AllignToTrajectoryPathRecursively(WaypointTrajectorySequence sequence, TIDateTime endTime, Vector3 targetDisplacement);

		// Token: 0x06006044 RID: 24644
		void HoldRecursively();

		// Token: 0x06006045 RID: 24645
		void UpdatePathRender(TIDateTime timingStart, Camera cam, Vector3 shipPosition);
	}
}

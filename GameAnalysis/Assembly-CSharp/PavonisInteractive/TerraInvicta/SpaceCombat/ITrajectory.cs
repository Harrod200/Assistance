using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009EC RID: 2540
	public interface ITrajectory : IPathDetail
	{
		// Token: 0x06006039 RID: 24633
		ITrajectory TrajectoryAt(TIDateTime time);

		// Token: 0x0600603A RID: 24634
		void UpdatePathNodes(TIDateTime timingCutoff, Camera cam, Vector3 shipPosition);
	}
}

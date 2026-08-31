using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009EE RID: 2542
	public interface IMovableWaypoint : IWaypoint
	{
		// Token: 0x1700109F RID: 4255
		// (get) Token: 0x06006046 RID: 24646
		// (set) Token: 0x06006047 RID: 24647
		bool IsInputLocked { get; set; }

		// Token: 0x14000028 RID: 40
		// (add) Token: 0x06006048 RID: 24648
		// (remove) Token: 0x06006049 RID: 24649
		event Action OnPositionRotationChange;

		// Token: 0x0600604A RID: 24650
		bool ProposePlacement(Vector3 position, AccelerationConstraints overrideConstraints = null, bool preserveRoll = false, float forceAcceleration = -1f);

		// Token: 0x0600604B RID: 24651
		bool ProposeHeading(Vector3 heading);

		// Token: 0x0600604C RID: 24652
		void ResetNextWaypointSequence();

		// Token: 0x0600604D RID: 24653
		bool RequestRemoval();
	}
}

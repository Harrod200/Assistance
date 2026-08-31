using System;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009E9 RID: 2537
	public interface IPreviousWaypoint : IWaypoint
	{
		// Token: 0x06006028 RID: 24616
		void SetNextWaypoint(INextWaypoint nextWaypoint);

		// Token: 0x06006029 RID: 24617
		void ResetNextWaypointSequence();
	}
}

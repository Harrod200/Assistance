using System;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009EA RID: 2538
	public interface IPreviousTrajectory : ITrajectory, IPathDetail, IWaypoint
	{
		// Token: 0x0600602A RID: 24618
		void SetNextTrajectory(ITrajectory nextTrajectory);
	}
}

using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007CA RID: 1994
	public abstract class Trajectory_WithOrbitalElements : Trajectory
	{
		// Token: 0x0600478A RID: 18314 RVA: 0x001D337C File Offset: 0x001D157C
		public override bool HasOrbitalElements()
		{
			return true;
		}

		// Token: 0x04002979 RID: 10617
		public OrbitalElementsState transferOrbit;
	}
}

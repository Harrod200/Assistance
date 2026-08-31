using System;
using PavonisInteractive.TerraInvicta.SpaceCombat;

// Token: 0x02000406 RID: 1030
public static class IWaypointExtension
{
	// Token: 0x0600152B RID: 5419 RVA: 0x00067184 File Offset: 0x00065384
	public static string ToDetailedString(this IWaypoint wp)
	{
		return string.Concat(new string[]
		{
			"P=",
			wp.Position.ToDetailedString(),
			", V=",
			wp.Velocity.ToDetailedString(),
			", H=",
			wp.Heading.ToDetailedString(),
			", T=",
			wp.Timing.ToString()
		});
	}
}

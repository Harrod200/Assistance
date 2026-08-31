using System;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009E8 RID: 2536
	public interface IProposedWaypoint : IWaypoint
	{
		// Token: 0x1700109B RID: 4251
		// (get) Token: 0x06006025 RID: 24613
		// (set) Token: 0x06006026 RID: 24614
		bool RotationAllowed { get; set; }

		// Token: 0x1700109C RID: 4252
		// (get) Token: 0x06006027 RID: 24615
		bool IsPositionLocked { get; }
	}
}

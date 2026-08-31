using System;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009EF RID: 2543
	public class ProposedWaypoint : BasicWaypoint, IProposedWaypoint, IWaypoint
	{
		// Token: 0x170010A0 RID: 4256
		// (get) Token: 0x0600604E RID: 24654 RVA: 0x002D5B1A File Offset: 0x002D3D1A
		// (set) Token: 0x0600604F RID: 24655 RVA: 0x002D5B22 File Offset: 0x002D3D22
		public bool RotationAllowed { get; set; } = true;

		// Token: 0x170010A1 RID: 4257
		// (get) Token: 0x06006050 RID: 24656 RVA: 0x002D5B2B File Offset: 0x002D3D2B
		// (set) Token: 0x06006051 RID: 24657 RVA: 0x002D5B33 File Offset: 0x002D3D33
		public bool IsPositionLocked { get; set; }
	}
}

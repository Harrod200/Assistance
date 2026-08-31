using System;
using PavonisInteractive.TerraInvicta.Systems;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Components
{
	// Token: 0x020009CB RID: 2507
	public class MapComponent : MonoBehaviour
	{
		// Token: 0x04004367 RID: 17255
		public TISpaceBodyState State;

		// Token: 0x04004368 RID: 17256
		public MapController MapController;

		// Token: 0x04004369 RID: 17257
		public SpaceObjectController SpaceObjectController;

		// Token: 0x0400436A RID: 17258
		public SpaceObjectLODComponent LodComponentLink;

		// Token: 0x0400436B RID: 17259
		public SpaceObject SpaceObjectLink;
	}
}

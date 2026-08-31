using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Systems
{
	// Token: 0x02000995 RID: 2453
	[Serializable]
	public struct FleetTransferPlan : IComponentData
	{
		// Token: 0x04004258 RID: 16984
		public TISpaceFleetState fleet;

		// Token: 0x04004259 RID: 16985
		public Vector3d StartPoint;

		// Token: 0x0400425A RID: 16986
		public Vector3d EndPoint;

		// Token: 0x0400425B RID: 16987
		public double TotalSeconds;

		// Token: 0x0400425C RID: 16988
		public DateTime StartTime;

		// Token: 0x0400425D RID: 16989
		public DateTime EndTime;

		// Token: 0x0400425E RID: 16990
		public List<Orbit> TransferSegments;

		// Token: 0x0400425F RID: 16991
		public TINaturalSpaceObjectState commonBarycenter;

		// Token: 0x04004260 RID: 16992
		public bool planningOnly;
	}
}

using System;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Systems
{
	// Token: 0x02000990 RID: 2448
	[Serializable]
	public struct SpaceObject : IComponentData
	{
		// Token: 0x04004243 RID: 16963
		public SpaceObjectType ObjectType;

		// Token: 0x04004244 RID: 16964
		public Vector3d Position;

		// Token: 0x04004245 RID: 16965
		public double Mass;

		// Token: 0x04004246 RID: 16966
		public double MeanRadius;

		// Token: 0x04004247 RID: 16967
		public Quaterniond SpatialRotation;

		// Token: 0x04004248 RID: 16968
		public double ModelScale;

		// Token: 0x04004249 RID: 16969
		public double MapScale;

		// Token: 0x0400424A RID: 16970
		public DateTime Epoch;

		// Token: 0x0400424B RID: 16971
		public double SOI;
	}
}

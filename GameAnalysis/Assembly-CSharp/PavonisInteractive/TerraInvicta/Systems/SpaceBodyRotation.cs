using System;
using Unity.Entities;

namespace PavonisInteractive.TerraInvicta.Systems
{
	// Token: 0x02000992 RID: 2450
	[Serializable]
	public struct SpaceBodyRotation : IComponentData
	{
		// Token: 0x0400424E RID: 16974
		public double RotationPeriod_s;

		// Token: 0x0400424F RID: 16975
		public double RotationOffset_rad;
	}
}

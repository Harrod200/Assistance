using System;
using Unity.Entities;

namespace PavonisInteractive.TerraInvicta.Systems
{
	// Token: 0x02000993 RID: 2451
	[Serializable]
	public struct SurfacePosition : IComponentData
	{
		// Token: 0x04004250 RID: 16976
		public double Lat;

		// Token: 0x04004251 RID: 16977
		public double Lng;
	}
}

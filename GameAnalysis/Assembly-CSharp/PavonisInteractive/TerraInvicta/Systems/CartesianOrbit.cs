using System;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Systems
{
	// Token: 0x0200098E RID: 2446
	[Serializable]
	public struct CartesianOrbit : IComponentData
	{
		// Token: 0x0400422D RID: 16941
		public Vector3d Position;

		// Token: 0x0400422E RID: 16942
		public Vector3d Velocity;
	}
}

using System;
using Unity.Entities;

namespace PavonisInteractive.TerraInvicta.Systems
{
	// Token: 0x02000991 RID: 2449
	[Serializable]
	public struct LagrangePoint : IComponentData
	{
		// Token: 0x0400424C RID: 16972
		public Entity RelatedSpaceBody;

		// Token: 0x0400424D RID: 16973
		public int Point;
	}
}

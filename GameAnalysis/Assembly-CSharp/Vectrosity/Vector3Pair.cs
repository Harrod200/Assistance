using System;
using UnityEngine;

namespace Vectrosity
{
	// Token: 0x020004A6 RID: 1190
	public struct Vector3Pair
	{
		// Token: 0x06001AC3 RID: 6851 RVA: 0x0009140C File Offset: 0x0008F60C
		public Vector3Pair(Vector3 point1, Vector3 point2)
		{
			this.p1 = point1;
			this.p2 = point2;
		}

		// Token: 0x040016D1 RID: 5841
		public Vector3 p1;

		// Token: 0x040016D2 RID: 5842
		public Vector3 p2;
	}
}

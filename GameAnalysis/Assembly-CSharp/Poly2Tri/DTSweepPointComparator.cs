using System;
using System.Collections.Generic;

namespace Poly2Tri
{
	// Token: 0x020004CB RID: 1227
	public class DTSweepPointComparator : IComparer<TriangulationPoint>
	{
		// Token: 0x06001C00 RID: 7168 RVA: 0x00096764 File Offset: 0x00094964
		public int Compare(TriangulationPoint p1, TriangulationPoint p2)
		{
			if (p1.Y < p2.Y)
			{
				return -1;
			}
			if (p1.Y > p2.Y)
			{
				return 1;
			}
			if (p1.X < p2.X)
			{
				return -1;
			}
			if (p1.X > p2.X)
			{
				return 1;
			}
			return 0;
		}
	}
}

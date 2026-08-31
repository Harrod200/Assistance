using System;

namespace Poly2Tri
{
	// Token: 0x020004C7 RID: 1223
	public class DTSweepConstraint : TriangulationConstraint
	{
		// Token: 0x06001BDF RID: 7135 RVA: 0x000962EE File Offset: 0x000944EE
		public DTSweepConstraint(TriangulationPoint p1, TriangulationPoint p2)
			: base(p1, p2)
		{
			base.Q.AddEdge(this);
		}
	}
}

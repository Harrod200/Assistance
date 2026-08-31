using System;

namespace Poly2Tri
{
	// Token: 0x020004CC RID: 1228
	public class PointOnEdgeException : NotImplementedException
	{
		// Token: 0x06001C02 RID: 7170 RVA: 0x000967BA File Offset: 0x000949BA
		public PointOnEdgeException(string message, TriangulationPoint a, TriangulationPoint b, TriangulationPoint c)
			: base(message)
		{
			this.A = a;
			this.B = b;
			this.C = c;
		}

		// Token: 0x0400177C RID: 6012
		public readonly TriangulationPoint A;

		// Token: 0x0400177D RID: 6013
		public readonly TriangulationPoint B;

		// Token: 0x0400177E RID: 6014
		public readonly TriangulationPoint C;
	}
}

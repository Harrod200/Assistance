using System;

namespace Poly2Tri
{
	// Token: 0x020004D1 RID: 1233
	public class PolygonPoint : TriangulationPoint
	{
		// Token: 0x06001C77 RID: 7287 RVA: 0x00097950 File Offset: 0x00095B50
		public PolygonPoint(double x, double y)
			: base(x, y)
		{
		}

		// Token: 0x06001C78 RID: 7288 RVA: 0x0009795A File Offset: 0x00095B5A
		public PolygonPoint(double x, double y, float z)
			: base(x, y, z)
		{
		}

		// Token: 0x170003FF RID: 1023
		// (get) Token: 0x06001C79 RID: 7289 RVA: 0x00097965 File Offset: 0x00095B65
		// (set) Token: 0x06001C7A RID: 7290 RVA: 0x0009796D File Offset: 0x00095B6D
		public PolygonPoint Next { get; set; }

		// Token: 0x17000400 RID: 1024
		// (get) Token: 0x06001C7B RID: 7291 RVA: 0x00097976 File Offset: 0x00095B76
		// (set) Token: 0x06001C7C RID: 7292 RVA: 0x0009797E File Offset: 0x00095B7E
		public PolygonPoint Previous { get; set; }

		// Token: 0x06001C7D RID: 7293 RVA: 0x00097987 File Offset: 0x00095B87
		public static Point2D ToBasePoint(PolygonPoint p)
		{
			return p;
		}

		// Token: 0x06001C7E RID: 7294 RVA: 0x0009798A File Offset: 0x00095B8A
		public static TriangulationPoint ToTriangulationPoint(PolygonPoint p)
		{
			return p;
		}

		// Token: 0x04001790 RID: 6032
		public static PolygonPoint zero = new PolygonPoint(0.0, 0.0);
	}
}

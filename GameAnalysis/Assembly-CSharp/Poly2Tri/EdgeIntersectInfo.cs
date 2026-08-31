using System;

namespace Poly2Tri
{
	// Token: 0x020004D4 RID: 1236
	public class EdgeIntersectInfo
	{
		// Token: 0x06001C97 RID: 7319 RVA: 0x00098EF1 File Offset: 0x000970F1
		public EdgeIntersectInfo(Edge edgeOne, Edge edgeTwo, Point2D intersectionPoint)
		{
			this.EdgeOne = edgeOne;
			this.EdgeTwo = edgeTwo;
			this.IntersectionPoint = intersectionPoint;
		}

		// Token: 0x17000402 RID: 1026
		// (get) Token: 0x06001C98 RID: 7320 RVA: 0x00098F0E File Offset: 0x0009710E
		// (set) Token: 0x06001C99 RID: 7321 RVA: 0x00098F16 File Offset: 0x00097116
		public Edge EdgeOne { get; private set; }

		// Token: 0x17000403 RID: 1027
		// (get) Token: 0x06001C9A RID: 7322 RVA: 0x00098F1F File Offset: 0x0009711F
		// (set) Token: 0x06001C9B RID: 7323 RVA: 0x00098F27 File Offset: 0x00097127
		public Edge EdgeTwo { get; private set; }

		// Token: 0x17000404 RID: 1028
		// (get) Token: 0x06001C9C RID: 7324 RVA: 0x00098F30 File Offset: 0x00097130
		// (set) Token: 0x06001C9D RID: 7325 RVA: 0x00098F38 File Offset: 0x00097138
		public Point2D IntersectionPoint { get; private set; }
	}
}

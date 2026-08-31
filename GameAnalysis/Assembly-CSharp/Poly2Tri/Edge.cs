using System;

namespace Poly2Tri
{
	// Token: 0x020004DA RID: 1242
	public class Edge
	{
		// Token: 0x1700041A RID: 1050
		// (get) Token: 0x06001CFE RID: 7422 RVA: 0x0009A606 File Offset: 0x00098806
		// (set) Token: 0x06001CFF RID: 7423 RVA: 0x0009A60E File Offset: 0x0009880E
		public Point2D EdgeStart
		{
			get
			{
				return this.mP;
			}
			set
			{
				this.mP = value;
			}
		}

		// Token: 0x1700041B RID: 1051
		// (get) Token: 0x06001D00 RID: 7424 RVA: 0x0009A617 File Offset: 0x00098817
		// (set) Token: 0x06001D01 RID: 7425 RVA: 0x0009A61F File Offset: 0x0009881F
		public Point2D EdgeEnd
		{
			get
			{
				return this.mQ;
			}
			set
			{
				this.mQ = value;
			}
		}

		// Token: 0x06001D02 RID: 7426 RVA: 0x0009A628 File Offset: 0x00098828
		public Edge()
		{
			this.mP = null;
			this.mQ = null;
		}

		// Token: 0x06001D03 RID: 7427 RVA: 0x0009A63E File Offset: 0x0009883E
		public Edge(Point2D edgeStart, Point2D edgeEnd)
		{
			this.mP = edgeStart;
			this.mQ = edgeEnd;
		}

		// Token: 0x040017AF RID: 6063
		protected Point2D mP;

		// Token: 0x040017B0 RID: 6064
		protected Point2D mQ;
	}
}

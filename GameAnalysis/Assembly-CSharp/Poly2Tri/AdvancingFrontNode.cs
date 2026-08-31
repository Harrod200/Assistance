using System;

namespace Poly2Tri
{
	// Token: 0x020004C4 RID: 1220
	public class AdvancingFrontNode
	{
		// Token: 0x06001BB9 RID: 7097 RVA: 0x00094D28 File Offset: 0x00092F28
		public AdvancingFrontNode(TriangulationPoint point)
		{
			this.Point = point;
			this.Value = point.X;
		}

		// Token: 0x170003CB RID: 971
		// (get) Token: 0x06001BBA RID: 7098 RVA: 0x00094D43 File Offset: 0x00092F43
		public bool HasNext
		{
			get
			{
				return this.Next != null;
			}
		}

		// Token: 0x170003CC RID: 972
		// (get) Token: 0x06001BBB RID: 7099 RVA: 0x00094D4E File Offset: 0x00092F4E
		public bool HasPrev
		{
			get
			{
				return this.Prev != null;
			}
		}

		// Token: 0x04001762 RID: 5986
		public AdvancingFrontNode Next;

		// Token: 0x04001763 RID: 5987
		public AdvancingFrontNode Prev;

		// Token: 0x04001764 RID: 5988
		public double Value;

		// Token: 0x04001765 RID: 5989
		public TriangulationPoint Point;

		// Token: 0x04001766 RID: 5990
		public DelaunayTriangle Triangle;
	}
}

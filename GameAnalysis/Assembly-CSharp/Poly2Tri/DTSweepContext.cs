using System;

namespace Poly2Tri
{
	// Token: 0x020004C8 RID: 1224
	public class DTSweepContext : TriangulationContext
	{
		// Token: 0x170003CD RID: 973
		// (get) Token: 0x06001BE0 RID: 7136 RVA: 0x00096304 File Offset: 0x00094504
		// (set) Token: 0x06001BE1 RID: 7137 RVA: 0x0009630C File Offset: 0x0009450C
		public TriangulationPoint Head { get; set; }

		// Token: 0x170003CE RID: 974
		// (get) Token: 0x06001BE2 RID: 7138 RVA: 0x00096315 File Offset: 0x00094515
		// (set) Token: 0x06001BE3 RID: 7139 RVA: 0x0009631D File Offset: 0x0009451D
		public TriangulationPoint Tail { get; set; }

		// Token: 0x170003CF RID: 975
		// (get) Token: 0x06001BE4 RID: 7140 RVA: 0x00096326 File Offset: 0x00094526
		public override TriangulationAlgorithm Algorithm
		{
			get
			{
				return TriangulationAlgorithm.DTSweep;
			}
		}

		// Token: 0x06001BE5 RID: 7141 RVA: 0x00096329 File Offset: 0x00094529
		public DTSweepContext()
		{
			this.Clear();
		}

		// Token: 0x06001BE6 RID: 7142 RVA: 0x00096363 File Offset: 0x00094563
		public void RemoveFromList(DelaunayTriangle triangle)
		{
			this.Triangles.Remove(triangle);
		}

		// Token: 0x06001BE7 RID: 7143 RVA: 0x00096372 File Offset: 0x00094572
		public void MeshClean(DelaunayTriangle triangle)
		{
			this.MeshCleanReq(triangle);
		}

		// Token: 0x06001BE8 RID: 7144 RVA: 0x0009637C File Offset: 0x0009457C
		private void MeshCleanReq(DelaunayTriangle triangle)
		{
			if (triangle != null && !triangle.IsInterior)
			{
				triangle.IsInterior = true;
				base.Triangulatable.AddTriangle(triangle);
				for (int i = 0; i < 3; i++)
				{
					if (!triangle.EdgeIsConstrained[i])
					{
						this.MeshCleanReq(triangle.Neighbors[i]);
					}
				}
			}
		}

		// Token: 0x06001BE9 RID: 7145 RVA: 0x000963D6 File Offset: 0x000945D6
		public override void Clear()
		{
			base.Clear();
			this.Triangles.Clear();
		}

		// Token: 0x06001BEA RID: 7146 RVA: 0x000963E9 File Offset: 0x000945E9
		public void AddNode(AdvancingFrontNode node)
		{
			this.Front.AddNode(node);
		}

		// Token: 0x06001BEB RID: 7147 RVA: 0x000963F7 File Offset: 0x000945F7
		public void RemoveNode(AdvancingFrontNode node)
		{
			this.Front.RemoveNode(node);
		}

		// Token: 0x06001BEC RID: 7148 RVA: 0x00096405 File Offset: 0x00094605
		public AdvancingFrontNode LocateNode(TriangulationPoint point)
		{
			return this.Front.LocateNode(point);
		}

		// Token: 0x06001BED RID: 7149 RVA: 0x00096414 File Offset: 0x00094614
		public void CreateAdvancingFront()
		{
			DelaunayTriangle delaunayTriangle = new DelaunayTriangle(this.Points[0], this.Tail, this.Head);
			this.Triangles.Add(delaunayTriangle);
			AdvancingFrontNode advancingFrontNode = new AdvancingFrontNode(delaunayTriangle.Points[1]);
			advancingFrontNode.Triangle = delaunayTriangle;
			AdvancingFrontNode advancingFrontNode2 = new AdvancingFrontNode(delaunayTriangle.Points[0]);
			advancingFrontNode2.Triangle = delaunayTriangle;
			AdvancingFrontNode advancingFrontNode3 = new AdvancingFrontNode(delaunayTriangle.Points[2]);
			this.Front = new AdvancingFront(advancingFrontNode, advancingFrontNode3);
			this.Front.AddNode(advancingFrontNode2);
			this.Front.Head.Next = advancingFrontNode2;
			advancingFrontNode2.Next = this.Front.Tail;
			advancingFrontNode2.Prev = this.Front.Head;
			this.Front.Tail.Prev = advancingFrontNode2;
		}

		// Token: 0x06001BEE RID: 7150 RVA: 0x000964EC File Offset: 0x000946EC
		public void MapTriangleToNodes(DelaunayTriangle t)
		{
			for (int i = 0; i < 3; i++)
			{
				if (t.Neighbors[i] == null)
				{
					AdvancingFrontNode advancingFrontNode = this.Front.LocatePoint(t.PointCWFrom(t.Points[i]));
					if (advancingFrontNode != null)
					{
						advancingFrontNode.Triangle = t;
					}
				}
			}
		}

		// Token: 0x06001BEF RID: 7151 RVA: 0x0009653C File Offset: 0x0009473C
		public override void PrepareTriangulation(ITriangulatable t)
		{
			base.PrepareTriangulation(t);
			double num2;
			double num = (num2 = this.Points[0].X);
			double num4;
			double num3 = (num4 = this.Points[0].Y);
			foreach (TriangulationPoint triangulationPoint in this.Points)
			{
				if (triangulationPoint.X > num2)
				{
					num2 = triangulationPoint.X;
				}
				if (triangulationPoint.X < num)
				{
					num = triangulationPoint.X;
				}
				if (triangulationPoint.Y > num4)
				{
					num4 = triangulationPoint.Y;
				}
				if (triangulationPoint.Y < num3)
				{
					num3 = triangulationPoint.Y;
				}
			}
			double num5 = (double)this.ALPHA * (num2 - num);
			double num6 = (double)this.ALPHA * (num4 - num3);
			TriangulationPoint triangulationPoint2 = new TriangulationPoint(num2 + num5, num3 - num6);
			TriangulationPoint triangulationPoint3 = new TriangulationPoint(num - num5, num3 - num6);
			this.Head = triangulationPoint2;
			this.Tail = triangulationPoint3;
			this.Points.Sort(this._comparator);
		}

		// Token: 0x06001BF0 RID: 7152 RVA: 0x0009665C File Offset: 0x0009485C
		public void FinalizeTriangulation()
		{
			base.Triangulatable.AddTriangles(this.Triangles);
			this.Triangles.Clear();
		}

		// Token: 0x06001BF1 RID: 7153 RVA: 0x0009667A File Offset: 0x0009487A
		public override TriangulationConstraint NewConstraint(TriangulationPoint a, TriangulationPoint b)
		{
			return new DTSweepConstraint(a, b);
		}

		// Token: 0x0400176E RID: 5998
		private readonly float ALPHA = 0.3f;

		// Token: 0x0400176F RID: 5999
		public AdvancingFront Front;

		// Token: 0x04001772 RID: 6002
		public DTSweepBasin Basin = new DTSweepBasin();

		// Token: 0x04001773 RID: 6003
		public DTSweepEdgeEvent EdgeEvent = new DTSweepEdgeEvent();

		// Token: 0x04001774 RID: 6004
		private DTSweepPointComparator _comparator = new DTSweepPointComparator();
	}
}

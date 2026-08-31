using System;
using System.Collections.Generic;

namespace Poly2Tri
{
	// Token: 0x020004C2 RID: 1218
	public class DelaunayTriangle
	{
		// Token: 0x170003C9 RID: 969
		// (get) Token: 0x06001B81 RID: 7041 RVA: 0x00094226 File Offset: 0x00092426
		public FixedBitArray3 EdgeIsConstrained
		{
			get
			{
				return this.mEdgeIsConstrained;
			}
		}

		// Token: 0x170003CA RID: 970
		// (get) Token: 0x06001B82 RID: 7042 RVA: 0x0009422E File Offset: 0x0009242E
		// (set) Token: 0x06001B83 RID: 7043 RVA: 0x00094236 File Offset: 0x00092436
		public bool IsInterior { get; set; }

		// Token: 0x06001B84 RID: 7044 RVA: 0x0009423F File Offset: 0x0009243F
		public DelaunayTriangle(TriangulationPoint p1, TriangulationPoint p2, TriangulationPoint p3)
		{
			this.Points[0] = p1;
			this.Points[1] = p2;
			this.Points[2] = p3;
		}

		// Token: 0x06001B85 RID: 7045 RVA: 0x0009426E File Offset: 0x0009246E
		public int IndexOf(TriangulationPoint p)
		{
			int num = this.Points.IndexOf(p);
			if (num == -1)
			{
				throw new Exception("Calling index with a point that doesn't exist in triangle");
			}
			return num;
		}

		// Token: 0x06001B86 RID: 7046 RVA: 0x0009428B File Offset: 0x0009248B
		public int IndexCWFrom(TriangulationPoint p)
		{
			return (this.IndexOf(p) + 2) % 3;
		}

		// Token: 0x06001B87 RID: 7047 RVA: 0x00094298 File Offset: 0x00092498
		public int IndexCCWFrom(TriangulationPoint p)
		{
			return (this.IndexOf(p) + 1) % 3;
		}

		// Token: 0x06001B88 RID: 7048 RVA: 0x000942A5 File Offset: 0x000924A5
		public bool Contains(TriangulationPoint p)
		{
			return this.Points.Contains(p);
		}

		// Token: 0x06001B89 RID: 7049 RVA: 0x000942B4 File Offset: 0x000924B4
		private void MarkNeighbor(TriangulationPoint p1, TriangulationPoint p2, DelaunayTriangle t)
		{
			int num = this.EdgeIndex(p1, p2);
			if (num == -1)
			{
				throw new Exception("Error marking neighbors -- t doesn't contain edge p1-p2!");
			}
			this.Neighbors[num] = t;
		}

		// Token: 0x06001B8A RID: 7050 RVA: 0x000942E8 File Offset: 0x000924E8
		public void MarkNeighbor(DelaunayTriangle t)
		{
			bool flag = t.Contains(this.Points[0]);
			bool flag2 = t.Contains(this.Points[1]);
			bool flag3 = t.Contains(this.Points[2]);
			if (flag2 && flag3)
			{
				this.Neighbors[0] = t;
				t.MarkNeighbor(this.Points[1], this.Points[2], this);
				return;
			}
			if (flag && flag3)
			{
				this.Neighbors[1] = t;
				t.MarkNeighbor(this.Points[0], this.Points[2], this);
				return;
			}
			if (flag && flag2)
			{
				this.Neighbors[2] = t;
				t.MarkNeighbor(this.Points[0], this.Points[1], this);
				return;
			}
			throw new Exception("Failed to mark neighbor, doesn't share an edge!");
		}

		// Token: 0x06001B8B RID: 7051 RVA: 0x000943D0 File Offset: 0x000925D0
		public void ClearNeighbors()
		{
			this.Neighbors[0] = (this.Neighbors[1] = (this.Neighbors[2] = null));
		}

		// Token: 0x06001B8C RID: 7052 RVA: 0x00094408 File Offset: 0x00092608
		public void ClearNeighbor(DelaunayTriangle triangle)
		{
			if (this.Neighbors[0] == triangle)
			{
				this.Neighbors[0] = null;
				return;
			}
			if (this.Neighbors[1] == triangle)
			{
				this.Neighbors[1] = null;
				return;
			}
			if (this.Neighbors[2] == triangle)
			{
				this.Neighbors[2] = null;
			}
		}

		// Token: 0x06001B8D RID: 7053 RVA: 0x0009446C File Offset: 0x0009266C
		public void Clear()
		{
			for (int i = 0; i < 3; i++)
			{
				DelaunayTriangle delaunayTriangle = this.Neighbors[i];
				if (delaunayTriangle != null)
				{
					delaunayTriangle.ClearNeighbor(this);
				}
			}
			this.ClearNeighbors();
			this.Points[0] = (this.Points[1] = (this.Points[2] = null));
		}

		// Token: 0x06001B8E RID: 7054 RVA: 0x000944CD File Offset: 0x000926CD
		public TriangulationPoint OppositePoint(DelaunayTriangle t, TriangulationPoint p)
		{
			return this.PointCWFrom(t.PointCWFrom(p));
		}

		// Token: 0x06001B8F RID: 7055 RVA: 0x000944DC File Offset: 0x000926DC
		public DelaunayTriangle NeighborCWFrom(TriangulationPoint point)
		{
			return this.Neighbors[(this.Points.IndexOf(point) + 1) % 3];
		}

		// Token: 0x06001B90 RID: 7056 RVA: 0x000944F9 File Offset: 0x000926F9
		public DelaunayTriangle NeighborCCWFrom(TriangulationPoint point)
		{
			return this.Neighbors[(this.Points.IndexOf(point) + 2) % 3];
		}

		// Token: 0x06001B91 RID: 7057 RVA: 0x00094516 File Offset: 0x00092716
		public DelaunayTriangle NeighborAcrossFrom(TriangulationPoint point)
		{
			return this.Neighbors[this.Points.IndexOf(point)];
		}

		// Token: 0x06001B92 RID: 7058 RVA: 0x0009452F File Offset: 0x0009272F
		public TriangulationPoint PointCCWFrom(TriangulationPoint point)
		{
			return this.Points[(this.IndexOf(point) + 1) % 3];
		}

		// Token: 0x06001B93 RID: 7059 RVA: 0x00094547 File Offset: 0x00092747
		public TriangulationPoint PointCWFrom(TriangulationPoint point)
		{
			return this.Points[(this.IndexOf(point) + 2) % 3];
		}

		// Token: 0x06001B94 RID: 7060 RVA: 0x00094560 File Offset: 0x00092760
		private void RotateCW()
		{
			TriangulationPoint triangulationPoint = this.Points[2];
			this.Points[2] = this.Points[1];
			this.Points[1] = this.Points[0];
			this.Points[0] = triangulationPoint;
		}

		// Token: 0x06001B95 RID: 7061 RVA: 0x000945B7 File Offset: 0x000927B7
		public void Legalize(TriangulationPoint oPoint, TriangulationPoint nPoint)
		{
			this.RotateCW();
			this.Points[this.IndexCCWFrom(oPoint)] = nPoint;
		}

		// Token: 0x06001B96 RID: 7062 RVA: 0x000945D4 File Offset: 0x000927D4
		public override string ToString()
		{
			string[] array = new string[5];
			int num = 0;
			TriangulationPoint triangulationPoint = this.Points[0];
			array[num] = ((triangulationPoint != null) ? triangulationPoint.ToString() : null);
			array[1] = ",";
			int num2 = 2;
			TriangulationPoint triangulationPoint2 = this.Points[1];
			array[num2] = ((triangulationPoint2 != null) ? triangulationPoint2.ToString() : null);
			array[3] = ",";
			int num3 = 4;
			TriangulationPoint triangulationPoint3 = this.Points[2];
			array[num3] = ((triangulationPoint3 != null) ? triangulationPoint3.ToString() : null);
			return string.Concat(array);
		}

		// Token: 0x06001B97 RID: 7063 RVA: 0x00094650 File Offset: 0x00092850
		public void MarkNeighborEdges()
		{
			for (int i = 0; i < 3; i++)
			{
				if (this.EdgeIsConstrained[i] && this.Neighbors[i] != null)
				{
					this.Neighbors[i].MarkConstrainedEdge(this.Points[(i + 1) % 3], this.Points[(i + 2) % 3]);
				}
			}
		}

		// Token: 0x06001B98 RID: 7064 RVA: 0x000946BC File Offset: 0x000928BC
		public void MarkEdge(DelaunayTriangle triangle)
		{
			for (int i = 0; i < 3; i++)
			{
				if (this.EdgeIsConstrained[i])
				{
					triangle.MarkConstrainedEdge(this.Points[(i + 1) % 3], this.Points[(i + 2) % 3]);
				}
			}
		}

		// Token: 0x06001B99 RID: 7065 RVA: 0x0009470C File Offset: 0x0009290C
		public void MarkEdge(List<DelaunayTriangle> tList)
		{
			foreach (DelaunayTriangle delaunayTriangle in tList)
			{
				for (int i = 0; i < 3; i++)
				{
					if (delaunayTriangle.EdgeIsConstrained[i])
					{
						this.MarkConstrainedEdge(delaunayTriangle.Points[(i + 1) % 3], delaunayTriangle.Points[(i + 2) % 3]);
					}
				}
			}
		}

		// Token: 0x06001B9A RID: 7066 RVA: 0x00094798 File Offset: 0x00092998
		public void MarkConstrainedEdge(int index)
		{
			this.mEdgeIsConstrained[index] = true;
		}

		// Token: 0x06001B9B RID: 7067 RVA: 0x000947A7 File Offset: 0x000929A7
		public void MarkConstrainedEdge(DTSweepConstraint edge)
		{
			this.MarkConstrainedEdge(edge.P, edge.Q);
		}

		// Token: 0x06001B9C RID: 7068 RVA: 0x000947BC File Offset: 0x000929BC
		public void MarkConstrainedEdge(TriangulationPoint p, TriangulationPoint q)
		{
			int num = this.EdgeIndex(p, q);
			if (num != -1)
			{
				this.mEdgeIsConstrained[num] = true;
			}
		}

		// Token: 0x06001B9D RID: 7069 RVA: 0x000947E4 File Offset: 0x000929E4
		public double Area()
		{
			double num = this.Points[0].X - this.Points[1].X;
			double num2 = this.Points[2].Y - this.Points[1].Y;
			return Math.Abs(num * num2 * 0.5);
		}

		// Token: 0x06001B9E RID: 7070 RVA: 0x0009484C File Offset: 0x00092A4C
		public TriangulationPoint Centroid()
		{
			double num = (this.Points[0].X + this.Points[1].X + this.Points[2].X) / 3.0;
			double num2 = (this.Points[0].Y + this.Points[1].Y + this.Points[2].Y) / 3.0;
			return new TriangulationPoint(num, num2);
		}

		// Token: 0x06001B9F RID: 7071 RVA: 0x000948E0 File Offset: 0x00092AE0
		public int EdgeIndex(TriangulationPoint p1, TriangulationPoint p2)
		{
			int num = this.Points.IndexOf(p1);
			int num2 = this.Points.IndexOf(p2);
			bool flag = num == 0 || num2 == 0;
			bool flag2 = num == 1 || num2 == 1;
			bool flag3 = num == 2 || num2 == 2;
			if (flag2 && flag3)
			{
				return 0;
			}
			if (flag && flag3)
			{
				return 1;
			}
			if (flag && flag2)
			{
				return 2;
			}
			return -1;
		}

		// Token: 0x06001BA0 RID: 7072 RVA: 0x00094940 File Offset: 0x00092B40
		public bool GetConstrainedEdgeCCW(TriangulationPoint p)
		{
			return this.EdgeIsConstrained[(this.IndexOf(p) + 2) % 3];
		}

		// Token: 0x06001BA1 RID: 7073 RVA: 0x00094968 File Offset: 0x00092B68
		public bool GetConstrainedEdgeCW(TriangulationPoint p)
		{
			return this.EdgeIsConstrained[(this.IndexOf(p) + 1) % 3];
		}

		// Token: 0x06001BA2 RID: 7074 RVA: 0x00094990 File Offset: 0x00092B90
		public bool GetConstrainedEdgeAcross(TriangulationPoint p)
		{
			return this.EdgeIsConstrained[this.IndexOf(p)];
		}

		// Token: 0x06001BA3 RID: 7075 RVA: 0x000949B2 File Offset: 0x00092BB2
		protected void SetConstrainedEdge(int idx, bool ce)
		{
			this.mEdgeIsConstrained[idx] = ce;
		}

		// Token: 0x06001BA4 RID: 7076 RVA: 0x000949C4 File Offset: 0x00092BC4
		public void SetConstrainedEdgeCCW(TriangulationPoint p, bool ce)
		{
			int num = (this.IndexOf(p) + 2) % 3;
			this.SetConstrainedEdge(num, ce);
		}

		// Token: 0x06001BA5 RID: 7077 RVA: 0x000949E8 File Offset: 0x00092BE8
		public void SetConstrainedEdgeCW(TriangulationPoint p, bool ce)
		{
			int num = (this.IndexOf(p) + 1) % 3;
			this.SetConstrainedEdge(num, ce);
		}

		// Token: 0x06001BA6 RID: 7078 RVA: 0x00094A0C File Offset: 0x00092C0C
		public void SetConstrainedEdgeAcross(TriangulationPoint p, bool ce)
		{
			int num = this.IndexOf(p);
			this.SetConstrainedEdge(num, ce);
		}

		// Token: 0x06001BA7 RID: 7079 RVA: 0x00094A29 File Offset: 0x00092C29
		public bool GetDelaunayEdgeCCW(TriangulationPoint p)
		{
			return this.EdgeIsDelaunay[(this.IndexOf(p) + 2) % 3];
		}

		// Token: 0x06001BA8 RID: 7080 RVA: 0x00094A41 File Offset: 0x00092C41
		public bool GetDelaunayEdgeCW(TriangulationPoint p)
		{
			return this.EdgeIsDelaunay[(this.IndexOf(p) + 1) % 3];
		}

		// Token: 0x06001BA9 RID: 7081 RVA: 0x00094A59 File Offset: 0x00092C59
		public bool GetDelaunayEdgeAcross(TriangulationPoint p)
		{
			return this.EdgeIsDelaunay[this.IndexOf(p)];
		}

		// Token: 0x06001BAA RID: 7082 RVA: 0x00094A6D File Offset: 0x00092C6D
		public void SetDelaunayEdgeCCW(TriangulationPoint p, bool ce)
		{
			this.EdgeIsDelaunay[(this.IndexOf(p) + 2) % 3] = ce;
		}

		// Token: 0x06001BAB RID: 7083 RVA: 0x00094A86 File Offset: 0x00092C86
		public void SetDelaunayEdgeCW(TriangulationPoint p, bool ce)
		{
			this.EdgeIsDelaunay[(this.IndexOf(p) + 1) % 3] = ce;
		}

		// Token: 0x06001BAC RID: 7084 RVA: 0x00094A9F File Offset: 0x00092C9F
		public void SetDelaunayEdgeAcross(TriangulationPoint p, bool ce)
		{
			this.EdgeIsDelaunay[this.IndexOf(p)] = ce;
		}

		// Token: 0x06001BAD RID: 7085 RVA: 0x00094AB4 File Offset: 0x00092CB4
		public bool GetEdge(int idx, out DTSweepConstraint edge)
		{
			edge = null;
			if (idx < 0 || idx > 2)
			{
				return false;
			}
			TriangulationPoint triangulationPoint = this.Points[(idx + 1) % 3];
			TriangulationPoint triangulationPoint2 = this.Points[(idx + 2) % 3];
			return triangulationPoint.GetEdge(triangulationPoint2, out edge) || triangulationPoint2.GetEdge(triangulationPoint, out edge);
		}

		// Token: 0x06001BAE RID: 7086 RVA: 0x00094B0C File Offset: 0x00092D0C
		public bool GetEdgeCCW(TriangulationPoint p, out DTSweepConstraint edge)
		{
			int num = (this.IndexOf(p) + 2) % 3;
			return this.GetEdge(num, out edge);
		}

		// Token: 0x06001BAF RID: 7087 RVA: 0x00094B30 File Offset: 0x00092D30
		public bool GetEdgeCW(TriangulationPoint p, out DTSweepConstraint edge)
		{
			int num = (this.IndexOf(p) + 1) % 3;
			return this.GetEdge(num, out edge);
		}

		// Token: 0x06001BB0 RID: 7088 RVA: 0x00094B54 File Offset: 0x00092D54
		public bool GetEdgeAcross(TriangulationPoint p, out DTSweepConstraint edge)
		{
			int num = this.IndexOf(p);
			return this.GetEdge(num, out edge);
		}

		// Token: 0x0400175A RID: 5978
		public FixedArray3<TriangulationPoint> Points;

		// Token: 0x0400175B RID: 5979
		public FixedArray3<DelaunayTriangle> Neighbors;

		// Token: 0x0400175C RID: 5980
		private FixedBitArray3 mEdgeIsConstrained;

		// Token: 0x0400175D RID: 5981
		public FixedBitArray3 EdgeIsDelaunay;
	}
}

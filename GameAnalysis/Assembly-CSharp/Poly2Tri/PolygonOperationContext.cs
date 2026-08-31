using System;
using System.Collections.Generic;

namespace Poly2Tri
{
	// Token: 0x020004D6 RID: 1238
	public class PolygonOperationContext
	{
		// Token: 0x17000408 RID: 1032
		// (get) Token: 0x06001CB3 RID: 7347 RVA: 0x000992D4 File Offset: 0x000974D4
		public Point2DList Union
		{
			get
			{
				Point2DList point2DList = null;
				if (!this.mOutput.TryGetValue(1U, out point2DList))
				{
					point2DList = new Point2DList();
					this.mOutput.Add(1U, point2DList);
				}
				return point2DList;
			}
		}

		// Token: 0x17000409 RID: 1033
		// (get) Token: 0x06001CB4 RID: 7348 RVA: 0x00099308 File Offset: 0x00097508
		public Point2DList Intersect
		{
			get
			{
				Point2DList point2DList = null;
				if (!this.mOutput.TryGetValue(2U, out point2DList))
				{
					point2DList = new Point2DList();
					this.mOutput.Add(2U, point2DList);
				}
				return point2DList;
			}
		}

		// Token: 0x1700040A RID: 1034
		// (get) Token: 0x06001CB5 RID: 7349 RVA: 0x0009933C File Offset: 0x0009753C
		public Point2DList Subtract
		{
			get
			{
				Point2DList point2DList = null;
				if (!this.mOutput.TryGetValue(4U, out point2DList))
				{
					point2DList = new Point2DList();
					this.mOutput.Add(4U, point2DList);
				}
				return point2DList;
			}
		}

		// Token: 0x06001CB7 RID: 7351 RVA: 0x00099384 File Offset: 0x00097584
		public void Clear()
		{
			this.mOperations = PolygonUtil.PolyOperation.None;
			this.mOriginalPolygon1 = null;
			this.mOriginalPolygon2 = null;
			this.mPoly1 = null;
			this.mPoly2 = null;
			this.mIntersections = null;
			this.mStartingIndex = -1;
			this.mError = PolygonUtil.PolyUnionError.None;
			this.mPoly1VectorAngles = null;
			this.mPoly2VectorAngles = null;
			this.mOutput = new Dictionary<uint, Point2DList>();
		}

		// Token: 0x06001CB8 RID: 7352 RVA: 0x000993E4 File Offset: 0x000975E4
		public bool Init(PolygonUtil.PolyOperation operations, Point2DList polygon1, Point2DList polygon2)
		{
			this.Clear();
			this.mOperations = operations;
			this.mOriginalPolygon1 = polygon1;
			this.mOriginalPolygon2 = polygon2;
			this.mPoly1 = new Point2DList(polygon1);
			this.mPoly1.WindingOrder = Point2DList.WindingOrderType.CCW;
			this.mPoly2 = new Point2DList(polygon2);
			this.mPoly2.WindingOrder = Point2DList.WindingOrderType.CCW;
			if (!this.VerticesIntersect(this.mPoly1, this.mPoly2, out this.mIntersections))
			{
				this.mError = PolygonUtil.PolyUnionError.NoIntersections;
				return false;
			}
			int count = this.mIntersections.Count;
			for (int i = 0; i < count; i++)
			{
				for (int j = i + 1; j < count; j++)
				{
					if (this.mIntersections[i].EdgeOne.EdgeStart.Equals(this.mIntersections[j].EdgeOne.EdgeStart) && this.mIntersections[i].EdgeOne.EdgeEnd.Equals(this.mIntersections[j].EdgeOne.EdgeEnd))
					{
						this.mIntersections[j].EdgeOne.EdgeStart = this.mIntersections[i].IntersectionPoint;
					}
					if (this.mIntersections[i].EdgeTwo.EdgeStart.Equals(this.mIntersections[j].EdgeTwo.EdgeStart) && this.mIntersections[i].EdgeTwo.EdgeEnd.Equals(this.mIntersections[j].EdgeTwo.EdgeEnd))
					{
						this.mIntersections[j].EdgeTwo.EdgeStart = this.mIntersections[i].IntersectionPoint;
					}
				}
			}
			foreach (EdgeIntersectInfo edgeIntersectInfo in this.mIntersections)
			{
				if (!this.mPoly1.Contains(edgeIntersectInfo.IntersectionPoint))
				{
					this.mPoly1.Insert(this.mPoly1.IndexOf(edgeIntersectInfo.EdgeOne.EdgeStart) + 1, edgeIntersectInfo.IntersectionPoint);
				}
				if (!this.mPoly2.Contains(edgeIntersectInfo.IntersectionPoint))
				{
					this.mPoly2.Insert(this.mPoly2.IndexOf(edgeIntersectInfo.EdgeTwo.EdgeStart) + 1, edgeIntersectInfo.IntersectionPoint);
				}
			}
			this.mPoly1VectorAngles = new List<int>();
			for (int k = 0; k < this.mPoly2.Count; k++)
			{
				this.mPoly1VectorAngles.Add(-1);
			}
			this.mPoly2VectorAngles = new List<int>();
			for (int l = 0; l < this.mPoly1.Count; l++)
			{
				this.mPoly2VectorAngles.Add(-1);
			}
			int num = 0;
			for (;;)
			{
				bool flag = this.PointInPolygonAngle(this.mPoly1[num], this.mPoly2);
				this.mPoly2VectorAngles[num] = (flag ? 1 : 0);
				if (flag)
				{
					break;
				}
				num = this.mPoly1.NextIndex(num);
				if (num == 0)
				{
					goto IL_031E;
				}
			}
			this.mStartingIndex = num;
			IL_031E:
			if (this.mStartingIndex == -1)
			{
				this.mError = PolygonUtil.PolyUnionError.Poly1InsidePoly2;
				return false;
			}
			return true;
		}

		// Token: 0x06001CB9 RID: 7353 RVA: 0x00099734 File Offset: 0x00097934
		private bool VerticesIntersect(Point2DList polygon1, Point2DList polygon2, out List<EdgeIntersectInfo> intersections)
		{
			intersections = new List<EdgeIntersectInfo>();
			double num = Math.Min(polygon1.Epsilon, polygon2.Epsilon);
			for (int i = 0; i < polygon1.Count; i++)
			{
				Point2D point2D = polygon1[i];
				Point2D point2D2 = polygon1[polygon1.NextIndex(i)];
				for (int j = 0; j < polygon2.Count; j++)
				{
					Point2D point2D3 = new Point2D();
					Point2D point2D4 = polygon2[j];
					Point2D point2D5 = polygon2[polygon2.NextIndex(j)];
					if (TriangulationUtil.LinesIntersect2D(point2D, point2D2, point2D4, point2D5, ref point2D3, num))
					{
						intersections.Add(new EdgeIntersectInfo(new Edge(point2D, point2D2), new Edge(point2D4, point2D5), point2D3));
					}
				}
			}
			return intersections.Count > 0;
		}

		// Token: 0x06001CBA RID: 7354 RVA: 0x000997F4 File Offset: 0x000979F4
		public bool PointInPolygonAngle(Point2D point, Point2DList polygon)
		{
			double num = 0.0;
			for (int i = 0; i < polygon.Count; i++)
			{
				Point2D point2D = polygon[i] - point;
				Point2D point2D2 = polygon[polygon.NextIndex(i)] - point;
				num += this.VectorAngle(point2D, point2D2);
			}
			return Math.Abs(num) >= 3.141592653589793;
		}

		// Token: 0x06001CBB RID: 7355 RVA: 0x00099860 File Offset: 0x00097A60
		public double VectorAngle(Point2D p1, Point2D p2)
		{
			double num = Math.Atan2(p1.Y, p1.X);
			double num2;
			for (num2 = Math.Atan2(p2.Y, p2.X) - num; num2 > 3.141592653589793; num2 -= 6.283185307179586)
			{
			}
			while (num2 < -3.141592653589793)
			{
				num2 += 6.283185307179586;
			}
			return num2;
		}

		// Token: 0x04001799 RID: 6041
		public PolygonUtil.PolyOperation mOperations;

		// Token: 0x0400179A RID: 6042
		public Point2DList mOriginalPolygon1;

		// Token: 0x0400179B RID: 6043
		public Point2DList mOriginalPolygon2;

		// Token: 0x0400179C RID: 6044
		public Point2DList mPoly1;

		// Token: 0x0400179D RID: 6045
		public Point2DList mPoly2;

		// Token: 0x0400179E RID: 6046
		public List<EdgeIntersectInfo> mIntersections;

		// Token: 0x0400179F RID: 6047
		public int mStartingIndex;

		// Token: 0x040017A0 RID: 6048
		public PolygonUtil.PolyUnionError mError;

		// Token: 0x040017A1 RID: 6049
		public List<int> mPoly1VectorAngles;

		// Token: 0x040017A2 RID: 6050
		public List<int> mPoly2VectorAngles;

		// Token: 0x040017A3 RID: 6051
		public Dictionary<uint, Point2DList> mOutput = new Dictionary<uint, Point2DList>();
	}
}

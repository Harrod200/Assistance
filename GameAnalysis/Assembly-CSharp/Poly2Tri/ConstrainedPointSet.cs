using System;
using System.Collections.Generic;

namespace Poly2Tri
{
	// Token: 0x020004D7 RID: 1239
	public class ConstrainedPointSet : PointSet
	{
		// Token: 0x1700040B RID: 1035
		// (get) Token: 0x06001CBC RID: 7356 RVA: 0x000998C8 File Offset: 0x00097AC8
		public override TriangulationMode TriangulationMode
		{
			get
			{
				return TriangulationMode.Constrained;
			}
		}

		// Token: 0x06001CBD RID: 7357 RVA: 0x000998CB File Offset: 0x00097ACB
		public ConstrainedPointSet(List<TriangulationPoint> bounds)
			: base(bounds)
		{
			this.AddBoundaryConstraints();
		}

		// Token: 0x06001CBE RID: 7358 RVA: 0x000998F0 File Offset: 0x00097AF0
		public ConstrainedPointSet(List<TriangulationPoint> bounds, List<TriangulationConstraint> constraints)
			: base(bounds)
		{
			this.AddBoundaryConstraints();
			this.AddConstraints(constraints);
		}

		// Token: 0x06001CBF RID: 7359 RVA: 0x00099920 File Offset: 0x00097B20
		public ConstrainedPointSet(List<TriangulationPoint> bounds, int[] indices)
			: base(bounds)
		{
			this.AddBoundaryConstraints();
			List<TriangulationConstraint> list = new List<TriangulationConstraint>();
			for (int i = 0; i < indices.Length; i += 2)
			{
				TriangulationConstraint triangulationConstraint = new TriangulationConstraint(bounds[i], bounds[i + 1]);
				list.Add(triangulationConstraint);
			}
			this.AddConstraints(list);
		}

		// Token: 0x06001CC0 RID: 7360 RVA: 0x0009998C File Offset: 0x00097B8C
		protected void AddBoundaryConstraints()
		{
			TriangulationPoint triangulationPoint = null;
			TriangulationPoint triangulationPoint2 = null;
			TriangulationPoint triangulationPoint3 = null;
			TriangulationPoint triangulationPoint4 = null;
			if (!base.TryGetPoint(base.MinX, base.MinY, out triangulationPoint))
			{
				triangulationPoint = new TriangulationPoint(base.MinX, base.MinY);
				this.Add(triangulationPoint);
			}
			if (!base.TryGetPoint(base.MaxX, base.MinY, out triangulationPoint2))
			{
				triangulationPoint2 = new TriangulationPoint(base.MaxX, base.MinY);
				this.Add(triangulationPoint2);
			}
			if (!base.TryGetPoint(base.MaxX, base.MaxY, out triangulationPoint3))
			{
				triangulationPoint3 = new TriangulationPoint(base.MaxX, base.MaxY);
				this.Add(triangulationPoint3);
			}
			if (!base.TryGetPoint(base.MinX, base.MaxY, out triangulationPoint4))
			{
				triangulationPoint4 = new TriangulationPoint(base.MinX, base.MaxY);
				this.Add(triangulationPoint4);
			}
			TriangulationConstraint triangulationConstraint = new TriangulationConstraint(triangulationPoint, triangulationPoint2);
			this.AddConstraint(triangulationConstraint);
			TriangulationConstraint triangulationConstraint2 = new TriangulationConstraint(triangulationPoint2, triangulationPoint3);
			this.AddConstraint(triangulationConstraint2);
			TriangulationConstraint triangulationConstraint3 = new TriangulationConstraint(triangulationPoint3, triangulationPoint4);
			this.AddConstraint(triangulationConstraint3);
			TriangulationConstraint triangulationConstraint4 = new TriangulationConstraint(triangulationPoint4, triangulationPoint);
			this.AddConstraint(triangulationConstraint4);
		}

		// Token: 0x06001CC1 RID: 7361 RVA: 0x00099AA5 File Offset: 0x00097CA5
		public override void Add(Point2D p)
		{
			base.Add(p as TriangulationPoint, -1, true);
		}

		// Token: 0x06001CC2 RID: 7362 RVA: 0x00099AB6 File Offset: 0x00097CB6
		public override void Add(TriangulationPoint p)
		{
			base.Add(p, -1, true);
		}

		// Token: 0x06001CC3 RID: 7363 RVA: 0x00099AC4 File Offset: 0x00097CC4
		public override bool AddRange(List<TriangulationPoint> points)
		{
			bool flag = true;
			foreach (TriangulationPoint triangulationPoint in points)
			{
				flag = base.Add(triangulationPoint, -1, true) && flag;
			}
			return flag;
		}

		// Token: 0x06001CC4 RID: 7364 RVA: 0x00099B1C File Offset: 0x00097D1C
		public bool AddHole(List<TriangulationPoint> points, string name)
		{
			if (points == null)
			{
				return false;
			}
			List<Contour> list = new List<Contour>();
			int i = 0;
			Contour contour = new Contour(this, points, Point2DList.WindingOrderType.Unknown);
			list.Add(contour);
			if (this.mPoints.Count > 1)
			{
				int count = list[i].Count;
				for (int j = 0; j < count; j++)
				{
					base.ConstrainPointToBounds(list[i][j]);
				}
			}
			while (i < list.Count)
			{
				list[i].RemoveDuplicateNeighborPoints();
				list[i].WindingOrder = Point2DList.WindingOrderType.CCW;
				bool flag = true;
				Point2DList.PolygonError polygonError = list[i].CheckPolygon();
				while (flag && polygonError != Point2DList.PolygonError.None)
				{
					if ((polygonError & Point2DList.PolygonError.NotEnoughVertices) == Point2DList.PolygonError.NotEnoughVertices)
					{
						flag = false;
					}
					else if ((polygonError & Point2DList.PolygonError.NotSimple) == Point2DList.PolygonError.NotSimple)
					{
						List<Point2DList> list2 = PolygonUtil.SplitComplexPolygon(list[i], list[i].Epsilon);
						list.RemoveAt(i);
						foreach (Point2DList point2DList in list2)
						{
							Contour contour2 = new Contour(this);
							contour2.AddRange(point2DList);
							list.Add(contour2);
						}
						polygonError = list[i].CheckPolygon();
					}
					else if ((polygonError & Point2DList.PolygonError.Degenerate) == Point2DList.PolygonError.Degenerate)
					{
						list[i].Simplify(base.Epsilon);
						polygonError = list[i].CheckPolygon();
					}
					else if ((polygonError & Point2DList.PolygonError.AreaTooSmall) == Point2DList.PolygonError.AreaTooSmall || (polygonError & Point2DList.PolygonError.SidesTooCloseToParallel) == Point2DList.PolygonError.SidesTooCloseToParallel || (polygonError & Point2DList.PolygonError.TooThin) == Point2DList.PolygonError.TooThin || (polygonError & Point2DList.PolygonError.Unknown) == Point2DList.PolygonError.Unknown)
					{
						flag = false;
					}
				}
				if (!flag && list[i].Count != 2)
				{
					list.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}
			bool flag2 = true;
			i = 0;
			while (i < list.Count)
			{
				int count2 = list[i].Count;
				if (count2 < 2)
				{
					i++;
					flag2 = false;
				}
				else
				{
					if (count2 == 2)
					{
						uint num = TriangulationConstraint.CalculateContraintCode(list[i][0], list[i][1]);
						TriangulationConstraint triangulationConstraint = null;
						if (!this.mConstraintMap.TryGetValue(num, out triangulationConstraint))
						{
							triangulationConstraint = new TriangulationConstraint(list[i][0], list[i][1]);
							this.AddConstraint(triangulationConstraint);
						}
					}
					else
					{
						Contour contour3 = new Contour(this, list[i], Point2DList.WindingOrderType.Unknown);
						contour3.WindingOrder = Point2DList.WindingOrderType.CCW;
						contour3.Name = name + ":" + i.ToString();
						this.mHoles.Add(contour3);
					}
					i++;
				}
			}
			return flag2;
		}

		// Token: 0x06001CC5 RID: 7365 RVA: 0x00099DC4 File Offset: 0x00097FC4
		public bool AddConstraints(List<TriangulationConstraint> constraints)
		{
			if (constraints == null || constraints.Count < 1)
			{
				return false;
			}
			bool flag = true;
			foreach (TriangulationConstraint triangulationConstraint in constraints)
			{
				if (base.ConstrainPointToBounds(triangulationConstraint.P) || base.ConstrainPointToBounds(triangulationConstraint.Q))
				{
					triangulationConstraint.CalculateContraintCode();
				}
				TriangulationConstraint triangulationConstraint2 = null;
				if (!this.mConstraintMap.TryGetValue(triangulationConstraint.ConstraintCode, out triangulationConstraint2))
				{
					triangulationConstraint2 = triangulationConstraint;
					flag = this.AddConstraint(triangulationConstraint2) && flag;
				}
			}
			return flag;
		}

		// Token: 0x06001CC6 RID: 7366 RVA: 0x00099E64 File Offset: 0x00098064
		public bool AddConstraint(TriangulationConstraint tc)
		{
			if (tc == null || tc.P == null || tc.Q == null)
			{
				return false;
			}
			if (this.mConstraintMap.ContainsKey(tc.ConstraintCode))
			{
				return true;
			}
			TriangulationPoint triangulationPoint;
			if (base.TryGetPoint(tc.P.X, tc.P.Y, out triangulationPoint))
			{
				tc.P = triangulationPoint;
			}
			else
			{
				this.Add(tc.P);
			}
			if (base.TryGetPoint(tc.Q.X, tc.Q.Y, out triangulationPoint))
			{
				tc.Q = triangulationPoint;
			}
			else
			{
				this.Add(tc.Q);
			}
			this.mConstraintMap.Add(tc.ConstraintCode, tc);
			return true;
		}

		// Token: 0x06001CC7 RID: 7367 RVA: 0x00099F18 File Offset: 0x00098118
		public bool TryGetConstraint(uint constraintCode, out TriangulationConstraint tc)
		{
			return this.mConstraintMap.TryGetValue(constraintCode, out tc);
		}

		// Token: 0x06001CC8 RID: 7368 RVA: 0x00099F27 File Offset: 0x00098127
		public int GetNumConstraints()
		{
			return this.mConstraintMap.Count;
		}

		// Token: 0x06001CC9 RID: 7369 RVA: 0x00099F34 File Offset: 0x00098134
		public Dictionary<uint, TriangulationConstraint>.Enumerator GetConstraintEnumerator()
		{
			return this.mConstraintMap.GetEnumerator();
		}

		// Token: 0x06001CCA RID: 7370 RVA: 0x00099F44 File Offset: 0x00098144
		public int GetNumHoles()
		{
			int num = 0;
			foreach (Contour contour in this.mHoles)
			{
				num += contour.GetNumHoles(false);
			}
			return num;
		}

		// Token: 0x06001CCB RID: 7371 RVA: 0x00099FA0 File Offset: 0x000981A0
		public Contour GetHole(int idx)
		{
			if (idx < 0 || idx >= this.mHoles.Count)
			{
				return null;
			}
			return this.mHoles[idx];
		}

		// Token: 0x06001CCC RID: 7372 RVA: 0x00099FC4 File Offset: 0x000981C4
		public int GetActualHoles(out List<Contour> holes)
		{
			holes = new List<Contour>();
			foreach (Contour contour in this.mHoles)
			{
				contour.GetActualHoles(false, ref holes);
			}
			return holes.Count;
		}

		// Token: 0x06001CCD RID: 7373 RVA: 0x0009A024 File Offset: 0x00098224
		protected void InitializeHoles()
		{
			Contour.InitializeHoles(this.mHoles, this, this);
			foreach (Contour contour in this.mHoles)
			{
				contour.InitializeHoles(this);
			}
		}

		// Token: 0x06001CCE RID: 7374 RVA: 0x0009A084 File Offset: 0x00098284
		public override bool Initialize()
		{
			this.InitializeHoles();
			return base.Initialize();
		}

		// Token: 0x06001CCF RID: 7375 RVA: 0x0009A094 File Offset: 0x00098294
		public override void Prepare(TriangulationContext tcx)
		{
			if (!this.Initialize())
			{
				return;
			}
			base.Prepare(tcx);
			foreach (KeyValuePair<uint, TriangulationConstraint> keyValuePair in this.mConstraintMap)
			{
				TriangulationConstraint value = keyValuePair.Value;
				tcx.NewConstraint(value.P, value.Q);
			}
		}

		// Token: 0x06001CD0 RID: 7376 RVA: 0x0009A0EB File Offset: 0x000982EB
		public override void AddTriangle(DelaunayTriangle t)
		{
			base.Triangles.Add(t);
		}

		// Token: 0x040017A4 RID: 6052
		protected Dictionary<uint, TriangulationConstraint> mConstraintMap = new Dictionary<uint, TriangulationConstraint>();

		// Token: 0x040017A5 RID: 6053
		protected List<Contour> mHoles = new List<Contour>();
	}
}

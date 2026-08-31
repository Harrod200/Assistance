using System;
using System.Collections;
using System.Collections.Generic;

namespace Poly2Tri
{
	// Token: 0x020004CF RID: 1231
	public class Contour : Point2DList, ITriangulatable, IEnumerable<TriangulationPoint>, IEnumerable, IList<TriangulationPoint>, ICollection<TriangulationPoint>
	{
		// Token: 0x170003E2 RID: 994
		public TriangulationPoint this[int index]
		{
			get
			{
				return this.mPoints[index] as TriangulationPoint;
			}
			set
			{
				this.mPoints[index] = value;
			}
		}

		// Token: 0x170003E3 RID: 995
		// (get) Token: 0x06001C1A RID: 7194 RVA: 0x000967FB File Offset: 0x000949FB
		// (set) Token: 0x06001C1B RID: 7195 RVA: 0x00096803 File Offset: 0x00094A03
		public string Name
		{
			get
			{
				return this.mName;
			}
			set
			{
				this.mName = value;
			}
		}

		// Token: 0x170003E4 RID: 996
		// (get) Token: 0x06001C1C RID: 7196 RVA: 0x0009680C File Offset: 0x00094A0C
		// (set) Token: 0x06001C1D RID: 7197 RVA: 0x00096818 File Offset: 0x00094A18
		public IList<DelaunayTriangle> Triangles
		{
			get
			{
				throw new NotImplementedException("PolyHole.Triangles should never get called");
			}
			private set
			{
			}
		}

		// Token: 0x170003E5 RID: 997
		// (get) Token: 0x06001C1E RID: 7198 RVA: 0x0009681A File Offset: 0x00094A1A
		public TriangulationMode TriangulationMode
		{
			get
			{
				return this.mParent.TriangulationMode;
			}
		}

		// Token: 0x170003E6 RID: 998
		// (get) Token: 0x06001C1F RID: 7199 RVA: 0x00096827 File Offset: 0x00094A27
		// (set) Token: 0x06001C20 RID: 7200 RVA: 0x00096834 File Offset: 0x00094A34
		public string FileName
		{
			get
			{
				return this.mParent.FileName;
			}
			set
			{
			}
		}

		// Token: 0x170003E7 RID: 999
		// (get) Token: 0x06001C21 RID: 7201 RVA: 0x00096836 File Offset: 0x00094A36
		// (set) Token: 0x06001C22 RID: 7202 RVA: 0x00096843 File Offset: 0x00094A43
		public bool DisplayFlipX
		{
			get
			{
				return this.mParent.DisplayFlipX;
			}
			set
			{
			}
		}

		// Token: 0x170003E8 RID: 1000
		// (get) Token: 0x06001C23 RID: 7203 RVA: 0x00096845 File Offset: 0x00094A45
		// (set) Token: 0x06001C24 RID: 7204 RVA: 0x00096852 File Offset: 0x00094A52
		public bool DisplayFlipY
		{
			get
			{
				return this.mParent.DisplayFlipY;
			}
			set
			{
			}
		}

		// Token: 0x170003E9 RID: 1001
		// (get) Token: 0x06001C25 RID: 7205 RVA: 0x00096854 File Offset: 0x00094A54
		// (set) Token: 0x06001C26 RID: 7206 RVA: 0x00096861 File Offset: 0x00094A61
		public float DisplayRotate
		{
			get
			{
				return this.mParent.DisplayRotate;
			}
			set
			{
			}
		}

		// Token: 0x170003EA RID: 1002
		// (get) Token: 0x06001C27 RID: 7207 RVA: 0x00096863 File Offset: 0x00094A63
		// (set) Token: 0x06001C28 RID: 7208 RVA: 0x00096870 File Offset: 0x00094A70
		public double Precision
		{
			get
			{
				return this.mParent.Precision;
			}
			set
			{
			}
		}

		// Token: 0x170003EB RID: 1003
		// (get) Token: 0x06001C29 RID: 7209 RVA: 0x00096872 File Offset: 0x00094A72
		public double MinX
		{
			get
			{
				return this.mBoundingBox.MinX;
			}
		}

		// Token: 0x170003EC RID: 1004
		// (get) Token: 0x06001C2A RID: 7210 RVA: 0x0009687F File Offset: 0x00094A7F
		public double MaxX
		{
			get
			{
				return this.mBoundingBox.MaxX;
			}
		}

		// Token: 0x170003ED RID: 1005
		// (get) Token: 0x06001C2B RID: 7211 RVA: 0x0009688C File Offset: 0x00094A8C
		public double MinY
		{
			get
			{
				return this.mBoundingBox.MinY;
			}
		}

		// Token: 0x170003EE RID: 1006
		// (get) Token: 0x06001C2C RID: 7212 RVA: 0x00096899 File Offset: 0x00094A99
		public double MaxY
		{
			get
			{
				return this.mBoundingBox.MaxY;
			}
		}

		// Token: 0x170003EF RID: 1007
		// (get) Token: 0x06001C2D RID: 7213 RVA: 0x000968A6 File Offset: 0x00094AA6
		public Rect2D Bounds
		{
			get
			{
				return this.mBoundingBox;
			}
		}

		// Token: 0x06001C2E RID: 7214 RVA: 0x000968AE File Offset: 0x00094AAE
		public Contour(ITriangulatable parent)
		{
			this.mParent = parent;
		}

		// Token: 0x06001C2F RID: 7215 RVA: 0x000968D3 File Offset: 0x00094AD3
		public Contour(ITriangulatable parent, IList<TriangulationPoint> points, Point2DList.WindingOrderType windingOrder)
		{
			this.mParent = parent;
			this.AddRange(points, windingOrder);
		}

		// Token: 0x06001C30 RID: 7216 RVA: 0x00096900 File Offset: 0x00094B00
		public override string ToString()
		{
			return this.mName + " : " + base.ToString();
		}

		// Token: 0x06001C31 RID: 7217 RVA: 0x00096918 File Offset: 0x00094B18
		IEnumerator<TriangulationPoint> IEnumerable<TriangulationPoint>.GetEnumerator()
		{
			return new TriangulationPointEnumerator(this.mPoints);
		}

		// Token: 0x06001C32 RID: 7218 RVA: 0x00096925 File Offset: 0x00094B25
		public int IndexOf(TriangulationPoint p)
		{
			return this.mPoints.IndexOf(p);
		}

		// Token: 0x06001C33 RID: 7219 RVA: 0x00096933 File Offset: 0x00094B33
		public void Add(TriangulationPoint p)
		{
			this.Add(p, -1, true);
		}

		// Token: 0x06001C34 RID: 7220 RVA: 0x00096940 File Offset: 0x00094B40
		protected override void Add(Point2D p, int idx, bool bCalcWindingOrderAndEpsilon)
		{
			TriangulationPoint triangulationPoint;
			if (p is TriangulationPoint)
			{
				triangulationPoint = p as TriangulationPoint;
			}
			else
			{
				triangulationPoint = new TriangulationPoint(p.X, p.Y);
			}
			if (idx < 0)
			{
				this.mPoints.Add(triangulationPoint);
			}
			else
			{
				this.mPoints.Insert(idx, triangulationPoint);
			}
			this.mBoundingBox.AddPoint(triangulationPoint);
			if (bCalcWindingOrderAndEpsilon)
			{
				if (this.mWindingOrder == Point2DList.WindingOrderType.Unknown)
				{
					this.mWindingOrder = base.CalculateWindingOrder();
				}
				this.mEpsilon = base.CalculateEpsilon();
			}
		}

		// Token: 0x06001C35 RID: 7221 RVA: 0x000969C4 File Offset: 0x00094BC4
		public override void AddRange(IEnumerator<Point2D> iter, Point2DList.WindingOrderType windingOrder)
		{
			if (iter == null)
			{
				return;
			}
			if (this.mWindingOrder == Point2DList.WindingOrderType.Unknown && base.Count == 0)
			{
				this.mWindingOrder = windingOrder;
			}
			bool flag = base.WindingOrder != Point2DList.WindingOrderType.Unknown && windingOrder != Point2DList.WindingOrderType.Unknown && base.WindingOrder != windingOrder;
			bool flag2 = true;
			int count = this.mPoints.Count;
			iter.Reset();
			while (iter.MoveNext())
			{
				TriangulationPoint triangulationPoint;
				if (iter.Current is TriangulationPoint)
				{
					triangulationPoint = iter.Current as TriangulationPoint;
				}
				else
				{
					triangulationPoint = new TriangulationPoint(iter.Current.X, iter.Current.Y);
				}
				if (!flag2)
				{
					flag2 = true;
					this.mPoints.Add(triangulationPoint);
				}
				else if (flag)
				{
					this.mPoints.Insert(count, triangulationPoint);
				}
				else
				{
					this.mPoints.Add(triangulationPoint);
				}
				this.mBoundingBox.AddPoint(iter.Current);
			}
			if (this.mWindingOrder == Point2DList.WindingOrderType.Unknown && windingOrder == Point2DList.WindingOrderType.Unknown)
			{
				this.mWindingOrder = base.CalculateWindingOrder();
			}
			this.mEpsilon = base.CalculateEpsilon();
		}

		// Token: 0x06001C36 RID: 7222 RVA: 0x00096ACC File Offset: 0x00094CCC
		public void AddRange(IList<TriangulationPoint> points, Point2DList.WindingOrderType windingOrder)
		{
			if (points == null || points.Count < 1)
			{
				return;
			}
			if (this.mWindingOrder == Point2DList.WindingOrderType.Unknown && base.Count == 0)
			{
				this.mWindingOrder = windingOrder;
			}
			int count = points.Count;
			bool flag = base.WindingOrder != Point2DList.WindingOrderType.Unknown && windingOrder != Point2DList.WindingOrderType.Unknown && base.WindingOrder != windingOrder;
			for (int i = 0; i < count; i++)
			{
				int num = i;
				if (flag)
				{
					num = points.Count - i - 1;
				}
				this.Add(points[num], -1, false);
			}
			if (this.mWindingOrder == Point2DList.WindingOrderType.Unknown)
			{
				this.mWindingOrder = base.CalculateWindingOrder();
			}
			this.mEpsilon = base.CalculateEpsilon();
		}

		// Token: 0x06001C37 RID: 7223 RVA: 0x00096B6E File Offset: 0x00094D6E
		public void Insert(int idx, TriangulationPoint p)
		{
			this.Add(p, idx, true);
		}

		// Token: 0x06001C38 RID: 7224 RVA: 0x00096B79 File Offset: 0x00094D79
		public bool Remove(TriangulationPoint p)
		{
			return this.Remove(p);
		}

		// Token: 0x06001C39 RID: 7225 RVA: 0x00096B82 File Offset: 0x00094D82
		public bool Contains(TriangulationPoint p)
		{
			return this.mPoints.Contains(p);
		}

		// Token: 0x06001C3A RID: 7226 RVA: 0x00096B90 File Offset: 0x00094D90
		public void CopyTo(TriangulationPoint[] array, int arrayIndex)
		{
			int num = Math.Min(base.Count, array.Length - arrayIndex);
			for (int i = 0; i < num; i++)
			{
				array[arrayIndex + i] = this.mPoints[i] as TriangulationPoint;
			}
		}

		// Token: 0x06001C3B RID: 7227 RVA: 0x00096BD0 File Offset: 0x00094DD0
		protected void AddHole(Contour c)
		{
			c.mParent = this;
			this.mHoles.Add(c);
		}

		// Token: 0x06001C3C RID: 7228 RVA: 0x00096BE8 File Offset: 0x00094DE8
		public int GetNumHoles(bool parentIsHole)
		{
			int num = (parentIsHole ? 0 : 1);
			foreach (Contour contour in this.mHoles)
			{
				num += contour.GetNumHoles(!parentIsHole);
			}
			return num;
		}

		// Token: 0x06001C3D RID: 7229 RVA: 0x00096C4C File Offset: 0x00094E4C
		public int GetNumHoles()
		{
			return this.mHoles.Count;
		}

		// Token: 0x06001C3E RID: 7230 RVA: 0x00096C59 File Offset: 0x00094E59
		public Contour GetHole(int idx)
		{
			if (idx < 0 || idx >= this.mHoles.Count)
			{
				return null;
			}
			return this.mHoles[idx];
		}

		// Token: 0x06001C3F RID: 7231 RVA: 0x00096C7C File Offset: 0x00094E7C
		public void GetActualHoles(bool parentIsHole, ref List<Contour> holes)
		{
			if (parentIsHole)
			{
				holes.Add(this);
			}
			foreach (Contour contour in this.mHoles)
			{
				contour.GetActualHoles(!parentIsHole, ref holes);
			}
		}

		// Token: 0x06001C40 RID: 7232 RVA: 0x00096CDC File Offset: 0x00094EDC
		public List<Contour>.Enumerator GetHoleEnumerator()
		{
			return this.mHoles.GetEnumerator();
		}

		// Token: 0x06001C41 RID: 7233 RVA: 0x00096CEC File Offset: 0x00094EEC
		public void InitializeHoles(ConstrainedPointSet cps)
		{
			Contour.InitializeHoles(this.mHoles, this, cps);
			foreach (Contour contour in this.mHoles)
			{
				contour.InitializeHoles(cps);
			}
		}

		// Token: 0x06001C42 RID: 7234 RVA: 0x00096D4C File Offset: 0x00094F4C
		public static void InitializeHoles(List<Contour> holes, ITriangulatable parent, ConstrainedPointSet cps)
		{
			int num = holes.Count;
			int i;
			for (i = 0; i < num; i++)
			{
				int j = i + 1;
				while (j < num)
				{
					if (PolygonUtil.PolygonsAreSame2D(holes[i], holes[j]))
					{
						holes.RemoveAt(j);
						num--;
					}
					else
					{
						j++;
					}
				}
			}
			i = 0;
			while (i < num)
			{
				bool flag = true;
				int k = i + 1;
				while (k < num)
				{
					if (PolygonUtil.PolygonContainsPolygon(holes[i], holes[i].Bounds, holes[k], holes[k].Bounds, false))
					{
						holes[i].AddHole(holes[k]);
						holes.RemoveAt(k);
						num--;
					}
					else
					{
						if (PolygonUtil.PolygonContainsPolygon(holes[k], holes[k].Bounds, holes[i], holes[i].Bounds, false))
						{
							holes[k].AddHole(holes[i]);
							holes.RemoveAt(i);
							num--;
							flag = false;
							break;
						}
						if (PolygonUtil.PolygonsIntersect2D(holes[i], holes[i].Bounds, holes[k], holes[k].Bounds))
						{
							PolygonOperationContext polygonOperationContext = new PolygonOperationContext();
							if (!polygonOperationContext.Init(PolygonUtil.PolyOperation.Union | PolygonUtil.PolyOperation.Intersect, holes[i], holes[k]))
							{
								if (polygonOperationContext.mError == PolygonUtil.PolyUnionError.Poly1InsidePoly2)
								{
									holes[k].AddHole(holes[i]);
									holes.RemoveAt(i);
									num--;
									flag = false;
									break;
								}
								throw new Exception("PolygonOperationContext.Init had an error during initialization");
							}
							else
							{
								if (PolygonUtil.PolygonOperation(polygonOperationContext) != PolygonUtil.PolyUnionError.None)
								{
									throw new Exception("PolygonOperation had an error!");
								}
								Point2DList union = polygonOperationContext.Union;
								Point2DList intersect = polygonOperationContext.Intersect;
								Contour contour = new Contour(parent);
								contour.AddRange(union);
								contour.Name = string.Concat(new string[]
								{
									"(",
									holes[i].Name,
									" UNION ",
									holes[k].Name,
									")"
								});
								contour.WindingOrder = Point2DList.WindingOrderType.CCW;
								int num2 = holes[i].GetNumHoles();
								for (int l = 0; l < num2; l++)
								{
									contour.AddHole(holes[i].GetHole(l));
								}
								num2 = holes[k].GetNumHoles();
								for (int m = 0; m < num2; m++)
								{
									contour.AddHole(holes[k].GetHole(m));
								}
								Contour contour2 = new Contour(contour);
								contour2.AddRange(intersect);
								contour2.Name = string.Concat(new string[]
								{
									"(",
									holes[i].Name,
									" INTERSECT ",
									holes[k].Name,
									")"
								});
								contour2.WindingOrder = Point2DList.WindingOrderType.CCW;
								contour.AddHole(contour2);
								holes[i] = contour;
								holes.RemoveAt(k);
								num--;
								k = i + 1;
							}
						}
						else
						{
							k++;
						}
					}
				}
				if (flag)
				{
					i++;
				}
			}
			num = holes.Count;
			for (i = 0; i < num; i++)
			{
				int count = holes[i].Count;
				for (int n = 0; n < count; n++)
				{
					int num3 = holes[i].NextIndex(n);
					uint num4 = TriangulationConstraint.CalculateContraintCode(holes[i][n], holes[i][num3]);
					TriangulationConstraint triangulationConstraint = null;
					if (!cps.TryGetConstraint(num4, out triangulationConstraint))
					{
						triangulationConstraint = new TriangulationConstraint(holes[i][n], holes[i][num3]);
						cps.AddConstraint(triangulationConstraint);
					}
					if (holes[i][n].VertexCode == triangulationConstraint.P.VertexCode)
					{
						holes[i][n] = triangulationConstraint.P;
					}
					else if (holes[i][num3].VertexCode == triangulationConstraint.P.VertexCode)
					{
						holes[i][num3] = triangulationConstraint.P;
					}
					if (holes[i][n].VertexCode == triangulationConstraint.Q.VertexCode)
					{
						holes[i][n] = triangulationConstraint.Q;
					}
					else if (holes[i][num3].VertexCode == triangulationConstraint.Q.VertexCode)
					{
						holes[i][num3] = triangulationConstraint.Q;
					}
				}
			}
		}

		// Token: 0x06001C43 RID: 7235 RVA: 0x00097208 File Offset: 0x00095408
		public void Prepare(TriangulationContext tcx)
		{
			throw new NotImplementedException("PolyHole.Prepare should never get called");
		}

		// Token: 0x06001C44 RID: 7236 RVA: 0x00097214 File Offset: 0x00095414
		public void AddTriangle(DelaunayTriangle t)
		{
			throw new NotImplementedException("PolyHole.AddTriangle should never get called");
		}

		// Token: 0x06001C45 RID: 7237 RVA: 0x00097220 File Offset: 0x00095420
		public void AddTriangles(IEnumerable<DelaunayTriangle> list)
		{
			throw new NotImplementedException("PolyHole.AddTriangles should never get called");
		}

		// Token: 0x06001C46 RID: 7238 RVA: 0x0009722C File Offset: 0x0009542C
		public void ClearTriangles()
		{
			throw new NotImplementedException("PolyHole.ClearTriangles should never get called");
		}

		// Token: 0x06001C47 RID: 7239 RVA: 0x00097238 File Offset: 0x00095438
		public Point2D FindPointInContour()
		{
			if (base.Count < 3)
			{
				return null;
			}
			Point2D centroid = base.GetCentroid();
			if (this.IsPointInsideContour(centroid))
			{
				return centroid;
			}
			Random random = new Random();
			do
			{
				centroid.X = random.NextDouble() * (this.MaxX - this.MinX) + this.MinX;
				centroid.Y = random.NextDouble() * (this.MaxY - this.MinY) + this.MinY;
			}
			while (!this.IsPointInsideContour(centroid));
			return centroid;
		}

		// Token: 0x06001C48 RID: 7240 RVA: 0x000972B4 File Offset: 0x000954B4
		public bool IsPointInsideContour(Point2D p)
		{
			if (PolygonUtil.PointInPolygon2D(this, p))
			{
				using (List<Contour>.Enumerator enumerator = this.mHoles.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.IsPointInsideContour(p))
						{
							return false;
						}
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x04001783 RID: 6019
		private List<Contour> mHoles = new List<Contour>();

		// Token: 0x04001784 RID: 6020
		private ITriangulatable mParent;

		// Token: 0x04001785 RID: 6021
		private string mName = "";
	}
}

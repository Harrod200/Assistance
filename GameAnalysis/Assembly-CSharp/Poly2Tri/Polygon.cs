using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Poly2Tri
{
	// Token: 0x020004D0 RID: 1232
	public class Polygon : Point2DList, ITriangulatable, IEnumerable<TriangulationPoint>, IEnumerable, IList<TriangulationPoint>, ICollection<TriangulationPoint>
	{
		// Token: 0x170003F0 RID: 1008
		// (get) Token: 0x06001C49 RID: 7241 RVA: 0x0009731C File Offset: 0x0009551C
		public IList<TriangulationPoint> Points
		{
			get
			{
				return this;
			}
		}

		// Token: 0x170003F1 RID: 1009
		// (get) Token: 0x06001C4A RID: 7242 RVA: 0x0009731F File Offset: 0x0009551F
		public IList<DelaunayTriangle> Triangles
		{
			get
			{
				return this.mTriangles;
			}
		}

		// Token: 0x170003F2 RID: 1010
		// (get) Token: 0x06001C4B RID: 7243 RVA: 0x00097327 File Offset: 0x00095527
		public TriangulationMode TriangulationMode
		{
			get
			{
				return TriangulationMode.Polygon;
			}
		}

		// Token: 0x170003F3 RID: 1011
		// (get) Token: 0x06001C4C RID: 7244 RVA: 0x0009732A File Offset: 0x0009552A
		// (set) Token: 0x06001C4D RID: 7245 RVA: 0x00097332 File Offset: 0x00095532
		public string FileName { get; set; }

		// Token: 0x170003F4 RID: 1012
		// (get) Token: 0x06001C4E RID: 7246 RVA: 0x0009733B File Offset: 0x0009553B
		// (set) Token: 0x06001C4F RID: 7247 RVA: 0x00097343 File Offset: 0x00095543
		public bool DisplayFlipX { get; set; }

		// Token: 0x170003F5 RID: 1013
		// (get) Token: 0x06001C50 RID: 7248 RVA: 0x0009734C File Offset: 0x0009554C
		// (set) Token: 0x06001C51 RID: 7249 RVA: 0x00097354 File Offset: 0x00095554
		public bool DisplayFlipY { get; set; }

		// Token: 0x170003F6 RID: 1014
		// (get) Token: 0x06001C52 RID: 7250 RVA: 0x0009735D File Offset: 0x0009555D
		// (set) Token: 0x06001C53 RID: 7251 RVA: 0x00097365 File Offset: 0x00095565
		public float DisplayRotate { get; set; }

		// Token: 0x170003F7 RID: 1015
		// (get) Token: 0x06001C54 RID: 7252 RVA: 0x0009736E File Offset: 0x0009556E
		// (set) Token: 0x06001C55 RID: 7253 RVA: 0x00097376 File Offset: 0x00095576
		public double Precision
		{
			get
			{
				return this.mPrecision;
			}
			set
			{
				this.mPrecision = value;
			}
		}

		// Token: 0x170003F8 RID: 1016
		// (get) Token: 0x06001C56 RID: 7254 RVA: 0x0009737F File Offset: 0x0009557F
		public double MinX
		{
			get
			{
				return this.mBoundingBox.MinX;
			}
		}

		// Token: 0x170003F9 RID: 1017
		// (get) Token: 0x06001C57 RID: 7255 RVA: 0x0009738C File Offset: 0x0009558C
		public double MaxX
		{
			get
			{
				return this.mBoundingBox.MaxX;
			}
		}

		// Token: 0x170003FA RID: 1018
		// (get) Token: 0x06001C58 RID: 7256 RVA: 0x00097399 File Offset: 0x00095599
		public double MinY
		{
			get
			{
				return this.mBoundingBox.MinY;
			}
		}

		// Token: 0x170003FB RID: 1019
		// (get) Token: 0x06001C59 RID: 7257 RVA: 0x000973A6 File Offset: 0x000955A6
		public double MaxY
		{
			get
			{
				return this.mBoundingBox.MaxY;
			}
		}

		// Token: 0x170003FC RID: 1020
		// (get) Token: 0x06001C5A RID: 7258 RVA: 0x000973B3 File Offset: 0x000955B3
		public Rect2D Bounds
		{
			get
			{
				return this.mBoundingBox;
			}
		}

		// Token: 0x170003FD RID: 1021
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

		// Token: 0x170003FE RID: 1022
		// (get) Token: 0x06001C5D RID: 7261 RVA: 0x000973DD File Offset: 0x000955DD
		public IList<Polygon> Holes
		{
			get
			{
				return this.mHoles;
			}
		}

		// Token: 0x06001C5E RID: 7262 RVA: 0x000973E5 File Offset: 0x000955E5
		public Polygon(IList<PolygonPoint> points)
		{
			if (points.Count < 3)
			{
				throw new ArgumentException("List has fewer than 3 points", "points");
			}
			this.AddRange(points, Point2DList.WindingOrderType.Unknown);
		}

		// Token: 0x06001C5F RID: 7263 RVA: 0x00097424 File Offset: 0x00095624
		public Polygon(IEnumerable<PolygonPoint> points)
			: this((points as IList<PolygonPoint>) ?? points.ToArray<PolygonPoint>())
		{
		}

		// Token: 0x06001C60 RID: 7264 RVA: 0x0009743C File Offset: 0x0009563C
		public Polygon(params PolygonPoint[] points)
			: this(points)
		{
		}

		// Token: 0x06001C61 RID: 7265 RVA: 0x00097445 File Offset: 0x00095645
		IEnumerator<TriangulationPoint> IEnumerable<TriangulationPoint>.GetEnumerator()
		{
			return new TriangulationPointEnumerator(this.mPoints);
		}

		// Token: 0x06001C62 RID: 7266 RVA: 0x00097452 File Offset: 0x00095652
		public int IndexOf(TriangulationPoint p)
		{
			return this.mPoints.IndexOf(p);
		}

		// Token: 0x06001C63 RID: 7267 RVA: 0x00097460 File Offset: 0x00095660
		public override void Add(Point2D p)
		{
			this.Add(p, -1, true);
		}

		// Token: 0x06001C64 RID: 7268 RVA: 0x0009746B File Offset: 0x0009566B
		public void Add(TriangulationPoint p)
		{
			this.Add(p, -1, true);
		}

		// Token: 0x06001C65 RID: 7269 RVA: 0x00097476 File Offset: 0x00095676
		public void Add(PolygonPoint p)
		{
			this.Add(p, -1, true);
		}

		// Token: 0x06001C66 RID: 7270 RVA: 0x00097484 File Offset: 0x00095684
		protected override void Add(Point2D p, int idx, bool bCalcWindingOrderAndEpsilon)
		{
			TriangulationPoint triangulationPoint = p as TriangulationPoint;
			if (triangulationPoint == null)
			{
				return;
			}
			if (this.mPointMap.ContainsKey(triangulationPoint.VertexCode))
			{
				return;
			}
			this.mPointMap.Add(triangulationPoint.VertexCode, triangulationPoint);
			base.Add(p, idx, bCalcWindingOrderAndEpsilon);
			PolygonPoint polygonPoint = p as PolygonPoint;
			if (polygonPoint != null)
			{
				polygonPoint.Previous = this._last;
				if (this._last != null)
				{
					polygonPoint.Next = this._last.Next;
					this._last.Next = polygonPoint;
				}
				this._last = polygonPoint;
			}
		}

		// Token: 0x06001C67 RID: 7271 RVA: 0x00097510 File Offset: 0x00095710
		public void AddRange(IList<PolygonPoint> points, Point2DList.WindingOrderType windingOrder)
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

		// Token: 0x06001C68 RID: 7272 RVA: 0x000975B4 File Offset: 0x000957B4
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

		// Token: 0x06001C69 RID: 7273 RVA: 0x00097656 File Offset: 0x00095856
		public void Insert(int idx, TriangulationPoint p)
		{
			this.Add(p, idx, true);
		}

		// Token: 0x06001C6A RID: 7274 RVA: 0x00097661 File Offset: 0x00095861
		public bool Remove(TriangulationPoint p)
		{
			return base.Remove(p);
		}

		// Token: 0x06001C6B RID: 7275 RVA: 0x0009766C File Offset: 0x0009586C
		public void RemovePoint(PolygonPoint p)
		{
			PolygonPoint next = p.Next;
			PolygonPoint previous = p.Previous;
			previous.Next = next;
			next.Previous = previous;
			this.mPoints.Remove(p);
			this.mBoundingBox.Clear();
			foreach (Point2D point2D in this.mPoints)
			{
				PolygonPoint polygonPoint = (PolygonPoint)point2D;
				this.mBoundingBox.AddPoint(polygonPoint);
			}
		}

		// Token: 0x06001C6C RID: 7276 RVA: 0x00097700 File Offset: 0x00095900
		public bool Contains(TriangulationPoint p)
		{
			return this.mPoints.Contains(p);
		}

		// Token: 0x06001C6D RID: 7277 RVA: 0x00097710 File Offset: 0x00095910
		public void CopyTo(TriangulationPoint[] array, int arrayIndex)
		{
			int num = Math.Min(base.Count, array.Length - arrayIndex);
			for (int i = 0; i < num; i++)
			{
				array[arrayIndex + i] = this.mPoints[i] as TriangulationPoint;
			}
		}

		// Token: 0x06001C6E RID: 7278 RVA: 0x00097750 File Offset: 0x00095950
		public void AddSteinerPoint(TriangulationPoint point)
		{
			if (this.mSteinerPoints == null)
			{
				this.mSteinerPoints = new List<TriangulationPoint>();
			}
			this.mSteinerPoints.Add(point);
		}

		// Token: 0x06001C6F RID: 7279 RVA: 0x00097771 File Offset: 0x00095971
		public void AddSteinerPoints(List<TriangulationPoint> points)
		{
			if (this.mSteinerPoints == null)
			{
				this.mSteinerPoints = new List<TriangulationPoint>();
			}
			this.mSteinerPoints.AddRange(points);
		}

		// Token: 0x06001C70 RID: 7280 RVA: 0x00097792 File Offset: 0x00095992
		public void ClearSteinerPoints()
		{
			if (this.mSteinerPoints != null)
			{
				this.mSteinerPoints.Clear();
			}
		}

		// Token: 0x06001C71 RID: 7281 RVA: 0x000977A7 File Offset: 0x000959A7
		public void AddHole(Polygon poly)
		{
			if (this.mHoles == null)
			{
				this.mHoles = new List<Polygon>();
			}
			this.mHoles.Add(poly);
		}

		// Token: 0x06001C72 RID: 7282 RVA: 0x000977C8 File Offset: 0x000959C8
		public void AddTriangle(DelaunayTriangle t)
		{
			this.mTriangles.Add(t);
		}

		// Token: 0x06001C73 RID: 7283 RVA: 0x000977D6 File Offset: 0x000959D6
		public void AddTriangles(IEnumerable<DelaunayTriangle> list)
		{
			this.mTriangles.AddRange(list);
		}

		// Token: 0x06001C74 RID: 7284 RVA: 0x000977E4 File Offset: 0x000959E4
		public void ClearTriangles()
		{
			if (this.mTriangles != null)
			{
				this.mTriangles.Clear();
			}
		}

		// Token: 0x06001C75 RID: 7285 RVA: 0x000977F9 File Offset: 0x000959F9
		public bool IsPointInside(TriangulationPoint p)
		{
			return PolygonUtil.PointInPolygon2D(this, p);
		}

		// Token: 0x06001C76 RID: 7286 RVA: 0x00097804 File Offset: 0x00095A04
		public void Prepare(TriangulationContext tcx)
		{
			if (this.mTriangles == null)
			{
				this.mTriangles = new List<DelaunayTriangle>(this.mPoints.Count);
			}
			else
			{
				this.mTriangles.Clear();
			}
			for (int i = 0; i < this.mPoints.Count - 1; i++)
			{
				tcx.NewConstraint(this[i], this[i + 1]);
			}
			tcx.NewConstraint(this[base.Count - 1], this[0]);
			tcx.Points.AddRange(this);
			if (this.mHoles != null)
			{
				foreach (Polygon polygon in this.mHoles)
				{
					for (int j = 0; j < polygon.mPoints.Count - 1; j++)
					{
						tcx.NewConstraint(polygon[j], polygon[j + 1]);
					}
					tcx.NewConstraint(polygon[polygon.Count - 1], polygon[0]);
					tcx.Points.AddRange(polygon);
				}
			}
			if (this.mSteinerPoints != null)
			{
				tcx.Points.AddRange(this.mSteinerPoints);
			}
		}

		// Token: 0x04001786 RID: 6022
		protected Dictionary<uint, TriangulationPoint> mPointMap = new Dictionary<uint, TriangulationPoint>();

		// Token: 0x04001787 RID: 6023
		protected List<DelaunayTriangle> mTriangles;

		// Token: 0x0400178C RID: 6028
		private double mPrecision = TriangulationPoint.kVertexCodeDefaultPrecision;

		// Token: 0x0400178D RID: 6029
		protected List<Polygon> mHoles;

		// Token: 0x0400178E RID: 6030
		protected List<TriangulationPoint> mSteinerPoints;

		// Token: 0x0400178F RID: 6031
		protected PolygonPoint _last;
	}
}

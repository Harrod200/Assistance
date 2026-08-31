using System;
using System.Collections;
using System.Collections.Generic;

namespace Poly2Tri
{
	// Token: 0x020004D8 RID: 1240
	public class PointSet : Point2DList, ITriangulatable, IEnumerable<TriangulationPoint>, IEnumerable, IList<TriangulationPoint>, ICollection<TriangulationPoint>
	{
		// Token: 0x1700040C RID: 1036
		// (get) Token: 0x06001CD1 RID: 7377 RVA: 0x0009A0F9 File Offset: 0x000982F9
		// (set) Token: 0x06001CD2 RID: 7378 RVA: 0x0009A0FC File Offset: 0x000982FC
		public IList<TriangulationPoint> Points
		{
			get
			{
				return this;
			}
			private set
			{
			}
		}

		// Token: 0x1700040D RID: 1037
		// (get) Token: 0x06001CD3 RID: 7379 RVA: 0x0009A0FE File Offset: 0x000982FE
		// (set) Token: 0x06001CD4 RID: 7380 RVA: 0x0009A106 File Offset: 0x00098306
		public IList<DelaunayTriangle> Triangles { get; private set; }

		// Token: 0x1700040E RID: 1038
		// (get) Token: 0x06001CD5 RID: 7381 RVA: 0x0009A10F File Offset: 0x0009830F
		// (set) Token: 0x06001CD6 RID: 7382 RVA: 0x0009A117 File Offset: 0x00098317
		public string FileName { get; set; }

		// Token: 0x1700040F RID: 1039
		// (get) Token: 0x06001CD7 RID: 7383 RVA: 0x0009A120 File Offset: 0x00098320
		// (set) Token: 0x06001CD8 RID: 7384 RVA: 0x0009A128 File Offset: 0x00098328
		public bool DisplayFlipX { get; set; }

		// Token: 0x17000410 RID: 1040
		// (get) Token: 0x06001CD9 RID: 7385 RVA: 0x0009A131 File Offset: 0x00098331
		// (set) Token: 0x06001CDA RID: 7386 RVA: 0x0009A139 File Offset: 0x00098339
		public bool DisplayFlipY { get; set; }

		// Token: 0x17000411 RID: 1041
		// (get) Token: 0x06001CDB RID: 7387 RVA: 0x0009A142 File Offset: 0x00098342
		// (set) Token: 0x06001CDC RID: 7388 RVA: 0x0009A14A File Offset: 0x0009834A
		public float DisplayRotate { get; set; }

		// Token: 0x17000412 RID: 1042
		// (get) Token: 0x06001CDD RID: 7389 RVA: 0x0009A153 File Offset: 0x00098353
		// (set) Token: 0x06001CDE RID: 7390 RVA: 0x0009A15B File Offset: 0x0009835B
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

		// Token: 0x17000413 RID: 1043
		// (get) Token: 0x06001CDF RID: 7391 RVA: 0x0009A164 File Offset: 0x00098364
		public double MinX
		{
			get
			{
				return this.mBoundingBox.MinX;
			}
		}

		// Token: 0x17000414 RID: 1044
		// (get) Token: 0x06001CE0 RID: 7392 RVA: 0x0009A171 File Offset: 0x00098371
		public double MaxX
		{
			get
			{
				return this.mBoundingBox.MaxX;
			}
		}

		// Token: 0x17000415 RID: 1045
		// (get) Token: 0x06001CE1 RID: 7393 RVA: 0x0009A17E File Offset: 0x0009837E
		public double MinY
		{
			get
			{
				return this.mBoundingBox.MinY;
			}
		}

		// Token: 0x17000416 RID: 1046
		// (get) Token: 0x06001CE2 RID: 7394 RVA: 0x0009A18B File Offset: 0x0009838B
		public double MaxY
		{
			get
			{
				return this.mBoundingBox.MaxY;
			}
		}

		// Token: 0x17000417 RID: 1047
		// (get) Token: 0x06001CE3 RID: 7395 RVA: 0x0009A198 File Offset: 0x00098398
		public Rect2D Bounds
		{
			get
			{
				return this.mBoundingBox;
			}
		}

		// Token: 0x17000418 RID: 1048
		// (get) Token: 0x06001CE4 RID: 7396 RVA: 0x0009A1A0 File Offset: 0x000983A0
		public virtual TriangulationMode TriangulationMode
		{
			get
			{
				return TriangulationMode.Unconstrained;
			}
		}

		// Token: 0x17000419 RID: 1049
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

		// Token: 0x06001CE7 RID: 7399 RVA: 0x0009A1C8 File Offset: 0x000983C8
		public PointSet(List<TriangulationPoint> bounds)
		{
			foreach (TriangulationPoint triangulationPoint in bounds)
			{
				this.Add(triangulationPoint, -1, false);
				this.mBoundingBox.AddPoint(triangulationPoint);
			}
			this.mEpsilon = base.CalculateEpsilon();
			this.mWindingOrder = Point2DList.WindingOrderType.Unknown;
		}

		// Token: 0x06001CE8 RID: 7400 RVA: 0x0009A254 File Offset: 0x00098454
		IEnumerator<TriangulationPoint> IEnumerable<TriangulationPoint>.GetEnumerator()
		{
			return new TriangulationPointEnumerator(this.mPoints);
		}

		// Token: 0x06001CE9 RID: 7401 RVA: 0x0009A261 File Offset: 0x00098461
		public int IndexOf(TriangulationPoint p)
		{
			return this.mPoints.IndexOf(p);
		}

		// Token: 0x06001CEA RID: 7402 RVA: 0x0009A26F File Offset: 0x0009846F
		public override void Add(Point2D p)
		{
			this.Add(p as TriangulationPoint, -1, false);
		}

		// Token: 0x06001CEB RID: 7403 RVA: 0x0009A280 File Offset: 0x00098480
		public virtual void Add(TriangulationPoint p)
		{
			this.Add(p, -1, false);
		}

		// Token: 0x06001CEC RID: 7404 RVA: 0x0009A28C File Offset: 0x0009848C
		protected override void Add(Point2D p, int idx, bool constrainToBounds)
		{
			this.Add(p as TriangulationPoint, idx, constrainToBounds);
		}

		// Token: 0x06001CED RID: 7405 RVA: 0x0009A2A0 File Offset: 0x000984A0
		protected bool Add(TriangulationPoint p, int idx, bool constrainToBounds)
		{
			if (p == null)
			{
				return false;
			}
			if (constrainToBounds)
			{
				this.ConstrainPointToBounds(p);
			}
			if (this.mPointMap.ContainsKey(p.VertexCode))
			{
				return true;
			}
			this.mPointMap.Add(p.VertexCode, p);
			if (idx < 0)
			{
				this.mPoints.Add(p);
			}
			else
			{
				this.mPoints.Insert(idx, p);
			}
			return true;
		}

		// Token: 0x06001CEE RID: 7406 RVA: 0x0009A304 File Offset: 0x00098504
		public override void AddRange(IEnumerator<Point2D> iter, Point2DList.WindingOrderType windingOrder)
		{
			if (iter == null)
			{
				return;
			}
			iter.Reset();
			while (iter.MoveNext())
			{
				Point2D point2D = iter.Current;
				this.Add(point2D);
			}
		}

		// Token: 0x06001CEF RID: 7407 RVA: 0x0009A328 File Offset: 0x00098528
		public virtual bool AddRange(List<TriangulationPoint> points)
		{
			bool flag = true;
			foreach (TriangulationPoint triangulationPoint in points)
			{
				flag = this.Add(triangulationPoint, -1, false) && flag;
			}
			return flag;
		}

		// Token: 0x06001CF0 RID: 7408 RVA: 0x0009A380 File Offset: 0x00098580
		public bool TryGetPoint(double x, double y, out TriangulationPoint p)
		{
			uint num = TriangulationPoint.CreateVertexCode(x, y, this.Precision);
			return this.mPointMap.TryGetValue(num, out p);
		}

		// Token: 0x06001CF1 RID: 7409 RVA: 0x0009A3AD File Offset: 0x000985AD
		public void Insert(int idx, TriangulationPoint item)
		{
			this.mPoints.Insert(idx, item);
		}

		// Token: 0x06001CF2 RID: 7410 RVA: 0x0009A3BC File Offset: 0x000985BC
		public override bool Remove(Point2D p)
		{
			return this.mPoints.Remove(p);
		}

		// Token: 0x06001CF3 RID: 7411 RVA: 0x0009A3CA File Offset: 0x000985CA
		public bool Remove(TriangulationPoint p)
		{
			return this.mPoints.Remove(p);
		}

		// Token: 0x06001CF4 RID: 7412 RVA: 0x0009A3D8 File Offset: 0x000985D8
		public override void RemoveAt(int idx)
		{
			if (idx < 0 || idx >= base.Count)
			{
				return;
			}
			this.mPoints.RemoveAt(idx);
		}

		// Token: 0x06001CF5 RID: 7413 RVA: 0x0009A3F4 File Offset: 0x000985F4
		public bool Contains(TriangulationPoint p)
		{
			return this.mPoints.Contains(p);
		}

		// Token: 0x06001CF6 RID: 7414 RVA: 0x0009A404 File Offset: 0x00098604
		public void CopyTo(TriangulationPoint[] array, int arrayIndex)
		{
			int num = Math.Min(base.Count, array.Length - arrayIndex);
			for (int i = 0; i < num; i++)
			{
				array[arrayIndex + i] = this.mPoints[i] as TriangulationPoint;
			}
		}

		// Token: 0x06001CF7 RID: 7415 RVA: 0x0009A444 File Offset: 0x00098644
		protected bool ConstrainPointToBounds(Point2D p)
		{
			double x = p.X;
			double y = p.Y;
			p.X = Math.Max(this.MinX, p.X);
			p.X = Math.Min(this.MaxX, p.X);
			p.Y = Math.Max(this.MinY, p.Y);
			p.Y = Math.Min(this.MaxY, p.Y);
			return p.X != x || p.Y != y;
		}

		// Token: 0x06001CF8 RID: 7416 RVA: 0x0009A4D4 File Offset: 0x000986D4
		protected bool ConstrainPointToBounds(TriangulationPoint p)
		{
			double x = p.X;
			double y = p.Y;
			p.X = Math.Max(this.MinX, p.X);
			p.X = Math.Min(this.MaxX, p.X);
			p.Y = Math.Max(this.MinY, p.Y);
			p.Y = Math.Min(this.MaxY, p.Y);
			return p.X != x || p.Y != y;
		}

		// Token: 0x06001CF9 RID: 7417 RVA: 0x0009A562 File Offset: 0x00098762
		public virtual void AddTriangle(DelaunayTriangle t)
		{
			this.Triangles.Add(t);
		}

		// Token: 0x06001CFA RID: 7418 RVA: 0x0009A570 File Offset: 0x00098770
		public void AddTriangles(IEnumerable<DelaunayTriangle> list)
		{
			foreach (DelaunayTriangle delaunayTriangle in list)
			{
				this.AddTriangle(delaunayTriangle);
			}
		}

		// Token: 0x06001CFB RID: 7419 RVA: 0x0009A5B8 File Offset: 0x000987B8
		public void ClearTriangles()
		{
			this.Triangles.Clear();
		}

		// Token: 0x06001CFC RID: 7420 RVA: 0x0009A5C5 File Offset: 0x000987C5
		public virtual bool Initialize()
		{
			return true;
		}

		// Token: 0x06001CFD RID: 7421 RVA: 0x0009A5C8 File Offset: 0x000987C8
		public virtual void Prepare(TriangulationContext tcx)
		{
			if (this.Triangles == null)
			{
				this.Triangles = new List<DelaunayTriangle>(this.Points.Count);
			}
			else
			{
				this.Triangles.Clear();
			}
			tcx.Points.AddRange(this.Points);
		}

		// Token: 0x040017A6 RID: 6054
		protected Dictionary<uint, TriangulationPoint> mPointMap = new Dictionary<uint, TriangulationPoint>();

		// Token: 0x040017AC RID: 6060
		protected double mPrecision = TriangulationPoint.kVertexCodeDefaultPrecision;
	}
}

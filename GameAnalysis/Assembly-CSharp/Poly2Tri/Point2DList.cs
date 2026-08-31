using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Poly2Tri
{
	// Token: 0x020004EA RID: 1258
	public class Point2DList : IEnumerable<Point2D>, IEnumerable, IList<Point2D>, ICollection<Point2D>
	{
		// Token: 0x17000435 RID: 1077
		// (get) Token: 0x06001DB6 RID: 7606 RVA: 0x0009C0D8 File Offset: 0x0009A2D8
		public Rect2D BoundingBox
		{
			get
			{
				return this.mBoundingBox;
			}
		}

		// Token: 0x17000436 RID: 1078
		// (get) Token: 0x06001DB7 RID: 7607 RVA: 0x0009C0E0 File Offset: 0x0009A2E0
		// (set) Token: 0x06001DB8 RID: 7608 RVA: 0x0009C0E8 File Offset: 0x0009A2E8
		public Point2DList.WindingOrderType WindingOrder
		{
			get
			{
				return this.mWindingOrder;
			}
			set
			{
				if (this.mWindingOrder == Point2DList.WindingOrderType.Unknown)
				{
					this.mWindingOrder = this.CalculateWindingOrder();
				}
				if (value != this.mWindingOrder)
				{
					this.mPoints.Reverse();
					this.mWindingOrder = value;
				}
			}
		}

		// Token: 0x17000437 RID: 1079
		// (get) Token: 0x06001DB9 RID: 7609 RVA: 0x0009C11A File Offset: 0x0009A31A
		public double Epsilon
		{
			get
			{
				return this.mEpsilon;
			}
		}

		// Token: 0x17000438 RID: 1080
		public Point2D this[int index]
		{
			get
			{
				return this.mPoints[index];
			}
			set
			{
				this.mPoints[index] = value;
			}
		}

		// Token: 0x17000439 RID: 1081
		// (get) Token: 0x06001DBC RID: 7612 RVA: 0x0009C13F File Offset: 0x0009A33F
		public int Count
		{
			get
			{
				return this.mPoints.Count;
			}
		}

		// Token: 0x1700043A RID: 1082
		// (get) Token: 0x06001DBD RID: 7613 RVA: 0x0009C14C File Offset: 0x0009A34C
		public virtual bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06001DBE RID: 7614 RVA: 0x0009C14F File Offset: 0x0009A34F
		public Point2DList()
		{
		}

		// Token: 0x06001DBF RID: 7615 RVA: 0x0009C17F File Offset: 0x0009A37F
		public Point2DList(int capacity)
		{
			this.mPoints.Capacity = capacity;
		}

		// Token: 0x06001DC0 RID: 7616 RVA: 0x0009C1BB File Offset: 0x0009A3BB
		public Point2DList(IList<Point2D> l)
		{
			this.AddRange(l.GetEnumerator(), Point2DList.WindingOrderType.Unknown);
		}

		// Token: 0x06001DC1 RID: 7617 RVA: 0x0009C1F8 File Offset: 0x0009A3F8
		public Point2DList(Point2DList l)
		{
			int count = l.Count;
			for (int i = 0; i < count; i++)
			{
				this.mPoints.Add(l[i]);
			}
			this.mBoundingBox.Set(l.BoundingBox);
			this.mEpsilon = l.Epsilon;
			this.mWindingOrder = l.WindingOrder;
		}

		// Token: 0x06001DC2 RID: 7618 RVA: 0x0009C284 File Offset: 0x0009A484
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.Count; i++)
			{
				stringBuilder.Append(this[i].ToString());
				if (i < this.Count - 1)
				{
					stringBuilder.Append(" ");
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06001DC3 RID: 7619 RVA: 0x0009C2D8 File Offset: 0x0009A4D8
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.mPoints.GetEnumerator();
		}

		// Token: 0x06001DC4 RID: 7620 RVA: 0x0009C2EA File Offset: 0x0009A4EA
		IEnumerator<Point2D> IEnumerable<Point2D>.GetEnumerator()
		{
			return new Point2DEnumerator(this.mPoints);
		}

		// Token: 0x06001DC5 RID: 7621 RVA: 0x0009C2F7 File Offset: 0x0009A4F7
		public void Clear()
		{
			this.mPoints.Clear();
			this.mBoundingBox.Clear();
			this.mEpsilon = MathUtil.EPSILON;
			this.mWindingOrder = Point2DList.WindingOrderType.Unknown;
		}

		// Token: 0x06001DC6 RID: 7622 RVA: 0x0009C321 File Offset: 0x0009A521
		public int IndexOf(Point2D p)
		{
			return this.mPoints.IndexOf(p);
		}

		// Token: 0x06001DC7 RID: 7623 RVA: 0x0009C32F File Offset: 0x0009A52F
		public virtual void Add(Point2D p)
		{
			this.Add(p, -1, true);
		}

		// Token: 0x06001DC8 RID: 7624 RVA: 0x0009C33C File Offset: 0x0009A53C
		protected virtual void Add(Point2D p, int idx, bool bCalcWindingOrderAndEpsilon)
		{
			if (idx < 0)
			{
				this.mPoints.Add(p);
			}
			else
			{
				this.mPoints.Insert(idx, p);
			}
			this.mBoundingBox.AddPoint(p);
			if (bCalcWindingOrderAndEpsilon)
			{
				if (this.mWindingOrder == Point2DList.WindingOrderType.Unknown)
				{
					this.mWindingOrder = this.CalculateWindingOrder();
				}
				this.mEpsilon = this.CalculateEpsilon();
			}
		}

		// Token: 0x06001DC9 RID: 7625 RVA: 0x0009C398 File Offset: 0x0009A598
		public virtual void AddRange(Point2DList l)
		{
			this.AddRange(l.mPoints.GetEnumerator(), l.WindingOrder);
		}

		// Token: 0x06001DCA RID: 7626 RVA: 0x0009C3B8 File Offset: 0x0009A5B8
		public virtual void AddRange(IEnumerator<Point2D> iter, Point2DList.WindingOrderType windingOrder)
		{
			if (iter == null)
			{
				return;
			}
			if (this.mWindingOrder == Point2DList.WindingOrderType.Unknown && this.Count == 0)
			{
				this.mWindingOrder = windingOrder;
			}
			bool flag = this.WindingOrder != Point2DList.WindingOrderType.Unknown && windingOrder != Point2DList.WindingOrderType.Unknown && this.WindingOrder != windingOrder;
			bool flag2 = true;
			int count = this.mPoints.Count;
			iter.Reset();
			while (iter.MoveNext())
			{
				if (!flag2)
				{
					flag2 = true;
					this.mPoints.Add(iter.Current);
				}
				else if (flag)
				{
					this.mPoints.Insert(count, iter.Current);
				}
				else
				{
					this.mPoints.Add(iter.Current);
				}
				this.mBoundingBox.AddPoint(iter.Current);
			}
			if (this.mWindingOrder == Point2DList.WindingOrderType.Unknown && windingOrder == Point2DList.WindingOrderType.Unknown)
			{
				this.mWindingOrder = this.CalculateWindingOrder();
			}
			this.mEpsilon = this.CalculateEpsilon();
		}

		// Token: 0x06001DCB RID: 7627 RVA: 0x0009C492 File Offset: 0x0009A692
		public virtual void Insert(int idx, Point2D item)
		{
			this.Add(item, idx, true);
		}

		// Token: 0x06001DCC RID: 7628 RVA: 0x0009C49D File Offset: 0x0009A69D
		public virtual bool Remove(Point2D p)
		{
			if (this.mPoints.Remove(p))
			{
				this.CalculateBounds();
				this.mEpsilon = this.CalculateEpsilon();
				return true;
			}
			return false;
		}

		// Token: 0x06001DCD RID: 7629 RVA: 0x0009C4C2 File Offset: 0x0009A6C2
		public virtual void RemoveAt(int idx)
		{
			if (idx < 0 || idx >= this.Count)
			{
				return;
			}
			this.mPoints.RemoveAt(idx);
			this.CalculateBounds();
			this.mEpsilon = this.CalculateEpsilon();
		}

		// Token: 0x06001DCE RID: 7630 RVA: 0x0009C4F0 File Offset: 0x0009A6F0
		public virtual void RemoveRange(int idxStart, int count)
		{
			if (idxStart < 0 || idxStart >= this.Count)
			{
				return;
			}
			if (count == 0)
			{
				return;
			}
			this.mPoints.RemoveRange(idxStart, count);
			this.CalculateBounds();
			this.mEpsilon = this.CalculateEpsilon();
		}

		// Token: 0x06001DCF RID: 7631 RVA: 0x0009C523 File Offset: 0x0009A723
		public bool Contains(Point2D p)
		{
			return this.mPoints.Contains(p);
		}

		// Token: 0x06001DD0 RID: 7632 RVA: 0x0009C534 File Offset: 0x0009A734
		public void CopyTo(Point2D[] array, int arrayIndex)
		{
			int num = Math.Min(this.Count, array.Length - arrayIndex);
			for (int i = 0; i < num; i++)
			{
				array[arrayIndex + i] = this.mPoints[i];
			}
		}

		// Token: 0x06001DD1 RID: 7633 RVA: 0x0009C570 File Offset: 0x0009A770
		public void CalculateBounds()
		{
			this.mBoundingBox.Clear();
			foreach (Point2D point2D in this.mPoints)
			{
				this.mBoundingBox.AddPoint(point2D);
			}
		}

		// Token: 0x06001DD2 RID: 7634 RVA: 0x0009C5D4 File Offset: 0x0009A7D4
		public double CalculateEpsilon()
		{
			return Math.Max(Math.Min(this.mBoundingBox.Width, this.mBoundingBox.Height) * 0.0010000000474974513, MathUtil.EPSILON);
		}

		// Token: 0x06001DD3 RID: 7635 RVA: 0x0009C608 File Offset: 0x0009A808
		public Point2DList.WindingOrderType CalculateWindingOrder()
		{
			double signedArea = this.GetSignedArea();
			if (signedArea < 0.0)
			{
				return Point2DList.WindingOrderType.CW;
			}
			if (signedArea > 0.0)
			{
				return Point2DList.WindingOrderType.CCW;
			}
			return Point2DList.WindingOrderType.Unknown;
		}

		// Token: 0x06001DD4 RID: 7636 RVA: 0x0009C639 File Offset: 0x0009A839
		public int NextIndex(int index)
		{
			if (index == this.Count - 1)
			{
				return 0;
			}
			return index + 1;
		}

		// Token: 0x06001DD5 RID: 7637 RVA: 0x0009C64B File Offset: 0x0009A84B
		public int PreviousIndex(int index)
		{
			if (index == 0)
			{
				return this.Count - 1;
			}
			return index - 1;
		}

		// Token: 0x06001DD6 RID: 7638 RVA: 0x0009C65C File Offset: 0x0009A85C
		public double GetSignedArea()
		{
			double num = 0.0;
			for (int i = 0; i < this.Count; i++)
			{
				int num2 = (i + 1) % this.Count;
				num += this[i].X * this[num2].Y;
				num -= this[i].Y * this[num2].X;
			}
			return num / 2.0;
		}

		// Token: 0x06001DD7 RID: 7639 RVA: 0x0009C6D4 File Offset: 0x0009A8D4
		public double GetArea()
		{
			double num = 0.0;
			for (int i = 0; i < this.Count; i++)
			{
				int num2 = (i + 1) % this.Count;
				num += this[i].X * this[num2].Y;
				num -= this[i].Y * this[num2].X;
			}
			num /= 2.0;
			if (num >= 0.0)
			{
				return num;
			}
			return -num;
		}

		// Token: 0x06001DD8 RID: 7640 RVA: 0x0009C75C File Offset: 0x0009A95C
		public Point2D GetCentroid()
		{
			Point2D point2D = new Point2D();
			double num = 0.0;
			Point2D point2D2 = new Point2D();
			for (int i = 0; i < this.Count; i++)
			{
				Point2D point2D3 = point2D2;
				Point2D point2D4 = this[i];
				Point2D point2D5 = ((i + 1 < this.Count) ? this[i + 1] : this[0]);
				Point2D point2D6 = point2D4 - point2D3;
				Point2D point2D7 = point2D5 - point2D3;
				double num2 = Point2D.Cross(point2D6, point2D7);
				double num3 = 0.5 * num2;
				num += num3;
				point2D += num3 * 0.3333333333333333 * (point2D3 + point2D4 + point2D5);
			}
			return point2D * (1.0 / num);
		}

		// Token: 0x06001DD9 RID: 7641 RVA: 0x0009C82C File Offset: 0x0009AA2C
		public void Translate(Point2D vector)
		{
			for (int i = 0; i < this.Count; i++)
			{
				int num = i;
				this[num] += vector;
			}
		}

		// Token: 0x06001DDA RID: 7642 RVA: 0x0009C860 File Offset: 0x0009AA60
		public void Scale(Point2D value)
		{
			for (int i = 0; i < this.Count; i++)
			{
				int num = i;
				this[num] *= value;
			}
		}

		// Token: 0x06001DDB RID: 7643 RVA: 0x0009C894 File Offset: 0x0009AA94
		public void Rotate(double radians)
		{
			double num = Math.Cos(radians);
			double num2 = Math.Sin(radians);
			foreach (Point2D point2D in this.mPoints)
			{
				double x = point2D.X;
				point2D.X = x * num - point2D.Y * num2;
				point2D.Y = x * num2 + point2D.Y * num;
			}
		}

		// Token: 0x06001DDC RID: 7644 RVA: 0x0009C91C File Offset: 0x0009AB1C
		public bool IsDegenerate()
		{
			if (this.Count < 3)
			{
				return false;
			}
			if (this.Count < 3)
			{
				return false;
			}
			for (int i = 0; i < this.Count; i++)
			{
				int num = this.PreviousIndex(i);
				if (this.mPoints[num].Equals(this.mPoints[i], this.Epsilon))
				{
					return true;
				}
				int num2 = this.PreviousIndex(num);
				if (TriangulationUtil.Orient2d(this.mPoints[num2], this.mPoints[num], this.mPoints[i]) == Orientation.Collinear)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001DDD RID: 7645 RVA: 0x0009C9B8 File Offset: 0x0009ABB8
		public bool IsConvex()
		{
			bool flag = false;
			for (int i = 0; i < this.Count; i++)
			{
				int num = ((i == 0) ? (this.Count - 1) : (i - 1));
				int num2 = i;
				int num3 = ((i == this.Count - 1) ? 0 : (i + 1));
				double num4 = this[num2].X - this[num].X;
				double num5 = this[num2].Y - this[num].Y;
				double num6 = this[num3].X - this[num2].X;
				double num7 = this[num3].Y - this[num2].Y;
				bool flag2 = num4 * num7 - num6 * num5 >= 0.0;
				if (i == 0)
				{
					flag = flag2;
				}
				else if (flag != flag2)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06001DDE RID: 7646 RVA: 0x0009CA9C File Offset: 0x0009AC9C
		public bool IsSimple()
		{
			for (int i = 0; i < this.Count; i++)
			{
				int num = this.NextIndex(i);
				for (int j = i + 1; j < this.Count; j++)
				{
					int num2 = this.NextIndex(j);
					Point2D point2D = null;
					if (TriangulationUtil.LinesIntersect2D(this.mPoints[i], this.mPoints[num], this.mPoints[j], this.mPoints[num2], ref point2D, this.mEpsilon))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06001DDF RID: 7647 RVA: 0x0009CB24 File Offset: 0x0009AD24
		public Point2DList.PolygonError CheckPolygon()
		{
			Point2DList.PolygonError polygonError = Point2DList.PolygonError.None;
			if (this.Count < 3 || this.Count > Point2DList.kMaxPolygonVertices)
			{
				return polygonError | Point2DList.PolygonError.NotEnoughVertices;
			}
			if (this.IsDegenerate())
			{
				polygonError |= Point2DList.PolygonError.Degenerate;
			}
			if (!this.IsSimple())
			{
				polygonError |= Point2DList.PolygonError.NotSimple;
			}
			if (this.GetArea() < MathUtil.EPSILON)
			{
				polygonError |= Point2DList.PolygonError.AreaTooSmall;
			}
			if ((polygonError & Point2DList.PolygonError.NotSimple) != Point2DList.PolygonError.NotSimple)
			{
				bool flag = false;
				Point2DList.WindingOrderType windingOrderType = Point2DList.WindingOrderType.CCW;
				Point2DList.WindingOrderType windingOrderType2 = Point2DList.WindingOrderType.CW;
				if (this.WindingOrder == windingOrderType2)
				{
					this.WindingOrder = windingOrderType;
					flag = true;
				}
				Point2D[] array = new Point2D[this.Count];
				Point2DList point2DList = new Point2DList(this.Count);
				for (int i = 0; i < this.Count; i++)
				{
					point2DList.Add(new Point2D(this[i].X, this[i].Y));
					int num = i;
					int num2 = this.NextIndex(i);
					Point2D point2D = new Point2D(this[num2].X - this[num].X, this[num2].Y - this[num].Y);
					array[i] = Point2D.Perpendicular(point2D, 1.0);
					array[i].Normalize();
				}
				for (int j = 0; j < this.Count; j++)
				{
					int num3 = this.PreviousIndex(j);
					if ((double)Math.Abs((float)Math.Asin(MathUtil.Clamp(Point2D.Cross(array[num3], array[j]), -1.0, 1.0))) <= Point2DList.kAngularSlop)
					{
						polygonError |= Point2DList.PolygonError.SidesTooCloseToParallel;
						break;
					}
				}
				if (flag)
				{
					this.WindingOrder = windingOrderType2;
				}
			}
			return polygonError;
		}

		// Token: 0x06001DE0 RID: 7648 RVA: 0x0009CCCC File Offset: 0x0009AECC
		public static string GetErrorString(Point2DList.PolygonError error)
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			if (error == Point2DList.PolygonError.None)
			{
				stringBuilder.AppendFormat("No errors.\n", Array.Empty<object>());
			}
			else
			{
				if ((error & Point2DList.PolygonError.NotEnoughVertices) == Point2DList.PolygonError.NotEnoughVertices)
				{
					stringBuilder.AppendFormat("NotEnoughVertices: must have between 3 and {0} vertices.\n", Point2DList.kMaxPolygonVertices);
				}
				if ((error & Point2DList.PolygonError.NotConvex) == Point2DList.PolygonError.NotConvex)
				{
					stringBuilder.AppendFormat("NotConvex: Polygon is not convex.\n", Array.Empty<object>());
				}
				if ((error & Point2DList.PolygonError.NotSimple) == Point2DList.PolygonError.NotSimple)
				{
					stringBuilder.AppendFormat("NotSimple: Polygon is not simple (i.e. it intersects itself).\n", Array.Empty<object>());
				}
				if ((error & Point2DList.PolygonError.AreaTooSmall) == Point2DList.PolygonError.AreaTooSmall)
				{
					stringBuilder.AppendFormat("AreaTooSmall: Polygon's area is too small.\n", Array.Empty<object>());
				}
				if ((error & Point2DList.PolygonError.SidesTooCloseToParallel) == Point2DList.PolygonError.SidesTooCloseToParallel)
				{
					stringBuilder.AppendFormat("SidesTooCloseToParallel: Polygon's sides are too close to parallel.\n", Array.Empty<object>());
				}
				if ((error & Point2DList.PolygonError.TooThin) == Point2DList.PolygonError.TooThin)
				{
					stringBuilder.AppendFormat("TooThin: Polygon is too thin or core shape generation would move edge past centroid.\n", Array.Empty<object>());
				}
				if ((error & Point2DList.PolygonError.Degenerate) == Point2DList.PolygonError.Degenerate)
				{
					stringBuilder.AppendFormat("Degenerate: Polygon is degenerate (contains collinear points or duplicate coincident points).\n", Array.Empty<object>());
				}
				if ((error & Point2DList.PolygonError.Unknown) == Point2DList.PolygonError.Unknown)
				{
					stringBuilder.AppendFormat("Unknown: Unknown Polygon error!.\n", Array.Empty<object>());
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06001DE1 RID: 7649 RVA: 0x0009CDD0 File Offset: 0x0009AFD0
		public void RemoveDuplicateNeighborPoints()
		{
			int num = this.Count;
			int num2 = num - 1;
			int num3 = 0;
			while (num > 1 && num3 < num)
			{
				if (this.mPoints[num2].Equals(this.mPoints[num3]))
				{
					int num4 = Math.Max(num2, num3);
					this.mPoints.RemoveAt(num4);
					num--;
					if (num2 >= num)
					{
						num2 = num - 1;
					}
				}
				else
				{
					num2 = this.NextIndex(num2);
					num3++;
				}
			}
		}

		// Token: 0x06001DE2 RID: 7650 RVA: 0x0009CE41 File Offset: 0x0009B041
		public void Simplify()
		{
			this.Simplify(0.0);
		}

		// Token: 0x06001DE3 RID: 7651 RVA: 0x0009CE54 File Offset: 0x0009B054
		public void Simplify(double bias)
		{
			if (this.Count < 3)
			{
				return;
			}
			int num = 0;
			int num2 = this.Count;
			double num3 = bias * bias;
			while (num < num2 && num2 >= 3)
			{
				int num4 = this.PreviousIndex(num);
				int num5 = this.NextIndex(num);
				Point2D point2D = this[num4];
				Point2D point2D2 = this[num];
				Point2D point2D3 = this[num5];
				if ((point2D - point2D2).MagnitudeSquared() <= num3)
				{
					this.RemoveAt(num);
					num2--;
				}
				else if (TriangulationUtil.Orient2d(point2D, point2D2, point2D3) == Orientation.Collinear)
				{
					this.RemoveAt(num);
					num2--;
				}
				else
				{
					num++;
				}
			}
		}

		// Token: 0x06001DE4 RID: 7652 RVA: 0x0009CEEC File Offset: 0x0009B0EC
		public void MergeParallelEdges(double tolerance)
		{
			if (this.Count <= 3)
			{
				return;
			}
			bool[] array = new bool[this.Count];
			int num = this.Count;
			for (int i = 0; i < this.Count; i++)
			{
				int num2 = ((i == 0) ? (this.Count - 1) : (i - 1));
				int num3 = i;
				int num4 = ((i == this.Count - 1) ? 0 : (i + 1));
				double num5 = this[num3].X - this[num2].X;
				double num6 = this[num3].Y - this[num2].Y;
				double num7 = this[num4].Y - this[num3].X;
				double num8 = this[num4].Y - this[num3].Y;
				double num9 = Math.Sqrt(num5 * num5 + num6 * num6);
				double num10 = Math.Sqrt(num7 * num7 + num8 * num8);
				if ((num9 <= 0.0 || num10 <= 0.0) && num > 3)
				{
					array[i] = true;
					num--;
				}
				double num11 = num5 / num9;
				num6 /= num9;
				num7 /= num10;
				num8 /= num10;
				double num12 = num11 * num8 - num7 * num6;
				double num13 = num11 * num7 + num6 * num8;
				if (Math.Abs(num12) < tolerance && num13 > 0.0 && num > 3)
				{
					array[i] = true;
					num--;
				}
				else
				{
					array[i] = false;
				}
			}
			if (num == this.Count || num == 0)
			{
				return;
			}
			int num14 = 0;
			Point2DList point2DList = new Point2DList(this);
			this.Clear();
			for (int j = 0; j < point2DList.Count; j++)
			{
				if (!array[j] && num != 0 && num14 != num)
				{
					if (num14 >= num)
					{
						throw new Exception(string.Concat(new string[]
						{
							"Point2DList::MergeParallelEdges - currIndex[ ",
							num14.ToString(),
							"] >= newNVertices[",
							num.ToString(),
							"]"
						}));
					}
					this.mPoints.Add(point2DList[j]);
					this.mBoundingBox.AddPoint(point2DList[j]);
					num14++;
				}
			}
			this.mWindingOrder = this.CalculateWindingOrder();
			this.mEpsilon = this.CalculateEpsilon();
		}

		// Token: 0x06001DE5 RID: 7653 RVA: 0x0009D138 File Offset: 0x0009B338
		public void ProjectToAxis(Point2D axis, out double min, out double max)
		{
			double num = Point2D.Dot(axis, this[0]);
			min = num;
			max = num;
			for (int i = 0; i < this.Count; i++)
			{
				num = Point2D.Dot(this[i], axis);
				if (num < min)
				{
					min = num;
				}
				else if (num > max)
				{
					max = num;
				}
			}
		}

		// Token: 0x040017D1 RID: 6097
		public static readonly int kMaxPolygonVertices = 100000;

		// Token: 0x040017D2 RID: 6098
		public static readonly double kLinearSlop = 0.005;

		// Token: 0x040017D3 RID: 6099
		public static readonly double kAngularSlop = 0.0035367765131532297;

		// Token: 0x040017D4 RID: 6100
		protected List<Point2D> mPoints = new List<Point2D>();

		// Token: 0x040017D5 RID: 6101
		protected Rect2D mBoundingBox = new Rect2D();

		// Token: 0x040017D6 RID: 6102
		protected Point2DList.WindingOrderType mWindingOrder = Point2DList.WindingOrderType.Unknown;

		// Token: 0x040017D7 RID: 6103
		protected double mEpsilon = MathUtil.EPSILON;

		// Token: 0x02000C6D RID: 3181
		public enum WindingOrderType
		{
			// Token: 0x04004E5B RID: 20059
			CW,
			// Token: 0x04004E5C RID: 20060
			CCW,
			// Token: 0x04004E5D RID: 20061
			Unknown,
			// Token: 0x04004E5E RID: 20062
			Default = 1
		}

		// Token: 0x02000C6E RID: 3182
		[Flags]
		public enum PolygonError : uint
		{
			// Token: 0x04004E60 RID: 20064
			None = 0U,
			// Token: 0x04004E61 RID: 20065
			NotEnoughVertices = 1U,
			// Token: 0x04004E62 RID: 20066
			NotConvex = 2U,
			// Token: 0x04004E63 RID: 20067
			NotSimple = 4U,
			// Token: 0x04004E64 RID: 20068
			AreaTooSmall = 8U,
			// Token: 0x04004E65 RID: 20069
			SidesTooCloseToParallel = 16U,
			// Token: 0x04004E66 RID: 20070
			TooThin = 32U,
			// Token: 0x04004E67 RID: 20071
			Degenerate = 64U,
			// Token: 0x04004E68 RID: 20072
			Unknown = 1073741824U
		}
	}
}

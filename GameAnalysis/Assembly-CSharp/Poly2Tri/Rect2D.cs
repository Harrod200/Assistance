using System;

namespace Poly2Tri
{
	// Token: 0x020004EB RID: 1259
	public class Rect2D
	{
		// Token: 0x1700043B RID: 1083
		// (get) Token: 0x06001DE7 RID: 7655 RVA: 0x0009D1B2 File Offset: 0x0009B3B2
		// (set) Token: 0x06001DE8 RID: 7656 RVA: 0x0009D1BA File Offset: 0x0009B3BA
		public double MinX
		{
			get
			{
				return this.mMinX;
			}
			set
			{
				this.mMinX = value;
			}
		}

		// Token: 0x1700043C RID: 1084
		// (get) Token: 0x06001DE9 RID: 7657 RVA: 0x0009D1C3 File Offset: 0x0009B3C3
		// (set) Token: 0x06001DEA RID: 7658 RVA: 0x0009D1CB File Offset: 0x0009B3CB
		public double MaxX
		{
			get
			{
				return this.mMaxX;
			}
			set
			{
				this.mMaxX = value;
			}
		}

		// Token: 0x1700043D RID: 1085
		// (get) Token: 0x06001DEB RID: 7659 RVA: 0x0009D1D4 File Offset: 0x0009B3D4
		// (set) Token: 0x06001DEC RID: 7660 RVA: 0x0009D1DC File Offset: 0x0009B3DC
		public double MinY
		{
			get
			{
				return this.mMinY;
			}
			set
			{
				this.mMinY = value;
			}
		}

		// Token: 0x1700043E RID: 1086
		// (get) Token: 0x06001DED RID: 7661 RVA: 0x0009D1E5 File Offset: 0x0009B3E5
		// (set) Token: 0x06001DEE RID: 7662 RVA: 0x0009D1ED File Offset: 0x0009B3ED
		public double MaxY
		{
			get
			{
				return this.mMaxY;
			}
			set
			{
				this.mMaxY = value;
			}
		}

		// Token: 0x1700043F RID: 1087
		// (get) Token: 0x06001DEF RID: 7663 RVA: 0x0009D1F6 File Offset: 0x0009B3F6
		// (set) Token: 0x06001DF0 RID: 7664 RVA: 0x0009D1FE File Offset: 0x0009B3FE
		public double Left
		{
			get
			{
				return this.mMinX;
			}
			set
			{
				this.mMinX = value;
			}
		}

		// Token: 0x17000440 RID: 1088
		// (get) Token: 0x06001DF1 RID: 7665 RVA: 0x0009D207 File Offset: 0x0009B407
		// (set) Token: 0x06001DF2 RID: 7666 RVA: 0x0009D20F File Offset: 0x0009B40F
		public double Right
		{
			get
			{
				return this.mMaxX;
			}
			set
			{
				this.mMaxX = value;
			}
		}

		// Token: 0x17000441 RID: 1089
		// (get) Token: 0x06001DF3 RID: 7667 RVA: 0x0009D218 File Offset: 0x0009B418
		// (set) Token: 0x06001DF4 RID: 7668 RVA: 0x0009D220 File Offset: 0x0009B420
		public double Top
		{
			get
			{
				return this.mMaxY;
			}
			set
			{
				this.mMaxY = value;
			}
		}

		// Token: 0x17000442 RID: 1090
		// (get) Token: 0x06001DF5 RID: 7669 RVA: 0x0009D229 File Offset: 0x0009B429
		// (set) Token: 0x06001DF6 RID: 7670 RVA: 0x0009D231 File Offset: 0x0009B431
		public double Bottom
		{
			get
			{
				return this.mMinY;
			}
			set
			{
				this.mMinY = value;
			}
		}

		// Token: 0x17000443 RID: 1091
		// (get) Token: 0x06001DF7 RID: 7671 RVA: 0x0009D23A File Offset: 0x0009B43A
		public double Width
		{
			get
			{
				return this.Right - this.Left;
			}
		}

		// Token: 0x17000444 RID: 1092
		// (get) Token: 0x06001DF8 RID: 7672 RVA: 0x0009D249 File Offset: 0x0009B449
		public double Height
		{
			get
			{
				return this.Top - this.Bottom;
			}
		}

		// Token: 0x17000445 RID: 1093
		// (get) Token: 0x06001DF9 RID: 7673 RVA: 0x0009D258 File Offset: 0x0009B458
		public bool Empty
		{
			get
			{
				return this.Left == this.Right || this.Top == this.Bottom;
			}
		}

		// Token: 0x06001DFA RID: 7674 RVA: 0x0009D278 File Offset: 0x0009B478
		public Rect2D()
		{
			this.Clear();
		}

		// Token: 0x06001DFB RID: 7675 RVA: 0x0009D286 File Offset: 0x0009B486
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x06001DFC RID: 7676 RVA: 0x0009D290 File Offset: 0x0009B490
		public override bool Equals(object obj)
		{
			Rect2D rect2D = obj as Rect2D;
			if (rect2D != null)
			{
				return this.Equals(rect2D);
			}
			return base.Equals(obj);
		}

		// Token: 0x06001DFD RID: 7677 RVA: 0x0009D2B6 File Offset: 0x0009B4B6
		public bool Equals(Rect2D r)
		{
			return this.Equals(r, MathUtil.EPSILON);
		}

		// Token: 0x06001DFE RID: 7678 RVA: 0x0009D2C4 File Offset: 0x0009B4C4
		public bool Equals(Rect2D r, double epsilon)
		{
			return MathUtil.AreValuesEqual(this.MinX, r.MinX, epsilon) && MathUtil.AreValuesEqual(this.MaxX, r.MaxX) && MathUtil.AreValuesEqual(this.MinY, r.MinY, epsilon) && MathUtil.AreValuesEqual(this.MaxY, r.MaxY, epsilon);
		}

		// Token: 0x06001DFF RID: 7679 RVA: 0x0009D329 File Offset: 0x0009B529
		public void Clear()
		{
			this.MinX = double.MaxValue;
			this.MaxX = double.MinValue;
			this.MinY = double.MaxValue;
			this.MaxY = double.MinValue;
		}

		// Token: 0x06001E00 RID: 7680 RVA: 0x0009D367 File Offset: 0x0009B567
		public void Set(double xmin, double xmax, double ymin, double ymax)
		{
			this.MinX = xmin;
			this.MaxX = xmax;
			this.MinY = ymin;
			this.MaxY = ymax;
			this.Normalize();
		}

		// Token: 0x06001E01 RID: 7681 RVA: 0x0009D38C File Offset: 0x0009B58C
		public void Set(Rect2D b)
		{
			this.MinX = b.MinX;
			this.MaxX = b.MaxX;
			this.MinY = b.MinY;
			this.MaxY = b.MaxY;
		}

		// Token: 0x06001E02 RID: 7682 RVA: 0x0009D3BE File Offset: 0x0009B5BE
		public void SetSize(double w, double h)
		{
			this.Right = this.Left + w;
			this.Top = this.Bottom + h;
		}

		// Token: 0x06001E03 RID: 7683 RVA: 0x0009D3DC File Offset: 0x0009B5DC
		public bool Contains(double x, double y)
		{
			return x > this.Left && y > this.Bottom && x < this.Right && y < this.Top;
		}

		// Token: 0x06001E04 RID: 7684 RVA: 0x0009D404 File Offset: 0x0009B604
		public bool Contains(Point2D p)
		{
			return this.Contains(p.X, p.Y);
		}

		// Token: 0x06001E05 RID: 7685 RVA: 0x0009D418 File Offset: 0x0009B618
		public bool Contains(Rect2D r)
		{
			return this.Left < r.Left && this.Right > r.Right && this.Top < r.Top && this.Bottom > r.Bottom;
		}

		// Token: 0x06001E06 RID: 7686 RVA: 0x0009D454 File Offset: 0x0009B654
		public bool ContainsInclusive(double x, double y)
		{
			return x >= this.Left && y >= this.Top && x <= this.Right && y <= this.Bottom;
		}

		// Token: 0x06001E07 RID: 7687 RVA: 0x0009D47F File Offset: 0x0009B67F
		public bool ContainsInclusive(double x, double y, double epsilon)
		{
			return x + epsilon >= this.Left && y + epsilon >= this.Top && x - epsilon <= this.Right && y - epsilon <= this.Bottom;
		}

		// Token: 0x06001E08 RID: 7688 RVA: 0x0009D4B2 File Offset: 0x0009B6B2
		public bool ContainsInclusive(Point2D p)
		{
			return this.ContainsInclusive(p.X, p.Y);
		}

		// Token: 0x06001E09 RID: 7689 RVA: 0x0009D4C6 File Offset: 0x0009B6C6
		public bool ContainsInclusive(Point2D p, double epsilon)
		{
			return this.ContainsInclusive(p.X, p.Y, epsilon);
		}

		// Token: 0x06001E0A RID: 7690 RVA: 0x0009D4DB File Offset: 0x0009B6DB
		public bool ContainsInclusive(Rect2D r)
		{
			return this.Left <= r.Left && this.Right >= r.Right && this.Top <= r.Top && this.Bottom >= r.Bottom;
		}

		// Token: 0x06001E0B RID: 7691 RVA: 0x0009D51C File Offset: 0x0009B71C
		public bool ContainsInclusive(Rect2D r, double epsilon)
		{
			return this.Left - epsilon <= r.Left && this.Right + epsilon >= r.Right && this.Top - epsilon <= r.Top && this.Bottom + epsilon >= r.Bottom;
		}

		// Token: 0x06001E0C RID: 7692 RVA: 0x0009D56E File Offset: 0x0009B76E
		public bool Intersects(Rect2D r)
		{
			return this.Right > r.Left && this.Left < r.Right && this.Bottom < r.Top && this.Top > r.Bottom;
		}

		// Token: 0x06001E0D RID: 7693 RVA: 0x0009D5AA File Offset: 0x0009B7AA
		public Point2D GetCenter()
		{
			return new Point2D((this.Left + this.Right) / 2.0, (this.Bottom + this.Top) / 2.0);
		}

		// Token: 0x06001E0E RID: 7694 RVA: 0x0009D5DF File Offset: 0x0009B7DF
		public bool IsNormalized()
		{
			return this.Right >= this.Left && this.Bottom <= this.Top;
		}

		// Token: 0x06001E0F RID: 7695 RVA: 0x0009D602 File Offset: 0x0009B802
		public void Normalize()
		{
			if (this.Left > this.Right)
			{
				MathUtil.Swap<double>(ref this.mMinX, ref this.mMaxX);
			}
			if (this.Bottom < this.Top)
			{
				MathUtil.Swap<double>(ref this.mMinY, ref this.mMaxY);
			}
		}

		// Token: 0x06001E10 RID: 7696 RVA: 0x0009D644 File Offset: 0x0009B844
		public void AddPoint(Point2D p)
		{
			this.MinX = Math.Min(this.MinX, p.X);
			this.MaxX = Math.Max(this.MaxX, p.X);
			this.MinY = Math.Min(this.MinY, p.Y);
			this.MaxY = Math.Max(this.MaxY, p.Y);
		}

		// Token: 0x06001E11 RID: 7697 RVA: 0x0009D6AD File Offset: 0x0009B8AD
		public void Inflate(double w, double h)
		{
			this.Left -= w;
			this.Top += h;
			this.Right += w;
			this.Bottom -= h;
		}

		// Token: 0x06001E12 RID: 7698 RVA: 0x0009D6E7 File Offset: 0x0009B8E7
		public void Inflate(double left, double top, double right, double bottom)
		{
			this.Left -= left;
			this.Top += top;
			this.Right += right;
			this.Bottom -= bottom;
		}

		// Token: 0x06001E13 RID: 7699 RVA: 0x0009D722 File Offset: 0x0009B922
		public void Offset(double w, double h)
		{
			this.Left += w;
			this.Top += h;
			this.Right += w;
			this.Bottom += h;
		}

		// Token: 0x06001E14 RID: 7700 RVA: 0x0009D75C File Offset: 0x0009B95C
		public void SetPosition(double x, double y)
		{
			double num = this.Right - this.Left;
			double num2 = this.Bottom - this.Top;
			this.Left = x;
			this.Bottom = y;
			this.Right = x + num;
			this.Top = y + num2;
		}

		// Token: 0x06001E15 RID: 7701 RVA: 0x0009D7A8 File Offset: 0x0009B9A8
		public bool Intersection(Rect2D r1, Rect2D r2)
		{
			if (!TriangulationUtil.RectsIntersect(r1, r2))
			{
				this.Left = (this.Right = (this.Top = (this.Bottom = 0.0)));
				return false;
			}
			this.Left = ((r1.Left > r2.Left) ? r1.Left : r2.Left);
			this.Top = ((r1.Top < r2.Top) ? r1.Top : r2.Top);
			this.Right = ((r1.Right < r2.Right) ? r1.Right : r2.Right);
			this.Bottom = ((r1.Bottom > r2.Bottom) ? r1.Bottom : r2.Bottom);
			return true;
		}

		// Token: 0x06001E16 RID: 7702 RVA: 0x0009D874 File Offset: 0x0009BA74
		public void Union(Rect2D r1, Rect2D r2)
		{
			if (r2.Right == r2.Left || r2.Bottom == r2.Top)
			{
				this.Set(r1);
				return;
			}
			if (r1.Right == r1.Left || r1.Bottom == r1.Top)
			{
				this.Set(r2);
				return;
			}
			this.Left = ((r1.Left < r2.Left) ? r1.Left : r2.Left);
			this.Top = ((r1.Top > r2.Top) ? r1.Top : r2.Top);
			this.Right = ((r1.Right > r2.Right) ? r1.Right : r2.Right);
			this.Bottom = ((r1.Bottom < r2.Bottom) ? r1.Bottom : r2.Bottom);
		}

		// Token: 0x040017D8 RID: 6104
		private double mMinX;

		// Token: 0x040017D9 RID: 6105
		private double mMaxX;

		// Token: 0x040017DA RID: 6106
		private double mMinY;

		// Token: 0x040017DB RID: 6107
		private double mMaxY;
	}
}

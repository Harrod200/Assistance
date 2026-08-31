using System;

namespace Poly2Tri
{
	// Token: 0x020004E8 RID: 1256
	public class Point2D : IComparable<Point2D>
	{
		// Token: 0x1700042E RID: 1070
		// (get) Token: 0x06001D6C RID: 7532 RVA: 0x0009B8C6 File Offset: 0x00099AC6
		// (set) Token: 0x06001D6D RID: 7533 RVA: 0x0009B8CE File Offset: 0x00099ACE
		public virtual double X
		{
			get
			{
				return this.mX;
			}
			set
			{
				this.mX = value;
			}
		}

		// Token: 0x1700042F RID: 1071
		// (get) Token: 0x06001D6E RID: 7534 RVA: 0x0009B8D7 File Offset: 0x00099AD7
		// (set) Token: 0x06001D6F RID: 7535 RVA: 0x0009B8DF File Offset: 0x00099ADF
		public virtual double Y
		{
			get
			{
				return this.mY;
			}
			set
			{
				this.mY = value;
			}
		}

		// Token: 0x17000430 RID: 1072
		// (get) Token: 0x06001D70 RID: 7536 RVA: 0x0009B8E8 File Offset: 0x00099AE8
		// (set) Token: 0x06001D71 RID: 7537 RVA: 0x0009B8F0 File Offset: 0x00099AF0
		public virtual float Zf
		{
			get
			{
				return this.mZf;
			}
			set
			{
				this.mZf = value;
			}
		}

		// Token: 0x17000431 RID: 1073
		// (get) Token: 0x06001D72 RID: 7538 RVA: 0x0009B8F9 File Offset: 0x00099AF9
		public float Xf
		{
			get
			{
				return (float)this.X;
			}
		}

		// Token: 0x17000432 RID: 1074
		// (get) Token: 0x06001D73 RID: 7539 RVA: 0x0009B902 File Offset: 0x00099B02
		public float Yf
		{
			get
			{
				return (float)this.Y;
			}
		}

		// Token: 0x06001D74 RID: 7540 RVA: 0x0009B90B File Offset: 0x00099B0B
		public Point2D()
		{
			this.mX = 0.0;
			this.mY = 0.0;
			this.mZf = 0f;
		}

		// Token: 0x06001D75 RID: 7541 RVA: 0x0009B93C File Offset: 0x00099B3C
		public Point2D(double x, double y)
		{
			this.mX = x;
			this.mY = y;
			this.mZf = 0f;
		}

		// Token: 0x06001D76 RID: 7542 RVA: 0x0009B95D File Offset: 0x00099B5D
		public Point2D(double x, double y, float z)
		{
			this.mX = x;
			this.mY = y;
			this.mZf = z;
		}

		// Token: 0x06001D77 RID: 7543 RVA: 0x0009B97A File Offset: 0x00099B7A
		public Point2D(Point2D p)
		{
			this.mX = p.X;
			this.mY = p.Y;
			this.mZf = p.Zf;
		}

		// Token: 0x06001D78 RID: 7544 RVA: 0x0009B9A8 File Offset: 0x00099BA8
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"[",
				this.X.ToString(),
				",",
				this.Y.ToString(),
				",",
				this.Zf.ToString(),
				"]"
			});
		}

		// Token: 0x06001D79 RID: 7545 RVA: 0x0009BA13 File Offset: 0x00099C13
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x06001D7A RID: 7546 RVA: 0x0009BA1C File Offset: 0x00099C1C
		public override bool Equals(object obj)
		{
			Point2D point2D = obj as Point2D;
			if (point2D != null)
			{
				return this.Equals(point2D);
			}
			return base.Equals(obj);
		}

		// Token: 0x06001D7B RID: 7547 RVA: 0x0009BA42 File Offset: 0x00099C42
		public bool Equals(Point2D p)
		{
			return this.Equals(p, TriangulationPoint.kVertexCodeDefaultPrecision);
		}

		// Token: 0x06001D7C RID: 7548 RVA: 0x0009BA50 File Offset: 0x00099C50
		public bool Equals(Point2D p, double epsilon)
		{
			return p != null && MathUtil.AreValuesEqual(this.X, p.X, epsilon) && MathUtil.AreValuesEqual(this.Y, p.Y, epsilon);
		}

		// Token: 0x06001D7D RID: 7549 RVA: 0x0009BA80 File Offset: 0x00099C80
		public int CompareTo(Point2D other)
		{
			if (this.Y < other.Y)
			{
				return -1;
			}
			if (this.Y > other.Y)
			{
				return 1;
			}
			if (this.X < other.X)
			{
				return -1;
			}
			if (this.X > other.X)
			{
				return 1;
			}
			return 0;
		}

		// Token: 0x06001D7E RID: 7550 RVA: 0x0009BACE File Offset: 0x00099CCE
		public virtual void Set(double x, double y)
		{
			this.X = x;
			this.Y = y;
		}

		// Token: 0x06001D7F RID: 7551 RVA: 0x0009BADE File Offset: 0x00099CDE
		public virtual void Set(Point2D p)
		{
			this.X = p.X;
			this.Y = p.Y;
		}

		// Token: 0x06001D80 RID: 7552 RVA: 0x0009BAF8 File Offset: 0x00099CF8
		public void Add(Point2D p)
		{
			this.X += p.X;
			this.Y += p.Y;
		}

		// Token: 0x06001D81 RID: 7553 RVA: 0x0009BB20 File Offset: 0x00099D20
		public void Add(double scalar)
		{
			this.X += scalar;
			this.Y += scalar;
		}

		// Token: 0x06001D82 RID: 7554 RVA: 0x0009BB3E File Offset: 0x00099D3E
		public void Subtract(Point2D p)
		{
			this.X -= p.X;
			this.Y -= p.Y;
		}

		// Token: 0x06001D83 RID: 7555 RVA: 0x0009BB66 File Offset: 0x00099D66
		public void Subtract(double scalar)
		{
			this.X -= scalar;
			this.Y -= scalar;
		}

		// Token: 0x06001D84 RID: 7556 RVA: 0x0009BB84 File Offset: 0x00099D84
		public void Multiply(Point2D p)
		{
			this.X *= p.X;
			this.Y *= p.Y;
		}

		// Token: 0x06001D85 RID: 7557 RVA: 0x0009BBAC File Offset: 0x00099DAC
		public void Multiply(double scalar)
		{
			this.X *= scalar;
			this.Y *= scalar;
		}

		// Token: 0x06001D86 RID: 7558 RVA: 0x0009BBCA File Offset: 0x00099DCA
		public void Divide(Point2D p)
		{
			this.X /= p.X;
			this.Y /= p.Y;
		}

		// Token: 0x06001D87 RID: 7559 RVA: 0x0009BBF2 File Offset: 0x00099DF2
		public void Divide(double scalar)
		{
			this.X /= scalar;
			this.Y /= scalar;
		}

		// Token: 0x06001D88 RID: 7560 RVA: 0x0009BC10 File Offset: 0x00099E10
		public void Negate()
		{
			this.X = -this.X;
			this.Y = -this.Y;
		}

		// Token: 0x06001D89 RID: 7561 RVA: 0x0009BC2C File Offset: 0x00099E2C
		public double Magnitude()
		{
			return Math.Sqrt(this.X * this.X + this.Y * this.Y);
		}

		// Token: 0x06001D8A RID: 7562 RVA: 0x0009BC4E File Offset: 0x00099E4E
		public double MagnitudeSquared()
		{
			return this.X * this.X + this.Y * this.Y;
		}

		// Token: 0x06001D8B RID: 7563 RVA: 0x0009BC6B File Offset: 0x00099E6B
		public double MagnitudeReciprocal()
		{
			return 1.0 / this.Magnitude();
		}

		// Token: 0x06001D8C RID: 7564 RVA: 0x0009BC7D File Offset: 0x00099E7D
		public void Normalize()
		{
			this.Multiply(this.MagnitudeReciprocal());
		}

		// Token: 0x06001D8D RID: 7565 RVA: 0x0009BC8B File Offset: 0x00099E8B
		public double Dot(Point2D p)
		{
			return this.X * p.X + this.Y * p.Y;
		}

		// Token: 0x06001D8E RID: 7566 RVA: 0x0009BCA8 File Offset: 0x00099EA8
		public double Cross(Point2D p)
		{
			return this.X * p.Y - this.Y * p.X;
		}

		// Token: 0x06001D8F RID: 7567 RVA: 0x0009BCC8 File Offset: 0x00099EC8
		public void Clamp(Point2D low, Point2D high)
		{
			this.X = Math.Max(low.X, Math.Min(this.X, high.X));
			this.Y = Math.Max(low.Y, Math.Min(this.Y, high.Y));
		}

		// Token: 0x06001D90 RID: 7568 RVA: 0x0009BD19 File Offset: 0x00099F19
		public void Abs()
		{
			this.X = Math.Abs(this.X);
			this.Y = Math.Abs(this.Y);
		}

		// Token: 0x06001D91 RID: 7569 RVA: 0x0009BD40 File Offset: 0x00099F40
		public void Reciprocal()
		{
			if (this.X != 0.0 && this.Y != 0.0)
			{
				this.X = 1.0 / this.X;
				this.Y = 1.0 / this.Y;
			}
		}

		// Token: 0x06001D92 RID: 7570 RVA: 0x0009BD9B File Offset: 0x00099F9B
		public void Translate(Point2D vector)
		{
			this.Add(vector);
		}

		// Token: 0x06001D93 RID: 7571 RVA: 0x0009BDA4 File Offset: 0x00099FA4
		public void Translate(double x, double y)
		{
			this.X += x;
			this.Y += y;
		}

		// Token: 0x06001D94 RID: 7572 RVA: 0x0009BDC2 File Offset: 0x00099FC2
		public void Scale(Point2D vector)
		{
			this.Multiply(vector);
		}

		// Token: 0x06001D95 RID: 7573 RVA: 0x0009BDCB File Offset: 0x00099FCB
		public void Scale(double scalar)
		{
			this.Multiply(scalar);
		}

		// Token: 0x06001D96 RID: 7574 RVA: 0x0009BDD4 File Offset: 0x00099FD4
		public void Scale(double x, double y)
		{
			this.X *= x;
			this.Y *= y;
		}

		// Token: 0x06001D97 RID: 7575 RVA: 0x0009BDF4 File Offset: 0x00099FF4
		public void Rotate(double radians)
		{
			double num = Math.Cos(radians);
			double num2 = Math.Sin(radians);
			double x = this.X;
			double y = this.Y;
			this.X = x * num - y * num2;
			this.Y = x * num2 + y * num;
		}

		// Token: 0x06001D98 RID: 7576 RVA: 0x0009BE38 File Offset: 0x0009A038
		public void RotateDegrees(double degrees)
		{
			double num = degrees * 3.141592653589793 / 180.0;
			this.Rotate(num);
		}

		// Token: 0x06001D99 RID: 7577 RVA: 0x0009BE62 File Offset: 0x0009A062
		public static double Dot(Point2D lhs, Point2D rhs)
		{
			return lhs.X * rhs.X + lhs.Y * rhs.Y;
		}

		// Token: 0x06001D9A RID: 7578 RVA: 0x0009BE7F File Offset: 0x0009A07F
		public static double Cross(Point2D lhs, Point2D rhs)
		{
			return lhs.X * rhs.Y - lhs.Y * rhs.X;
		}

		// Token: 0x06001D9B RID: 7579 RVA: 0x0009BE9C File Offset: 0x0009A09C
		public static Point2D Clamp(Point2D a, Point2D low, Point2D high)
		{
			Point2D point2D = new Point2D(a);
			point2D.Clamp(low, high);
			return point2D;
		}

		// Token: 0x06001D9C RID: 7580 RVA: 0x0009BEAC File Offset: 0x0009A0AC
		public static Point2D Min(Point2D a, Point2D b)
		{
			return new Point2D
			{
				X = Math.Min(a.X, b.X),
				Y = Math.Min(a.Y, b.Y)
			};
		}

		// Token: 0x06001D9D RID: 7581 RVA: 0x0009BEE1 File Offset: 0x0009A0E1
		public static Point2D Max(Point2D a, Point2D b)
		{
			return new Point2D
			{
				X = Math.Max(a.X, b.X),
				Y = Math.Max(a.Y, b.Y)
			};
		}

		// Token: 0x06001D9E RID: 7582 RVA: 0x0009BF16 File Offset: 0x0009A116
		public static Point2D Abs(Point2D a)
		{
			return new Point2D(Math.Abs(a.X), Math.Abs(a.Y));
		}

		// Token: 0x06001D9F RID: 7583 RVA: 0x0009BF33 File Offset: 0x0009A133
		public static Point2D Reciprocal(Point2D a)
		{
			return new Point2D(1.0 / a.X, 1.0 / a.Y);
		}

		// Token: 0x06001DA0 RID: 7584 RVA: 0x0009BF5A File Offset: 0x0009A15A
		public static Point2D Perpendicular(Point2D lhs, double scalar)
		{
			return new Point2D(lhs.Y * scalar, lhs.X * -scalar);
		}

		// Token: 0x06001DA1 RID: 7585 RVA: 0x0009BF72 File Offset: 0x0009A172
		public static Point2D Perpendicular(double scalar, Point2D rhs)
		{
			return new Point2D(-scalar * rhs.Y, scalar * rhs.X);
		}

		// Token: 0x06001DA2 RID: 7586 RVA: 0x0009BF8A File Offset: 0x0009A18A
		public static Point2D operator +(Point2D lhs, Point2D rhs)
		{
			Point2D point2D = new Point2D(lhs);
			point2D.Add(rhs);
			return point2D;
		}

		// Token: 0x06001DA3 RID: 7587 RVA: 0x0009BF99 File Offset: 0x0009A199
		public static Point2D operator +(Point2D lhs, double scalar)
		{
			Point2D point2D = new Point2D(lhs);
			point2D.Add(scalar);
			return point2D;
		}

		// Token: 0x06001DA4 RID: 7588 RVA: 0x0009BFA8 File Offset: 0x0009A1A8
		public static Point2D operator -(Point2D lhs, Point2D rhs)
		{
			Point2D point2D = new Point2D(lhs);
			point2D.Subtract(rhs);
			return point2D;
		}

		// Token: 0x06001DA5 RID: 7589 RVA: 0x0009BFB7 File Offset: 0x0009A1B7
		public static Point2D operator -(Point2D lhs, double scalar)
		{
			Point2D point2D = new Point2D(lhs);
			point2D.Subtract(scalar);
			return point2D;
		}

		// Token: 0x06001DA6 RID: 7590 RVA: 0x0009BFC6 File Offset: 0x0009A1C6
		public static Point2D operator *(Point2D lhs, Point2D rhs)
		{
			Point2D point2D = new Point2D(lhs);
			point2D.Multiply(rhs);
			return point2D;
		}

		// Token: 0x06001DA7 RID: 7591 RVA: 0x0009BFD5 File Offset: 0x0009A1D5
		public static Point2D operator *(Point2D lhs, double scalar)
		{
			Point2D point2D = new Point2D(lhs);
			point2D.Multiply(scalar);
			return point2D;
		}

		// Token: 0x06001DA8 RID: 7592 RVA: 0x0009BFE4 File Offset: 0x0009A1E4
		public static Point2D operator *(double scalar, Point2D lhs)
		{
			Point2D point2D = new Point2D(lhs);
			point2D.Multiply(scalar);
			return point2D;
		}

		// Token: 0x06001DA9 RID: 7593 RVA: 0x0009BFF3 File Offset: 0x0009A1F3
		public static Point2D operator /(Point2D lhs, Point2D rhs)
		{
			Point2D point2D = new Point2D(lhs);
			point2D.Divide(rhs);
			return point2D;
		}

		// Token: 0x06001DAA RID: 7594 RVA: 0x0009C002 File Offset: 0x0009A202
		public static Point2D operator /(Point2D lhs, double scalar)
		{
			Point2D point2D = new Point2D(lhs);
			point2D.Divide(scalar);
			return point2D;
		}

		// Token: 0x06001DAB RID: 7595 RVA: 0x0009C011 File Offset: 0x0009A211
		public static Point2D operator -(Point2D p)
		{
			Point2D point2D = new Point2D(p);
			point2D.Negate();
			return point2D;
		}

		// Token: 0x06001DAC RID: 7596 RVA: 0x0009C01F File Offset: 0x0009A21F
		public static bool operator <(Point2D lhs, Point2D rhs)
		{
			return lhs.CompareTo(rhs) == -1;
		}

		// Token: 0x06001DAD RID: 7597 RVA: 0x0009C02E File Offset: 0x0009A22E
		public static bool operator >(Point2D lhs, Point2D rhs)
		{
			return lhs.CompareTo(rhs) == 1;
		}

		// Token: 0x06001DAE RID: 7598 RVA: 0x0009C03D File Offset: 0x0009A23D
		public static bool operator <=(Point2D lhs, Point2D rhs)
		{
			return lhs.CompareTo(rhs) <= 0;
		}

		// Token: 0x06001DAF RID: 7599 RVA: 0x0009C04C File Offset: 0x0009A24C
		public static bool operator >=(Point2D lhs, Point2D rhs)
		{
			return lhs.CompareTo(rhs) >= 0;
		}

		// Token: 0x040017CC RID: 6092
		protected double mX;

		// Token: 0x040017CD RID: 6093
		protected double mY;

		// Token: 0x040017CE RID: 6094
		protected float mZf;
	}
}

using System;
using System.Collections.Generic;

namespace Poly2Tri
{
	// Token: 0x020004DF RID: 1247
	public class TriangulationPoint : Point2D
	{
		// Token: 0x17000425 RID: 1061
		// (get) Token: 0x06001D1F RID: 7455 RVA: 0x0009A8CD File Offset: 0x00098ACD
		// (set) Token: 0x06001D20 RID: 7456 RVA: 0x0009A8D5 File Offset: 0x00098AD5
		public override double X
		{
			get
			{
				return this.mX;
			}
			set
			{
				if (value != this.mX)
				{
					this.mX = value;
					this.mVertexCode = TriangulationPoint.CreateVertexCode(this.mX, this.mY, TriangulationPoint.kVertexCodeDefaultPrecision);
				}
			}
		}

		// Token: 0x17000426 RID: 1062
		// (get) Token: 0x06001D21 RID: 7457 RVA: 0x0009A903 File Offset: 0x00098B03
		// (set) Token: 0x06001D22 RID: 7458 RVA: 0x0009A90B File Offset: 0x00098B0B
		public override double Y
		{
			get
			{
				return this.mY;
			}
			set
			{
				if (value != this.mY)
				{
					this.mY = value;
					this.mVertexCode = TriangulationPoint.CreateVertexCode(this.mX, this.mY, TriangulationPoint.kVertexCodeDefaultPrecision);
				}
			}
		}

		// Token: 0x17000427 RID: 1063
		// (get) Token: 0x06001D23 RID: 7459 RVA: 0x0009A939 File Offset: 0x00098B39
		public uint VertexCode
		{
			get
			{
				return this.mVertexCode;
			}
		}

		// Token: 0x17000428 RID: 1064
		// (get) Token: 0x06001D24 RID: 7460 RVA: 0x0009A941 File Offset: 0x00098B41
		// (set) Token: 0x06001D25 RID: 7461 RVA: 0x0009A949 File Offset: 0x00098B49
		public List<DTSweepConstraint> Edges { get; private set; }

		// Token: 0x17000429 RID: 1065
		// (get) Token: 0x06001D26 RID: 7462 RVA: 0x0009A952 File Offset: 0x00098B52
		public bool HasEdges
		{
			get
			{
				return this.Edges != null;
			}
		}

		// Token: 0x06001D27 RID: 7463 RVA: 0x0009A95D File Offset: 0x00098B5D
		public TriangulationPoint(double x, double y)
			: this(x, y, 0f, TriangulationPoint.kVertexCodeDefaultPrecision)
		{
		}

		// Token: 0x06001D28 RID: 7464 RVA: 0x0009A971 File Offset: 0x00098B71
		public TriangulationPoint(double x, double y, float z)
			: this(x, y, z, TriangulationPoint.kVertexCodeDefaultPrecision)
		{
		}

		// Token: 0x06001D29 RID: 7465 RVA: 0x0009A981 File Offset: 0x00098B81
		public TriangulationPoint(double x, double y, float z, double precision)
			: base(x, y, z)
		{
			this.mVertexCode = TriangulationPoint.CreateVertexCode(x, y, precision);
		}

		// Token: 0x06001D2A RID: 7466 RVA: 0x0009A99B File Offset: 0x00098B9B
		public override string ToString()
		{
			return base.ToString() + ":{" + this.mVertexCode.ToString() + "}";
		}

		// Token: 0x06001D2B RID: 7467 RVA: 0x0009A9BD File Offset: 0x00098BBD
		public override int GetHashCode()
		{
			return (int)this.mVertexCode;
		}

		// Token: 0x06001D2C RID: 7468 RVA: 0x0009A9C8 File Offset: 0x00098BC8
		public override bool Equals(object obj)
		{
			TriangulationPoint triangulationPoint = obj as TriangulationPoint;
			if (triangulationPoint != null)
			{
				return this.mVertexCode == triangulationPoint.VertexCode;
			}
			return base.Equals(obj);
		}

		// Token: 0x06001D2D RID: 7469 RVA: 0x0009A9F5 File Offset: 0x00098BF5
		public override void Set(double x, double y)
		{
			if (x != this.mX || y != this.mY)
			{
				this.mX = x;
				this.mY = y;
				this.mVertexCode = TriangulationPoint.CreateVertexCode(this.mX, this.mY, TriangulationPoint.kVertexCodeDefaultPrecision);
			}
		}

		// Token: 0x06001D2E RID: 7470 RVA: 0x0009AA34 File Offset: 0x00098C34
		public static uint CreateVertexCode(double x, double y, double precision)
		{
			float num = (float)MathUtil.RoundWithPrecision(x, precision);
			float num2 = (float)MathUtil.RoundWithPrecision(y, precision);
			uint num3 = MathUtil.Jenkins32Hash(BitConverter.GetBytes(num), 0U);
			return MathUtil.Jenkins32Hash(BitConverter.GetBytes(num2), num3);
		}

		// Token: 0x06001D2F RID: 7471 RVA: 0x0009AA6C File Offset: 0x00098C6C
		public void AddEdge(DTSweepConstraint e)
		{
			if (this.Edges == null)
			{
				this.Edges = new List<DTSweepConstraint>();
			}
			this.Edges.Add(e);
		}

		// Token: 0x06001D30 RID: 7472 RVA: 0x0009AA90 File Offset: 0x00098C90
		public bool HasEdge(TriangulationPoint p)
		{
			DTSweepConstraint dtsweepConstraint = null;
			return this.GetEdge(p, out dtsweepConstraint);
		}

		// Token: 0x06001D31 RID: 7473 RVA: 0x0009AAA8 File Offset: 0x00098CA8
		public bool GetEdge(TriangulationPoint p, out DTSweepConstraint edge)
		{
			edge = null;
			if (this.Edges == null || this.Edges.Count < 1 || p == null || p.Equals(this))
			{
				return false;
			}
			foreach (DTSweepConstraint dtsweepConstraint in this.Edges)
			{
				if ((dtsweepConstraint.P.Equals(this) && dtsweepConstraint.Q.Equals(p)) || (dtsweepConstraint.P.Equals(p) && dtsweepConstraint.Q.Equals(this)))
				{
					edge = dtsweepConstraint;
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001D32 RID: 7474 RVA: 0x0009AB60 File Offset: 0x00098D60
		public static Point2D ToPoint2D(TriangulationPoint p)
		{
			return p;
		}

		// Token: 0x06001D33 RID: 7475 RVA: 0x0009AB63 File Offset: 0x00098D63
		public void Reset()
		{
			this.Edges = null;
		}

		// Token: 0x040017BD RID: 6077
		public static readonly double kVertexCodeDefaultPrecision = 8.0;

		// Token: 0x040017BE RID: 6078
		protected uint mVertexCode;
	}
}

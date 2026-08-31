using System;

namespace Poly2Tri
{
	// Token: 0x020004DB RID: 1243
	public class TriangulationConstraint : Edge
	{
		// Token: 0x1700041C RID: 1052
		// (get) Token: 0x06001D04 RID: 7428 RVA: 0x0009A654 File Offset: 0x00098854
		// (set) Token: 0x06001D05 RID: 7429 RVA: 0x0009A661 File Offset: 0x00098861
		public TriangulationPoint P
		{
			get
			{
				return this.mP as TriangulationPoint;
			}
			set
			{
				if (value != null && this.mP != value)
				{
					this.mP = value;
					this.CalculateContraintCode();
				}
			}
		}

		// Token: 0x1700041D RID: 1053
		// (get) Token: 0x06001D06 RID: 7430 RVA: 0x0009A67C File Offset: 0x0009887C
		// (set) Token: 0x06001D07 RID: 7431 RVA: 0x0009A689 File Offset: 0x00098889
		public TriangulationPoint Q
		{
			get
			{
				return this.mQ as TriangulationPoint;
			}
			set
			{
				if (value != null && this.mQ != value)
				{
					this.mQ = value;
					this.CalculateContraintCode();
				}
			}
		}

		// Token: 0x1700041E RID: 1054
		// (get) Token: 0x06001D08 RID: 7432 RVA: 0x0009A6A4 File Offset: 0x000988A4
		public uint ConstraintCode
		{
			get
			{
				return this.mContraintCode;
			}
		}

		// Token: 0x06001D09 RID: 7433 RVA: 0x0009A6AC File Offset: 0x000988AC
		public TriangulationConstraint(TriangulationPoint p1, TriangulationPoint p2)
		{
			this.mP = p1;
			this.mQ = p2;
			if (p1.Y > p2.Y)
			{
				this.mQ = p1;
				this.mP = p2;
			}
			else if (p1.Y == p2.Y)
			{
				if (p1.X > p2.X)
				{
					this.mQ = p1;
					this.mP = p2;
				}
				else
				{
					double x = p1.X;
					double x2 = p2.X;
				}
			}
			this.CalculateContraintCode();
		}

		// Token: 0x06001D0A RID: 7434 RVA: 0x0009A72C File Offset: 0x0009892C
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"[P=",
				this.P.ToString(),
				", Q=",
				this.Q.ToString(),
				" : {",
				this.mContraintCode.ToString(),
				"}]"
			});
		}

		// Token: 0x06001D0B RID: 7435 RVA: 0x0009A78E File Offset: 0x0009898E
		public void CalculateContraintCode()
		{
			this.mContraintCode = TriangulationConstraint.CalculateContraintCode(this.P, this.Q);
		}

		// Token: 0x06001D0C RID: 7436 RVA: 0x0009A7A8 File Offset: 0x000989A8
		public static uint CalculateContraintCode(TriangulationPoint p, TriangulationPoint q)
		{
			if (p == null || p == null)
			{
				throw new ArgumentNullException();
			}
			uint num = MathUtil.Jenkins32Hash(BitConverter.GetBytes(p.VertexCode), 0U);
			return MathUtil.Jenkins32Hash(BitConverter.GetBytes(q.VertexCode), num);
		}

		// Token: 0x040017B1 RID: 6065
		private uint mContraintCode;
	}
}

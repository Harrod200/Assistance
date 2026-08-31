using System;
using Poly2Tri;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200055C RID: 1372
	[Serializable]
	public struct CurvedPolyPoint
	{
		// Token: 0x17000514 RID: 1300
		// (get) Token: 0x06002442 RID: 9282 RVA: 0x000C0868 File Offset: 0x000BEA68
		// (set) Token: 0x06002443 RID: 9283 RVA: 0x000C0875 File Offset: 0x000BEA75
		public float x
		{
			get
			{
				return this.anchor.x;
			}
			set
			{
				this.anchor.x = value;
			}
		}

		// Token: 0x17000515 RID: 1301
		// (get) Token: 0x06002444 RID: 9284 RVA: 0x000C0883 File Offset: 0x000BEA83
		// (set) Token: 0x06002445 RID: 9285 RVA: 0x000C0890 File Offset: 0x000BEA90
		public float y
		{
			get
			{
				return this.anchor.y;
			}
			set
			{
				this.anchor.y = value;
			}
		}

		// Token: 0x06002446 RID: 9286 RVA: 0x000C089E File Offset: 0x000BEA9E
		public CurvedPolyPoint(Vector2 val)
		{
			this.bezier = false;
			this.anchor = new Vector2(val.x, val.y);
			this.bezier1 = Vector2.zero;
			this.bezier2 = Vector2.zero;
		}

		// Token: 0x06002447 RID: 9287 RVA: 0x000C08D4 File Offset: 0x000BEAD4
		public CurvedPolyPoint(float x, float y)
		{
			this.bezier = false;
			this.anchor = new Vector2(x, y);
			this.bezier1 = Vector2.zero;
			this.bezier2 = Vector2.zero;
		}

		// Token: 0x06002448 RID: 9288 RVA: 0x000C0900 File Offset: 0x000BEB00
		public CurvedPolyPoint(float x, float y, float b1x, float b1y, float b2x, float b2y)
		{
			this.bezier = true;
			this.anchor = new Vector2(x, y);
			this.bezier1 = new Vector2(b1x, b1y);
			this.bezier2 = new Vector2(b2x, b2y);
		}

		// Token: 0x06002449 RID: 9289 RVA: 0x000C0933 File Offset: 0x000BEB33
		public CurvedPolyPoint(double x, double y, double b1x, double b1y, double b2x, double b2y)
		{
			this.bezier = true;
			this.anchor = new Vector2((float)x, (float)y);
			this.bezier1 = new Vector2((float)b1x, (float)b1y);
			this.bezier2 = new Vector2((float)b2x, (float)b2y);
		}

		// Token: 0x0600244A RID: 9290 RVA: 0x000C096C File Offset: 0x000BEB6C
		public CurvedPolyPoint(Vector6d val)
		{
			this.bezier = true;
			this.bezier1 = new Vector2((float)val.v[0], (float)val.v[1]);
			this.bezier2 = new Vector2((float)val.v[2], (float)val.v[3]);
			this.anchor = new Vector2((float)val.v[4], (float)val.v[5]);
		}

		// Token: 0x0600244B RID: 9291 RVA: 0x000C09D8 File Offset: 0x000BEBD8
		public CurvedPolyPoint NormalizeToRadial(float width, float height)
		{
			this.anchor.x = this.ScaleWidth(this.anchor.x, width);
			this.anchor.y = this.ScaleHeight(this.anchor.y, height);
			if (this.bezier)
			{
				this.bezier1.x = this.ScaleWidth(this.bezier1.x, width);
				this.bezier1.y = this.ScaleHeight(this.bezier1.y, height);
				this.bezier2.x = this.ScaleWidth(this.bezier2.x, width);
				this.bezier2.y = this.ScaleHeight(this.bezier2.y, height);
			}
			return this;
		}

		// Token: 0x0600244C RID: 9292 RVA: 0x000C0AA4 File Offset: 0x000BECA4
		public CurvedPolyPoint Scale(float xScale, float yScale, float zScale = 1f)
		{
			if (this.bezier)
			{
				return new CurvedPolyPoint(xScale * this.anchor.x, -yScale * this.anchor.y, xScale * this.bezier1.x, -yScale * this.bezier1.y, xScale * this.bezier2.x, -yScale * this.bezier2.y);
			}
			return new CurvedPolyPoint(xScale * this.x, -yScale * this.y);
		}

		// Token: 0x0600244D RID: 9293 RVA: 0x000C0B26 File Offset: 0x000BED26
		private float ScaleWidth(float x, float width)
		{
			return 6.2831855f * (x / width - 0.5f);
		}

		// Token: 0x0600244E RID: 9294 RVA: 0x000C0B37 File Offset: 0x000BED37
		private float ScaleHeight(float y, float height)
		{
			return 3.1415927f * (y / height - 0.5f);
		}

		// Token: 0x0600244F RID: 9295 RVA: 0x000C0B48 File Offset: 0x000BED48
		public static explicit operator Vector2(CurvedPolyPoint p)
		{
			return new Vector2(p.anchor.x, p.anchor.x);
		}

		// Token: 0x06002450 RID: 9296 RVA: 0x000C0B65 File Offset: 0x000BED65
		public static explicit operator PolygonPoint(CurvedPolyPoint p)
		{
			return new PolygonPoint((double)p.anchor.x, (double)p.anchor.x);
		}

		// Token: 0x06002451 RID: 9297 RVA: 0x000C0B84 File Offset: 0x000BED84
		public static bool operator ==(CurvedPolyPoint lhs, CurvedPolyPoint rhs)
		{
			return lhs.Equals(rhs);
		}

		// Token: 0x06002452 RID: 9298 RVA: 0x000C0B99 File Offset: 0x000BED99
		public static bool operator !=(CurvedPolyPoint lhs, CurvedPolyPoint rhs)
		{
			return !lhs.Equals(rhs);
		}

		// Token: 0x06002453 RID: 9299 RVA: 0x000C0BB4 File Offset: 0x000BEDB4
		public override bool Equals(object other)
		{
			if (other is CurvedPolyPoint)
			{
				CurvedPolyPoint curvedPolyPoint = (CurvedPolyPoint)other;
				return this.bezier.Equals(curvedPolyPoint.bezier) && this.anchor.Equals(curvedPolyPoint.anchor) && (!this.bezier || (this.bezier1.Equals(curvedPolyPoint.bezier1) && this.bezier2.Equals(curvedPolyPoint.bezier2)));
			}
			return false;
		}

		// Token: 0x06002454 RID: 9300 RVA: 0x000C0C30 File Offset: 0x000BEE30
		public override int GetHashCode()
		{
			int num = this.anchor.GetHashCode();
			if (this.bezier)
			{
				num += (this.bezier1.GetHashCode() << 2) ^ (this.bezier1.GetHashCode() >> 2);
			}
			return num;
		}

		// Token: 0x04001B55 RID: 6997
		[SerializeField]
		public bool bezier;

		// Token: 0x04001B56 RID: 6998
		[SerializeField]
		public Vector2 anchor;

		// Token: 0x04001B57 RID: 6999
		[SerializeField]
		public Vector2 bezier1;

		// Token: 0x04001B58 RID: 7000
		[SerializeField]
		public Vector2 bezier2;
	}
}

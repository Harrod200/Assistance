using System;
using System.Collections;
using System.Collections.Generic;

namespace Poly2Tri
{
	// Token: 0x020004E9 RID: 1257
	public class Point2DEnumerator : IEnumerator<Point2D>, IEnumerator, IDisposable
	{
		// Token: 0x06001DB0 RID: 7600 RVA: 0x0009C05B File Offset: 0x0009A25B
		public Point2DEnumerator(IList<Point2D> points)
		{
			this.mPoints = points;
		}

		// Token: 0x06001DB1 RID: 7601 RVA: 0x0009C071 File Offset: 0x0009A271
		public bool MoveNext()
		{
			this.position++;
			return this.position < this.mPoints.Count;
		}

		// Token: 0x06001DB2 RID: 7602 RVA: 0x0009C094 File Offset: 0x0009A294
		public void Reset()
		{
			this.position = -1;
		}

		// Token: 0x06001DB3 RID: 7603 RVA: 0x0009C09D File Offset: 0x0009A29D
		void IDisposable.Dispose()
		{
		}

		// Token: 0x17000433 RID: 1075
		// (get) Token: 0x06001DB4 RID: 7604 RVA: 0x0009C09F File Offset: 0x0009A29F
		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		// Token: 0x17000434 RID: 1076
		// (get) Token: 0x06001DB5 RID: 7605 RVA: 0x0009C0A7 File Offset: 0x0009A2A7
		public Point2D Current
		{
			get
			{
				if (this.position < 0 || this.position >= this.mPoints.Count)
				{
					return null;
				}
				return this.mPoints[this.position];
			}
		}

		// Token: 0x040017CF RID: 6095
		protected IList<Point2D> mPoints;

		// Token: 0x040017D0 RID: 6096
		protected int position = -1;
	}
}

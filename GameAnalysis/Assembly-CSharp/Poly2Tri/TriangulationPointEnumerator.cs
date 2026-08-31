using System;
using System.Collections;
using System.Collections.Generic;

namespace Poly2Tri
{
	// Token: 0x020004E0 RID: 1248
	public class TriangulationPointEnumerator : IEnumerator<TriangulationPoint>, IEnumerator, IDisposable
	{
		// Token: 0x06001D35 RID: 7477 RVA: 0x0009AB7C File Offset: 0x00098D7C
		public TriangulationPointEnumerator(IList<Point2D> points)
		{
			this.mPoints = points;
		}

		// Token: 0x06001D36 RID: 7478 RVA: 0x0009AB92 File Offset: 0x00098D92
		public bool MoveNext()
		{
			this.position++;
			return this.position < this.mPoints.Count;
		}

		// Token: 0x06001D37 RID: 7479 RVA: 0x0009ABB5 File Offset: 0x00098DB5
		public void Reset()
		{
			this.position = -1;
		}

		// Token: 0x06001D38 RID: 7480 RVA: 0x0009ABBE File Offset: 0x00098DBE
		void IDisposable.Dispose()
		{
		}

		// Token: 0x1700042A RID: 1066
		// (get) Token: 0x06001D39 RID: 7481 RVA: 0x0009ABC0 File Offset: 0x00098DC0
		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		// Token: 0x1700042B RID: 1067
		// (get) Token: 0x06001D3A RID: 7482 RVA: 0x0009ABC8 File Offset: 0x00098DC8
		public TriangulationPoint Current
		{
			get
			{
				if (this.position < 0 || this.position >= this.mPoints.Count)
				{
					return null;
				}
				return this.mPoints[this.position] as TriangulationPoint;
			}
		}

		// Token: 0x040017C0 RID: 6080
		protected IList<Point2D> mPoints;

		// Token: 0x040017C1 RID: 6081
		protected int position = -1;
	}
}

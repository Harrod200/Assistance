using System;

namespace Poly2Tri
{
	// Token: 0x020004DD RID: 1245
	public abstract class TriangulationDebugContext
	{
		// Token: 0x06001D1D RID: 7453 RVA: 0x0009A8BE File Offset: 0x00098ABE
		public TriangulationDebugContext(TriangulationContext tcx)
		{
			this._tcx = tcx;
		}

		// Token: 0x06001D1E RID: 7454
		public abstract void Clear();

		// Token: 0x040017B8 RID: 6072
		protected TriangulationContext _tcx;
	}
}

using System;
using System.Collections.Generic;

namespace Poly2Tri
{
	// Token: 0x020004DC RID: 1244
	public abstract class TriangulationContext
	{
		// Token: 0x1700041F RID: 1055
		// (get) Token: 0x06001D0D RID: 7437 RVA: 0x0009A7E6 File Offset: 0x000989E6
		// (set) Token: 0x06001D0E RID: 7438 RVA: 0x0009A7EE File Offset: 0x000989EE
		public TriangulationDebugContext DebugContext { get; protected set; }

		// Token: 0x17000420 RID: 1056
		// (get) Token: 0x06001D0F RID: 7439 RVA: 0x0009A7F7 File Offset: 0x000989F7
		// (set) Token: 0x06001D10 RID: 7440 RVA: 0x0009A7FF File Offset: 0x000989FF
		public TriangulationMode TriangulationMode { get; protected set; }

		// Token: 0x17000421 RID: 1057
		// (get) Token: 0x06001D11 RID: 7441 RVA: 0x0009A808 File Offset: 0x00098A08
		// (set) Token: 0x06001D12 RID: 7442 RVA: 0x0009A810 File Offset: 0x00098A10
		public ITriangulatable Triangulatable { get; private set; }

		// Token: 0x17000422 RID: 1058
		// (get) Token: 0x06001D13 RID: 7443 RVA: 0x0009A819 File Offset: 0x00098A19
		// (set) Token: 0x06001D14 RID: 7444 RVA: 0x0009A821 File Offset: 0x00098A21
		public int StepCount { get; private set; }

		// Token: 0x06001D15 RID: 7445 RVA: 0x0009A82C File Offset: 0x00098A2C
		public void Done()
		{
			int stepCount = this.StepCount;
			this.StepCount = stepCount + 1;
		}

		// Token: 0x17000423 RID: 1059
		// (get) Token: 0x06001D16 RID: 7446
		public abstract TriangulationAlgorithm Algorithm { get; }

		// Token: 0x06001D17 RID: 7447 RVA: 0x0009A849 File Offset: 0x00098A49
		public virtual void PrepareTriangulation(ITriangulatable t)
		{
			this.Triangulatable = t;
			this.TriangulationMode = t.TriangulationMode;
			t.Prepare(this);
		}

		// Token: 0x06001D18 RID: 7448
		public abstract TriangulationConstraint NewConstraint(TriangulationPoint a, TriangulationPoint b);

		// Token: 0x06001D19 RID: 7449 RVA: 0x0009A865 File Offset: 0x00098A65
		public void Update(string message)
		{
		}

		// Token: 0x06001D1A RID: 7450 RVA: 0x0009A867 File Offset: 0x00098A67
		public virtual void Clear()
		{
			this.Points.Clear();
			if (this.DebugContext != null)
			{
				this.DebugContext.Clear();
			}
			this.StepCount = 0;
		}

		// Token: 0x17000424 RID: 1060
		// (get) Token: 0x06001D1B RID: 7451 RVA: 0x0009A88E File Offset: 0x00098A8E
		public DTSweepDebugContext DTDebugContext
		{
			get
			{
				return this.DebugContext as DTSweepDebugContext;
			}
		}

		// Token: 0x040017B3 RID: 6067
		public readonly List<DelaunayTriangle> Triangles = new List<DelaunayTriangle>();

		// Token: 0x040017B4 RID: 6068
		public readonly List<TriangulationPoint> Points = new List<TriangulationPoint>(200);
	}
}

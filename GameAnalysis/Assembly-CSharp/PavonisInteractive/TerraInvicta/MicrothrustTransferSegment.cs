using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000798 RID: 1944
	public class MicrothrustTransferSegment : IPatchedTransferSegment
	{
		// Token: 0x17000AFA RID: 2810
		// (get) Token: 0x06003E65 RID: 15973 RVA: 0x00194FCE File Offset: 0x001931CE
		// (set) Token: 0x06003E66 RID: 15974 RVA: 0x00194FD6 File Offset: 0x001931D6
		public TIDateTime startTime { get; set; }

		// Token: 0x17000AFB RID: 2811
		// (get) Token: 0x06003E67 RID: 15975 RVA: 0x00194FDF File Offset: 0x001931DF
		// (set) Token: 0x06003E68 RID: 15976 RVA: 0x00194FE7 File Offset: 0x001931E7
		public TIDateTime endTime { get; set; }

		// Token: 0x17000AFC RID: 2812
		// (get) Token: 0x06003E69 RID: 15977 RVA: 0x00194FF0 File Offset: 0x001931F0
		// (set) Token: 0x06003E6A RID: 15978 RVA: 0x00194FF8 File Offset: 0x001931F8
		public TINaturalSpaceObjectState barycenter { get; set; }

		// Token: 0x17000AFD RID: 2813
		// (get) Token: 0x06003E6B RID: 15979 RVA: 0x00195001 File Offset: 0x00193201
		public double endAnomaly
		{
			get
			{
				return this.startAnomaly_Rad + this.anomalyDelta_Rad;
			}
		}

		// Token: 0x17000AFE RID: 2814
		// (get) Token: 0x06003E6C RID: 15980 RVA: 0x00195010 File Offset: 0x00193210
		public double DV_mps
		{
			get
			{
				return Mathd.Abs(Mathd.Sqrt(this.barycenter.mu / this.startRadius_m) - Mathd.Sqrt(this.barycenter.mu / this.endRadius_m));
			}
		}

		// Token: 0x040026D8 RID: 9944
		public double startAnomaly_Rad;

		// Token: 0x040026D9 RID: 9945
		public double anomalyDelta_Rad;

		// Token: 0x040026DA RID: 9946
		public double startRadius_m;

		// Token: 0x040026DB RID: 9947
		public double endRadius_m;

		// Token: 0x040026DC RID: 9948
		public double eccentricity;

		// Token: 0x040026DD RID: 9949
		public double ascendingNode_rad;

		// Token: 0x040026DE RID: 9950
		public double inclination_rad;

		// Token: 0x040026DF RID: 9951
		public double argP_rad;
	}
}

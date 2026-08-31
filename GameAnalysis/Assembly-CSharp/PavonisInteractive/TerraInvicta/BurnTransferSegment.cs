using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200079C RID: 1948
	public class BurnTransferSegment : IPatchedTransferSegment
	{
		// Token: 0x17000B09 RID: 2825
		// (get) Token: 0x06003E82 RID: 16002 RVA: 0x00195312 File Offset: 0x00193512
		public TIDateTime startTime
		{
			get
			{
				return new TIDateTime(this.midpointTime, -this.duration_s * 0.5);
			}
		}

		// Token: 0x17000B0A RID: 2826
		// (get) Token: 0x06003E83 RID: 16003 RVA: 0x00195330 File Offset: 0x00193530
		public TIDateTime endTime
		{
			get
			{
				return new TIDateTime(this.midpointTime, this.duration_s * 0.5);
			}
		}

		// Token: 0x17000B0B RID: 2827
		// (get) Token: 0x06003E84 RID: 16004 RVA: 0x0019534D File Offset: 0x0019354D
		// (set) Token: 0x06003E85 RID: 16005 RVA: 0x00195355 File Offset: 0x00193555
		public TIDateTime midpointTime { get; set; }

		// Token: 0x17000B0C RID: 2828
		// (get) Token: 0x06003E86 RID: 16006 RVA: 0x0019535E File Offset: 0x0019355E
		// (set) Token: 0x06003E87 RID: 16007 RVA: 0x00195366 File Offset: 0x00193566
		public TINaturalSpaceObjectState barycenter { get; set; }

		// Token: 0x17000B0D RID: 2829
		// (get) Token: 0x06003E88 RID: 16008 RVA: 0x0019536F File Offset: 0x0019356F
		// (set) Token: 0x06003E89 RID: 16009 RVA: 0x00195377 File Offset: 0x00193577
		public double DV_mps { get; set; }

		// Token: 0x17000B0E RID: 2830
		// (get) Token: 0x06003E8A RID: 16010 RVA: 0x00195380 File Offset: 0x00193580
		public double duration_s
		{
			get
			{
				return this.DV_mps / this.fleetAcceleration_mps2;
			}
		}

		// Token: 0x040026F4 RID: 9972
		public double fleetAcceleration_mps2;
	}
}

using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200079B RID: 1947
	public class ImpulseTransferSegment : IPatchedTransferSegment
	{
		// Token: 0x17000B05 RID: 2821
		// (get) Token: 0x06003E7C RID: 15996 RVA: 0x001952C6 File Offset: 0x001934C6
		public TIDateTime startTime
		{
			get
			{
				return this.lambert.launchTime;
			}
		}

		// Token: 0x17000B06 RID: 2822
		// (get) Token: 0x06003E7D RID: 15997 RVA: 0x001952D3 File Offset: 0x001934D3
		public TIDateTime endTime
		{
			get
			{
				return this.lambert.arrivalTime;
			}
		}

		// Token: 0x17000B07 RID: 2823
		// (get) Token: 0x06003E7E RID: 15998 RVA: 0x001952E0 File Offset: 0x001934E0
		// (set) Token: 0x06003E7F RID: 15999 RVA: 0x001952E8 File Offset: 0x001934E8
		public TINaturalSpaceObjectState barycenter { get; set; }

		// Token: 0x17000B08 RID: 2824
		// (get) Token: 0x06003E80 RID: 16000 RVA: 0x001952F1 File Offset: 0x001934F1
		public double DV_mps
		{
			get
			{
				return this.lambert.boost_DV_mps + this.lambert.decel_DV_mps;
			}
		}

		// Token: 0x040026F1 RID: 9969
		public TwoBurnLambertTransfer lambert;
	}
}

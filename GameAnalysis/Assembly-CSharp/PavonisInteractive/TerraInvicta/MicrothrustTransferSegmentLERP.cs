using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200079A RID: 1946
	public class MicrothrustTransferSegmentLERP : IPatchedTransferSegment
	{
		// Token: 0x17000B00 RID: 2816
		// (get) Token: 0x06003E72 RID: 15986 RVA: 0x00195181 File Offset: 0x00193381
		// (set) Token: 0x06003E73 RID: 15987 RVA: 0x00195189 File Offset: 0x00193389
		public TIDateTime startTime { get; set; }

		// Token: 0x17000B01 RID: 2817
		// (get) Token: 0x06003E74 RID: 15988 RVA: 0x00195192 File Offset: 0x00193392
		// (set) Token: 0x06003E75 RID: 15989 RVA: 0x0019519A File Offset: 0x0019339A
		public TIDateTime endTime { get; set; }

		// Token: 0x17000B02 RID: 2818
		// (get) Token: 0x06003E76 RID: 15990 RVA: 0x001951A3 File Offset: 0x001933A3
		// (set) Token: 0x06003E77 RID: 15991 RVA: 0x001951AB File Offset: 0x001933AB
		public TINaturalSpaceObjectState barycenter { get; set; }

		// Token: 0x17000B03 RID: 2819
		// (get) Token: 0x06003E78 RID: 15992 RVA: 0x001951B4 File Offset: 0x001933B4
		public double DV_mps
		{
			get
			{
				if (this.effectiveFleetAcceleration_mps2 == 0.0)
				{
					return Mathd.Abs(Mathd.Sqrt(this.barycenter.mu / this.start.radius_m) - Mathd.Sqrt(this.barycenter.mu / this.end.radius_m));
				}
				return this.endTime.DifferenceInSeconds(this.startTime) * this.trueFleetAcceleration_mps2;
			}
		}

		// Token: 0x17000B04 RID: 2820
		// (get) Token: 0x06003E79 RID: 15993 RVA: 0x00195229 File Offset: 0x00193429
		public double anomalyDelta_Rad
		{
			get
			{
				return this.end.meanAnomaly_Rad - this.start.meanAnomaly_Rad;
			}
		}

		// Token: 0x06003E7A RID: 15994 RVA: 0x00195244 File Offset: 0x00193444
		public MicrothrustTransferSegmentLERP Copy()
		{
			return new MicrothrustTransferSegmentLERP
			{
				startTime = new TIDateTime(this.startTime),
				endTime = new TIDateTime(this.endTime),
				barycenter = this.barycenter,
				start = this.start.Clone(),
				end = this.end.Clone(),
				effectiveFleetAcceleration_mps2 = this.effectiveFleetAcceleration_mps2,
				trueFleetAcceleration_mps2 = this.trueFleetAcceleration_mps2
			};
		}

		// Token: 0x040026EC RID: 9964
		public MicrothrustTransferLERPvalues start;

		// Token: 0x040026ED RID: 9965
		public MicrothrustTransferLERPvalues end;

		// Token: 0x040026EE RID: 9966
		public double effectiveFleetAcceleration_mps2;

		// Token: 0x040026EF RID: 9967
		public double trueFleetAcceleration_mps2;
	}
}

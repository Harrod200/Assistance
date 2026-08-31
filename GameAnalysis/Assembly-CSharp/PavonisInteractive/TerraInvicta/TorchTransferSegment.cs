using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200079E RID: 1950
	public class TorchTransferSegment : IPatchedTransferSegment
	{
		// Token: 0x17000B1C RID: 2844
		// (get) Token: 0x06003E9B RID: 16027 RVA: 0x001954B4 File Offset: 0x001936B4
		public TIDateTime startTime
		{
			get
			{
				return this.torch.launchTime;
			}
		}

		// Token: 0x17000B1D RID: 2845
		// (get) Token: 0x06003E9C RID: 16028 RVA: 0x001954C1 File Offset: 0x001936C1
		public TIDateTime endTime
		{
			get
			{
				return this.torch.arrivalTime;
			}
		}

		// Token: 0x17000B1E RID: 2846
		// (get) Token: 0x06003E9D RID: 16029 RVA: 0x001954CE File Offset: 0x001936CE
		// (set) Token: 0x06003E9E RID: 16030 RVA: 0x001954D6 File Offset: 0x001936D6
		public TINaturalSpaceObjectState barycenter { get; set; }

		// Token: 0x17000B1F RID: 2847
		// (get) Token: 0x06003E9F RID: 16031 RVA: 0x001954DF File Offset: 0x001936DF
		public double DV_mps
		{
			get
			{
				return this.torch.DV_mps + (this.initialGravwellDuration_s + this.finalGravwellDuration_s) * this.fleetAcceleration_mps;
			}
		}

		// Token: 0x04002701 RID: 9985
		public TorchTransfer torch;

		// Token: 0x04002702 RID: 9986
		public double initialGravwellDuration_s;

		// Token: 0x04002703 RID: 9987
		public double finalGravwellDuration_s;

		// Token: 0x04002704 RID: 9988
		public double fleetAcceleration_mps;

		// Token: 0x04002705 RID: 9989
		public Vector3d initialGlobalVelocity_mps;

		// Token: 0x04002706 RID: 9990
		public Vector3d finalGlobalVelocity_mps;
	}
}

using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000797 RID: 1943
	public interface IPatchedTransferSegment
	{
		// Token: 0x17000AF6 RID: 2806
		// (get) Token: 0x06003E61 RID: 15969
		TIDateTime startTime { get; }

		// Token: 0x17000AF7 RID: 2807
		// (get) Token: 0x06003E62 RID: 15970
		TIDateTime endTime { get; }

		// Token: 0x17000AF8 RID: 2808
		// (get) Token: 0x06003E63 RID: 15971
		TINaturalSpaceObjectState barycenter { get; }

		// Token: 0x17000AF9 RID: 2809
		// (get) Token: 0x06003E64 RID: 15972
		double DV_mps { get; }
	}
}

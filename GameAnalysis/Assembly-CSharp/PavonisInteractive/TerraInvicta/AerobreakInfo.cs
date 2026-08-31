using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200079F RID: 1951
	public class AerobreakInfo
	{
		// Token: 0x17000B20 RID: 2848
		// (get) Token: 0x06003EA1 RID: 16033 RVA: 0x00195509 File Offset: 0x00193709
		// (set) Token: 0x06003EA2 RID: 16034 RVA: 0x00195511 File Offset: 0x00193711
		public TINaturalSpaceObjectState barycenter { get; set; }

		// Token: 0x17000B21 RID: 2849
		// (get) Token: 0x06003EA3 RID: 16035 RVA: 0x0019551A File Offset: 0x0019371A
		// (set) Token: 0x06003EA4 RID: 16036 RVA: 0x00195522 File Offset: 0x00193722
		public OrbitalElementsState hohmannOrbit { get; set; }

		// Token: 0x17000B22 RID: 2850
		// (get) Token: 0x06003EA5 RID: 16037 RVA: 0x0019552B File Offset: 0x0019372B
		// (set) Token: 0x06003EA6 RID: 16038 RVA: 0x00195533 File Offset: 0x00193733
		public MicrothrustTransferSegmentLERP microthrustSpiral { get; set; }

		// Token: 0x06003EA7 RID: 16039 RVA: 0x0019553C File Offset: 0x0019373C
		public AerobreakInfo Copy()
		{
			AerobreakInfo aerobreakInfo = new AerobreakInfo();
			aerobreakInfo.barycenter = this.barycenter;
			aerobreakInfo.hohmannOrbit = new OrbitalElementsState(this.hohmannOrbit);
			aerobreakInfo.aerobreakTime = new TIDateTime(this.aerobreakTime);
			aerobreakInfo.arrivalTime = new TIDateTime(this.arrivalTime);
			MicrothrustTransferSegmentLERP microthrustSpiral = this.microthrustSpiral;
			aerobreakInfo.microthrustSpiral = ((microthrustSpiral != null) ? microthrustSpiral.Copy() : null);
			return aerobreakInfo;
		}

		// Token: 0x04002709 RID: 9993
		public TIDateTime aerobreakTime;

		// Token: 0x0400270A RID: 9994
		public TIDateTime arrivalTime;
	}
}

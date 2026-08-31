using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007C6 RID: 1990
	public abstract class TrajectorySolver
	{
		// Token: 0x17000DE9 RID: 3561
		// (get) Token: 0x060046E5 RID: 18149 RVA: 0x001CFD1D File Offset: 0x001CDF1D
		// (set) Token: 0x060046E6 RID: 18150 RVA: 0x001CFD25 File Offset: 0x001CDF25
		public double boost_DV_mps { get; protected set; }

		// Token: 0x17000DEA RID: 3562
		// (get) Token: 0x060046E7 RID: 18151 RVA: 0x001CFD2E File Offset: 0x001CDF2E
		// (set) Token: 0x060046E8 RID: 18152 RVA: 0x001CFD36 File Offset: 0x001CDF36
		public double decel_DV_mps { get; protected set; }

		// Token: 0x17000DEB RID: 3563
		// (get) Token: 0x060046E9 RID: 18153 RVA: 0x001CFD3F File Offset: 0x001CDF3F
		// (set) Token: 0x060046EA RID: 18154 RVA: 0x001CFD47 File Offset: 0x001CDF47
		public double DV_mps { get; protected set; }

		// Token: 0x17000DEC RID: 3564
		// (get) Token: 0x060046EB RID: 18155 RVA: 0x001CFD50 File Offset: 0x001CDF50
		// (set) Token: 0x060046EC RID: 18156 RVA: 0x001CFD58 File Offset: 0x001CDF58
		public virtual TIDateTime launchTime { get; protected set; }

		// Token: 0x17000DED RID: 3565
		// (get) Token: 0x060046ED RID: 18157 RVA: 0x001CFD61 File Offset: 0x001CDF61
		// (set) Token: 0x060046EE RID: 18158 RVA: 0x001CFD69 File Offset: 0x001CDF69
		public virtual TIDateTime arrivalTime { get; protected set; }

		// Token: 0x17000DEE RID: 3566
		// (get) Token: 0x060046EF RID: 18159 RVA: 0x001CFD72 File Offset: 0x001CDF72
		// (set) Token: 0x060046F0 RID: 18160 RVA: 0x001CFD7A File Offset: 0x001CDF7A
		public virtual double transitDuration_s { get; protected set; }
	}
}

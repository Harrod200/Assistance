using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200077F RID: 1919
	public struct FinishedTechData
	{
		// Token: 0x06003BC9 RID: 15305 RVA: 0x00168F36 File Offset: 0x00167136
		public FinishedTechData(int slot, TIFactionState winningCouncil)
		{
			this.slot = slot;
			this.winningCouncil = winningCouncil;
		}

		// Token: 0x040025E4 RID: 9700
		public int slot;

		// Token: 0x040025E5 RID: 9701
		public TIFactionState winningCouncil;
	}
}

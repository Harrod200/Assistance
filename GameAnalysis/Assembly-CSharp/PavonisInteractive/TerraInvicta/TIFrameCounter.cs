using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000710 RID: 1808
	public static class TIFrameCounter
	{
		// Token: 0x170005EA RID: 1514
		// (get) Token: 0x06002B3A RID: 11066 RVA: 0x000EB13B File Offset: 0x000E933B
		public static int FrameCount
		{
			get
			{
				return TIMutableFrameCounter.FrameCount;
			}
		}
	}
}

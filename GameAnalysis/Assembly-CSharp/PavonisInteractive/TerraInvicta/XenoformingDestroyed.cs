using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000614 RID: 1556
	public class XenoformingDestroyed : GameEvent
	{
		// Token: 0x06002839 RID: 10297 RVA: 0x000DA009 File Offset: 0x000D8209
		public XenoformingDestroyed(TIRegionXenoformingState xenoforming)
		{
			this.xenoforming = xenoforming;
		}

		// Token: 0x04001E45 RID: 7749
		public TIRegionXenoformingState xenoforming;
	}
}

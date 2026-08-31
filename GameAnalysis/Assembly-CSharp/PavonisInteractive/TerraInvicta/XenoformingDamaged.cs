using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000613 RID: 1555
	public class XenoformingDamaged : GameEvent
	{
		// Token: 0x06002838 RID: 10296 RVA: 0x000D9FFA File Offset: 0x000D81FA
		public XenoformingDamaged(TIRegionXenoformingState xenoforming)
		{
			this.xenoforming = xenoforming;
		}

		// Token: 0x04001E44 RID: 7748
		public TIRegionXenoformingState xenoforming;
	}
}

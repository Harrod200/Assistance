using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005D0 RID: 1488
	public class MyActiveAssetPanelResized : GameEvent
	{
		// Token: 0x060027F5 RID: 10229 RVA: 0x000D9B6F File Offset: 0x000D7D6F
		public MyActiveAssetPanelResized(float height_px)
		{
			this.height_px = height_px;
		}

		// Token: 0x04001DF0 RID: 7664
		public float height_px;
	}
}

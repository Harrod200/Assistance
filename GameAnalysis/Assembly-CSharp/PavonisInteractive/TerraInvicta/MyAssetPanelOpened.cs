using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005CF RID: 1487
	public class MyAssetPanelOpened : GameEvent
	{
		// Token: 0x060027F4 RID: 10228 RVA: 0x000D9B59 File Offset: 0x000D7D59
		public MyAssetPanelOpened(AssetPanel assetPanel, float height_px)
		{
			this.assetPanel = assetPanel;
			this.height_px = height_px;
		}

		// Token: 0x04001DEE RID: 7662
		public AssetPanel assetPanel;

		// Token: 0x04001DEF RID: 7663
		public float height_px;
	}
}

using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005D1 RID: 1489
	public class MyAssetPanelClosed : GameEvent
	{
		// Token: 0x060027F6 RID: 10230 RVA: 0x000D9B7E File Offset: 0x000D7D7E
		public MyAssetPanelClosed(AssetPanel assetPanel)
		{
			this.assetPanel = assetPanel;
		}

		// Token: 0x04001DF1 RID: 7665
		public AssetPanel assetPanel;
	}
}

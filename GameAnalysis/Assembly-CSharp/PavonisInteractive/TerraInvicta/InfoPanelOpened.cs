using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005CB RID: 1483
	public class InfoPanelOpened : GameEvent
	{
		// Token: 0x060027F0 RID: 10224 RVA: 0x000D9B1D File Offset: 0x000D7D1D
		public InfoPanelOpened(InfoPanel infoPanel, float height_px)
		{
			this.infoPanel = infoPanel;
			this.height_px = height_px;
		}

		// Token: 0x04001DEA RID: 7658
		public InfoPanel infoPanel;

		// Token: 0x04001DEB RID: 7659
		public float height_px;
	}
}

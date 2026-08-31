using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005CC RID: 1484
	public class InfoPanelClosed : GameEvent
	{
		// Token: 0x060027F1 RID: 10225 RVA: 0x000D9B33 File Offset: 0x000D7D33
		public InfoPanelClosed(InfoPanel infoPanel)
		{
			this.infoPanel = infoPanel;
		}

		// Token: 0x04001DEC RID: 7660
		public InfoPanel infoPanel;
	}
}

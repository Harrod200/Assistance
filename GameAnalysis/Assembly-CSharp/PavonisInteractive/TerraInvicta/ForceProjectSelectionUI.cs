using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005DC RID: 1500
	public class ForceProjectSelectionUI : GameEvent
	{
		// Token: 0x06002801 RID: 10241 RVA: 0x000D9C23 File Offset: 0x000D7E23
		public ForceProjectSelectionUI(TIFactionState councilState)
		{
			this.councilState = councilState;
		}

		// Token: 0x04001DFC RID: 7676
		public TIFactionState councilState;
	}
}

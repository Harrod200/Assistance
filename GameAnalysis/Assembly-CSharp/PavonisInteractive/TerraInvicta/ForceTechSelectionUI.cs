using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005DD RID: 1501
	public class ForceTechSelectionUI : GameEvent
	{
		// Token: 0x06002802 RID: 10242 RVA: 0x000D9C32 File Offset: 0x000D7E32
		public ForceTechSelectionUI(TIFactionState councilState)
		{
			this.councilState = councilState;
		}

		// Token: 0x04001DFD RID: 7677
		public TIFactionState councilState;
	}
}

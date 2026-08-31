using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000674 RID: 1652
	public class CurrentOtherStateDeselected : GameEvent
	{
		// Token: 0x0600289C RID: 10396 RVA: 0x000DA7CE File Offset: 0x000D89CE
		public CurrentOtherStateDeselected(TIGameState oldSelectedState, TIGameState newSelectedState = null)
		{
			this.oldSelectedState = oldSelectedState;
			this.newSelectedState = newSelectedState;
		}

		// Token: 0x04001ED3 RID: 7891
		public TIGameState oldSelectedState;

		// Token: 0x04001ED4 RID: 7892
		public TIGameState newSelectedState;
	}
}

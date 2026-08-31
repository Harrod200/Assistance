using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000606 RID: 1542
	public class NationFlashEvent : GameEvent
	{
		// Token: 0x0600282B RID: 10283 RVA: 0x000D9F0B File Offset: 0x000D810B
		public NationFlashEvent(TINationState nation)
		{
			this.nation = nation;
		}

		// Token: 0x04001E36 RID: 7734
		public TINationState nation;
	}
}

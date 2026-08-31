using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005EA RID: 1514
	public class NationDataUpdated : GameEvent
	{
		// Token: 0x0600280F RID: 10255 RVA: 0x000D9D0B File Offset: 0x000D7F0B
		public NationDataUpdated(TINationState nation)
		{
			this.nation = nation;
		}

		// Token: 0x04001E0D RID: 7693
		public TINationState nation;
	}
}

using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005C2 RID: 1474
	public class SetOptionsMenuStatus : GameEvent
	{
		// Token: 0x060027E7 RID: 10215 RVA: 0x000D9A73 File Offset: 0x000D7C73
		public SetOptionsMenuStatus(bool enabled)
		{
			this.enabled = enabled;
		}

		// Token: 0x04001DDC RID: 7644
		public bool enabled;
	}
}

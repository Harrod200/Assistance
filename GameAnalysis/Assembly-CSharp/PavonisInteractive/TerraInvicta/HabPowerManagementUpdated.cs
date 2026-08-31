using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000650 RID: 1616
	public class HabPowerManagementUpdated : GameEvent
	{
		// Token: 0x06002877 RID: 10359 RVA: 0x000DA4E2 File Offset: 0x000D86E2
		public HabPowerManagementUpdated(TIHabState hab)
		{
			this.hab = hab;
		}

		// Token: 0x04001EAA RID: 7850
		public TIHabState hab;
	}
}

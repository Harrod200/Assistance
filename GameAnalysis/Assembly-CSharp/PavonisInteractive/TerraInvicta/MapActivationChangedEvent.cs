using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005BE RID: 1470
	public class MapActivationChangedEvent : GameEvent
	{
		// Token: 0x060027E3 RID: 10211 RVA: 0x000D9A30 File Offset: 0x000D7C30
		public MapActivationChangedEvent(bool active, MapController map)
		{
			this.active = active;
			this.map = map;
		}

		// Token: 0x04001DD7 RID: 7639
		public bool active;

		// Token: 0x04001DD8 RID: 7640
		public MapController map;
	}
}

using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006A7 RID: 1703
	public class AlarmAdded : GameEvent
	{
		// Token: 0x060028D3 RID: 10451 RVA: 0x000DAB3F File Offset: 0x000D8D3F
		public AlarmAdded(TIFactionState faction, TIDateTime dateTime)
		{
			this.faction = faction;
			this.dateTime = dateTime;
		}

		// Token: 0x04001F0F RID: 7951
		public TIFactionState faction;

		// Token: 0x04001F10 RID: 7952
		public TIDateTime dateTime;
	}
}

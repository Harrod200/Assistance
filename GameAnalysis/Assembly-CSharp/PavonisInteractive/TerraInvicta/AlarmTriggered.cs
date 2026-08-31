using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006A8 RID: 1704
	public class AlarmTriggered : GameEvent
	{
		// Token: 0x060028D4 RID: 10452 RVA: 0x000DAB55 File Offset: 0x000D8D55
		public AlarmTriggered(TIFactionState faction, TIGameState target)
		{
			this.triggeringFaction = faction;
			this.target = target;
		}

		// Token: 0x04001F11 RID: 7953
		public TIFactionState triggeringFaction;

		// Token: 0x04001F12 RID: 7954
		public TIGameState target;
	}
}

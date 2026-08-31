using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005F0 RID: 1520
	public class HabModuleUnlocked : GameEvent
	{
		// Token: 0x06002815 RID: 10261 RVA: 0x000D9D73 File Offset: 0x000D7F73
		public HabModuleUnlocked(TIFactionState faction)
		{
			this.faction = faction;
		}

		// Token: 0x04001E15 RID: 7701
		public TIFactionState faction;
	}
}

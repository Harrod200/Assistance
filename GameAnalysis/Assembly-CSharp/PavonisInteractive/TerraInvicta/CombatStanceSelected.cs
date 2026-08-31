using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006A9 RID: 1705
	public class CombatStanceSelected : GameEvent
	{
		// Token: 0x060028D5 RID: 10453 RVA: 0x000DAB6B File Offset: 0x000D8D6B
		public CombatStanceSelected(TIFactionState faction)
		{
			this.faction = faction;
		}

		// Token: 0x04001F13 RID: 7955
		public TIFactionState faction;
	}
}

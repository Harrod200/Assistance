using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006C1 RID: 1729
	public class EndCombatStanceChanged : GameEvent
	{
		// Token: 0x060028ED RID: 10477 RVA: 0x000DAD04 File Offset: 0x000D8F04
		public EndCombatStanceChanged(TIFactionState faction)
		{
			this.faction = faction;
		}

		// Token: 0x04001F32 RID: 7986
		public readonly TIFactionState faction;
	}
}

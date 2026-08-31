using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200061C RID: 1564
	public class ArmyAssignedToFaction : GameEvent
	{
		// Token: 0x06002841 RID: 10305 RVA: 0x000DA096 File Offset: 0x000D8296
		public ArmyAssignedToFaction(TIArmyState army, TIFactionState faction)
		{
			this.army = army;
			this.faction = faction;
		}

		// Token: 0x04001E50 RID: 7760
		public TIArmyState army;

		// Token: 0x04001E51 RID: 7761
		public TIFactionState faction;
	}
}

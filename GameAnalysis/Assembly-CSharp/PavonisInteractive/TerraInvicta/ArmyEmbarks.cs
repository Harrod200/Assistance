using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200061E RID: 1566
	public class ArmyEmbarks : GameEvent
	{
		// Token: 0x06002843 RID: 10307 RVA: 0x000DA0C2 File Offset: 0x000D82C2
		public ArmyEmbarks(TIArmyState army)
		{
			this.army = army;
		}

		// Token: 0x04001E54 RID: 7764
		public TIArmyState army;
	}
}

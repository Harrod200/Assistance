using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000623 RID: 1571
	public class ArmyMajorStatusUpdate : GameEvent
	{
		// Token: 0x06002848 RID: 10312 RVA: 0x000DA11B File Offset: 0x000D831B
		public ArmyMajorStatusUpdate(TIArmyState army)
		{
			this.army = army;
		}

		// Token: 0x04001E5B RID: 7771
		public TIArmyState army;
	}
}

using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000625 RID: 1573
	public class ArmyPathChanged : GameEvent
	{
		// Token: 0x0600284A RID: 10314 RVA: 0x000DA139 File Offset: 0x000D8339
		public ArmyPathChanged(TIArmyState army)
		{
			this.army = army;
		}

		// Token: 0x04001E5D RID: 7773
		public TIArmyState army;
	}
}

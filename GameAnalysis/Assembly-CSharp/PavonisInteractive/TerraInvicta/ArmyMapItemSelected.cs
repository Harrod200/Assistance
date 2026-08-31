using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000669 RID: 1641
	public class ArmyMapItemSelected : GameEvent
	{
		// Token: 0x06002890 RID: 10384 RVA: 0x000DA675 File Offset: 0x000D8875
		public ArmyMapItemSelected(TIArmyState army)
		{
			this.army = army;
		}

		// Token: 0x04001EC7 RID: 7879
		public TIArmyState army;
	}
}

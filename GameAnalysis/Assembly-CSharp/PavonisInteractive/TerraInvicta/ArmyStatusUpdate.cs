using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200061D RID: 1565
	public class ArmyStatusUpdate : GameEvent
	{
		// Token: 0x06002842 RID: 10306 RVA: 0x000DA0AC File Offset: 0x000D82AC
		public ArmyStatusUpdate(TIArmyState army, TIGameState eventState = null)
		{
			this.army = army;
			this.eventState = eventState;
		}

		// Token: 0x04001E52 RID: 7762
		public TIArmyState army;

		// Token: 0x04001E53 RID: 7763
		public TIGameState eventState;
	}
}

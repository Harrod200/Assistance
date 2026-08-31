using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A5E RID: 2654
	public class SetAIControlAction : PlayerAction
	{
		// Token: 0x06006509 RID: 25865 RVA: 0x002FAEBC File Offset: 0x002F90BC
		public SetAIControlAction(TISpaceShipState ship, bool setting)
		{
			this.shipID = ship.ID;
			this.setting = setting;
		}

		// Token: 0x0600650A RID: 25866 RVA: 0x002FAED7 File Offset: 0x002F90D7
		public override void Execute()
		{
			this.shipID.GetState<TISpaceShipState>(false).SetAIControl(this.setting);
		}

		// Token: 0x0400472E RID: 18222
		private GameStateID shipID;

		// Token: 0x0400472F RID: 18223
		private bool setting;
	}
}

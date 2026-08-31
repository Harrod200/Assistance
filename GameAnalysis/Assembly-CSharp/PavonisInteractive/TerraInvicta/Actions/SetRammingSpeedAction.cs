using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A64 RID: 2660
	public class SetRammingSpeedAction : PlayerAction
	{
		// Token: 0x06006515 RID: 25877 RVA: 0x002FB23F File Offset: 0x002F943F
		public SetRammingSpeedAction(TISpaceShipState ship, bool enabled)
		{
			this.shipID = ship.ID;
			this.Enabled = enabled;
		}

		// Token: 0x06006516 RID: 25878 RVA: 0x002FB25C File Offset: 0x002F945C
		public override void Execute()
		{
			this.shipID.GetState<TISpaceShipState>(false).SetRammingSpeed(this.Enabled);
		}

		// Token: 0x04004738 RID: 18232
		private readonly GameStateID shipID;

		// Token: 0x04004739 RID: 18233
		private readonly bool Enabled;
	}
}

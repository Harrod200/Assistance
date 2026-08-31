using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A58 RID: 2648
	public class ChangeShipBio : PlayerAction
	{
		// Token: 0x060064FD RID: 25853 RVA: 0x002FAD5B File Offset: 0x002F8F5B
		public ChangeShipBio(TISpaceShipState ship, string name)
		{
			this.shipID = ship.ID;
			this.name = name;
		}

		// Token: 0x060064FE RID: 25854 RVA: 0x002FAD76 File Offset: 0x002F8F76
		public override void Execute()
		{
			this.shipID.GetState<TISpaceShipState>(false).SetDisplayName(this.name);
		}

		// Token: 0x04004725 RID: 18213
		private GameStateID shipID;

		// Token: 0x04004726 RID: 18214
		private string name;
	}
}

using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A70 RID: 2672
	public class DeleteShipDesignAction : PlayerAction
	{
		// Token: 0x0600652E RID: 25902 RVA: 0x002FB7C2 File Offset: 0x002F99C2
		public DeleteShipDesignAction(TIFactionState faction, TISpaceShipTemplate shipDesign)
		{
			this.factionID = faction.ID;
			this.shipDesign = shipDesign;
		}

		// Token: 0x0600652F RID: 25903 RVA: 0x002FB7DD File Offset: 0x002F99DD
		public override void Execute()
		{
			this.factionID.GetState<TIFactionState>(false).DeleteShipDesign(this.shipDesign);
		}

		// Token: 0x04004758 RID: 18264
		public GameStateID factionID;

		// Token: 0x04004759 RID: 18265
		public TISpaceShipTemplate shipDesign;
	}
}

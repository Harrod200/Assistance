using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A84 RID: 2692
	public class SaveShipDesignAction : PlayerAction
	{
		// Token: 0x0600655C RID: 25948 RVA: 0x002FC674 File Offset: 0x002FA874
		public SaveShipDesignAction(TIFactionState faction, TISpaceShipTemplate shipDesign)
		{
			this.factionID = faction.ID;
			this.shipDesign = shipDesign;
		}

		// Token: 0x0600655D RID: 25949 RVA: 0x002FC68F File Offset: 0x002FA88F
		public override void Execute()
		{
			TIFactionState state = this.factionID.GetState<TIFactionState>(false);
			this.shipDesign.CacheTemplateValues(false);
			state.SaveShipDesign(this.shipDesign);
		}

		// Token: 0x0400478E RID: 18318
		public GameStateID factionID;

		// Token: 0x0400478F RID: 18319
		public TISpaceShipTemplate shipDesign;
	}
}

using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000AA1 RID: 2721
	public class ToggleObsoleteShipDesignAction : PlayerAction
	{
		// Token: 0x06006599 RID: 26009 RVA: 0x002FD673 File Offset: 0x002FB873
		public ToggleObsoleteShipDesignAction(TIFactionState faction, string shipDesignDataName)
		{
			this.factionID = faction.ID;
			this.shipDesignDataName = shipDesignDataName;
		}

		// Token: 0x0600659A RID: 26010 RVA: 0x002FD690 File Offset: 0x002FB890
		public override void Execute()
		{
			TIFactionState state = this.factionID.GetState<TIFactionState>(false);
			if (state.obsoleteShipDesigns.Contains(this.shipDesignDataName))
			{
				state.obsoleteShipDesigns.Remove(this.shipDesignDataName);
				return;
			}
			state.obsoleteShipDesigns.Add(this.shipDesignDataName);
		}

		// Token: 0x040047E9 RID: 18409
		public GameStateID factionID;

		// Token: 0x040047EA RID: 18410
		public string shipDesignDataName;
	}
}

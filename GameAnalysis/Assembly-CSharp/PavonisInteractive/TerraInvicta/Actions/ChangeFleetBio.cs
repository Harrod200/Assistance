using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A56 RID: 2646
	public class ChangeFleetBio : PlayerAction
	{
		// Token: 0x060064F9 RID: 25849 RVA: 0x002FACAB File Offset: 0x002F8EAB
		public ChangeFleetBio(TISpaceFleetState fleet, TIFactionState playerFaction, string name)
		{
			this.fleetID = fleet.ID;
			this.name = name;
			this.playerFaction = playerFaction;
		}

		// Token: 0x060064FA RID: 25850 RVA: 0x002FACCD File Offset: 0x002F8ECD
		public override void Execute()
		{
			this.fleetID.GetState<TISpaceFleetState>(false).SetDisplayName(this.playerFaction, this.name, true);
		}

		// Token: 0x0400471F RID: 18207
		private GameStateID fleetID;

		// Token: 0x04004720 RID: 18208
		private string name;

		// Token: 0x04004721 RID: 18209
		private TIFactionState playerFaction;
	}
}

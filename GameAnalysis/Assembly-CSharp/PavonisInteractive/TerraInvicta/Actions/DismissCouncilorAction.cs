using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A73 RID: 2675
	public class DismissCouncilorAction : PlayerAction
	{
		// Token: 0x06006535 RID: 25909 RVA: 0x002FBA04 File Offset: 0x002F9C04
		public DismissCouncilorAction(TICouncilorState councilor, TIFactionState faction, TIFactionState dismissingFaction)
		{
			this.councilorID = councilor.ID;
			this.factionID = faction.ID;
			this.dismissingFactionID = dismissingFaction.ID;
		}

		// Token: 0x06006536 RID: 25910 RVA: 0x002FBA30 File Offset: 0x002F9C30
		public override void Execute()
		{
			TIFactionState state = this.factionID.GetState<TIFactionState>(false);
			TIFactionState state2 = this.dismissingFactionID.GetState<TIFactionState>(false);
			TICouncilorState state3 = this.councilorID.GetState<TICouncilorState>(false);
			state.DismissCouncilor(state3, state2);
		}

		// Token: 0x04004762 RID: 18274
		private GameStateID councilorID;

		// Token: 0x04004763 RID: 18275
		private GameStateID factionID;

		// Token: 0x04004764 RID: 18276
		private GameStateID dismissingFactionID;
	}
}

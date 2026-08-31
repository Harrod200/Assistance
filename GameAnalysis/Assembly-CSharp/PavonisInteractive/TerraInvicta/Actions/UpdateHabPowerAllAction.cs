using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A76 RID: 2678
	public class UpdateHabPowerAllAction : PlayerAction
	{
		// Token: 0x0600653D RID: 25917 RVA: 0x002FC058 File Offset: 0x002FA258
		public UpdateHabPowerAllAction(TIHabState hab)
		{
			this.habID = hab.ID;
		}

		// Token: 0x0600653E RID: 25918 RVA: 0x002FC06C File Offset: 0x002FA26C
		public override void Execute()
		{
			TIHabState state = this.habID.GetState<TIHabState>(false);
			state.UpdatePowerManagement(true, null, state.coreFaction.player.isAI);
		}

		// Token: 0x04004769 RID: 18281
		private GameStateID habID;
	}
}

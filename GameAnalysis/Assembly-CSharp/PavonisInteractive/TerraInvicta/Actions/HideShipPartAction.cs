using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A78 RID: 2680
	public class HideShipPartAction : PlayerAction
	{
		// Token: 0x06006541 RID: 25921 RVA: 0x002FC0FD File Offset: 0x002FA2FD
		public HideShipPartAction(TIFactionState faction, TIShipPartTemplate part, bool setHidden)
		{
			this.factionID = faction.ID;
			this.part = part;
			this.setHidden = setHidden;
		}

		// Token: 0x06006542 RID: 25922 RVA: 0x002FC120 File Offset: 0x002FA320
		public override void Execute()
		{
			TIFactionState state = this.factionID.GetState<TIFactionState>(false);
			if (this.setHidden)
			{
				state.SetShipPartObsolete(this.part, true);
				return;
			}
			state.SetShipPartNotObsolete(this.part, true);
		}

		// Token: 0x0400476D RID: 18285
		public GameStateID factionID;

		// Token: 0x0400476E RID: 18286
		public TIShipPartTemplate part;

		// Token: 0x0400476F RID: 18287
		public bool setHidden;
	}
}

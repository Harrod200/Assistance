using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A77 RID: 2679
	public class HideProjectAction : PlayerAction
	{
		// Token: 0x0600653F RID: 25919 RVA: 0x002FC09E File Offset: 0x002FA29E
		public HideProjectAction(TIFactionState faction, string projectDataName, bool setHidden)
		{
			this.factionID = faction.ID;
			this.projectDataName = projectDataName;
			this.setHidden = setHidden;
		}

		// Token: 0x06006540 RID: 25920 RVA: 0x002FC0C0 File Offset: 0x002FA2C0
		public override void Execute()
		{
			TIFactionState state = this.factionID.GetState<TIFactionState>(false);
			if (this.setHidden)
			{
				state.SetProjectHidden(this.projectDataName);
				return;
			}
			state.SetProjectUnhidden(this.projectDataName);
		}

		// Token: 0x0400476A RID: 18282
		public GameStateID factionID;

		// Token: 0x0400476B RID: 18283
		public string projectDataName;

		// Token: 0x0400476C RID: 18284
		public bool setHidden;
	}
}

using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A74 RID: 2676
	public class FavorProjectAction : PlayerAction
	{
		// Token: 0x06006537 RID: 25911 RVA: 0x002FBA6A File Offset: 0x002F9C6A
		public FavorProjectAction(TIFactionState faction, string projectDataName, bool setFavored)
		{
			this.factionID = faction.ID;
			this.projectDataName = projectDataName;
			this.setFavored = setFavored;
		}

		// Token: 0x06006538 RID: 25912 RVA: 0x002FBA8C File Offset: 0x002F9C8C
		public override void Execute()
		{
			TIFactionState state = this.factionID.GetState<TIFactionState>(false);
			if (this.setFavored)
			{
				state.SetProjectFavored(this.projectDataName);
				return;
			}
			state.SetProjectUnfavored(this.projectDataName);
		}

		// Token: 0x04004765 RID: 18277
		public GameStateID factionID;

		// Token: 0x04004766 RID: 18278
		public string projectDataName;

		// Token: 0x04004767 RID: 18279
		public bool setFavored;
	}
}

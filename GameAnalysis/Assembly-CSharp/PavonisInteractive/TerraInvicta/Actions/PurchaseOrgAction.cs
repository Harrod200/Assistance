using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A79 RID: 2681
	public class PurchaseOrgAction : PlayerAction
	{
		// Token: 0x06006543 RID: 25923 RVA: 0x002FC15D File Offset: 0x002FA35D
		public PurchaseOrgAction(TIOrgState org, TIFactionState faction, TICouncilorState councilor)
		{
			if (councilor != null)
			{
				this.councilorID = councilor.ID;
			}
			else
			{
				this.straightToPool = true;
			}
			this.orgID = org.ID;
			this.factionID = faction.ID;
		}

		// Token: 0x06006544 RID: 25924 RVA: 0x002FC19C File Offset: 0x002FA39C
		public override void Execute()
		{
			TIOrgState state = this.orgID.GetState<TIOrgState>(false);
			this.factionID.GetState<TIFactionState>(false).PurchaseOrg(state.hasFactionbutNoCouncilor, state, this.straightToPool ? null : this.councilorID.GetState<TICouncilorState>(false), this.straightToPool);
		}

		// Token: 0x06006545 RID: 25925 RVA: 0x002FC1EB File Offset: 0x002FA3EB
		public TIOrgState GetOrg()
		{
			return this.orgID.GetState<TIOrgState>(false);
		}

		// Token: 0x06006546 RID: 25926 RVA: 0x002FC1F9 File Offset: 0x002FA3F9
		public bool HasAssignment()
		{
			return this.GetCouncilorAssignment() != null;
		}

		// Token: 0x06006547 RID: 25927 RVA: 0x002FC207 File Offset: 0x002FA407
		public TICouncilorState GetCouncilorAssignment()
		{
			return this.councilorID.GetState<TICouncilorState>(false);
		}

		// Token: 0x04004770 RID: 18288
		private GameStateID factionID;

		// Token: 0x04004771 RID: 18289
		private GameStateID councilorID;

		// Token: 0x04004772 RID: 18290
		private GameStateID orgID;

		// Token: 0x04004773 RID: 18291
		private bool straightToPool;
	}
}

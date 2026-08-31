using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200065A RID: 1626
	public class SpaceBodyProspected : GameEvent
	{
		// Token: 0x06002881 RID: 10369 RVA: 0x000DA57F File Offset: 0x000D877F
		public SpaceBodyProspected(TIFactionState faction, TISpaceBodyState spaceBody)
		{
			this.faction = faction;
			this.spaceBody = spaceBody;
		}

		// Token: 0x04001EB5 RID: 7861
		public TIFactionState faction;

		// Token: 0x04001EB6 RID: 7862
		public TISpaceBodyState spaceBody;
	}
}

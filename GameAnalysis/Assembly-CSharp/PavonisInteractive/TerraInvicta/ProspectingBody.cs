using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000659 RID: 1625
	public class ProspectingBody : GameEvent
	{
		// Token: 0x06002880 RID: 10368 RVA: 0x000DA569 File Offset: 0x000D8769
		public ProspectingBody(TIFactionState faction, TISpaceBodyState spaceBody)
		{
			this.faction = faction;
			this.spaceBody = spaceBody;
		}

		// Token: 0x04001EB3 RID: 7859
		public TIFactionState faction;

		// Token: 0x04001EB4 RID: 7860
		public TISpaceBodyState spaceBody;
	}
}

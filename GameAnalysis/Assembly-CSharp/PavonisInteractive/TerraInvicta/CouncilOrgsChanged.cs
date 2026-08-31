using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000600 RID: 1536
	public class CouncilOrgsChanged : GameEvent
	{
		// Token: 0x06002825 RID: 10277 RVA: 0x000D9E9C File Offset: 0x000D809C
		public CouncilOrgsChanged(TIFactionState faction)
		{
			this.faction = faction;
		}

		// Token: 0x04001E2D RID: 7725
		public TIFactionState faction;
	}
}

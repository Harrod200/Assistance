using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200065B RID: 1627
	public class FactionExplorationRangeChanged : GameEvent
	{
		// Token: 0x06002882 RID: 10370 RVA: 0x000DA595 File Offset: 0x000D8795
		public FactionExplorationRangeChanged(TIFactionState faction)
		{
			this.faction = faction;
		}

		// Token: 0x04001EB7 RID: 7863
		public TIFactionState faction;
	}
}

using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005EF RID: 1519
	public class ResearchUpdated : GameEvent
	{
		// Token: 0x06002814 RID: 10260 RVA: 0x000D9D64 File Offset: 0x000D7F64
		public ResearchUpdated(TIFactionState faction)
		{
			this.faction = faction;
		}

		// Token: 0x04001E14 RID: 7700
		public TIFactionState faction;
	}
}

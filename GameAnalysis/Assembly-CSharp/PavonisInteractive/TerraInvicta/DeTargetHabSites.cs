using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000681 RID: 1665
	public class DeTargetHabSites : GameEvent
	{
		// Token: 0x060028A9 RID: 10409 RVA: 0x000DA8C2 File Offset: 0x000D8AC2
		public DeTargetHabSites(TIFactionState faction)
		{
			this.faction = faction;
		}

		// Token: 0x04001EE7 RID: 7911
		public TIFactionState faction;
	}
}

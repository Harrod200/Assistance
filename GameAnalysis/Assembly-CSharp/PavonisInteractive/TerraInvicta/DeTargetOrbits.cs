using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200067B RID: 1659
	public class DeTargetOrbits : GameEvent
	{
		// Token: 0x060028A3 RID: 10403 RVA: 0x000DA868 File Offset: 0x000D8A68
		public DeTargetOrbits(TIFactionState faction)
		{
			this.faction = faction;
		}

		// Token: 0x04001EE1 RID: 7905
		public TIFactionState faction;
	}
}

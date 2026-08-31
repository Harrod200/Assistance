using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005F8 RID: 1528
	public class FactionFinalizesMissions : GameEvent
	{
		// Token: 0x0600281D RID: 10269 RVA: 0x000D9DEB File Offset: 0x000D7FEB
		public FactionFinalizesMissions(TIFactionState faction)
		{
			this.faction = faction;
		}

		// Token: 0x04001E1D RID: 7709
		public TIFactionState faction;
	}
}

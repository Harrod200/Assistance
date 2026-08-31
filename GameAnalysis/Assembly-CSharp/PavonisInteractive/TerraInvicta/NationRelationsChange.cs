using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005E2 RID: 1506
	public class NationRelationsChange : GameEvent
	{
		// Token: 0x06002807 RID: 10247 RVA: 0x000D9CA8 File Offset: 0x000D7EA8
		public NationRelationsChange(TINationState nation)
		{
			this.nation = nation;
		}

		// Token: 0x04001E08 RID: 7688
		public TINationState nation;
	}
}

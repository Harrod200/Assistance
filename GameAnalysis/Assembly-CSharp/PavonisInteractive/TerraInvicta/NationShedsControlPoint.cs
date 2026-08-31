using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000603 RID: 1539
	public class NationShedsControlPoint : GameEvent
	{
		// Token: 0x06002828 RID: 10280 RVA: 0x000D9ED0 File Offset: 0x000D80D0
		public NationShedsControlPoint(TINationState nation)
		{
			this.nation = nation;
		}

		// Token: 0x04001E31 RID: 7729
		public TINationState nation;
	}
}

using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000637 RID: 1591
	public class MoonSymbolVisibilityChange : GameEvent
	{
		// Token: 0x0600285C RID: 10332 RVA: 0x000DA29B File Offset: 0x000D849B
		public MoonSymbolVisibilityChange(TISpaceBodyState moon, bool active)
		{
			this.moon = moon;
			this.active = active;
		}

		// Token: 0x04001E7B RID: 7803
		public TISpaceBodyState moon;

		// Token: 0x04001E7C RID: 7804
		public bool active;
	}
}

using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005D9 RID: 1497
	public class NavigationUIRequested : GameEvent
	{
		// Token: 0x060027FE RID: 10238 RVA: 0x000D9BF6 File Offset: 0x000D7DF6
		public NavigationUIRequested(TISpaceBodyState spaceBody)
		{
			this.spaceBody = spaceBody;
		}

		// Token: 0x04001DF9 RID: 7673
		public TISpaceBodyState spaceBody;
	}
}

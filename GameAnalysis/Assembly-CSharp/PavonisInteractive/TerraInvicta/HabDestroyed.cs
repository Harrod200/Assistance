using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000653 RID: 1619
	public class HabDestroyed : GameEvent
	{
		// Token: 0x0600287A RID: 10362 RVA: 0x000DA516 File Offset: 0x000D8716
		public HabDestroyed(TIHabState hab, TISpaceFleetState byFleet)
		{
			this.hab = hab;
			this.byFleet = byFleet;
		}

		// Token: 0x04001EAE RID: 7854
		public TIHabState hab;

		// Token: 0x04001EAF RID: 7855
		public TISpaceFleetState byFleet;
	}
}

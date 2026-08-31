using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000651 RID: 1617
	public class HabDefendInterestsUpdated : GameEvent
	{
		// Token: 0x06002878 RID: 10360 RVA: 0x000DA4F1 File Offset: 0x000D86F1
		public HabDefendInterestsUpdated(TIHabState hab)
		{
			this.hab = hab;
		}

		// Token: 0x04001EAB RID: 7851
		public TIHabState hab;
	}
}

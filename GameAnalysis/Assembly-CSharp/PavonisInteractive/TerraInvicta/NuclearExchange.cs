using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000784 RID: 1924
	public struct NuclearExchange
	{
		// Token: 0x06003C51 RID: 15441 RVA: 0x0016E4E6 File Offset: 0x0016C6E6
		public NuclearExchange(TINationState attacker, TIRegionState target, TINationState enemyTargeted)
		{
			this.attacker = attacker;
			this.target = target;
			this.enemyTargeted = enemyTargeted;
		}

		// Token: 0x0400265A RID: 9818
		public TINationState attacker;

		// Token: 0x0400265B RID: 9819
		public TIRegionState target;

		// Token: 0x0400265C RID: 9820
		public TINationState enemyTargeted;
	}
}

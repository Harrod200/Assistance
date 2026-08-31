using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007B7 RID: 1975
	public struct PreservedFleetRecord
	{
		// Token: 0x04002835 RID: 10293
		public Dictionary<TIFactionState, string> namesByFaction;

		// Token: 0x04002836 RID: 10294
		public FactionGoal_Fleet goal;

		// Token: 0x04002837 RID: 10295
		public List<TISpaceShipState> ships;

		// Token: 0x04002838 RID: 10296
		public TIHabState homeport;
	}
}

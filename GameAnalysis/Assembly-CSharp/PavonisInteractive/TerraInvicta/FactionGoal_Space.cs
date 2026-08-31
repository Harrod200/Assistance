using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200073B RID: 1851
	public abstract class FactionGoal_Space : FactionGoal_Fleet
	{
		// Token: 0x170006FD RID: 1789
		// (get) Token: 0x06002EF3 RID: 12019
		public abstract List<Type> spaceOperations { get; }
	}
}

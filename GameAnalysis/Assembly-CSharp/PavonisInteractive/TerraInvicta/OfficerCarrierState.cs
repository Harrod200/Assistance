using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007BF RID: 1983
	public interface OfficerCarrierState
	{
		// Token: 0x06004514 RID: 17684
		TIGameState GetState();

		// Token: 0x06004515 RID: 17685
		List<TIOfficerState> GetOfficers();
	}
}

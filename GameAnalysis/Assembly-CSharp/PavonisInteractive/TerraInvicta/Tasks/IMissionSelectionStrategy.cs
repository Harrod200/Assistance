using System;
using PavonisInteractive.TerraInvicta.Entities;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x0200094A RID: 2378
	public interface IMissionSelectionStrategy
	{
		// Token: 0x06005AD5 RID: 23253
		TIMissionTemplate SelectMission(Councilor councilor);

		// Token: 0x06005AD6 RID: 23254
		TIGameState SelectTarget(Councilor councilor, TIMissionTemplate mission);

		// Token: 0x06005AD7 RID: 23255
		float SelectResources(Councilor councilor, TIMissionTemplate mission, TIGameState target);
	}
}

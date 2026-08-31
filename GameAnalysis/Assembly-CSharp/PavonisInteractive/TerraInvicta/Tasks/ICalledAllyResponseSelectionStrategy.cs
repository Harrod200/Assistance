using System;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x02000949 RID: 2377
	public interface ICalledAllyResponseSelectionStrategy
	{
		// Token: 0x06005AD4 RID: 23252
		bool SelectCalledAllyReply(TINationState respondingNation, TINationState enemyNation);
	}
}

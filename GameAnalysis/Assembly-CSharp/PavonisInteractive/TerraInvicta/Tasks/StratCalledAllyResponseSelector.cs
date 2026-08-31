using System;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x0200094F RID: 2383
	public class StratCalledAllyResponseSelector : ICalledAllyResponseSelectionStrategy
	{
		// Token: 0x06005ADF RID: 23263 RVA: 0x002BB060 File Offset: 0x002B9260
		public bool SelectCalledAllyReply(TINationState respondingNation, TINationState enemyNation)
		{
			return true;
		}
	}
}

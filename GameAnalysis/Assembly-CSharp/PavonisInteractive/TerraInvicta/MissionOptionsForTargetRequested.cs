using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200068B RID: 1675
	public class MissionOptionsForTargetRequested : GameEvent
	{
		// Token: 0x060028B3 RID: 10419 RVA: 0x000DA927 File Offset: 0x000D8B27
		public MissionOptionsForTargetRequested(TIGameState target)
		{
			this.target = target;
		}

		// Token: 0x04001EEA RID: 7914
		public TIGameState target;
	}
}

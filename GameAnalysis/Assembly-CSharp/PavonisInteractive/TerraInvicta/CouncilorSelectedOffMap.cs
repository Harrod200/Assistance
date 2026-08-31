using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200066F RID: 1647
	public class CouncilorSelectedOffMap : GameEvent
	{
		// Token: 0x06002897 RID: 10391 RVA: 0x000DA77C File Offset: 0x000D897C
		public CouncilorSelectedOffMap(TICouncilorState councilor)
		{
			this.councilor = councilor;
		}

		// Token: 0x04001ECD RID: 7885
		public TICouncilorState councilor;
	}
}

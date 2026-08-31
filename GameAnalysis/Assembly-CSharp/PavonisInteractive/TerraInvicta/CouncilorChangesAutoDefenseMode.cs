using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005F7 RID: 1527
	public class CouncilorChangesAutoDefenseMode : GameEvent
	{
		// Token: 0x0600281C RID: 10268 RVA: 0x000D9DD5 File Offset: 0x000D7FD5
		public CouncilorChangesAutoDefenseMode(TICouncilorState councilor, bool setting)
		{
			this.councilor = councilor;
			this.setting = setting;
		}

		// Token: 0x04001E1B RID: 7707
		public TICouncilorState councilor;

		// Token: 0x04001E1C RID: 7708
		public bool setting;
	}
}

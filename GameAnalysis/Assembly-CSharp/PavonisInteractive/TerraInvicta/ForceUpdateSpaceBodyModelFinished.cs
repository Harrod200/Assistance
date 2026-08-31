using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005C0 RID: 1472
	public class ForceUpdateSpaceBodyModelFinished : GameEvent
	{
		// Token: 0x060027E5 RID: 10213 RVA: 0x000D9A55 File Offset: 0x000D7C55
		public ForceUpdateSpaceBodyModelFinished(TISpaceBodyState spaceBody)
		{
			this.spaceBody = spaceBody;
		}

		// Token: 0x04001DDA RID: 7642
		public TISpaceBodyState spaceBody;
	}
}

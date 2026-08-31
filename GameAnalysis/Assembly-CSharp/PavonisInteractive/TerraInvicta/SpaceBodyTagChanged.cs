using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005C7 RID: 1479
	public class SpaceBodyTagChanged : GameEvent
	{
		// Token: 0x060027EC RID: 10220 RVA: 0x000D9AF6 File Offset: 0x000D7CF6
		public SpaceBodyTagChanged(TISpaceBodyState body)
		{
			this.body = body;
		}

		// Token: 0x04001DE9 RID: 7657
		public TISpaceBodyState body;
	}
}

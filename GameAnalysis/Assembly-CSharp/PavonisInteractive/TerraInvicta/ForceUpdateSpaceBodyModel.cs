using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005BF RID: 1471
	public class ForceUpdateSpaceBodyModel : GameEvent
	{
		// Token: 0x060027E4 RID: 10212 RVA: 0x000D9A46 File Offset: 0x000D7C46
		public ForceUpdateSpaceBodyModel(TISpaceBodyState spaceBody)
		{
			this.spaceBody = spaceBody;
		}

		// Token: 0x04001DD9 RID: 7641
		public TISpaceBodyState spaceBody;
	}
}

using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200061B RID: 1563
	public class MajorRegionStatusChange : GameEvent
	{
		// Token: 0x06002840 RID: 10304 RVA: 0x000DA087 File Offset: 0x000D8287
		public MajorRegionStatusChange(TIRegionState region)
		{
			this.region = region;
		}

		// Token: 0x04001E4F RID: 7759
		public TIRegionState region;
	}
}

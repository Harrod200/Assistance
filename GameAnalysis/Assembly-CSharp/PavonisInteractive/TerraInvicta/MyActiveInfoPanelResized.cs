using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005CD RID: 1485
	public class MyActiveInfoPanelResized : GameEvent
	{
		// Token: 0x060027F2 RID: 10226 RVA: 0x000D9B42 File Offset: 0x000D7D42
		public MyActiveInfoPanelResized(float height_px)
		{
			this.height_px = height_px;
		}

		// Token: 0x04001DED RID: 7661
		public float height_px;
	}
}

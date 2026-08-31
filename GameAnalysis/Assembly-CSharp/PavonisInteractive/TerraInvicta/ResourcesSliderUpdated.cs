using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200068F RID: 1679
	public class ResourcesSliderUpdated : GameEvent
	{
		// Token: 0x060028B7 RID: 10423 RVA: 0x000DA971 File Offset: 0x000D8B71
		public ResourcesSliderUpdated(float newSetting, TIGameState target)
		{
			this.newSetting = newSetting;
			this.target = target;
		}

		// Token: 0x04001EF0 RID: 7920
		public float newSetting;

		// Token: 0x04001EF1 RID: 7921
		public TIGameState target;
	}
}

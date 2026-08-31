using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000672 RID: 1650
	public class AlienRegionMapEntitySelected : GameEvent
	{
		// Token: 0x0600289A RID: 10394 RVA: 0x000DA7A9 File Offset: 0x000D89A9
		public AlienRegionMapEntitySelected(TIRegionAlienEntityState alienEntity)
		{
			this.alienEntity = alienEntity;
		}

		// Token: 0x04001ED0 RID: 7888
		public TIRegionAlienEntityState alienEntity;
	}
}

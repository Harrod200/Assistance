using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000671 RID: 1649
	public class AlienAssetTargetSelected : GameEvent
	{
		// Token: 0x06002899 RID: 10393 RVA: 0x000DA79A File Offset: 0x000D899A
		public AlienAssetTargetSelected(TIRegionAlienAssetState alienAsset)
		{
			this.alienAsset = alienAsset;
		}

		// Token: 0x04001ECF RID: 7887
		public TIRegionAlienAssetState alienAsset;
	}
}

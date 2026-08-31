using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000656 RID: 1622
	public class SpaceAssetDetected : GameEvent
	{
		// Token: 0x0600287D RID: 10365 RVA: 0x000DA543 File Offset: 0x000D8743
		public SpaceAssetDetected(TIFactionState faction, TISpaceAssetState asset)
		{
			this.faction = faction;
			this.asset = asset;
		}

		// Token: 0x04001EB1 RID: 7857
		public TIFactionState faction;

		// Token: 0x04001EB2 RID: 7858
		public TISpaceAssetState asset;
	}
}

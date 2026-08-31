using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000673 RID: 1651
	public class CurrentAssetDeSelected : GameEvent
	{
		// Token: 0x0600289B RID: 10395 RVA: 0x000DA7B8 File Offset: 0x000D89B8
		public CurrentAssetDeSelected(TIGameState oldAsset, TIGameState newAsset = null)
		{
			this.oldAsset = oldAsset;
			this.newAsset = newAsset;
		}

		// Token: 0x04001ED1 RID: 7889
		public TIGameState oldAsset;

		// Token: 0x04001ED2 RID: 7890
		public TIGameState newAsset;
	}
}

using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000704 RID: 1796
	public struct SpaceAssetName : INamelistKey<SpaceAssetName>, INamelistKey, IEquatable<SpaceAssetName>
	{
		// Token: 0x06002A84 RID: 10884 RVA: 0x000E6B4B File Offset: 0x000E4D4B
		public SpaceAssetName(string assetGroup, string suggestedRegion)
		{
			this.assetGroup = assetGroup;
			this.suggestedRegion = suggestedRegion;
		}

		// Token: 0x06002A85 RID: 10885 RVA: 0x000E6B5B File Offset: 0x000E4D5B
		public bool Equals(SpaceAssetName key)
		{
			return this.assetGroup == key.assetGroup;
		}

		// Token: 0x06002A86 RID: 10886 RVA: 0x000E6B6E File Offset: 0x000E4D6E
		public SpaceAssetName Any()
		{
			return new SpaceAssetName(this.assetGroup, string.Empty);
		}

		// Token: 0x040020A7 RID: 8359
		private readonly string assetGroup;

		// Token: 0x040020A8 RID: 8360
		private readonly string suggestedRegion;
	}
}

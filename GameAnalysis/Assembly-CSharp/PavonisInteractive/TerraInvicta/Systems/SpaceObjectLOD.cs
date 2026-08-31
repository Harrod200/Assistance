using System;
using Unity.Entities;

namespace PavonisInteractive.TerraInvicta.Systems
{
	// Token: 0x02000994 RID: 2452
	[Serializable]
	public struct SpaceObjectLOD : IComponentData
	{
		// Token: 0x04004252 RID: 16978
		public bool DisplayModel;

		// Token: 0x04004253 RID: 16979
		public bool DisplaySymbol;

		// Token: 0x04004254 RID: 16980
		public bool DisplaySymbolName;

		// Token: 0x04004255 RID: 16981
		public bool DisplaySurface;

		// Token: 0x04004256 RID: 16982
		public bool DisplayOrbitTrail;

		// Token: 0x04004257 RID: 16983
		public bool JustPoppedIn;
	}
}

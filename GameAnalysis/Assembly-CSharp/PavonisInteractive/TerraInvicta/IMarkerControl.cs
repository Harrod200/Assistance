using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000554 RID: 1364
	public interface IMarkerControl
	{
		// Token: 0x0600239D RID: 9117
		void InitializeWithRegion(RegionController region, MarkerContainerController container);

		// Token: 0x0600239E RID: 9118
		void UpdateMarker();
	}
}

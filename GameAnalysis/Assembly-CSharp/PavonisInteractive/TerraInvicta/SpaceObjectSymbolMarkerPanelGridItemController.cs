using System;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005A6 RID: 1446
	public class SpaceObjectSymbolMarkerPanelGridItemController : MonoBehaviour
	{
		// Token: 0x06002757 RID: 10071 RVA: 0x000D7626 File Offset: 0x000D5826
		public void UpdateFleetGridItem(TIFactionState faction)
		{
			this.typedFactionIcon.sprite = faction.fleetIcon;
		}

		// Token: 0x06002758 RID: 10072 RVA: 0x000D7639 File Offset: 0x000D5839
		public void UpdateBaseGridItem(TIFactionState faction)
		{
			this.typedFactionIcon.sprite = faction.baseIcon;
		}

		// Token: 0x06002759 RID: 10073 RVA: 0x000D764C File Offset: 0x000D584C
		public void UpdateStationGridItem(TIFactionState faction)
		{
			this.typedFactionIcon.sprite = faction.stationIcon;
		}

		// Token: 0x0600275A RID: 10074 RVA: 0x000D765F File Offset: 0x000D585F
		public void UpdateCouncilorGridItem(TIFactionState faction)
		{
			this.typedFactionIcon.sprite = faction.factionIcon128;
		}

		// Token: 0x04001D3E RID: 7486
		public Image typedFactionIcon;
	}
}

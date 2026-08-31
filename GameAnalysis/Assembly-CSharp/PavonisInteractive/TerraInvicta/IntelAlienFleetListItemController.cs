using System;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000877 RID: 2167
	public class IntelAlienFleetListItemController : MonoBehaviour
	{
		// Token: 0x0600510E RID: 20750 RVA: 0x00236FD0 File Offset: 0x002351D0
		public void UpdateListItem(TISpaceFleetState fleet)
		{
			this.fleet = fleet;
			this.fleetName.SetText(fleet.GetDisplayName(GameControl.control.activePlayer));
			this.location.SetText(fleet.GetLocationDescription(GameControl.control.activePlayer, true, false));
		}

		// Token: 0x0600510F RID: 20751 RVA: 0x0023701C File Offset: 0x0023521C
		public void OnClick()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			TIUtilities.GotoGameState(this.fleet, true, true, true, true, false, -1f);
		}

		// Token: 0x040034E1 RID: 13537
		public TMP_Text fleetName;

		// Token: 0x040034E2 RID: 13538
		private TISpaceFleetState fleet;

		// Token: 0x040034E3 RID: 13539
		public TMP_Text location;
	}
}

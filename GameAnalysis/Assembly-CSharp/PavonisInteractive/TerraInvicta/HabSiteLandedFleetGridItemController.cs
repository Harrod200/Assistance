using System;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.SolarSystem;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200059E RID: 1438
	internal class HabSiteLandedFleetGridItemController : MonoBehaviour
	{
		// Token: 0x060026AE RID: 9902 RVA: 0x000D27F0 File Offset: 0x000D09F0
		public void UpdateFleetGridItem(TISpaceFleetState fleet)
		{
			this.fleet = fleet;
			this.typedFactionIcon.sprite = fleet.icon;
			this.TT.SetDelegate("BodyText", () => fleet.GetDisplayName(GameControl.control.activePlayer));
		}

		// Token: 0x060026AF RID: 9903 RVA: 0x000D2848 File Offset: 0x000D0A48
		public void OnClickFleetIcon()
		{
			if (this.fleet != null && this.fleet.exists)
			{
				AudioManager.PlayOneShot((this.fleet.faction == GameControl.control.activePlayer) ? "event:/SFX/UI_SFX/trig_SFX_MyFleetSelect" : (this.fleet.faction.IsAlienFaction ? "event:/SFX/UI_SFX/trig_SFX_AlienFleetSelect" : "event:/SFX/UI_SFX/trig_SFX_OtherHumanFleetSelect"), false, false);
				TIUtilities.GotoGameState(this.fleet, false, true, true, false, false, -1f);
				SpaceObjectSelection.BlockSelectionFrame();
			}
		}

		// Token: 0x04001CC0 RID: 7360
		public Image typedFactionIcon;

		// Token: 0x04001CC1 RID: 7361
		public TooltipTrigger TT;

		// Token: 0x04001CC2 RID: 7362
		private TISpaceFleetState fleet;
	}
}

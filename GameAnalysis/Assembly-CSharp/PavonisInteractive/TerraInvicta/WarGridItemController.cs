using System;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000899 RID: 2201
	public class WarGridItemController : MonoBehaviour
	{
		// Token: 0x0600530C RID: 21260 RVA: 0x0024DAC6 File Offset: 0x0024BCC6
		public void ItemSelected()
		{
			if (this.nation.extant)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_RegionSelect", false, false);
				TIUtilities.GotoGameState(this.nation, true, true, true, true, false, -1f);
			}
		}

		// Token: 0x0600530D RID: 21261 RVA: 0x0024DAF8 File Offset: 0x0024BCF8
		public void UpdateGridItem(TINationState viewNation, TINationState warNationState)
		{
			this.nation = warNationState;
			this.warFlag.sprite = warNationState.flag;
			this.warGridTrigger.SetDelegate("BodyText", () => this.WarTooltip(viewNation, warNationState));
			this.nukesImage.enabled = warNationState.numNuclearWeapons > 0;
			this.armyImage.enabled = warNationState.numStandardArmies > 0;
		}

		// Token: 0x0600530E RID: 21262 RVA: 0x0024DB90 File Offset: 0x0024BD90
		private string WarTooltip(TINationState viewNation, TINationState warEnemy)
		{
			return warEnemy.displayName;
		}

		// Token: 0x04003814 RID: 14356
		public Image warFlag;

		// Token: 0x04003815 RID: 14357
		public TooltipTrigger warGridTrigger;

		// Token: 0x04003816 RID: 14358
		public TINationState nation;

		// Token: 0x04003817 RID: 14359
		public Image armyImage;

		// Token: 0x04003818 RID: 14360
		public Image nukesImage;
	}
}

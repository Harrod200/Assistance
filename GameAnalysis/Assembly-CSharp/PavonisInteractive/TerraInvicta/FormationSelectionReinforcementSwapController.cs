using System;
using ModelShark;
using PavonisInteractive.TerraInvicta.SpaceCombat.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008CC RID: 2252
	internal class FormationSelectionReinforcementSwapController : MonoBehaviour
	{
		// Token: 0x17000F15 RID: 3861
		// (get) Token: 0x0600565C RID: 22108 RVA: 0x002781C8 File Offset: 0x002763C8
		// (set) Token: 0x0600565D RID: 22109 RVA: 0x002781D0 File Offset: 0x002763D0
		public TISpaceShipState ship { get; private set; }

		// Token: 0x0600565E RID: 22110 RVA: 0x002781DC File Offset: 0x002763DC
		public void SetListItem(TISpaceShipState ship, SpaceCombatCanvasController controller)
		{
			this.ship = ship;
			this.buttonDefaultSprite = this.button.image.sprite;
			this.shipName.SetText(ship.NameWithDamageIcons());
			this.shipClass.SetText(ship.template.fullClassName);
			this.shipTip.SetDelegate("BodyText", () => ship.template.quickSummary(false, ship, false, true, true));
			this.fleetIcon.SetGridItem_Alt(ship, () => ship.template.quickSummary(false, ship, false, true, true), false);
			this.fleetIcon.gameObject.SetActive(true);
			this.controller = controller;
		}

		// Token: 0x0600565F RID: 22111 RVA: 0x0027829B File Offset: 0x0027649B
		public void SetButtonInteractable(bool setting)
		{
			this.button.interactable = setting;
		}

		// Token: 0x06005660 RID: 22112 RVA: 0x002782A9 File Offset: 0x002764A9
		public void OnButtonPressed()
		{
			this.controller.OnFormationSettingReinforcementShipSelected(this.ship);
		}

		// Token: 0x06005661 RID: 22113 RVA: 0x002782BC File Offset: 0x002764BC
		public void HighlightButtonAfterSelection(bool highlighted)
		{
			if (highlighted)
			{
				this.button.image.sprite = this.button.spriteState.highlightedSprite;
				return;
			}
			this.button.image.sprite = this.buttonDefaultSprite;
		}

		// Token: 0x04003D6A RID: 15722
		public TMP_Text shipName;

		// Token: 0x04003D6B RID: 15723
		public TMP_Text shipClass;

		// Token: 0x04003D6C RID: 15724
		public Button button;

		// Token: 0x04003D6D RID: 15725
		public TooltipTrigger shipTip;

		// Token: 0x04003D6F RID: 15727
		public FleetShipGridItemController fleetIcon;

		// Token: 0x04003D70 RID: 15728
		private Sprite buttonDefaultSprite;

		// Token: 0x04003D71 RID: 15729
		private SpaceCombatCanvasController controller;
	}
}

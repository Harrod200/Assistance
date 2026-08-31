using System;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.SpaceCombat.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008CE RID: 2254
	public class ReinforcementReorderListItemController : MonoBehaviour
	{
		// Token: 0x17000F16 RID: 3862
		// (get) Token: 0x06005666 RID: 22118 RVA: 0x00278326 File Offset: 0x00276526
		// (set) Token: 0x06005667 RID: 22119 RVA: 0x0027832E File Offset: 0x0027652E
		public TISpaceShipState ship { get; private set; }

		// Token: 0x06005668 RID: 22120 RVA: 0x00278338 File Offset: 0x00276538
		public void SetListItem(TISpaceShipState ship, SpaceCombatCanvasController controller)
		{
			this.ship = ship;
			this.shipName.SetText(ship.NameWithDamageIcons());
			this.shipClass.SetText(ship.template.fullClassName);
			this.shipTip.SetDelegate("BodyText", () => ship.template.quickSummary(false, ship, false, true, true));
			this.fleetIcon.SetGridItem_Alt(ship, () => ship.template.quickSummary(false, ship, false, true, true), false);
			this.fleetIcon.gameObject.SetActive(true);
			this.controller = controller;
			this.SetReorderButtonsInteractable();
		}

		// Token: 0x06005669 RID: 22121 RVA: 0x002783E8 File Offset: 0x002765E8
		public void SetReorderButtonsInteractable()
		{
			int num = this.controller.leftHandFleetController.reinforcements.IndexOf(this.ship);
			this.Up1StepButton.interactable = num > 0;
			this.UpAllStepsButton.interactable = num > 0;
			this.Down1StepButton.interactable = num + 1 < this.controller.leftHandFleetController.reinforcements.Count;
			this.DownAllStepsButton.interactable = num + 1 < this.controller.leftHandFleetController.reinforcements.Count;
		}

		// Token: 0x0600566A RID: 22122 RVA: 0x00278479 File Offset: 0x00276679
		public void OnPress_Up1StepButton()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.controller.RepositionShipInReinforcements(this.ship, -1);
		}

		// Token: 0x0600566B RID: 22123 RVA: 0x00278499 File Offset: 0x00276699
		public void OnPress_UpAllStepsButton()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.controller.RepositionShipInReinforcements(this.ship, -9999);
		}

		// Token: 0x0600566C RID: 22124 RVA: 0x002784BD File Offset: 0x002766BD
		public void OnPress_Down1StepButton()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
			this.controller.RepositionShipInReinforcements(this.ship, 1);
		}

		// Token: 0x0600566D RID: 22125 RVA: 0x002784DD File Offset: 0x002766DD
		public void OnPress_DownAllStepsButton()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
			this.controller.RepositionShipInReinforcements(this.ship, 9999);
		}

		// Token: 0x04003D72 RID: 15730
		public TMP_Text shipName;

		// Token: 0x04003D73 RID: 15731
		public TMP_Text shipClass;

		// Token: 0x04003D74 RID: 15732
		public TooltipTrigger shipTip;

		// Token: 0x04003D76 RID: 15734
		public FleetShipGridItemController fleetIcon;

		// Token: 0x04003D77 RID: 15735
		private SpaceCombatCanvasController controller;

		// Token: 0x04003D78 RID: 15736
		public Button Up1StepButton;

		// Token: 0x04003D79 RID: 15737
		public Button UpAllStepsButton;

		// Token: 0x04003D7A RID: 15738
		public Button Down1StepButton;

		// Token: 0x04003D7B RID: 15739
		public Button DownAllStepsButton;
	}
}

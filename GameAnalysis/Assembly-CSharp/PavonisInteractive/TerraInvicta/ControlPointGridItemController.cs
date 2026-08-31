using System;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200088E RID: 2190
	public class ControlPointGridItemController : MonoBehaviour
	{
		// Token: 0x060051E3 RID: 20963 RVA: 0x0023FC78 File Offset: 0x0023DE78
		public void SetGridItem(NationInfoController controller, TINationState nationState, TIControlPoint controlPoint, Image flagImage)
		{
			this.controller = controller;
			this.CPButton.interactable = false;
			this.toHitText.enabled = false;
			this.controlPoint = controlPoint;
			int numArmiesAtControlPoint = nationState.GetNumArmiesAtControlPoint(controlPoint.positionInNation);
			if (numArmiesAtControlPoint > 0)
			{
				this.armyPanel.SetActive(true);
				this.armyImage.enabled = true;
				if (numArmiesAtControlPoint > 1)
				{
					this.armyCount.SetText(numArmiesAtControlPoint.ToString());
					this.armyCount.enabled = true;
				}
				else
				{
					this.armyCount.enabled = false;
				}
			}
			else
			{
				this.armyPanel.SetActive(false);
			}
			this.crackdownStatusPanel.enabled = controlPoint.benefitsDisabled;
			this.defendStatusPanel.enabled = controlPoint.defended;
			this.executiveStatusPanel.enabled = controlPoint.executive;
			this.controlPointImage.sprite = controlPoint.GetIcon(true, true);
			this.controlPointImage.enabled = true;
			if (!controlPoint.owned)
			{
				this.controlPointImage.color = controlPoint.nation.template.UIColor;
				this.controlPointTooltip.SetImage("Icon", flagImage.sprite);
			}
			else
			{
				this.controlPointImage.color = Color.white;
				this.controlPointTooltip.SetImage("Icon", this.controlPointImage.sprite);
			}
			this.controlPointTooltip.SetDelegate("BodyText", () => NationInfoController.ControlPointTooltip(nationState, controlPoint));
			this.controlPointTooltip.enabled = true;
			base.enabled = true;
			this.SetControlPointButton();
		}

		// Token: 0x060051E4 RID: 20964 RVA: 0x0023FE44 File Offset: 0x0023E044
		private void SetControlPointButton()
		{
			if (GeneralControlsController.CurrentValidTarget(this.controlPoint) || (this.controller.targetingNeutralCP && GeneralControlsController.CurrentValidTarget(this.controlPoint.nation) && this.controlPoint.nextOpenControlPoint))
			{
				this.CPButton.enabled = true;
				this.CPButton.interactable = true;
				string text = "0%";
				if (this.controller.targetingOwnedCPs)
				{
					text = this.controller.currentMission.resolutionMethod.GetSuccessChanceString(this.controller.currentMission, this.controller.currentMissionCouncilor, this.controlPoint, 0f, false, 2);
				}
				else if (this.controller.targetingNeutralCP)
				{
					text = this.controller.currentMission.resolutionMethod.GetSuccessChanceString(this.controller.currentMission, this.controller.currentMissionCouncilor, this.controlPoint.nation, 0f, false, 2);
				}
				this.toHitText.enabled = true;
				this.toHitText.SetText(text);
				return;
			}
			this.CPButton.interactable = false;
			this.toHitText.enabled = false;
		}

		// Token: 0x060051E5 RID: 20965 RVA: 0x0023FF78 File Offset: 0x0023E178
		public void DisableControlPoint()
		{
			this.controlPointTooltip.enabled = false;
			this.crackdownStatusPanel.enabled = false;
			this.defendStatusPanel.enabled = false;
			this.executiveStatusPanel.enabled = false;
			this.controlPointImage.enabled = false;
			this.armyPanel.SetActive(false);
			this.CPButton.interactable = false;
			this.toHitText.enabled = false;
		}

		// Token: 0x060051E6 RID: 20966 RVA: 0x0023FFE5 File Offset: 0x0023E1E5
		public void OnControlPointButtonClicked()
		{
			SoundEffectController.PlaySelectSound(this.controlPoint);
			GameControl.eventManager.TriggerEvent(new ControlPointTargetSelected(this.controlPoint), null, Array.Empty<object>());
		}

		// Token: 0x0400368A RID: 13962
		private NationInfoController controller;

		// Token: 0x0400368B RID: 13963
		private TIControlPoint controlPoint;

		// Token: 0x0400368C RID: 13964
		public GameObject armyPanel;

		// Token: 0x0400368D RID: 13965
		public TMP_Text armyCount;

		// Token: 0x0400368E RID: 13966
		public Image crackdownStatusPanel;

		// Token: 0x0400368F RID: 13967
		public Image defendStatusPanel;

		// Token: 0x04003690 RID: 13968
		public Image executiveStatusPanel;

		// Token: 0x04003691 RID: 13969
		public TooltipTrigger controlPointTooltip;

		// Token: 0x04003692 RID: 13970
		public Image controlPointImage;

		// Token: 0x04003693 RID: 13971
		public Image armyImage;

		// Token: 0x04003694 RID: 13972
		public Button CPButton;

		// Token: 0x04003695 RID: 13973
		public TMP_Text toHitText;
	}
}

using System;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008AD RID: 2221
	internal class FuelSharingListItemController : MonoBehaviour
	{
		// Token: 0x17000EF6 RID: 3830
		// (get) Token: 0x06005428 RID: 21544 RVA: 0x002612EB File Offset: 0x0025F4EB
		// (set) Token: 0x06005429 RID: 21545 RVA: 0x002612F3 File Offset: 0x0025F4F3
		public TISpaceShipState ship { get; private set; }

		// Token: 0x0600542A RID: 21546 RVA: 0x002612FC File Offset: 0x0025F4FC
		public void SetListItem(TISpaceShipState ship, int column, OperationCanvasController controller)
		{
			this.shipName.SetText(ship.NameWithDamageIcons());
			this.baselinePropellant_tons.SetText(Loc.T("UI.Operations.PropellantStatus", new object[]
			{
				ship.propellant_tons.ToString("N0"),
				ship.template.propellantMass_tons.ToString("N0")
			}));
			this.baselineDV.SetText(Loc.T("UI.Fleets.DVValue", new object[]
			{
				TIUtilities.FormatBigOrSmallNumber(ship.currentDeltaV_kps, 1, 7, 0, false, false),
				TIUtilities.FormatBigOrSmallNumber(ship.currentMaxDeltaV_kps, 1, 7, 0, false, false)
			}));
			this.column = column;
			this.ship = ship;
			this.controller = controller;
			if (column != 1)
			{
				if (column == 2)
				{
					this.mainButton.interactable = true;
				}
			}
			else
			{
				this.mainButton.interactable = true;
			}
			this.fleetIcon.SetGridItem_Alt(ship, () => this.Tooltip(), true);
			this.UpdateForProposedTransfers();
		}

		// Token: 0x0600542B RID: 21547 RVA: 0x002613FC File Offset: 0x0025F5FC
		private string Tooltip()
		{
			StringBuilder stringBuilder = new StringBuilder();
			switch (this.column)
			{
			case 0:
				stringBuilder.AppendLine(Loc.T("UI.Operations.AvailableGiversTip"));
				break;
			case 1:
				stringBuilder.AppendLine(Loc.T("UI.Operations.SelectedTakersTip"));
				break;
			case 2:
				stringBuilder.AppendLine(Loc.T("UI.Operations.AvailableTakersTip"));
				break;
			}
			stringBuilder.AppendLine(this.ship.template.quickSummary(true, this.ship, false, false, true));
			return stringBuilder.ToString();
		}

		// Token: 0x0600542C RID: 21548 RVA: 0x00261488 File Offset: 0x0025F688
		public void UpdateForProposedTransfers()
		{
			int num = this.column;
			if (num != 0)
			{
				if (num != 1)
				{
					return;
				}
				if (this.controller.selectedTakers.Contains(this.ship))
				{
					float num2 = this.controller.propellantSharingEvents.Where<PropellantSharingEvent>((PropellantSharingEvent x) => x.taker == this.ship).Sum<PropellantSharingEvent>((PropellantSharingEvent x) => x.amount_tons);
					if (num2 > 0f)
					{
						this.proposedPropellantChange.SetText(TIUtilities.GreenLine(Loc.T("UI.Operations.PropellantGain", new object[] { num2.ToString("N0") })));
						this.proposedNewDV.SetText(TIUtilities.GreenLine(Loc.T("UI.Operations.DVGain", new object[] { this.ship.NotionalDeltaVChange_kps(num2).ToString("N0") })));
					}
					else
					{
						this.proposedPropellantChange.SetText(Loc.T("UI.Operations.PropellantGain", new object[] { 0.ToString("N0") }));
						this.proposedNewDV.SetText(Loc.T("UI.Operations.DVGain", new object[] { 0.ToString("N0") }));
					}
					GameControl.assetLoader.LoadAssetForImageAssignment((!this.controller.lockedTakers.Contains(this.ship)) ? "ui_spacecombat/ICO_CancelPadlockPrimaryTargetCommand_off" : "ui_spacecombat/ICO_PadlockPrimaryTargetCommand_off", this.upperButtonImage);
					this.lowerButton.interactable = num2 > 0f && !this.controller.lockedTakers.Contains(this.ship);
				}
			}
			else if (this.controller.availableGivers.Contains(this.ship))
			{
				float num3 = this.controller.propellantSharingEvents.Where<PropellantSharingEvent>((PropellantSharingEvent x) => x.giver == this.ship).Sum<PropellantSharingEvent>((PropellantSharingEvent x) => x.amount_tons);
				if (num3 > 0f)
				{
					this.proposedPropellantChange.SetText(TIUtilities.RedLine(Loc.T("UI.Operations.PropellantLoss", new object[] { num3.ToString("N0") })));
					this.proposedNewDV.SetText(TIUtilities.RedLine(Loc.T("UI.Operations.DVLoss", new object[] { this.ship.NotionalDeltaVChange_kps(-num3).ToString("N0") })));
				}
				else
				{
					this.proposedPropellantChange.SetText(Loc.T("UI.Fleets.Tons", new object[] { 0.ToString("N0") }));
					this.proposedNewDV.SetText(Loc.T("UI.Fleets.SingleDV", new object[] { 0.ToString("N0") }));
				}
				this.upperButton.interactable = num3 < this.ship.propellant_tons && this.controller.selectedTakers.Count > 0;
				this.lowerButton.interactable = num3 > 0f;
				return;
			}
		}

		// Token: 0x0600542D RID: 21549 RVA: 0x002617A4 File Offset: 0x0025F9A4
		public void OnMainButtonSelected()
		{
			int num = this.column;
			if (num != 1)
			{
				if (num != 2)
				{
					return;
				}
				this.controller.OnTakerAdded(this.ship);
			}
			else if (!this.controller.lockedTakers.Contains(this.ship))
			{
				this.controller.OnTakerRemoved(this.ship);
				this.UpdateForProposedTransfers();
				return;
			}
		}

		// Token: 0x0600542E RID: 21550 RVA: 0x00261804 File Offset: 0x0025FA04
		public void OnTopButtonPressed()
		{
			int num = 100;
			if (TIInputManager.IsShiftKeyDown)
			{
				if (TIInputManager.IsControlKeyDown)
				{
					num = 10000;
				}
				else
				{
					if (TIInputManager.IsAltKeyDown)
					{
					}
					num = 1000;
				}
			}
			else if (TIInputManager.IsAltKeyDown)
			{
				num = 1;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			int num2 = this.column;
			if (num2 == 0)
			{
				this.controller.AttemptGivePropellant(this.ship, (float)num);
				return;
			}
			if (num2 != 1)
			{
				return;
			}
			if (this.controller.lockedTakers.Contains(this.ship))
			{
				this.controller.UnlockTaker(this.ship);
				this.lowerButton.interactable = this.controller.propellantSharingEvents.Where<PropellantSharingEvent>((PropellantSharingEvent x) => x.taker == this.ship).Sum<PropellantSharingEvent>((PropellantSharingEvent x) => x.amount_tons) > 0f;
			}
			else
			{
				this.controller.LockTaker(this.ship);
				this.lowerButton.interactable = false;
			}
			GameControl.assetLoader.LoadAssetForImageAssignment((!this.controller.lockedTakers.Contains(this.ship)) ? "ui_spacecombat/ICO_CancelPadlockPrimaryTargetCommand_off" : "ui_spacecombat/ICO_PadlockPrimaryTargetCommand_off", this.upperButtonImage);
		}

		// Token: 0x0600542F RID: 21551 RVA: 0x00261944 File Offset: 0x0025FB44
		public void OnBottomButtonPressed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
			int num = this.column;
			if (num == 0)
			{
				this.controller.ResetPropellantGiver(this.ship);
				return;
			}
			if (num != 1)
			{
				return;
			}
			if (!this.controller.lockedTakers.Contains(this.ship))
			{
				this.controller.ResetPropellantTaker(this.ship);
			}
		}

		// Token: 0x04003A74 RID: 14964
		public TMP_Text shipName;

		// Token: 0x04003A75 RID: 14965
		public TMP_Text baselinePropellant_tons;

		// Token: 0x04003A76 RID: 14966
		public TMP_Text baselineDV;

		// Token: 0x04003A77 RID: 14967
		public TMP_Text proposedPropellantChange;

		// Token: 0x04003A78 RID: 14968
		public TMP_Text proposedNewDV;

		// Token: 0x04003A79 RID: 14969
		public Button mainButton;

		// Token: 0x04003A7A RID: 14970
		public Button upperButton;

		// Token: 0x04003A7B RID: 14971
		public Button lowerButton;

		// Token: 0x04003A7C RID: 14972
		public Image upperButtonImage;

		// Token: 0x04003A7D RID: 14973
		public FleetShipGridItemController fleetIcon;

		// Token: 0x04003A7F RID: 14975
		private int column;

		// Token: 0x04003A80 RID: 14976
		private OperationCanvasController controller;
	}
}

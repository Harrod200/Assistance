using System;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.SolarSystem;
using TMPro;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008B5 RID: 2229
	public class SplitFleetShipListItemController : MonoBehaviour
	{
		// Token: 0x060054F7 RID: 21751 RVA: 0x00269B88 File Offset: 0x00267D88
		public void SetListItem(OperationCanvasController controller, TISpaceShipState ship, bool originFleet, bool includeScuttleCost = false)
		{
			this.shipName.SetText(Loc.T("UI.Operations.ShipItem", new object[]
			{
				ship.NameWithDamageIcons(),
				ship.template.fullClassName
			}));
			this.controller = controller;
			this.originFleetItem = originFleet;
			this.ship = ship;
			this.tooltip.SetDelegate("BodyText", () => this.GetTooltip(ship));
			if (includeScuttleCost)
			{
				this.shipDataLineItem.SetActive(false);
				TIResourcesCost tiresourcesCost = ship.ScuttleCost();
				if (tiresourcesCost.anyDebit || tiresourcesCost.anyCredit)
				{
					if (!ship.fleet.dockedOrLanded)
					{
						this.scuttleCost.SetText(new StringBuilder("-").Append(tiresourcesCost.ToString("Relevant", false, false, null, false, FactionResource.None)));
					}
					else
					{
						this.scuttleCost.SetText(tiresourcesCost.ToString("Relevant", false, false, null, false, FactionResource.None));
					}
				}
				else
				{
					this.scuttleCost.SetText(string.Empty);
				}
				this.singleTextLineItem.SetActive(true);
				return;
			}
			this.singleTextLineItem.SetActive(false);
			this.combatScore.SetText(ship.SpaceCombatValue(false, 0f).ToString("N0"));
			this.assaultScore.SetText(ship.AssaultCombatValue(false).ToString("N0"));
			this.acceleration.SetText(FleetsScreenController.dualAccelerationStr(ship));
			this.DV.SetText(Loc.T("UI.Fleets.DVValue", new object[]
			{
				TIUtilities.FormatBigOrSmallNumber(ship.currentDeltaV_kps, 1, 7, 0, false, false),
				TIUtilities.FormatBigOrSmallNumber(ship.currentMaxDeltaV_kps, 1, 7, 0, false, false)
			}));
			this.shipDataLineItem.SetActive(true);
		}

		// Token: 0x060054F8 RID: 21752 RVA: 0x00269D88 File Offset: 0x00267F88
		public void OnClicked()
		{
			SpaceObjectSelection.BlockSelectionFrame();
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_HoverActionIcon", false, false);
			this.tooltip.ForceHideTooltip();
			this.controller.SwapItem(this.ship, this.originFleetItem);
		}

		// Token: 0x060054F9 RID: 21753 RVA: 0x00269DBD File Offset: 0x00267FBD
		public string GetTooltip(TISpaceShipState ship)
		{
			return ship.template.quickSummary(true, ship, false, false, true);
		}

		// Token: 0x04003B27 RID: 15143
		private OperationCanvasController controller;

		// Token: 0x04003B28 RID: 15144
		public TMP_Text shipName;

		// Token: 0x04003B29 RID: 15145
		public TMP_Text scuttleCost;

		// Token: 0x04003B2A RID: 15146
		private TISpaceShipState ship;

		// Token: 0x04003B2B RID: 15147
		private bool originFleetItem;

		// Token: 0x04003B2C RID: 15148
		public GameObject singleTextLineItem;

		// Token: 0x04003B2D RID: 15149
		public GameObject shipDataLineItem;

		// Token: 0x04003B2E RID: 15150
		public TMP_Text combatScore;

		// Token: 0x04003B2F RID: 15151
		public TMP_Text assaultScore;

		// Token: 0x04003B30 RID: 15152
		public TMP_Text acceleration;

		// Token: 0x04003B31 RID: 15153
		public TMP_Text DV;

		// Token: 0x04003B32 RID: 15154
		public TooltipTrigger tooltip;
	}
}

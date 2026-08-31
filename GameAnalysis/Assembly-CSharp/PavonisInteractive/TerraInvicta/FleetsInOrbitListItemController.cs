using System;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008D4 RID: 2260
	public class FleetsInOrbitListItemController : MonoBehaviour
	{
		// Token: 0x06005699 RID: 22169 RVA: 0x00279E48 File Offset: 0x00278048
		public void UpdateListItem(TISpaceFleetState fleet, TINaturalSpaceObjectState viewedObject, bool victoryConditionTargetFleet)
		{
			this.fleet = fleet;
			StringBuilder stringBuilder = new StringBuilder(fleet.GetDisplayName(GameControl.control.activePlayer));
			if (fleet.dockedAtStation)
			{
				stringBuilder.Append(TIGlobalConfig.globalConfig.pathInlineHabStationIcon);
			}
			else if (fleet.landedAtBase)
			{
				stringBuilder.Append(TIGlobalConfig.globalConfig.pathInlineHabBaseIcon);
			}
			else if (fleet.landedInOutback)
			{
				stringBuilder.Append(TIGlobalConfig.globalConfig.ruggedRegionInlineSpritePath);
			}
			else if (fleet.inTransfer)
			{
				stringBuilder.Append(TIGlobalConfig.globalConfig.pathInlineEscapeVelocityIcon);
			}
			if (victoryConditionTargetFleet)
			{
				stringBuilder.Append(TIGlobalConfig.globalConfig.victoryItemInlineSpritePath);
			}
			this.fleetName.SetText(stringBuilder.ToString());
			this.smallShips.SetText(fleet.smallShips.Count.ToString());
			this.mediumShips.SetText(fleet.mediumShips.Count.ToString());
			this.largeShips.SetText(fleet.largeShips.Count.ToString());
			this.factionIcon.sprite = fleet.faction.factionIcon64;
			if (fleet.inTransfer)
			{
				this.orbitAltitude.SetText(Loc.T("UI.Space.InTransit"));
			}
			else if (fleet.ref_naturalSpaceObject == viewedObject)
			{
				this.orbitAltitude.SetText(Loc.T("UI.Space.Distkm", new object[] { fleet.altitude_km.ToString("N0") }));
			}
			else
			{
				this.orbitAltitude.SetText(fleet.ref_naturalSpaceObject.displayName);
			}
			this.DV_kps.SetText(fleet.currentDeltaV_kps.ToString("N0"));
			if (fleet.CurrentOperations().Count > 0)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(fleet.CurrentOperations()[0].operation.GetOperationIconImagePath_Off(), this.operation);
				this.operation.enabled = true;
			}
			else
			{
				this.operation.enabled = false;
			}
			this.tip.SetDelegate("BodyText", () => fleet.FleetQuickDescription(GameControl.control.activePlayer));
		}

		// Token: 0x0600569A RID: 22170 RVA: 0x0027A0D1 File Offset: 0x002782D1
		public void OnFleetButtonPressed()
		{
			SoundEffectController.PlaySelectSound(this.fleet);
			TIUtilities.GotoGameState(this.fleet, false, true, true, true, false, -1f);
		}

		// Token: 0x04003DB3 RID: 15795
		private TISpaceFleetState fleet;

		// Token: 0x04003DB4 RID: 15796
		public Image factionIcon;

		// Token: 0x04003DB5 RID: 15797
		public TMP_Text fleetName;

		// Token: 0x04003DB6 RID: 15798
		public TMP_Text smallShips;

		// Token: 0x04003DB7 RID: 15799
		public TMP_Text mediumShips;

		// Token: 0x04003DB8 RID: 15800
		public TMP_Text largeShips;

		// Token: 0x04003DB9 RID: 15801
		public TMP_Text orbitAltitude;

		// Token: 0x04003DBA RID: 15802
		public TMP_Text DV_kps;

		// Token: 0x04003DBB RID: 15803
		public Image operation;

		// Token: 0x04003DBC RID: 15804
		public TooltipTrigger tip;
	}
}

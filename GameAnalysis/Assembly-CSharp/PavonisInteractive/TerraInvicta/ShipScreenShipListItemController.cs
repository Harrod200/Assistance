using System;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200085A RID: 2138
	public class ShipScreenShipListItemController : MonoBehaviour
	{
		// Token: 0x17000EB5 RID: 3765
		// (get) Token: 0x06004E68 RID: 20072 RVA: 0x0021BB74 File Offset: 0x00219D74
		// (set) Token: 0x06004E69 RID: 20073 RVA: 0x0021BB7C File Offset: 0x00219D7C
		public TISpaceShipState ship { get; private set; }

		// Token: 0x06004E6A RID: 20074 RVA: 0x0021BB88 File Offset: 0x00219D88
		public void SetListItem(TISpaceShipState ship, FleetsScreenController controller)
		{
			if (!TIGameState.Valid(ship) || !TIGameState.Valid(ship.fleet))
			{
				return;
			}
			this.ship = ship;
			this.controller = controller;
			if (this.defaultButtonSprite == null)
			{
				this.defaultButtonSprite = this.button.image.sprite;
			}
			this.shipName.SetText(ship.NameWithDamageIcons());
			this.className.SetText(ship.template.fullClassName);
			this.fleetName.SetText(ship.fleet.GetDisplayName(GameControl.control.activePlayer));
			TIGameState tigameState;
			Sprite parentBodyIconResource = SpaceObjectDetailController.GetParentBodyIconResource(ship.fleet, out tigameState);
			if (parentBodyIconResource != null)
			{
				this.locationImage.sprite = parentBodyIconResource;
				this.locationImage.enabled = true;
			}
			else
			{
				this.locationImage.enabled = false;
			}
			this.factionImage.sprite = ship.faction.factionIcon64;
		}

		// Token: 0x06004E6B RID: 20075 RVA: 0x0021BC78 File Offset: 0x00219E78
		public void UpdateNames(TISpaceShipState ship, FleetsScreenController controller)
		{
			this.ship = ship;
			this.controller = controller;
			this.shipName.SetText(ship.displayName);
			this.fleetName.SetText(ship.fleet.GetDisplayName(GameControl.control.activePlayer));
			controller.fleetListDirty = true;
		}

		// Token: 0x06004E6C RID: 20076 RVA: 0x0021BCCB File Offset: 0x00219ECB
		public void OnShipScreenListItemClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
			this.controller.UpdateIndividualDataScreen(this.ship);
		}

		// Token: 0x06004E6D RID: 20077 RVA: 0x0021BCEC File Offset: 0x00219EEC
		public void OnNewShipSelected(TISpaceShipState selectedShip)
		{
			this.button.image.sprite = ((selectedShip == this.ship) ? this.button.spriteState.pressedSprite : this.defaultButtonSprite);
		}

		// Token: 0x04003200 RID: 12800
		public TMP_Text shipName;

		// Token: 0x04003201 RID: 12801
		public TMP_Text className;

		// Token: 0x04003202 RID: 12802
		public TMP_Text fleetName;

		// Token: 0x04003203 RID: 12803
		public Image locationImage;

		// Token: 0x04003204 RID: 12804
		public Image factionImage;

		// Token: 0x04003205 RID: 12805
		private FleetsScreenController controller;

		// Token: 0x04003206 RID: 12806
		public Button button;

		// Token: 0x04003207 RID: 12807
		private Sprite defaultButtonSprite;
	}
}

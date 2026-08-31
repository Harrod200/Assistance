using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000860 RID: 2144
	public class GlobalSearchListItemController : MonoBehaviour
	{
		// Token: 0x06004F9C RID: 20380 RVA: 0x002261EC File Offset: 0x002243EC
		public void UpdateListItem(GlobalSearchListItem_Data data)
		{
			this.activePlayer = GameControl.control.activePlayer;
			this.gameState = data.gameState;
			this.controller = data.controller;
			this.nameText.SetText(data.gameState.GetDisplayName(this.activePlayer));
			this.icon.sprite = null;
			this.iconBackground.enabled = false;
			if (this.gameState.isOrgState)
			{
				this.icon.sprite = this.gameState.ref_org.icon;
			}
			else if (this.gameState.isNationState)
			{
				this.icon.sprite = this.gameState.ref_nation.flag;
			}
			else if (this.gameState.isRegionState)
			{
				Image image = this.icon;
				TINationState nation = this.gameState.ref_region.nation;
				image.sprite = ((nation != null) ? nation.flag : null);
				TMP_Text tmp_Text = this.nameText;
				string text = "UI.Notifications.TwoPointLocation";
				object[] array = new object[2];
				array[0] = this.gameState.ref_region.displayName;
				int num = 1;
				TINationState nation2 = this.gameState.ref_region.nation;
				array[num] = ((nation2 != null) ? nation2.displayNameWithArticle : null);
				tmp_Text.SetText(Loc.T(text, array));
			}
			else if (this.gameState.isCouncilorState)
			{
				this.icon.sprite = this.gameState.ref_councilor.GetIcon(false);
			}
			else if (this.gameState.isSpaceFleetState)
			{
				this.icon.sprite = this.gameState.ref_fleet.icon;
			}
			else if (this.gameState.isSpaceShipState)
			{
				this.icon.sprite = this.gameState.ref_ship.fleet.icon;
			}
			else if (this.gameState.isHabState)
			{
				this.icon.sprite = this.gameState.ref_hab.icon;
			}
			else if (this.gameState.isHabSiteState)
			{
				if (this.gameState.ref_habSite.hasOperatingBase)
				{
					this.icon.sprite = this.gameState.ref_habSite.hab.icon;
				}
				else
				{
					this.icon.sprite = GameControl.control._assetLoader.LoadAssetForSpriteAssignment(this.activePlayer.Prospected(this.gameState.ref_habSite) ? TemplateManager.global.pathProspectedHabSite : TemplateManager.global.pathNotProspectedHabSite);
				}
			}
			else if (this.gameState.isNaturalSpaceObjectState)
			{
				this.icon.sprite = this.gameState.ref_naturalSpaceObject.icon;
			}
			else if (this.gameState.isArmyState)
			{
				this.icon.sprite = GameControl.control._assetLoader.LoadAssetForSpriteAssignment(this.gameState.ref_army.GetIconForegroundResource);
				this.iconBackground.enabled = true;
				this.iconBackground.sprite = this.gameState.ref_army.GetIconBackgroundSprite;
				this.iconBackground.color = this.gameState.ref_army.GetIconBackgroundResourceColor;
			}
			else if (this.gameState.isFactionState)
			{
				this.icon.sprite = this.gameState.ref_faction.factionIcon64;
			}
			this.icon.enabled = this.icon.sprite != null;
		}

		// Token: 0x06004F9D RID: 20381 RVA: 0x00226570 File Offset: 0x00224770
		public void OnClickListItem()
		{
			if (this.gameState.deleted)
			{
				this.controller.OnGlobalSearchInputUpdated();
				return;
			}
			if (!this.controller.CanDisplaySearchableGameStateWithIntel(this.gameState))
			{
				this.controller.OnGlobalSearchInputUpdated();
				return;
			}
			if (this.gameState.isOrgState && this.gameState.ref_org.assignedCouncilor != null && this.activePlayer.HasIntelOnCouncilorDetails(this.gameState.ref_org.assignedCouncilor))
			{
				TIUtilities.GotoGameState(this.gameState.ref_org.assignedCouncilor, false, true, true);
				return;
			}
			if (this.gameState.isSpaceShipState && this.gameState.ref_fleet != null)
			{
				TIUtilities.GotoGameState(this.gameState.ref_fleet, false, true, true, true, false, -1f);
				return;
			}
			TIUtilities.GotoGameState(this.gameState, false, true, true, true, false, -1f);
		}

		// Token: 0x040032FB RID: 13051
		public Image icon;

		// Token: 0x040032FC RID: 13052
		public Image iconBackground;

		// Token: 0x040032FD RID: 13053
		public TMP_Text nameText;

		// Token: 0x040032FE RID: 13054
		public TIGameState gameState;

		// Token: 0x040032FF RID: 13055
		public GeneralControlsController controller;

		// Token: 0x04003300 RID: 13056
		private TIFactionState activePlayer;
	}
}

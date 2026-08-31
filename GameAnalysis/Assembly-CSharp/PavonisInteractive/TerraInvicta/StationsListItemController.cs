using System;
using System.Linq;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008DB RID: 2267
	public class StationsListItemController : MonoBehaviour
	{
		// Token: 0x0600577B RID: 22395 RVA: 0x00284154 File Offset: 0x00282354
		public void UpdateListItem(TIHabState hab, TINaturalSpaceObjectState selectedObject, bool victoryAsset, SpaceObjectDetailController controller)
		{
			this.station = hab;
			this.controller = controller;
			string text = hab.displayName;
			if (victoryAsset)
			{
				text = new StringBuilder(text).Append(TIGlobalConfig.globalConfig.victoryItemInlineSpritePath).ToString();
			}
			if (hab.underAssault || hab.dockedFleets.Any<TISpaceFleetState>((TISpaceFleetState x) => !x.faction.permanentAlly(hab.faction)))
			{
				text = new StringBuilder(text).Append(TIGlobalConfig.globalConfig.armyBattleInlineSpritePath).ToString();
			}
			this.habName.SetText(text);
			for (int i = 0; i <= 4; i++)
			{
				if (i < hab.sectors.Count && hab.sectors[i].faction != null)
				{
					this.CPImage[i].sprite = hab.sectors[i].faction.factionIcon64UI;
					this.CPImage[i].gameObject.SetActive(true);
					this.CPImage[i].enabled = true;
				}
				else
				{
					this.CPImage[i].enabled = false;
					this.CPImage[i].gameObject.SetActive(false);
				}
			}
			if (hab.ref_naturalSpaceObject == selectedObject)
			{
				this.orbitAltitude.SetText(hab.altitude.ToString("N0"));
			}
			else
			{
				this.orbitAltitude.SetText(hab.ref_naturalSpaceObject.displayName);
			}
			this.tip.SetDelegate("BodyText", () => hab.BuildShortHabSummary(GameControl.control.activePlayer));
		}

		// Token: 0x0600577C RID: 22396 RVA: 0x0028431C File Offset: 0x0028251C
		public void OnStationButtonClicked()
		{
			SoundEffectController.PlaySelectSound(this.station);
			TIUtilities.GotoGameState(this.station, true, true, true, true, false, -1f);
		}

		// Token: 0x0600577D RID: 22397 RVA: 0x0028433E File Offset: 0x0028253E
		public void OnStationIconButtonClicked()
		{
			this.controller.HabSelectedFromSiteList(this.station);
		}

		// Token: 0x04003F3D RID: 16189
		private TIHabState station;

		// Token: 0x04003F3E RID: 16190
		public TMP_Text habName;

		// Token: 0x04003F3F RID: 16191
		public Image[] CPImage;

		// Token: 0x04003F40 RID: 16192
		public TMP_Text orbitAltitude;

		// Token: 0x04003F41 RID: 16193
		public TooltipTrigger tip;

		// Token: 0x04003F42 RID: 16194
		private SpaceObjectDetailController controller;
	}
}

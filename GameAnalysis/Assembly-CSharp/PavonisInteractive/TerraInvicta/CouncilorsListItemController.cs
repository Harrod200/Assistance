using System;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200088F RID: 2191
	public class CouncilorsListItemController : MonoBehaviour
	{
		// Token: 0x060051E8 RID: 20968 RVA: 0x00240015 File Offset: 0x0023E215
		public void Init(TICouncilorState councilor)
		{
			this.councilor = councilor;
		}

		// Token: 0x060051E9 RID: 20969 RVA: 0x00240020 File Offset: 0x0023E220
		public void UpdateListItem()
		{
			CouncilorView councilorView = new CouncilorView(this.councilor, GameControl.control.activePlayer);
			this.CouncilorName.SetText(councilorView.displayNameCurrent);
			this.CouncilorProfession.SetText(councilorView.councilorJobStringCurrent);
			if (councilorView.factionCurrent != null)
			{
				string factionIcon64Current = councilorView.factionIcon64Current;
				if (!string.IsNullOrEmpty(factionIcon64Current))
				{
					this.FactionImage.enabled = true;
					GameControl.assetLoader.LoadAssetForImageAssignment(factionIcon64Current, this.FactionImage);
				}
				else
				{
					this.FactionImage.enabled = false;
				}
			}
			else
			{
				this.FactionImage.enabled = false;
			}
			if (councilorView.HasMission)
			{
				TIMissionTemplate currentMissionTemplate = councilorView.currentMissionTemplate;
				string text = ((currentMissionTemplate != null) ? currentMissionTemplate.missionIconImagePath_Off : null) ?? string.Empty;
				if (!string.IsNullOrEmpty(text))
				{
					GameControl.assetLoader.LoadAssetForImageAssignment(text, this.CurrentMission);
					this.CurrentMission.enabled = true;
				}
				else
				{
					this.CurrentMission.enabled = false;
				}
			}
			else
			{
				this.CurrentMission.enabled = false;
			}
			this.CouncilorName.enabled = true;
			this.CouncilorProfession.enabled = true;
		}

		// Token: 0x060051EA RID: 20970 RVA: 0x00240140 File Offset: 0x0023E340
		public void OnCouncilorButtonClicked()
		{
			SoundEffectController.PlaySelectSound(this.councilor);
			TIUtilities.GotoGameState(this.councilor, true, true, true);
			if (GeneralControlsController.UIPlayerInTargetingMode && GeneralControlsController.CurrentlyTargetingStateType(typeof(TICouncilorState)))
			{
				GameControl.eventManager.TriggerEvent(new CouncilorMapItemSelected(this.councilor), null, Array.Empty<object>());
			}
		}

		// Token: 0x04003696 RID: 13974
		private TICouncilorState councilor;

		// Token: 0x04003697 RID: 13975
		public Image FactionImage;

		// Token: 0x04003698 RID: 13976
		public TMP_Text CouncilorName;

		// Token: 0x04003699 RID: 13977
		public TMP_Text CouncilorProfession;

		// Token: 0x0400369A RID: 13978
		public Image CurrentMission;
	}
}

using System;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008D9 RID: 2265
	public class SpaceDetailCouncilorsListItemController : MonoBehaviour
	{
		// Token: 0x060056AD RID: 22189 RVA: 0x0027A9F8 File Offset: 0x00278BF8
		public void SetListItem(TICouncilorState councilor, bool showProfession)
		{
			this.councilor = councilor;
			this.councilorView = new CouncilorView(councilor, GameControl.control.activePlayer);
			this.CouncilorName.SetText(this.councilorView.displayNameCurrent);
			if (showProfession)
			{
				this.CouncilorProfession.SetText(this.councilorView.councilorJobStringCurrent);
			}
			else
			{
				this.CouncilorProfession.SetText(this.councilorView.locationString(false));
			}
			if (this.councilorView.factionCurrent != null)
			{
				if (!string.IsNullOrEmpty(this.councilorView.factionIcon64Current))
				{
					this.FactionImage.enabled = true;
					GameControl.assetLoader.LoadAssetForImageAssignment(this.councilorView.factionIcon64Current, this.FactionImage);
				}
				else
				{
					this.FactionImage.enabled = false;
				}
			}
			else
			{
				this.FactionImage.sprite = null;
				this.FactionImage.enabled = false;
			}
			if (this.councilorView.HasMission)
			{
				TIMissionTemplate currentMissionTemplate = this.councilorView.currentMissionTemplate;
				if (string.IsNullOrEmpty(((currentMissionTemplate != null) ? currentMissionTemplate.missionIconImagePath_Off : null) ?? string.Empty))
				{
					this.CurrentMission.enabled = false;
					string text = "Missing Mission icon for ";
					TIMissionTemplate currentMissionTemplate2 = this.councilorView.currentMissionTemplate;
					Log.Warn(text + ((currentMissionTemplate2 != null) ? currentMissionTemplate2.displayName : null), Array.Empty<object>());
				}
				else
				{
					this.CurrentMission.enabled = true;
					GameControl.assetLoader.LoadAssetForImageAssignment(this.councilorView.currentMissionTemplate.missionIconImagePath_Off, this.CurrentMission);
				}
				this.CurrentMission.enabled = true;
				return;
			}
			this.CurrentMission.sprite = null;
			this.CurrentMission.enabled = false;
		}

		// Token: 0x060056AE RID: 22190 RVA: 0x0027ABA0 File Offset: 0x00278DA0
		public void SetListItem(TIOfficerState officer)
		{
			this.CouncilorName.SetText(officer.displayName);
			this.CouncilorProfession.SetText(officer.template.displayName);
			GameControl.assetLoader.LoadAssetForImageAssignment(officer.GetIconPath(), this.FactionImage);
			this.FactionImage.enabled = true;
			this.CurrentMission.sprite = null;
			this.CurrentMission.enabled = false;
		}

		// Token: 0x060056AF RID: 22191 RVA: 0x0027AC10 File Offset: 0x00278E10
		public void OnCouncilorButtonClicked()
		{
			if (this.councilor != null)
			{
				SoundEffectController.PlaySelectSound(this.councilor);
				if (this.councilor.faction == GameControl.control.activePlayer)
				{
					TIUtilities.GotoGameState(this.councilor, false, true, true);
					return;
				}
				TIUtilities.GotoGameState(this.councilorView, GameControl.control.activePlayer.HasIntelOnCouncilorLocation(this.councilor), false, true, true);
			}
		}

		// Token: 0x04003DD9 RID: 15833
		private TICouncilorState councilor;

		// Token: 0x04003DDA RID: 15834
		public Image FactionImage;

		// Token: 0x04003DDB RID: 15835
		public TMP_Text CouncilorName;

		// Token: 0x04003DDC RID: 15836
		public TMP_Text CouncilorProfession;

		// Token: 0x04003DDD RID: 15837
		public Image CurrentMission;

		// Token: 0x04003DDE RID: 15838
		private CouncilorView councilorView;
	}
}

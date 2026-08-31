using System;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200087A RID: 2170
	public class IntelCouncilorListItem : MonoBehaviour
	{
		// Token: 0x0600511A RID: 20762 RVA: 0x002371B8 File Offset: 0x002353B8
		public void Initialize(TICouncilorState councilor, IntelScreenController parentController)
		{
			this.councilor = councilor;
			this.parentController = parentController;
		}

		// Token: 0x0600511B RID: 20763 RVA: 0x002371C8 File Offset: 0x002353C8
		public void UpdateListItem()
		{
			CouncilorView viewofCouncilor = GameControl.control.activePlayer.GetViewofCouncilor(this.councilor);
			this.councilorName.SetText(viewofCouncilor.displayNameCurrent);
			this.councilorJob.SetText(viewofCouncilor.councilorJobStringCurrent);
			this.location.SetText(viewofCouncilor.associatedLocationString);
			string activeMissionIcon = viewofCouncilor.GetActiveMissionIcon();
			if (!string.IsNullOrEmpty(activeMissionIcon))
			{
				this.missionIcon.enabled = true;
				GameControl.assetLoader.LoadAssetForImageAssignment(activeMissionIcon, this.missionIcon);
			}
			else
			{
				this.missionIcon.enabled = false;
			}
			this.tooltip.SetDelegate("BodyText", () => this.councilor.VisibleSummary(GameControl.control.activePlayer));
			this.tooltip.enabled = viewofCouncilor.councilorJobCurrent != null;
		}

		// Token: 0x0600511C RID: 20764 RVA: 0x00237290 File Offset: 0x00235490
		public void OnIntelCouncilorListItemClicked()
		{
			if (!TIGameState.Valid(this.councilor) || this.councilor.faction == null || this.councilor.status == CouncilorStatus.Dead)
			{
				return;
			}
			if (GameControl.control.activePlayer.HasIntelOnCouncilorBasicData(this.councilor))
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
				this.parentController.CloseInfoScreen(false);
				TIUtilities.GotoGameState(GameControl.control.activePlayer.GetViewofCouncilor(this.councilor), true, false, true, true);
			}
		}

		// Token: 0x040034EF RID: 13551
		public TMP_Text councilorName;

		// Token: 0x040034F0 RID: 13552
		public TMP_Text councilorJob;

		// Token: 0x040034F1 RID: 13553
		public Image missionIcon;

		// Token: 0x040034F2 RID: 13554
		public TMP_Text location;

		// Token: 0x040034F3 RID: 13555
		public TooltipTrigger tooltip;

		// Token: 0x040034F4 RID: 13556
		private TICouncilorState councilor;

		// Token: 0x040034F5 RID: 13557
		private IntelScreenController parentController;
	}
}

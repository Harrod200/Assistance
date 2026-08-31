using System;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.UI;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000864 RID: 2148
	public class TargetingListItemController : MonoBehaviour
	{
		// Token: 0x06004FAA RID: 20394 RVA: 0x00226980 File Offset: 0x00224B80
		public void OnTargetingItemSelected()
		{
			CanvasManager existingManager = World.Active.GetExistingManager<CanvasManager>();
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			(existingManager.OperationCanvasController as OperationCanvasController).CloseForReverseSelection();
			(existingManager.CouncilorMissionController as CouncilorMissionCanvasController).ReverseSelectionTriggered(this.heldMission, this.heldCouncilor, this.heldTarget);
		}

		// Token: 0x06004FAB RID: 20395 RVA: 0x002269D4 File Offset: 0x00224BD4
		public void UpdateListItem(TIMissionTemplate mission, TICouncilorState councilor, string successChance, TIGameState target)
		{
			this.heldMission = mission;
			this.heldCouncilor = councilor;
			this.heldTarget = target;
			this.councilorName.SetText(councilor.displayName);
			StringBuilder stringBuilder = new StringBuilder(mission.displayName);
			if (mission.primaryResource != FactionResource.None)
			{
				stringBuilder.Append(" ").Append(TIUtilities.InlineResourceStr(mission.primaryResource));
			}
			this.missionName.SetText(stringBuilder);
			this.successChanceText.SetText(successChance);
			GameControl.assetLoader.LoadAssetForImageAssignment(councilor.iconResource, this.councilorImage);
			if (councilor.HasMission)
			{
				this.councilorBackgroundImage.color = Color.black;
			}
			else
			{
				this.councilorBackgroundImage.color = councilor.faction.template.color;
			}
			this.tooltip.SetImage("Icon", GameControl.assetLoader.LoadAsset<Sprite>(mission.missionIconImagePath_Off));
			this.tooltip.SetDelegate("BodyText", () => mission.description);
		}

		// Token: 0x04003311 RID: 13073
		public TMP_Text councilorName;

		// Token: 0x04003312 RID: 13074
		public TMP_Text missionName;

		// Token: 0x04003313 RID: 13075
		public TMP_Text successChanceText;

		// Token: 0x04003314 RID: 13076
		public Image councilorImage;

		// Token: 0x04003315 RID: 13077
		public Image councilorBackgroundImage;

		// Token: 0x04003316 RID: 13078
		public TooltipTrigger tooltip;

		// Token: 0x04003317 RID: 13079
		private TIMissionTemplate heldMission;

		// Token: 0x04003318 RID: 13080
		private TICouncilorState heldCouncilor;

		// Token: 0x04003319 RID: 13081
		private TIGameState heldTarget;
	}
}

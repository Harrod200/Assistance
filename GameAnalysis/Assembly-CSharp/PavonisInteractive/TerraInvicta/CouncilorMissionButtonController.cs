using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000842 RID: 2114
	public class CouncilorMissionButtonController : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x06004C72 RID: 19570 RVA: 0x002047D5 File Offset: 0x002029D5
		public void Init(CouncilorMissionCanvasController controller)
		{
			this.controller = controller;
		}

		// Token: 0x06004C73 RID: 19571 RVA: 0x002047E0 File Offset: 0x002029E0
		public void SetMissionData(TIMissionTemplate mission, TICouncilorState councilor)
		{
			this.missionType = mission;
			GameControl.assetLoader.LoadAssetForImageAssignment(mission.missionIconImagePath_Off, this.foregroundImage);
			this.interactable = mission.CanAfford(councilor.faction, councilor) && mission.target.GetValidTargets(mission, councilor).Count > 0;
			if (this.controller.forceAllowMissions)
			{
				this.interactable = true;
			}
			base.GetComponentInChildren<Button>().interactable = this.interactable;
			if (this.highlightImage != null)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(mission.missionIconImagePath_On, this.highlightImage);
				SpriteState spriteState = base.GetComponentInChildren<Button>().spriteState;
				spriteState.highlightedSprite = this.highlightImage.sprite;
				spriteState.pressedSprite = this.highlightImage.sprite;
				this.foregroundImage.color = (base.GetComponentInChildren<Button>().interactable ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.2f));
				spriteState.disabledSprite = this.foregroundImage.sprite;
				base.GetComponentInChildren<Button>().spriteState = spriteState;
			}
			if (this.interactable)
			{
				this.missionTT.enabled = false;
				return;
			}
			this.missionTT.enabled = true;
			this.missionTT.SetImage("Icon", this.foregroundImage.sprite);
			this.missionTT.SetDelegate("BodyText", () => this.NoMissionFeedback(councilor, mission, true));
		}

		// Token: 0x06004C74 RID: 19572 RVA: 0x002049C0 File Offset: 0x00202BC0
		private string NoMissionFeedback(TICouncilorState councilor, TIMissionTemplate mission, bool specific = true)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (specific)
			{
				List<string> list = new List<string>();
				bool flag = false;
				if (!mission.CanAfford(councilor.faction, null))
				{
					list.Add(Loc.T("UI.MissionPhase.CantAfford"));
					flag = true;
				}
				foreach (TIGameState tigameState in mission.target.GetAllPotentialTargets(councilor.faction))
				{
					List<string> list2 = mission.target.ValidateSingleTarget(mission, councilor, tigameState);
					list.AddRangeUnique<string>(list2);
				}
				list.Remove("_Pass");
				list.Remove("_Fail");
				if (list.Count == 0)
				{
					specific = false;
				}
				else
				{
					if (!flag)
					{
						stringBuilder.AppendLine(Loc.T("UI.MissionPhase.NoTargetsHelp"));
					}
					foreach (string text in list)
					{
						stringBuilder.Append("-").AppendLine(Loc.T(text));
					}
				}
			}
			if (!specific)
			{
				foreach (TIMissionCondition timissionCondition in mission.conditions)
				{
					foreach (string text2 in timissionCondition.feedback)
					{
						stringBuilder.Append("-").AppendLine(Loc.T(text2));
					}
				}
				if (stringBuilder.Length == 0)
				{
					stringBuilder.AppendLine(Loc.T("UI.MissionPhase.NoTargets"));
				}
				else
				{
					StringBuilder stringBuilder2 = new StringBuilder(Loc.T("UI.MissionPhase.NoTargetsHelp"));
					stringBuilder2.AppendLine().Append(stringBuilder);
					stringBuilder = stringBuilder2;
				}
				if (councilor.traits.Any<TITraitTemplate>((TITraitTemplate x) => x.restrictedLocations > RestrictedLocations.None))
				{
					stringBuilder.AppendLine().AppendLine(Loc.T("UI.MissionPhase.NoTargetLocation"));
					foreach (TITraitTemplate titraitTemplate in councilor.traits)
					{
						if (titraitTemplate.restrictedLocations != RestrictedLocations.None)
						{
							stringBuilder.Append("-").AppendLine(Loc.T("UI.MissionPhase.BadTrait", new object[]
							{
								titraitTemplate.displayName,
								TITraitTemplate.RestrictedLocationString(titraitTemplate)
							}));
						}
					}
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06004C75 RID: 19573 RVA: 0x00204C7C File Offset: 0x00202E7C
		public void OnButtonPressed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OptionSelect", false, false);
			this.controller.OnMissionSelected(this, null);
			GameControl.control.activePlayer.CompleteMilestone(CampaignMilestone.TutorialSelectMission);
		}

		// Token: 0x06004C76 RID: 19574 RVA: 0x00204CA8 File Offset: 0x00202EA8
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.interactable)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_HoverActionIcon", false, false);
			}
			if (this.controller != null)
			{
				this.controller.OnMissionPointerEnter(this);
			}
		}

		// Token: 0x06004C77 RID: 19575 RVA: 0x00204CD8 File Offset: 0x00202ED8
		public void OnPointerExit(PointerEventData eventData)
		{
			if (this.controller != null)
			{
				this.controller.OnMissionPointerExit(this);
			}
		}

		// Token: 0x04002E65 RID: 11877
		public TooltipTrigger missionTT;

		// Token: 0x04002E66 RID: 11878
		public Image foregroundImage;

		// Token: 0x04002E67 RID: 11879
		public Image highlightImage;

		// Token: 0x04002E68 RID: 11880
		public bool interactable;

		// Token: 0x04002E69 RID: 11881
		private CouncilorMissionCanvasController controller;

		// Token: 0x04002E6A RID: 11882
		public TIMissionTemplate missionType;
	}
}

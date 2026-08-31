using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000848 RID: 2120
	public class TargetOrgListItemController : MonoBehaviour
	{
		// Token: 0x17000EAB RID: 3755
		// (get) Token: 0x06004CFE RID: 19710 RVA: 0x0020B516 File Offset: 0x00209716
		// (set) Token: 0x06004CFF RID: 19711 RVA: 0x0020B51E File Offset: 0x0020971E
		public float toHitValue { get; private set; }

		// Token: 0x06004D00 RID: 19712 RVA: 0x0020B528 File Offset: 0x00209728
		public void SetListItem(TargetOrgListItem_Data data)
		{
			this.controller = data.controller;
			this.org = data.org;
			this.selectButtonText.SetText(data.selectButtonText);
			if (data.targetingCouncilor != null && data.missionTemplate != null && data.validTarget)
			{
				this.selectButton.interactable = true;
				this.toHitValue = data.toHitValue;
				this.successChance.SetText(data.successChance);
				this.warningIcon.enabled = !data.targetingCouncilor.CanAddExternalOrgValidatedForFaction(this.org);
			}
			else
			{
				this.toHitValue = -1f;
				this.selectButton.interactable = false;
				this.successChance.SetText(string.Empty);
				this.warningIcon.enabled = false;
			}
			this.orgName.SetText(data.orgName);
			this.orgIcon.sprite = data.orgIcon;
			this.orgIcon.enabled = true;
			this.orgDescription.SetDelegate("BodyText", () => this.org.description(true, GameControl.control.activePlayer, true, true));
			this.orgDescription.SetImage("Icon", this.orgIcon.sprite);
			this.orgDescription.enabled = true;
			this.tier.SetText(data.tier);
			if (this.org.assignedCouncilor != null)
			{
				this.owningCouncilorForeground.sprite = data.owningCouncilorForeground;
				this.owningCouncilorBackground.color = data.owningCouncilorBackgroundColor;
				this.owningCouncilorForeground.enabled = true;
				this.owningCouncilorBackground.enabled = true;
			}
			else
			{
				this.owningCouncilorForeground.sprite = data.owningCouncilorForeground;
				this.owningCouncilorForeground.enabled = true;
				this.owningCouncilorBackground.enabled = false;
			}
			this.persuasion.SetText(data.persuasion);
			this.investigation.SetText(data.investigation);
			this.espionage.SetText(data.espionage);
			this.command.SetText(data.command);
			this.administration.SetText(data.administration);
			this.science.SetText(data.science);
			this.security.SetText(data.security);
			this.money.SetText(data.money);
			this.influence.SetText(data.influence);
			this.ops.SetText(data.ops);
			this.research.SetText(data.research);
			this.boost.SetText(data.boost);
			this.missionControl.SetText(data.missionControl);
			this.projects.SetText(data.projects);
			this.priority_ECO.SetText(data.priority_ECO);
			this.priority_WEL.SetText(data.priority_WEL);
			this.priority_ENV.SetText(data.priority_ENV);
			this.priority_KNO.SetText(data.priority_KNO);
			this.priority_GOV.SetText(data.priority_GOV);
			this.priority_UNI.SetText(data.priority_UNI);
			this.priority_MIL.SetText(data.priority_MIL);
			this.priority_OPP.SetText(data.priority_OPP);
			this.priority_FUN.SetText(data.priority_FUN);
			this.priority_SPO.SetText(data.priority_SPO);
			this.priority_FLI.SetText(data.priority_FLI);
			this.priority_MC.SetText(data.priority_MC);
			this.miningBonus.SetText(data.miningBonus);
			this.techBonus.SetText(data.techBonus);
			this.missionIcons.SetListSize<OrgListMissionIconController>(this.org.missionsGranted.Count, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = this.missionIcons.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (TargetOrgListItemController.<>o__47.<>p__0 == null)
					{
						TargetOrgListItemController.<>o__47.<>p__0 = CallSite<Func<CallSite, object, OrgListMissionIconController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(OrgListMissionIconController), typeof(TargetOrgListItemController)));
					}
					TargetOrgListItemController.<>o__47.<>p__0.Target(TargetOrgListItemController.<>o__47.<>p__0, enumerator.Current).SetListItem(this.org.missionsGranted[num++]);
				}
			}
			this.missionsTip.SetDelegate("BodyText", delegate
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < this.org.missionsGranted.Count; i++)
				{
					stringBuilder.AppendLine(this.org.missionsGranted[i].displayName);
					if (i < this.org.missionsGranted.Count - 1)
					{
						stringBuilder.AppendLine();
					}
				}
				return stringBuilder.ToString();
			});
			base.gameObject.SetActive(true);
		}

		// Token: 0x06004D01 RID: 19713 RVA: 0x0020B9B0 File Offset: 0x00209BB0
		public void TurnOffListItem()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x06004D02 RID: 19714 RVA: 0x0020B9C0 File Offset: 0x00209BC0
		public void OnSelectButtonPressed()
		{
			SoundEffectController.PlaySelectSound(this.org);
			GameControl.eventManager.TriggerEvent(new OrgSelectedEvent(this.org), null, Array.Empty<object>());
			if (this.org.assignedCouncilor != null)
			{
				this.controller.CouncilorSelected(this.org.assignedCouncilor);
			}
			if (this.org.homeRegion != null)
			{
				TIUtilities.GotoGameState(this.org.homeRegion, true, false, false, false, false, -1f);
			}
		}

		// Token: 0x04002F4C RID: 12108
		private CouncilorMissionCanvasController controller;

		// Token: 0x04002F4D RID: 12109
		public Button selectButton;

		// Token: 0x04002F4E RID: 12110
		public TMP_Text selectButtonText;

		// Token: 0x04002F4F RID: 12111
		public TMP_Text successChance;

		// Token: 0x04002F50 RID: 12112
		public Image warningIcon;

		// Token: 0x04002F51 RID: 12113
		public Image owningCouncilorBackground;

		// Token: 0x04002F52 RID: 12114
		public Image owningCouncilorForeground;

		// Token: 0x04002F53 RID: 12115
		public Image orgIcon;

		// Token: 0x04002F54 RID: 12116
		public TMP_Text tier;

		// Token: 0x04002F55 RID: 12117
		public TMP_Text orgName;

		// Token: 0x04002F56 RID: 12118
		public TooltipTrigger orgDescription;

		// Token: 0x04002F57 RID: 12119
		public TMP_Text persuasion;

		// Token: 0x04002F58 RID: 12120
		public TMP_Text investigation;

		// Token: 0x04002F59 RID: 12121
		public TMP_Text espionage;

		// Token: 0x04002F5A RID: 12122
		public TMP_Text command;

		// Token: 0x04002F5B RID: 12123
		public TMP_Text administration;

		// Token: 0x04002F5C RID: 12124
		public TMP_Text science;

		// Token: 0x04002F5D RID: 12125
		public TMP_Text security;

		// Token: 0x04002F5E RID: 12126
		public TMP_Text money;

		// Token: 0x04002F5F RID: 12127
		public TMP_Text influence;

		// Token: 0x04002F60 RID: 12128
		public TMP_Text ops;

		// Token: 0x04002F61 RID: 12129
		public TMP_Text research;

		// Token: 0x04002F62 RID: 12130
		public TMP_Text boost;

		// Token: 0x04002F63 RID: 12131
		public TMP_Text missionControl;

		// Token: 0x04002F64 RID: 12132
		public TMP_Text projects;

		// Token: 0x04002F65 RID: 12133
		public TMP_Text priority_ECO;

		// Token: 0x04002F66 RID: 12134
		public TMP_Text priority_WEL;

		// Token: 0x04002F67 RID: 12135
		public TMP_Text priority_ENV;

		// Token: 0x04002F68 RID: 12136
		public TMP_Text priority_KNO;

		// Token: 0x04002F69 RID: 12137
		public TMP_Text priority_GOV;

		// Token: 0x04002F6A RID: 12138
		public TMP_Text priority_UNI;

		// Token: 0x04002F6B RID: 12139
		public TMP_Text priority_MIL;

		// Token: 0x04002F6C RID: 12140
		public TMP_Text priority_OPP;

		// Token: 0x04002F6D RID: 12141
		public TMP_Text priority_FUN;

		// Token: 0x04002F6E RID: 12142
		public TMP_Text priority_SPO;

		// Token: 0x04002F6F RID: 12143
		public TMP_Text priority_FLI;

		// Token: 0x04002F70 RID: 12144
		public TMP_Text priority_MC;

		// Token: 0x04002F71 RID: 12145
		public TMP_Text miningBonus;

		// Token: 0x04002F72 RID: 12146
		public TMP_Text techBonus;

		// Token: 0x04002F73 RID: 12147
		public ListManagerBase missionIcons;

		// Token: 0x04002F74 RID: 12148
		public HorizontalLayoutGroup missionIconsGroup;

		// Token: 0x04002F75 RID: 12149
		public TooltipTrigger missionsTip;

		// Token: 0x04002F76 RID: 12150
		private TIOrgState org;
	}
}

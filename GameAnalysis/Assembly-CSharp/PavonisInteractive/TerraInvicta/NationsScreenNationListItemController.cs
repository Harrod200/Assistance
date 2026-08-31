using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200089C RID: 2204
	public class NationsScreenNationListItemController : MonoBehaviour
	{
		// Token: 0x17000EF1 RID: 3825
		// (get) Token: 0x0600532A RID: 21290 RVA: 0x0024F80F File Offset: 0x0024DA0F
		private TIFactionState activePlayer
		{
			get
			{
				return this.controller.activePlayer;
			}
		}

		// Token: 0x0600532B RID: 21291 RVA: 0x0024F81C File Offset: 0x0024DA1C
		public void SetGameState(TIGameState targetState)
		{
			if (targetState == null)
			{
				this.nation = null;
				this.controlPoint = null;
				return;
			}
			this.controlPoint = targetState as TIControlPoint;
			if (this.controlPoint != null)
			{
				this.nation = this.controlPoint.nation;
				return;
			}
			this.nation = targetState as TINationState;
		}

		// Token: 0x0600532C RID: 21292 RVA: 0x0024F879 File Offset: 0x0024DA79
		public void UpdateListItem()
		{
		}

		// Token: 0x0600532D RID: 21293 RVA: 0x0024F87C File Offset: 0x0024DA7C
		public void UpdateNationItem(NationsScreenNationListItem_Data data)
		{
			this.nation = data.nation;
			this.controller = data.controller;
			this.nationName.SetText(data.nationName);
			this.nationFlag.sprite = this.nation.flag;
			if (this.nation.inFederation)
			{
				this.federationName.SetText(data.federationName);
				GameControl.assetLoader.LoadAssetForImageAssignment(this.nation.federation.flagResource, this.federationFlag);
				this.federationObject.SetActive(true);
			}
			else
			{
				this.federationObject.SetActive(false);
			}
			for (int i = 0; i <= 5; i++)
			{
				if (i <= this.nation.maxControlPointIndex)
				{
					TIControlPoint ticontrolPoint = this.nation.GetControlPoint(i);
					this.CPImage[i].sprite = ticontrolPoint.GetIcon(true, false);
					this.CPImage[i].color = (ticontrolPoint.owned ? Color.white : this.nation.template.UIColor);
					this.CPImage[i].gameObject.SetActive(true);
					this.CPImage[i].enabled = true;
					this.CPCrackdownImage[i].SetActive(ticontrolPoint.benefitsDisabled);
					this.CPDefendedImage[i].SetActive(ticontrolPoint.defended);
				}
				else
				{
					this.CPImage[i].sprite = null;
					this.CPImage[i].enabled = false;
					this.CPImage[i].gameObject.SetActive(false);
					this.CPCrackdownImage[i].SetActive(false);
					this.CPDefendedImage[i].SetActive(false);
				}
			}
			if (data.mostPopularFactionForIcon != null)
			{
				this.mostPopularFactionIcon.sprite = data.mostPopularFactionForIcon.factionIcon64UI;
				this.mostPopularFactionIcon.enabled = true;
			}
			else
			{
				this.mostPopularFactionIcon.enabled = false;
			}
			this.mostPopularFactionValue.SetText(data.mostPopularFactionValue);
			this.myFactionIcon.sprite = this.activePlayer.factionIcon64UI;
			this.myFactionValue.SetText(data.myFactionValue);
			this.population.SetText(data.population);
			GameControl.assetLoader.LoadAssetForImageAssignment(this.nation.SustainabilityIcon(), this.sustainabilityIcon);
			this.sustainability.SetText(data.sustainability);
			this.investmentPoints.SetText(data.investmentPoints);
			this.government.SetText(data.government);
			this.education.SetText(data.education);
			this.inequality.SetText(data.inequality);
			this.cohesion.SetText(data.cohesion);
			this.unrest.SetText(data.unrest);
			this.funding.SetText(data.funding);
			this.research.SetText(data.research);
			this.boost.SetText(data.boost);
			this.missionControl.SetText(data.missionControl);
			GameControl.assetLoader.LoadAssetForImageAssignment(this.nation.atWar ? TemplateManager.global.pathWarIcon : TemplateManager.global.pathPeaceIcon, this.warPeaceIcon);
			this.miltech.SetText(data.miltech);
			this.nukes.SetText(data.nukes);
			this.armies.SetText(this.nation.armies.Count.ToString("N0"));
			this.navies.SetText(data.navies);
			this.stoFighters.SetText(data.stoFighters);
			this.stoFighters.enabled = this.controller.canViewSTOFighters;
			if (this.nation.FactionHasControlPoint(GameControl.control.activePlayer))
			{
				this.controller.suppressDropDownAudio = true;
				NationInfoController.PopulateNationPriorityDropdown(this.nationPriorityPresetDropdown, this.nation, this.activePlayer, ref this.priorityPresetDictionary);
				this.controller.suppressDropDownAudio = false;
				this.nationPriorityPresetDropdown.gameObject.SetActive(true);
			}
			else
			{
				this.nationPriorityPresetDropdown.gameObject.SetActive(false);
			}
			this.federationTT.SetDelegate("BodyText", () => NationInfoController.BuildSpecialRelationshipTooltip(this.nation));
			string popularityTT = NationInfoController.BuildPublicOpinionTooltip(this.nation);
			this.mostPopularFactionValueTT.SetDelegate("BodyText", () => popularityTT);
			this.myFactionValueTT.SetDelegate("BodyText", () => popularityTT);
			this.populationTT.SetDelegate("BodyText", () => NationInfoController.BuildPopulationTooltip(this.nation));
			this.sustainabilityTT.SetDelegate("BodyText", () => NationInfoController.BuildSustainabilityTooltip(this.nation));
			this.investmentPointsTT.SetDelegate("BodyText", () => NationInfoController.BuildInvestmentTooltip(this.nation));
			this.perCapitaGDPTT.SetDelegate("BodyText", () => NationInfoController.BuildPerCapitaGDPTooltip(this.nation));
			this.governmentTT.SetDelegate("BodyText", () => NationInfoController.BuildDemocracyTooltip(this.nation));
			this.educationTT.SetDelegate("BodyText", () => NationInfoController.BuildEducationTooltip(this.nation));
			this.inequalityTT.SetDelegate("BodyText", () => NationInfoController.BuildInequalityTooltip(this.nation));
			this.cohesionTT.SetDelegate("BodyText", () => NationInfoController.BuildCohesionTooltip(this.nation));
			this.unrestTT.SetDelegate("BodyText", () => NationInfoController.BuildUnrestTooltip(this.nation));
			this.fundingTT.SetDelegate("BodyText", () => NationInfoController.BuildSpaceFundingTooltip(this.nation));
			this.researchTT.SetDelegate("BodyText", () => NationInfoController.BuildResearchTooltip(this.nation));
			this.boostTT.SetDelegate("BodyText", () => NationInfoController.BuildBoostTooltip(this.nation));
			this.missionControlTT.SetDelegate("BodyText", () => NationInfoController.BuildMissionControlTooltip(this.nation));
			this.warPeaceIconTT.SetDelegate("BodyText", () => this.BuildRelationsTooltip());
			this.policiesTT.SetDelegate("BodyText", () => NationInfoController.BuildPoliciesTooltip(this.nation, true));
			this.miltechTT.SetDelegate("BodyText", () => NationInfoController.BuildMiltechTooltip(this.nation));
			this.armiesTT.SetDelegate("BodyText", () => Loc.T("UI.Nations.Armies", new object[] { this.nation.armies.Count.ToString("N0") }));
			this.naviesTT.SetDelegate("BodyText", () => Loc.T("UI.Nation.NavalDetail"));
			this.stoFightersTT.SetDelegate("BodyText", () => Loc.T("UI.Nation.numSTOTip", new object[]
			{
				TIGlobalConfig.globalConfig.boostInlineSpritePath,
				7,
				14,
				4f
			}));
			this.nuclearWeaponsTT.SetDelegate("BodyText", () => NationInfoController.BuildNukesTooltip(this.nation));
			for (int j = 0; j <= this.nation.maxControlPointIndex; j++)
			{
				TIControlPoint controlPoint = this.nation.GetControlPoint(j);
				this.controlPointTT[j].SetDelegate("BodyText", () => this.SetControlPointTip(this.nation, controlPoint));
			}
		}

		// Token: 0x0600532E RID: 21294 RVA: 0x0024FFCC File Offset: 0x0024E1CC
		public string BuildRelationsTooltip()
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			if (this.nation.wars.Count > 0)
			{
				stringBuilder.AppendLine(Loc.T("UI.Nations.Wars"));
				foreach (TINationState tinationState in this.nation.wars)
				{
					stringBuilder.Append(" ").AppendLine(tinationState.displayName);
				}
				stringBuilder.AppendLine().AppendLine();
				flag = true;
			}
			if (this.nation.allies.Count > 0)
			{
				stringBuilder.AppendLine(Loc.T("UI.Nations.Allies"));
				foreach (TINationState tinationState2 in this.nation.allies)
				{
					stringBuilder.Append(" ").AppendLine(tinationState2.displayName);
				}
				stringBuilder.AppendLine().AppendLine();
				flag = true;
			}
			if (this.nation.rivals.Count > 0)
			{
				stringBuilder.AppendLine(Loc.T("UI.Nations.Rivals"));
				foreach (TINationState tinationState3 in this.nation.rivals)
				{
					stringBuilder.Append(" ").AppendLine(tinationState3.displayName);
				}
				stringBuilder.AppendLine();
				flag = true;
			}
			if (!flag)
			{
				return Loc.T("UI.Nation.AtPeace");
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600532F RID: 21295 RVA: 0x00250194 File Offset: 0x0024E394
		private string SetControlPointTip(TINationState nation, TIControlPoint controlPoint)
		{
			return NationInfoController.ControlPointTooltip(nation, controlPoint);
		}

		// Token: 0x06005330 RID: 21296 RVA: 0x002501A0 File Offset: 0x0024E3A0
		public void OnClickNationListItemController()
		{
			if (this.nationLine)
			{
				this.controller.nationOpenedStatus[this] = !this.controller.nationOpenedStatus[this];
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
				foreach (NationsScreenNationListItemController nationsScreenNationListItemController in this.controller.nationItemDictionary.Keys)
				{
					if (!nationsScreenNationListItemController.nationLine && nationsScreenNationListItemController.nation == this.nation && nationsScreenNationListItemController.controlPoint != null && (this.controller.filterFaction == null || nationsScreenNationListItemController.controlPoint.faction == this.controller.filterFaction))
					{
						nationsScreenNationListItemController.gameObject.SetActive(this.controller.nationOpenedStatus[this]);
						if (nationsScreenNationListItemController.gameObject.activeSelf)
						{
							nationsScreenNationListItemController.UpdateListItem();
						}
					}
				}
			}
		}

		// Token: 0x06005331 RID: 21297 RVA: 0x002502C4 File Offset: 0x0024E4C4
		public void OnNationPriorityTemplateChanged()
		{
			if (!this.controller.suppressDropDownAudio)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", true, false);
			}
			using (IEnumerator<TIPriorityPresetTemplate> enumerator = TemplateManager.IterateByClass<TIPriorityPresetTemplate>(true).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.displayName == this.nationPriorityPresetDropdown.options[this.nationPriorityPresetDropdown.value].text)
					{
						TIPriorityPresetTemplate key = this.priorityPresetDictionary.FirstOrDefault<KeyValuePair<TIPriorityPresetTemplate, int>>((KeyValuePair<TIPriorityPresetTemplate, int> x) => x.Value == this.nationPriorityPresetDropdown.value).Key;
						using (List<TIControlPoint>.Enumerator enumerator2 = this.nation.controlPoints.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								TIControlPoint ticontrolPoint = enumerator2.Current;
								if (ticontrolPoint.faction == this.activePlayer)
								{
									PlayerAction playerAction = new ApplyPriorityPresetToControlPoint(ticontrolPoint, ticontrolPoint.faction, key.dataName);
									this.activePlayer.playerControl.StartAction(playerAction);
								}
							}
							break;
						}
					}
				}
			}
			NationInfoController.UpdatePriorityPresetFromChanges(this.nationPriorityPresetDropdown, this.nation, this.priorityPresetDictionary);
		}

		// Token: 0x06005332 RID: 21298 RVA: 0x0025040C File Offset: 0x0024E60C
		public void OnSyncPrioritiesButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.controlPoint.nation.SyncAllPriorites(this.controlPoint.positionInNation);
			foreach (NationsScreenNationListItemController nationsScreenNationListItemController in this.controller.GetControlPointLinesForNation(this.nation))
			{
				if (nationsScreenNationListItemController.controlPoint.faction == this.activePlayer)
				{
					nationsScreenNationListItemController.UpdateListItem();
				}
			}
		}

		// Token: 0x06005333 RID: 21299 RVA: 0x002504A8 File Offset: 0x0024E6A8
		public void OnGotoNationClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			TIUtilities.GotoGameState(this.nation, true, true, true, true, false, -1f);
		}

		// Token: 0x06005334 RID: 21300 RVA: 0x002504CB File Offset: 0x0024E6CB
		public void OnCustomPriorityPresetsChanged(CustomPriorityPresetsChanged e)
		{
			if (this.nationPriorityPresetDropdown.gameObject.activeSelf)
			{
				NationInfoController.PopulateNationPriorityDropdown(this.nationPriorityPresetDropdown, this.nation, this.activePlayer, ref this.priorityPresetDictionary);
			}
		}

		// Token: 0x04003853 RID: 14419
		[HideInInspector]
		public NationsScreenController controller;

		// Token: 0x04003854 RID: 14420
		[HideInInspector]
		public TIControlPoint controlPoint;

		// Token: 0x04003855 RID: 14421
		[HideInInspector]
		public TINationState nation;

		// Token: 0x04003856 RID: 14422
		[HideInInspector]
		public bool nationLine;

		// Token: 0x04003857 RID: 14423
		private int selectedPresetValue = -1;

		// Token: 0x04003858 RID: 14424
		private bool customPresetInList;

		// Token: 0x04003859 RID: 14425
		private Dictionary<TIPriorityPresetTemplate, int> priorityPresetDictionary;

		// Token: 0x0400385A RID: 14426
		public CanvasGroup canvasGroup;

		// Token: 0x0400385B RID: 14427
		public LayoutElement layoutElement;

		// Token: 0x0400385C RID: 14428
		[Header("Nation Data")]
		public GameObject nationLineObject;

		// Token: 0x0400385D RID: 14429
		public TMP_Text nationName;

		// Token: 0x0400385E RID: 14430
		public GameObject federationObject;

		// Token: 0x0400385F RID: 14431
		public Image federationFlag;

		// Token: 0x04003860 RID: 14432
		public TMP_Text federationName;

		// Token: 0x04003861 RID: 14433
		public Image nationFlag;

		// Token: 0x04003862 RID: 14434
		public Image[] CPImage;

		// Token: 0x04003863 RID: 14435
		public GameObject[] CPCrackdownImage;

		// Token: 0x04003864 RID: 14436
		public GameObject[] CPDefendedImage;

		// Token: 0x04003865 RID: 14437
		public Image mostPopularFactionIcon;

		// Token: 0x04003866 RID: 14438
		public TMP_Text mostPopularFactionValue;

		// Token: 0x04003867 RID: 14439
		public Image myFactionIcon;

		// Token: 0x04003868 RID: 14440
		public Image sustainabilityIcon;

		// Token: 0x04003869 RID: 14441
		public TMP_Text myFactionValue;

		// Token: 0x0400386A RID: 14442
		public TMP_Text population;

		// Token: 0x0400386B RID: 14443
		public TMP_Text investmentPoints;

		// Token: 0x0400386C RID: 14444
		public TMP_Text perCapitaGDP;

		// Token: 0x0400386D RID: 14445
		public TMP_Text sustainability;

		// Token: 0x0400386E RID: 14446
		public TMP_Text government;

		// Token: 0x0400386F RID: 14447
		public TMP_Text education;

		// Token: 0x04003870 RID: 14448
		public TMP_Text inequality;

		// Token: 0x04003871 RID: 14449
		public TMP_Text cohesion;

		// Token: 0x04003872 RID: 14450
		public TMP_Text unrest;

		// Token: 0x04003873 RID: 14451
		public TMP_Text funding;

		// Token: 0x04003874 RID: 14452
		public TMP_Text research;

		// Token: 0x04003875 RID: 14453
		public TMP_Text boost;

		// Token: 0x04003876 RID: 14454
		public TMP_Text missionControl;

		// Token: 0x04003877 RID: 14455
		public Image warPeaceIcon;

		// Token: 0x04003878 RID: 14456
		public TMP_Text miltech;

		// Token: 0x04003879 RID: 14457
		public TMP_Text armies;

		// Token: 0x0400387A RID: 14458
		public TMP_Text navies;

		// Token: 0x0400387B RID: 14459
		public TMP_Text stoFighters;

		// Token: 0x0400387C RID: 14460
		public TMP_Text nukes;

		// Token: 0x0400387D RID: 14461
		public TMP_Dropdown nationPriorityPresetDropdown;

		// Token: 0x0400387E RID: 14462
		public TooltipTrigger federationTT;

		// Token: 0x0400387F RID: 14463
		public TooltipTrigger mostPopularFactionValueTT;

		// Token: 0x04003880 RID: 14464
		public TooltipTrigger myFactionValueTT;

		// Token: 0x04003881 RID: 14465
		public TooltipTrigger populationTT;

		// Token: 0x04003882 RID: 14466
		public TooltipTrigger investmentPointsTT;

		// Token: 0x04003883 RID: 14467
		public TooltipTrigger perCapitaGDPTT;

		// Token: 0x04003884 RID: 14468
		public TooltipTrigger sustainabilityTT;

		// Token: 0x04003885 RID: 14469
		public TooltipTrigger governmentTT;

		// Token: 0x04003886 RID: 14470
		public TooltipTrigger educationTT;

		// Token: 0x04003887 RID: 14471
		public TooltipTrigger inequalityTT;

		// Token: 0x04003888 RID: 14472
		public TooltipTrigger cohesionTT;

		// Token: 0x04003889 RID: 14473
		public TooltipTrigger unrestTT;

		// Token: 0x0400388A RID: 14474
		public TooltipTrigger fundingTT;

		// Token: 0x0400388B RID: 14475
		public TooltipTrigger researchTT;

		// Token: 0x0400388C RID: 14476
		public TooltipTrigger boostTT;

		// Token: 0x0400388D RID: 14477
		public TooltipTrigger missionControlTT;

		// Token: 0x0400388E RID: 14478
		public TooltipTrigger warPeaceIconTT;

		// Token: 0x0400388F RID: 14479
		public TooltipTrigger miltechTT;

		// Token: 0x04003890 RID: 14480
		public TooltipTrigger armiesTT;

		// Token: 0x04003891 RID: 14481
		public TooltipTrigger naviesTT;

		// Token: 0x04003892 RID: 14482
		public TooltipTrigger stoFightersTT;

		// Token: 0x04003893 RID: 14483
		public TooltipTrigger nuclearWeaponsTT;

		// Token: 0x04003894 RID: 14484
		public TooltipTrigger[] controlPointTT;

		// Token: 0x04003895 RID: 14485
		public TooltipTrigger policiesTT;
	}
}

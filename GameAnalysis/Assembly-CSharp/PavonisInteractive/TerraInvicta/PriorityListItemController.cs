using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000895 RID: 2197
	public class PriorityListItemController : MonoBehaviour
	{
		// Token: 0x17000EEF RID: 3823
		// (get) Token: 0x060052F0 RID: 21232 RVA: 0x0024C343 File Offset: 0x0024A543
		// (set) Token: 0x060052F1 RID: 21233 RVA: 0x0024C34B File Offset: 0x0024A54B
		public TINationState nation { get; private set; }

		// Token: 0x17000EF0 RID: 3824
		// (get) Token: 0x060052F2 RID: 21234 RVA: 0x0024C354 File Offset: 0x0024A554
		// (set) Token: 0x060052F3 RID: 21235 RVA: 0x0024C35C File Offset: 0x0024A55C
		public PriorityType priority { get; private set; }

		// Token: 0x060052F4 RID: 21236 RVA: 0x0024C365 File Offset: 0x0024A565
		public void Init(NationInfoController controller, PriorityType priority)
		{
			this.controller = controller;
			this.priority = priority;
		}

		// Token: 0x060052F5 RID: 21237 RVA: 0x0024C375 File Offset: 0x0024A575
		public static Sprite prioritySettingSprite(int weight)
		{
			return NationInfoController.weightSprite[weight];
		}

		// Token: 0x060052F6 RID: 21238 RVA: 0x0024C380 File Offset: 0x0024A580
		public static string priorityAccumulationStr(TINationState nation, PriorityType priority)
		{
			return Loc.T("UI.NationPriorityAccumulation", new object[]
			{
				(nation.GetAccumulatedInvestmentPoints(priority) > 0f) ? TIUtilities.FormatSmallNumber(nation.GetAccumulatedInvestmentPoints(priority), 2, 0, true, false) : "-",
				TIUtilities.FormatSmallNumber(nation.GetRequiredInvestmentPointsForPriority(priority), 2, 0, true, false)
			});
		}

		// Token: 0x060052F7 RID: 21239 RVA: 0x0024C3D8 File Offset: 0x0024A5D8
		public static string colorizePriorityStr(TINationState nation, PriorityType priority, string inputString)
		{
			if (priority != PriorityType.Welfare)
			{
				if (priority != PriorityType.Oppression)
				{
					if (priority == PriorityType.Spoils)
					{
						if (!nation.elitesHappy)
						{
							inputString = new StringBuilder(TIUtilities.YellowLine(inputString)).Append(TemplateManager.global.warningInlineSpritePath).ToString();
						}
					}
				}
				else if (nation.unrestMajorWarning)
				{
					inputString = new StringBuilder(TIUtilities.YellowLine(inputString)).Append(TemplateManager.global.warningInlineSpritePath).ToString();
				}
			}
			else if (nation.inequalityWarning)
			{
				inputString = new StringBuilder(TIUtilities.YellowLine(inputString)).Append(TemplateManager.global.warningInlineSpritePath).ToString();
			}
			return inputString;
		}

		// Token: 0x060052F8 RID: 21240 RVA: 0x0024C474 File Offset: 0x0024A674
		public static string priorityTipStr(TIFactionState faction, TINationState nation, PriorityType priority, string priorityLine)
		{
			string text = new StringBuilder("UI.Nation.").Append(priority.ToString()).Append("PriorityTip").ToString();
			string[] array = new string[0];
			switch (priority)
			{
			case PriorityType.Economy:
				array = new string[]
				{
					nation.economyPriorityPerCapitaIncomeChange.ToString("N2"),
					TIUtilities.FormatSmallNumber(nation.economyPriorityInequalityChange, 7, 1, true, false)
				};
				break;
			case PriorityType.Welfare:
				array = new string[] { TIUtilities.FormatSmallNumber(nation.welfarePriorityInequalityChange * -1f, 7, 1, true, false) };
				break;
			case PriorityType.Environment:
				array = new string[]
				{
					nation.SustainabilityChangeForDisplay(nation.environmentPrioritySustainabilityChange),
					nation.BestCurrentSustainabilityValueForDisplay(),
					TIUtilities.FormatSmallNumber(nation.EnvPriorityCO2Removed(), 7, 0, true, false),
					TIUtilities.FormatSmallNumber(nation.EnvPriorityCH4Removed(), 7, 0, true, false),
					TIUtilities.FormatSmallNumber(nation.EnvPriorityN2ORemoved(), 7, 0, true, false)
				};
				break;
			case PriorityType.Knowledge:
				array = new string[]
				{
					TIUtilities.FormatSmallNumber(nation.knowledgePriorityEducationChange, 7, 1, true, false),
					TIUtilities.FormatSmallNumber(nation.knowledgePriorityCohesionChange, 7, 1, true, false),
					TIUtilities.FormatSmallNumber(8.5f, 7, 0, true, false),
					TIUtilities.FormatSmallNumber(12f, 7, 0, true, false)
				};
				break;
			case PriorityType.Government:
				array = new string[] { TIUtilities.FormatSmallNumber(nation.governmentPriorityDemocracyChange, 7, 1, true, false) };
				break;
			case PriorityType.Unity:
				array = new string[]
				{
					TIUtilities.FormatSmallNumber(nation.unityPriorityCohesionChange, 7, 1, true, false),
					TIUtilities.FormatSmallNumber(nation.unityPriorityEducationChange, 7, 1, true, false)
				};
				break;
			case PriorityType.Oppression:
				if (nation.OppressionPriorityCohesionChange != 0f)
				{
					text = new StringBuilder(text).Append("1").ToString();
				}
				array = new string[]
				{
					TIUtilities.FormatSmallNumber(nation.OppressionPriorityUnrestChange, 7, 1, true, false),
					TIUtilities.FormatSmallNumber(nation.OppressionPriorityDemocracyChange, 7, 1, true, false),
					TIUtilities.FormatSmallNumber(5f, 7, 0, true, false),
					TIUtilities.FormatSmallNumber(nation.OppressionPriorityCohesionChange, 7, 1, true, false)
				};
				break;
			case PriorityType.Funding:
				array = new string[] { new StringBuilder(TemplateManager.global.moneyInlineSpritePath).Append(nation.spaceFundingPriorityIncomeChange.ToString()).ToString() };
				break;
			case PriorityType.Spoils:
				array = new string[]
				{
					new StringBuilder(TemplateManager.global.moneyInlineSpritePath).Append(TIUtilities.FormatSmallNumber(nation.spoilsPriorityMoneyPerControlPoint, 7, 0, true, false)).ToString(),
					TIUtilities.FormatSmallNumber(nation.spoilsPriorityInequalityChange, 7, 1, true, false),
					nation.SustainabilityChangeForDisplay(nation.spoilsSustainabilityChange),
					TIUtilities.FormatSmallNumber(nation.spoilsPriorityDemocracyChange, 7, 0, true, false),
					TIUtilities.RedLine(nation.corruption.ToPercent("P0"))
				};
				break;
			case PriorityType.Military:
				array = new string[] { TIUtilities.FormatSmallNumber(nation.militaryPriorityTechLevelChange, 7, 1, true, false) };
				break;
			case PriorityType.Military_BuildArmy:
				array = new string[]
				{
					TemplateManager.global.minPopulationForFirstArmy_millions.ToString(),
					TemplateManager.global.minPopulationForAdditionalArmiesPer_millions.ToString()
				};
				break;
			case PriorityType.Military_BuildNavy:
				array = new string[]
				{
					TemplateManager.global.minControlPointsForNavy.ToString(),
					TemplateManager.global.minControlPointsForNavyException.ToString(),
					TemplateManager.global.PCGDPForNavyException.ToString("N0")
				};
				break;
			}
			string text2 = text;
			object[] array2 = array;
			StringBuilder stringBuilder = new StringBuilder(Loc.T(text2, array2));
			stringBuilder.AppendLine().AppendLine().AppendLine(NationInfoController.PrioritySummaryString(priority, nation, true));
			switch (priority)
			{
			case PriorityType.Economy:
				if (nation.canAccumulateCoreOilTriggers)
				{
					TIRegionState nextCoreOilRegion = nation.GetNextCoreOilRegion();
					if (nextCoreOilRegion != null)
					{
						stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.Economy_CoreOilRegion", new object[]
						{
							nextCoreOilRegion.displayName,
							nextCoreOilRegion.accumulatedCoreOilRegionTriggers,
							TIGlobalConfig.globalConfig.numEcosForCoreOilRegion
						}));
					}
				}
				else if (nation.canAccumulateCoreMiningTriggers)
				{
					TIRegionState nextCoreMiningRegion = nation.GetNextCoreMiningRegion();
					if (nextCoreMiningRegion != null)
					{
						stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.Economy_CoreMiningRegion", new object[]
						{
							nextCoreMiningRegion.displayName,
							nextCoreMiningRegion.accumulatedCoreMiningRegionTriggers,
							TIGlobalConfig.globalConfig.numEcosForCoreMiningRegion
						}));
					}
				}
				else if (nation.canAccumulateCoreEconomyTriggers)
				{
					TIRegionState nextCoreEcoRegion = nation.GetNextCoreEcoRegion();
					if (nextCoreEcoRegion != null)
					{
						stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.Economy_CoreEconomicRegion", new object[]
						{
							nextCoreEcoRegion.displayName,
							nextCoreEcoRegion.accumulatedCoreEconomyRegionTriggers,
							TIGlobalConfig.globalConfig.numEcosForCoreEcoRegion
						}));
					}
				}
				break;
			case PriorityType.Welfare:
				if (nation.canAccumulateDecolonizeTriggers)
				{
					TIRegionState nextDecolonizeRegion = nation.GetNextDecolonizeRegion();
					if (nextDecolonizeRegion != null)
					{
						stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.Welfare_Decolonization", new object[] { nextDecolonizeRegion.displayName, nextDecolonizeRegion.accumulatedDecolonizeTriggers, 1000 }));
					}
				}
				break;
			case PriorityType.Environment:
				if (nation.canAccumulateDecontaminateTriggers)
				{
					TIRegionState nextDecontaminateRegion = nation.GetNextDecontaminateRegion();
					if (nextDecontaminateRegion != null)
					{
						stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.Environment_Decontamination", new object[] { nextDecontaminateRegion.displayNameSentIn, nextDecontaminateRegion.accumulatedDecontaminateTriggers, 100 }));
					}
				}
				break;
			case PriorityType.Government:
			case PriorityType.Unity:
				if (nation.canAccumulateLegitimizeClaimTriggers)
				{
					TIRegionState nextLegitimizeClaimRegion = nation.GetNextLegitimizeClaimRegion();
					if (nextLegitimizeClaimRegion != null)
					{
						stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.LegitimizeClaim", new object[]
						{
							nextLegitimizeClaimRegion.displayName,
							nation.accumulatedLegitimizeClaimTriggers,
							TIGlobalConfig.globalConfig.numPrioritiesForLegitimize
						}));
					}
				}
				break;
			}
			float num = faction.SumPriorityBonuses(priority, false);
			if (num != 0f)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.PriorityBonuses", new object[]
				{
					num.ToPercent("P0"),
					priorityLine
				}));
			}
			float num2 = nation.NationalPriorityBonuses(priority);
			if (num2 != 0f)
			{
				PriorityType priority2 = priority;
				if (priority2 != PriorityType.Economy)
				{
					if (priority2 - PriorityType.Military_BuildArmy <= 1)
					{
						stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.NationPriorityBonuses_BuildMilitaryAssets", new object[]
						{
							num2.ToPercent("P0"),
							priorityLine
						}));
					}
				}
				else
				{
					stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.NationPriorityBonuses_Economy", new object[]
					{
						num2.ToPercent("P0"),
						priorityLine
					}));
				}
			}
			if (TIControlPoint.priorityDiversityBonus.ContainsKey(priority) && TIControlPoint.priorityDiversityBonus[priority] > 0f)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.DiversityBonus", new object[]
				{
					TIUtilities.GetPriorityString(priority, false),
					TIControlPoint.priorityDiversityBonus[priority].ToPercent("P0")
				}));
			}
			if (nation.FactionHasControlPoint(GameControl.control.activePlayer))
			{
				float num3 = nation.FactionControlPoints(GameControl.control.activePlayer, true, false, true).Average<TIControlPoint>((TIControlPoint x) => x.diversityBonus[priority]);
				if (num3 > 0f)
				{
					stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.DiversityBonus3", new object[]
					{
						TIUtilities.GetPriorityString(priority, false),
						(num3 >= 10f) ? num3.ToPercent("P0") : num3.ToPercent("P1")
					}));
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060052F9 RID: 21241 RVA: 0x0024CCC8 File Offset: 0x0024AEC8
		public void SetListItem(TINationState nation, PriorityType priority, TIFactionState viewingFaction)
		{
			this.nation = nation;
			string priorityLine = TIUtilities.GetPriorityString(priority, true);
			this.priorityName.SetText(PriorityListItemController.colorizePriorityStr(nation, priority, priorityLine));
			this.priorityTip.SetDelegate("BodyText", () => PriorityListItemController.priorityTipStr(viewingFaction, nation, priority, priorityLine));
			this.priorityName.SetText(PriorityListItemController.colorizePriorityStr(nation, priority, priorityLine));
			this.priorityAccumulation.SetText(PriorityListItemController.priorityAccumulationStr(nation, priority));
			for (int i = 0; i <= nation.maxControlPointIndex; i++)
			{
				TIControlPoint controlPoint = nation.GetControlPoint(i);
				this.controlPointWeight_PH[i].sprite = PriorityListItemController.prioritySettingSprite(controlPoint.GetControlPointPriority(priority, false));
				this.controlPointWeight_PH[i].enabled = true;
				if (controlPoint.faction == this.controller.activePlayer)
				{
					this.priorityButton[i].interactable = true;
					this.rightClickButton[i].enabled = true;
				}
				else
				{
					this.priorityButton[i].interactable = false;
					this.rightClickButton[i].enabled = false;
				}
			}
			for (int j = nation.numControlPoints; j <= 5; j++)
			{
				this.controlPointWeight_PH[j].enabled = false;
				this.priorityButton[j].interactable = false;
				this.rightClickButton[j].enabled = false;
			}
			this.priorityTip.enabled = true;
			this.SetBonusColumnText(nation);
		}

		// Token: 0x060052FA RID: 21242 RVA: 0x0024CE8C File Offset: 0x0024B08C
		public void SetBonusColumnText(TINationState nation)
		{
			switch (this.controller.proportionColumnSetting)
			{
			case 0:
			{
				float num = nation.percentWeighttoPriority(this.priority);
				this.helperValue.SetText(num.ToPercent("P0"));
				return;
			}
			case 1:
				this.helperValue.SetText(TIUtilities.FormatBigOrSmallNumber(nation.ControlPointWeightsTotalToPriorityIP(this.priority) * 30.436874f, 1, 7, 0, false, false));
				return;
			case 2:
			{
				float num2 = nation.controlPoints.Average<TIControlPoint>((TIControlPoint x) => nation.ControlPointPriorityBonuses_Uncached(x, this.priority, true));
				if (num2 > 0f)
				{
					this.helperValue.SetText(Loc.T("UI.Global.PositiveValueWithSign", new object[] { num2.ToPercent("P0") }));
					return;
				}
				if (num2 < 0f)
				{
					this.helperValue.SetText(num2.ToPercent("P0"));
					return;
				}
				this.helperValue.SetText(string.Empty);
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x060052FB RID: 21243 RVA: 0x0024CFA0 File Offset: 0x0024B1A0
		private void IncrementPriority(TINationState nationState, PriorityType priority, int cp)
		{
			Player playerControl = this.controller.activePlayer.playerControl;
			PlayerAction playerAction = new CyclePrioritySettingAction(nationState.GetControlPoint(cp), this.controller.activePlayer, priority, false);
			playerControl.StartAction(playerAction);
			this.controller.UpdatePriorityList();
			this.controller.UpdateTinyControlPoints();
			NationInfoController.UpdatePriorityPresetFromChanges(this.controller.priorityPresetDropdown, this.nation, null);
		}

		// Token: 0x060052FC RID: 21244 RVA: 0x0024D00C File Offset: 0x0024B20C
		private void MassIncrementPrioirty(TINationState nation, PriorityType priority)
		{
			foreach (TIControlPoint ticontrolPoint in nation.FactionControlPoints(this.controller.activePlayer, true, false, true))
			{
				this.IncrementPriority(nation, priority, ticontrolPoint.positionInNation);
			}
		}

		// Token: 0x060052FD RID: 21245 RVA: 0x0024D074 File Offset: 0x0024B274
		private void DecrementPriority(TINationState nation, PriorityType priority, int cp)
		{
			this.controller.activePlayer.playerControl.StartAction(new CyclePrioritySettingAction(nation.GetControlPoint(cp), this.controller.activePlayer, priority, true));
			this.controller.UpdatePriorityList();
			this.controller.UpdateTinyControlPoints();
			NationInfoController.UpdatePriorityPresetFromChanges(this.controller.priorityPresetDropdown, nation, null);
		}

		// Token: 0x060052FE RID: 21246 RVA: 0x0024D0D8 File Offset: 0x0024B2D8
		private void MassDecrementPriority(TINationState nation, PriorityType priority)
		{
			foreach (TIControlPoint ticontrolPoint in nation.FactionControlPoints(this.controller.activePlayer, true, false, true))
			{
				this.DecrementPriority(nation, priority, ticontrolPoint.positionInNation);
			}
		}

		// Token: 0x060052FF RID: 21247 RVA: 0x0024D140 File Offset: 0x0024B340
		public void PriorityButtonPressed(int value)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			if (TIInputManager.IsControlKeyDown)
			{
				this.MassIncrementPrioirty(this.nation, this.priority);
			}
			else
			{
				this.IncrementPriority(this.nation, this.priority, value);
			}
			using (List<TIObjectiveTemplate>.Enumerator enumerator = this.controller.activePlayer.GetObjectivesByTypeAndStatus(ObjectiveType.Tutorial, ObjectiveStatus.Unlocked).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.targetMilestone == CampaignMilestone.TutorialCheckNationalPriority)
					{
						this.controller.activePlayer.CompleteMilestone(CampaignMilestone.TutorialCheckNationalPriority);
						break;
					}
				}
			}
		}

		// Token: 0x06005300 RID: 21248 RVA: 0x0024D1F0 File Offset: 0x0024B3F0
		public void RightPriorityButtonPressed(int value)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			if (TIInputManager.IsControlKeyDown)
			{
				this.MassDecrementPriority(this.nation, this.priority);
			}
			else
			{
				this.DecrementPriority(this.nation, this.priority, value);
			}
			using (List<TIObjectiveTemplate>.Enumerator enumerator = this.controller.activePlayer.GetObjectivesByTypeAndStatus(ObjectiveType.Tutorial, ObjectiveStatus.Unlocked).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.targetMilestone == CampaignMilestone.TutorialCheckNationalPriority)
					{
						this.controller.activePlayer.CompleteMilestone(CampaignMilestone.TutorialCheckNationalPriority);
						break;
					}
				}
			}
		}

		// Token: 0x040037EA RID: 14314
		private NationInfoController controller;

		// Token: 0x040037EB RID: 14315
		public TMP_Text priorityName;

		// Token: 0x040037EC RID: 14316
		public TMP_Text priorityAccumulation;

		// Token: 0x040037ED RID: 14317
		public Image[] controlPointWeight_PH = new Image[6];

		// Token: 0x040037EE RID: 14318
		public Button[] priorityButton = new Button[6];

		// Token: 0x040037EF RID: 14319
		public RightClickHandler[] rightClickButton = new RightClickHandler[6];

		// Token: 0x040037F0 RID: 14320
		public TMP_Text helperValue;

		// Token: 0x040037F1 RID: 14321
		public TooltipTrigger priorityTip;
	}
}

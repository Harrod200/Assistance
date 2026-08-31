using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200080D RID: 2061
	public class SkirmishShipListItemController : MonoBehaviour
	{
		// Token: 0x17000E92 RID: 3730
		// (get) Token: 0x06004A7D RID: 19069 RVA: 0x001F3B02 File Offset: 0x001F1D02
		private bool isAddShipButton
		{
			get
			{
				return this.shipIndex == -1;
			}
		}

		// Token: 0x17000E93 RID: 3731
		// (get) Token: 0x06004A7E RID: 19070 RVA: 0x001F3B0D File Offset: 0x001F1D0D
		private TISpaceShipTemplate shipTemplate
		{
			get
			{
				if (this.isAddShipButton)
				{
					return null;
				}
				return TemplateManager.Find<TISpaceShipTemplate>(this.fleetTemplate.shipsInFleet[this.shipIndex].shipTemplateName, false);
			}
		}

		// Token: 0x17000E94 RID: 3732
		// (get) Token: 0x06004A7F RID: 19071 RVA: 0x001F3B3A File Offset: 0x001F1D3A
		public SkirmishMenuController skirmishMenu
		{
			get
			{
				return base.GetComponentInParent<SkirmishMenuController>();
			}
		}

		// Token: 0x17000E95 RID: 3733
		// (get) Token: 0x06004A80 RID: 19072 RVA: 0x001F3B42 File Offset: 0x001F1D42
		private static string importString
		{
			get
			{
				return Loc.T("UI.StartScreen.Skirmish.Import");
			}
		}

		// Token: 0x06004A81 RID: 19073 RVA: 0x001F3B4E File Offset: 0x001F1D4E
		public void Initialize(StartMenuController masterController, TISpaceFleetTemplate fleetTemplate, int idx, int fleetIdx)
		{
			this.masterController = masterController;
			this.fleetTemplate = fleetTemplate;
			this.shipIndex = idx;
			this.fleetIdx = fleetIdx;
			this.PopulateShipDropdown();
		}

		// Token: 0x06004A82 RID: 19074 RVA: 0x001F3B74 File Offset: 0x001F1D74
		private void SetShipDamageImages()
		{
			if (this.isAddShipButton)
			{
				return;
			}
			TISpaceShipTemplate shipTemplate = this.shipTemplate;
			this.noseImage.sprite = TIUtilities.assetLoader.LoadAsset<Sprite>(shipTemplate.hullTemplate.combatUINosePath_OK(shipTemplate.hullAppearanceIndex));
			this.lateralImage.sprite = TIUtilities.assetLoader.LoadAsset<Sprite>(shipTemplate.hullTemplate.combatUIMidPath_OK(shipTemplate.hullAppearanceIndex));
			this.tailImage.sprite = TIUtilities.assetLoader.LoadAsset<Sprite>(shipTemplate.hullTemplate.combatUITailPath_OK(shipTemplate.hullAppearanceIndex));
			if (!shipTemplate.isAlien && !shipTemplate.hullTemplate.simpleHull)
			{
				this.radiatorImage.sprite = TIUtilities.assetLoader.LoadAsset<Sprite>(shipTemplate.radiatorTemplate.combatUIPath_On_OK(shipTemplate.hullTemplate, shipTemplate.hullAppearanceIndex));
				this.driveImage.sprite = TIUtilities.assetLoader.LoadAsset<Sprite>(shipTemplate.driveTemplate.combatUIPath_OK(shipTemplate.hullTemplate, shipTemplate.hullAppearanceIndex));
				this.radiatorImage.enabled = true;
				this.driveImage.enabled = true;
				return;
			}
			this.radiatorImage.enabled = false;
			this.driveImage.enabled = false;
		}

		// Token: 0x06004A83 RID: 19075 RVA: 0x001F3CA4 File Offset: 0x001F1EA4
		public void PopulateShipDropdown()
		{
			this.shipDropdown.ClearOptions();
			TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
			TISpaceShipTemplate shipTemplate = this.shipTemplate;
			foreach (TISpaceShipTemplate tispaceShipTemplate in this.masterController.ships)
			{
				if (!tispaceShipTemplate.hideInSkirmish && (this.fleetIdx == 1 || tispaceShipTemplate.factionName != "AlienCouncil"))
				{
					string text = Loc.T("UI.StartScreen.SkirmishShipDropdownLineItem", new object[]
					{
						tispaceShipTemplate.fullClassName,
						TemplateManager.global.spaceCombatScoreInlineSpritePath,
						tispaceShipTemplate.TemplateSpaceCombatValue(false, -1f, 1f, false).ToString("N0")
					});
					bool flag = !this.isAddShipButton && tispaceShipTemplate.fullClassName == shipTemplate.fullClassName;
					bool flag2 = this.masterController.ImportedShipTemplates.Contains(tispaceShipTemplate);
					if (tispaceShipTemplate.isAlien)
					{
						text = TIUtilities.PurpleLine(text);
					}
					else if (flag2)
					{
						text = TIUtilities.FactionLine(text, SkirmishShipListItemController.factions[tispaceShipTemplate.factionName]);
					}
					TMP_Dropdown.OptionData optionData2 = new TMP_Dropdown.OptionData(text);
					this.shipDropdown.options.Add(optionData2);
					if ((this.selectImportedDesign && flag2) || (!this.selectImportedDesign && flag))
					{
						optionData = optionData2;
					}
				}
			}
			this.shipDropdown.options.Add(new TMP_Dropdown.OptionData(SkirmishShipListItemController.importString));
			this.selectImportedDesign = false;
			if (!this.isAddShipButton)
			{
				this.shipDropdown.value = this.shipDropdown.options.IndexOf(optionData);
			}
			else
			{
				this.shipDropdown.SetValueWithoutNotify(this.shipDropdown.options.IndexOf(optionData));
			}
			this.shipDropdown.RefreshShownValue();
			this.SetTooltipDelegate();
			this.SetShipDamageImages();
		}

		// Token: 0x06004A84 RID: 19076 RVA: 0x001F3EA8 File Offset: 0x001F20A8
		public void OnShipDropdownChanged()
		{
			string text = this.shipDropdown.options[this.shipDropdown.value].text;
			int num = text.LastIndexOf(':');
			if (num != -1)
			{
				text = this.shipDropdown.options[this.shipDropdown.value].text.Substring(0, num);
			}
			text = new Regex("[\\uFF1A]").Split(text, 0)[0];
			if (text == SkirmishShipListItemController.importString)
			{
				this.masterController.loadMenuController.EnterImportMode(delegate(SaveStructure saveStructure)
				{
					SkirmishShipListItemController.factions = saveStructure.gamestates[typeof(TIPlayerState)].Values.Select<TIGameState, TIFactionState>((TIGameState x) => (x as TIPlayerState).faction).ToDictionary<TIFactionState, string, TIFactionState>((TIFactionState x) => x.templateName, (TIFactionState x) => x);
					SkirmishShipListItemController[] componentsInChildren = this.skirmishMenu.GetComponentsInChildren<SkirmishShipListItemController>();
					IEnumerable<TISpaceShipTemplate> enumerable = SkirmishShipListItemController.factions.Values.First<TIFactionState>((TIFactionState x) => !x.player.isAI).shipDesigns.ToList<TISpaceShipTemplate>();
					List<TISpaceShipTemplate> list = SkirmishShipListItemController.factions.Values.Where<TIFactionState>((TIFactionState x) => x.player.isAI).SelectMany<TIFactionState, TISpaceShipTemplate>(delegate(TIFactionState faction)
					{
						bool includeAllAIDesigns = TIGlobalConfig.globalConfig.importAllAIShipDesignsInSkirmish;
						IOrderedEnumerable<TISpaceShipTemplate> orderedEnumerable = (from x in faction.shipDesigns
							where includeAllAIDesigns || x.combatant
							orderby faction.ships.Count<TISpaceShipState>((TISpaceShipState y) => y.templateName == x.dataName) descending
							select x).ThenBy<TISpaceShipTemplate, int>(delegate(TISpaceShipTemplate x)
						{
							Func<ShipConstructionQueueItem, bool> <>9__15;
							return faction.nShipyardQueues.Sum<KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>>>(delegate(KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>> y)
							{
								IEnumerable<ShipConstructionQueueItem> value = y.Value;
								Func<ShipConstructionQueueItem, bool> func;
								if ((func = <>9__15) == null)
								{
									func = (<>9__15 = (ShipConstructionQueueItem z) => z.shipDesign == x);
								}
								return value.Count<ShipConstructionQueueItem>(func);
							});
						});
						if (includeAllAIDesigns)
						{
							return orderedEnumerable;
						}
						return (from x in orderedEnumerable
							group x by x.hullName).SelectMany<IGrouping<string, TISpaceShipTemplate>, TISpaceShipTemplate>((IGrouping<string, TISpaceShipTemplate> x) => x.Take<TISpaceShipTemplate>(1));
					}).ToList<TISpaceShipTemplate>();
					List<TISpaceShipTemplate> list2 = enumerable.Concat<TISpaceShipTemplate>(list).ToList<TISpaceShipTemplate>();
					this.masterController.ImportedShipTemplates = list2;
					SkirmishShipListItemController.suppressMasterNotify = true;
					this.selectImportedDesign = true;
					SkirmishShipListItemController[] array = componentsInChildren;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].PopulateShipDropdown();
					}
					SkirmishShipListItemController.suppressMasterNotify = false;
					this.masterController.OnShipDropdownChanged();
					this.masterController.menuManager.ShowMenu(this.skirmishMenu.menu);
				}, delegate
				{
					this.masterController.menuManager.ShowMenu(this.skirmishMenu.menu);
				});
				this.shipDropdown.SetValueWithoutNotify(this.previousShipDropdownValue);
				return;
			}
			text = Regex.Replace(text, "<[^>]+>|&nbsp;", "").Trim();
			if (!this.isAddShipButton)
			{
				this.fleetTemplate.shipsInFleet[this.shipIndex] = new TISpaceFleetTemplate.ShipFleetDefinition(this.masterController.shipDictionary[text].dataName);
			}
			else
			{
				this.AddSpecificShip(new TISpaceFleetTemplate.ShipFleetDefinition(this.masterController.shipDictionary[text].dataName));
			}
			if (!SkirmishShipListItemController.suppressMasterNotify)
			{
				this.masterController.OnShipDropdownChanged();
			}
			this.SetShipDamageImages();
			this.SetTooltipDelegate();
			this.previousShipDropdownValue = this.shipDropdown.value;
		}

		// Token: 0x06004A85 RID: 19077 RVA: 0x001F4007 File Offset: 0x001F2207
		public static void InsertImportedDesigns(Dictionary<string, TISpaceShipTemplate> shipDictionary, List<TISpaceShipTemplate> ships)
		{
		}

		// Token: 0x06004A86 RID: 19078 RVA: 0x001F4009 File Offset: 0x001F2209
		private void SetTooltipDelegate()
		{
			if (this.isAddShipButton)
			{
				this.SetAddShipButtonDropdownTooltipDelegates();
				return;
			}
			this.shipSummaryTip.SetDelegate("BodyText", () => this.shipTemplate.quickSummary(false, null, false, false, false));
		}

		// Token: 0x06004A87 RID: 19079 RVA: 0x001F4038 File Offset: 0x001F2238
		public void SetAddShipButtonDropdownTooltipDelegates()
		{
			if (!this.isAddShipButton || !this.shipDropdown.IsExpanded)
			{
				return;
			}
			this.dropdownItems = this.shipDropdown.transform.parent.GetComponentsInChildren<SkirmishAddShipDropdownItem>();
			if (this.dropdownItems == null || this.dropdownItems.Length == 0)
			{
				return;
			}
			List<TISpaceShipTemplate> list = new List<TISpaceShipTemplate>();
			foreach (TISpaceShipTemplate tispaceShipTemplate in this.masterController.ships)
			{
				if (!tispaceShipTemplate.hideInSkirmish && (this.fleetIdx == 1 || tispaceShipTemplate.factionName != "AlienCouncil"))
				{
					list.Add(tispaceShipTemplate);
				}
			}
			for (int i = 0; i < this.dropdownItems.Length; i++)
			{
				if (i == this.dropdownItems.Length - 1)
				{
					this.dropdownItems[i].SetTooltipDelegate(SkirmishShipListItemController.importString);
				}
				else
				{
					this.dropdownItems[i].SetTooltipDelegate(list[i].quickSummary(false, null, false, false, false));
				}
			}
		}

		// Token: 0x06004A88 RID: 19080 RVA: 0x001F4150 File Offset: 0x001F2350
		public void OnTrashSelected()
		{
			if (this.fleetTemplate.shipsInFleet.Count > 1)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
				this.fleetTemplate.shipsInFleet.Remove(this.fleetTemplate.shipsInFleet[this.shipIndex]);
				this.masterController.PopulateSkirmishDropdowns();
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x06004A89 RID: 19081 RVA: 0x001F41BC File Offset: 0x001F23BC
		public void AddShipSelected()
		{
			this.fleetTemplate.shipsInFleet.Add(new TISpaceFleetTemplate.ShipFleetDefinition(this.fleetTemplate.shipsInFleet[this.fleetTemplate.shipsInFleet.Count - 1].shipTemplateName));
			this.masterController.PopulateSkirmishDropdowns();
		}

		// Token: 0x06004A8A RID: 19082 RVA: 0x001F4210 File Offset: 0x001F2410
		public void AddSpecificShip(TISpaceFleetTemplate.ShipFleetDefinition NewShip)
		{
			this.fleetTemplate.shipsInFleet.Add(NewShip);
			this.masterController.PopulateSkirmishDropdowns();
		}

		// Token: 0x06004A8B RID: 19083 RVA: 0x001F422E File Offset: 0x001F242E
		public void DuplicateShip()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.AddSpecificShip(this.fleetTemplate.shipsInFleet[this.shipIndex]);
		}

		// Token: 0x04002B79 RID: 11129
		public TMP_Dropdown shipDropdown;

		// Token: 0x04002B7A RID: 11130
		private StartMenuController masterController;

		// Token: 0x04002B7B RID: 11131
		private TISpaceFleetTemplate fleetTemplate;

		// Token: 0x04002B7C RID: 11132
		private int shipIndex;

		// Token: 0x04002B7D RID: 11133
		private int fleetIdx;

		// Token: 0x04002B7E RID: 11134
		[Header("Only for Ship List")]
		public Image noseImage;

		// Token: 0x04002B7F RID: 11135
		public Image lateralImage;

		// Token: 0x04002B80 RID: 11136
		public Image tailImage;

		// Token: 0x04002B81 RID: 11137
		public Image driveImage;

		// Token: 0x04002B82 RID: 11138
		public Image radiatorImage;

		// Token: 0x04002B83 RID: 11139
		public TooltipTrigger shipSummaryTip;

		// Token: 0x04002B84 RID: 11140
		private SkirmishAddShipDropdownItem[] dropdownItems;

		// Token: 0x04002B85 RID: 11141
		private bool selectImportedDesign;

		// Token: 0x04002B86 RID: 11142
		private int previousShipDropdownValue = -1;

		// Token: 0x04002B87 RID: 11143
		private static bool suppressMasterNotify;

		// Token: 0x04002B88 RID: 11144
		private static Dictionary<string, TIFactionState> factions;
	}
}

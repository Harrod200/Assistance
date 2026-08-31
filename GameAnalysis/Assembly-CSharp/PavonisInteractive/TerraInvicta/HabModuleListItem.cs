using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200086D RID: 2157
	public class HabModuleListItem : DragItem
	{
		// Token: 0x0600500F RID: 20495 RVA: 0x00229231 File Offset: 0x00227431
		protected override void Awake()
		{
			base.Awake();
			this.Init();
		}

		// Token: 0x06005010 RID: 20496 RVA: 0x0022923F File Offset: 0x0022743F
		private void Start()
		{
			this.Init();
		}

		// Token: 0x06005011 RID: 20497 RVA: 0x00229247 File Offset: 0x00227447
		private void Init()
		{
			if (this.hasInit)
			{
				return;
			}
			this.CacheComponents();
			this.hasInit = true;
		}

		// Token: 0x06005012 RID: 20498 RVA: 0x00229260 File Offset: 0x00227460
		private void CacheComponents()
		{
			this.group = base.GetComponent<CanvasGroup>();
			this.tooltip = base.GetComponent<TooltipTrigger>();
			this.moduleIcon = base.gameObject.GetComponentOnChild<Image>("Icon");
			this.moduleName = base.gameObject.GetComponentOnChild<TMP_Text>("Name");
			this.moduleTypeIcon = base.gameObject.GetComponentOnChild<Image>("TypeIcon");
			this.moduleTierIcon = base.gameObject.GetComponentOnChild<Image>("TierIcon");
			this.moduleConstructionIcon = base.gameObject.GetComponentOnChild<Image>("ConstructionIcon");
			this.moduleDecommissionIcon = base.gameObject.GetComponentOnChild<Image>("DecommissionIcon");
			this.backgroundImage = base.gameObject.GetComponent<Image>();
			this.defaultBackgroundSprite = this.backgroundImage.sprite;
		}

		// Token: 0x06005013 RID: 20499 RVA: 0x0022932B File Offset: 0x0022752B
		public override void Drop(Transform parent)
		{
		}

		// Token: 0x06005014 RID: 20500 RVA: 0x0022932D File Offset: 0x0022752D
		public void SetModule(TIHabModuleState moduleState, HabType habType, bool upgradeLocationAvailable, HabGridCell gridCell = null)
		{
			this.moduleState = moduleState;
			this.moduleTemplate = this.moduleState.moduleTemplate;
			this.habGridCell = gridCell;
			this.UpdateItem(habType, upgradeLocationAvailable);
		}

		// Token: 0x06005015 RID: 20501 RVA: 0x00229357 File Offset: 0x00227557
		public void SetModuleTemplate(TIHabModuleTemplate moduleTemplate, HabType habType, bool upgradeLocationAvailable, HabGridCell gridCell = null)
		{
			this.moduleState = null;
			this.moduleTemplate = moduleTemplate;
			this.habGridCell = gridCell;
			this.UpdateItem(habType, upgradeLocationAvailable);
		}

		// Token: 0x06005016 RID: 20502 RVA: 0x00229377 File Offset: 0x00227577
		public TIHabModuleTemplate GetModuleTemplate()
		{
			return this.moduleTemplate;
		}

		// Token: 0x06005017 RID: 20503 RVA: 0x0022937F File Offset: 0x0022757F
		public TIHabModuleState GetModuleState()
		{
			return this.moduleState;
		}

		// Token: 0x06005018 RID: 20504 RVA: 0x00229388 File Offset: 0x00227588
		private void UpdateItem(HabType habType, bool upgradeLocationAvailable)
		{
			base.gameObject.name = this.moduleTemplate.displayName;
			StringBuilder stringBuilder = new StringBuilder(this.moduleTemplate.displayName);
			if (upgradeLocationAvailable)
			{
				stringBuilder.AppendLine().AppendLine(TIUtilities.HighlightLine(Loc.T("UI.Habs.Upgrade")));
			}
			this.moduleName.SetText(stringBuilder.ToString());
			string text = ((habType == HabType.Station) ? this.moduleTemplate.stationIconResource : this.moduleTemplate.baseIconResource);
			GameControl.assetLoader.LoadAssetForImageAssignment(text, this.moduleIcon);
			string text2 = new StringBuilder("icons_2d/ICO_MaxTier").Append(this.moduleTemplate.tier).ToString();
			GameControl.assetLoader.LoadAssetForImageAssignment(text2, this.moduleTierIcon);
			this.SetTypeIcon();
			this.moduleDecommissionIcon.gameObject.SetActive(this.moduleState != null && this.moduleState.decommissioning);
			this.moduleConstructionIcon.gameObject.SetActive(this.moduleState != null && this.moduleState.underConstruction);
			this.group.alpha = ((this.draggable || !this.prospective) ? 1f : 0.3f);
			this.dragItemType = ((this.draggable || !this.prospective) ? DragItemType.HAB : DragItemType.NONE);
		}

		// Token: 0x06005019 RID: 20505 RVA: 0x002294E8 File Offset: 0x002276E8
		private void SetTypeIcon()
		{
			string text = string.Empty;
			if (this.moduleTemplate.coreModule)
			{
				text = "icons_2d/ICO_hab_core";
			}
			else if (this.moduleTemplate.powerSource)
			{
				text = TemplateManager.global.pathHabPowerIcon;
			}
			else if (this.moduleTemplate.allowsShipConstruction)
			{
				text = TemplateManager.global.pathHabShipyardIcon;
			}
			else if (this.moduleTemplate.allowsResupply)
			{
				text = TemplateManager.global.pathHabResupplyIcon;
			}
			else if (this.moduleTemplate.constructionModule)
			{
				text = TemplateManager.global.pathHabModuleConstructionIcon;
			}
			else if (this.moduleTemplate.mine)
			{
				text = TemplateManager.global.pathSpaceMiningIcon;
			}
			else if (this.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.Farm))
			{
				text = "icons_2d/ICO_hab_farm";
			}
			else if (this.moduleTemplate.CombatTroops())
			{
				text = TemplateManager.global.pathSpaceAssaultScoreIcon;
			}
			else if (this.moduleTemplate.spaceCombatModule)
			{
				text = TemplateManager.global.pathSpaceCombatScoreIcon;
			}
			else if (this.moduleTemplate.incomeAntimatter_month > 0f || this.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.HarvestAntimatter))
			{
				text = TemplateManager.global.pathAntimatterIcon;
			}
			else if (this.moduleTemplate.incomeMoney_month > 0f || this.moduleTemplate.incomeInfluence_month > 0f || this.moduleTemplate.incomeResearch_month > 0f || this.moduleTemplate.incomeProjects > 0 || this.moduleTemplate.missionControl > 0 || this.moduleTemplate.incomeAntimatter_month > 0f || this.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.HarvestAntimatter))
			{
				Dictionary<float, string> dictionary = new Dictionary<float, string>();
				if (this.moduleTemplate.incomeMoney_month > 0f)
				{
					dictionary[this.moduleTemplate.incomeMoney_month] = TemplateManager.global.pathMoneyIcon;
				}
				if (this.moduleTemplate.incomeInfluence_month > 0f)
				{
					dictionary[this.moduleTemplate.incomeInfluence_month] = TemplateManager.global.pathInfluenceIcon;
				}
				if (this.moduleTemplate.incomeResearch_month > 0f)
				{
					dictionary[this.moduleTemplate.incomeResearch_month] = TemplateManager.global.pathResearchIcon;
				}
				if (this.moduleTemplate.incomeProjects > 0)
				{
					dictionary[(float)this.moduleTemplate.incomeProjects] = TemplateManager.global.pathProjectsIcon;
				}
				if (this.moduleTemplate.missionControl > 0)
				{
					dictionary[(float)this.moduleTemplate.missionControl] = TemplateManager.global.pathMissionControlIcon;
				}
				if (dictionary.Count > 0)
				{
					text = dictionary[dictionary.Keys.Max()];
				}
			}
			else if (this.moduleTemplate.ControlPointCapacity(this.moduleState != null && this.moduleState.hab.inEarthLEO) > 0)
			{
				text = TemplateManager.global.pathEmptyControlPoint;
			}
			else if (this.moduleTemplate.techBonuses.Any<TechBonus>((TechBonus x) => x.bonus > 0f))
			{
				text = "icons_2d/ICO_hab_tech";
			}
			else if (this.moduleTemplate.HasLEOBonus())
			{
				text = "icons_2d/ICO_Earth";
			}
			if (string.IsNullOrEmpty(text))
			{
				this.moduleTypeIcon.enabled = false;
				return;
			}
			GameControl.assetLoader.LoadAssetForImageAssignment(text, this.moduleTypeIcon);
			this.moduleTypeIcon.enabled = true;
		}

		// Token: 0x0600501A RID: 20506 RVA: 0x0022986C File Offset: 0x00227A6C
		private string SetTooltipText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			TIFactionState activePlayer = GameControl.control.activePlayer;
			stringBuilder.AppendLine(this.moduleTemplate.displayName);
			stringBuilder.AppendLine(this.moduleTemplate.benefitsAndCostsDescription(activePlayer, this.controller.habToDisplay, true));
			stringBuilder.AppendLine(this.moduleTemplate.extendedDescription).AppendLine();
			if (this.moduleTemplate.mine)
			{
				int missionControlRequirementFromNextMine = activePlayer.GetMissionControlRequirementFromNextMine(this.controller.habToDisplay.ref_spaceBody);
				if (missionControlRequirementFromNextMine > 0)
				{
					stringBuilder.AppendLine(Loc.T("UI.Habs.ExtraMissionControl", new object[] { missionControlRequirementFromNextMine.ToString() }));
				}
			}
			bool flag = false;
			bool flag2 = this.controller.habToDisplay.ModuleUpgradePrereqModuleAlreadyOnHab(this.moduleTemplate);
			if (!this.moduleTemplate.onePerHab || !flag2)
			{
				TIResourcesCost tiresourcesCost = this.moduleTemplate.CostFromSpace(activePlayer, this.controller.habToDisplay, false, false, 0, false);
				stringBuilder.AppendLine(Loc.T("UI.Habs.BaseSpaceCost", new object[] { tiresourcesCost.GetString("Relevant", false, true, false, 2, false, false, activePlayer, false, FactionResource.None) }));
				TIResourcesCost tiresourcesCost2 = this.moduleTemplate.CostFromEarth(activePlayer, this.controller.habToDisplay, false);
				stringBuilder.AppendLine(Loc.T("UI.Habs.CostFromEarth", new object[] { tiresourcesCost2.GetString("Relevant", false, true, false, 2, false, false, activePlayer, false, FactionResource.None) }));
				flag = tiresourcesCost2.CanAfford(activePlayer, 1f, null, float.PositiveInfinity);
				TIResourcesCost tiresourcesCost3 = this.moduleTemplate.CostFromSpace(activePlayer, this.controller.habToDisplay, false, true, 0, false);
				if (tiresourcesCost3.GetSingleCostValue(FactionResource.Boost) > 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Habs.CostFromSpace", new object[] { tiresourcesCost3.GetString("Relevant", false, true, false, 2, false, false, activePlayer, false, FactionResource.None) }));
				}
				flag = flag || tiresourcesCost3.CanAfford(activePlayer, 1f, null, float.PositiveInfinity);
			}
			if (flag2)
			{
				TIResourcesCost tiresourcesCost4 = this.moduleTemplate.CostFromEarth(activePlayer, this.controller.habToDisplay, true);
				stringBuilder.AppendLine(Loc.T("UI.Habs.UpgradeFromEarth", new object[]
				{
					this.moduleTemplate.UpgradesFrom.displayName,
					tiresourcesCost4.GetString("Relevant", false, true, false, 2, false, false, null, false, FactionResource.None)
				}));
				flag = flag || tiresourcesCost4.CanAfford(activePlayer, 1f, null, float.PositiveInfinity);
				List<TIHabModuleState> list = this.controller.habToDisplay.CompletedModules();
				if (((list != null) ? list.Where<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate == this.moduleTemplate.UpgradesFrom).FirstOrDefault<TIHabModuleState>() : null) == null)
				{
					(from x in this.controller.habToDisplay.AllModules()
						where x.underConstruction
						select x).MinBy<TIHabModuleState, DateTime>((TIHabModuleState x) => x.completionDate);
				}
				TIResourcesCost tiresourcesCost5 = this.moduleTemplate.CostFromSpace(activePlayer, this.controller.habToDisplay, true, true, 0, false);
				stringBuilder.AppendLine(Loc.T("UI.Habs.UpgradeFromSpace", new object[]
				{
					this.moduleTemplate.UpgradesFrom.displayName,
					tiresourcesCost5.GetString("Relevant", false, true, false, 2, false, false, null, false, FactionResource.None)
				}));
				flag = flag || tiresourcesCost5.CanAfford(activePlayer, 1f, null, float.PositiveInfinity);
			}
			TIHabModuleTemplate upgradesTo = this.moduleTemplate.UpgradesTo;
			if (this.controller.habToDisplay.AllowedModules(activePlayer).Contains(upgradesTo))
			{
				stringBuilder.AppendLine(Loc.T("UI.Habs.UpgradesTo", new object[] { upgradesTo.displayName }));
			}
			if (!flag)
			{
				stringBuilder.Append(Loc.T("UI.Habs.CantAfford"));
			}
			if (this.prospective)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Habs.RightClickToPlace"));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600501B RID: 20507 RVA: 0x00229C70 File Offset: 0x00227E70
		public void AssignTooltipDelegate()
		{
			this.tooltip.SetDelegate("BodyText", () => this.SetTooltipText());
		}

		// Token: 0x0600501C RID: 20508 RVA: 0x00229C90 File Offset: 0x00227E90
		public void OnClickItem()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
			this.controller.prospectiveModule = this.moduleTemplate;
			if (!this.prospective && this.habGridCell != null)
			{
				this.controller.SelectModule(this.habGridCell);
			}
			this.controller.SetMenuToSelectedModule(this);
			this.controller.UpdateModulePreviewText(this.prospective, false);
		}

		// Token: 0x0600501D RID: 20509 RVA: 0x00229D00 File Offset: 0x00227F00
		public void OnRightClickItem()
		{
			if (!this.prospective)
			{
				return;
			}
			this.controller.prospectiveModule = this.moduleTemplate;
			if (this.habGridCell != null)
			{
				this.controller.SelectModule(this.habGridCell);
			}
			this.controller.SetMenuToSelectedModule(this);
			this.controller.UpdateModulePreviewText(this.prospective, false);
			if (!this.controller.habToDisplay.ModuleUpgradePrereqModuleAlreadyOnHab(this.moduleTemplate))
			{
				int num;
				int num2;
				this.controller.GetEmptyModuleSlot(out num, out num2, this.moduleTemplate.mine);
				if (num != -1 && num2 != -1)
				{
					this.controller.StartModulePlacement(this.moduleTemplate, num, num2);
					return;
				}
			}
			else
			{
				int num3;
				int num4;
				this.controller.habToDisplay.GetUpgradeModuleLocation(this.moduleTemplate, out num3, out num4);
				if (num3 != -1 && num4 != -1)
				{
					this.controller.StartModulePlacement(this.moduleTemplate, num3, num4);
				}
			}
		}

		// Token: 0x0600501E RID: 20510 RVA: 0x00229DE8 File Offset: 0x00227FE8
		public void SetHighlight(bool highlight)
		{
			this.backgroundImage.sprite = (highlight ? this.highlightBackgroundSprite : this.defaultBackgroundSprite);
		}

		// Token: 0x0600501F RID: 20511 RVA: 0x00229E08 File Offset: 0x00228008
		public override void OnBeginDrag(PointerEventData eventData)
		{
			base.OnBeginDrag(eventData);
			if (this.habGridCell != null)
			{
				return;
			}
			if (eventData.button == PointerEventData.InputButton.Right)
			{
				return;
			}
			this.tooltip.enabled = false;
			this.moduleTypeIcon.enabled = false;
			this.moduleTierIcon.enabled = false;
			this.moduleConstructionIcon.enabled = false;
			this.moduleDecommissionIcon.enabled = false;
			this.moduleName.enabled = false;
			this.backgroundImage.enabled = false;
			this.originalSize = (base.transform as RectTransform).sizeDelta;
			(base.transform as RectTransform).sizeDelta = this.moduleIcon.rectTransform.sizeDelta;
		}

		// Token: 0x06005020 RID: 20512 RVA: 0x00229EBF File Offset: 0x002280BF
		public override void OnDrag(PointerEventData eventData)
		{
			if (this.dragging)
			{
				base.transform.position = eventData.position;
				if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
				{
					this.OnEndDrag(eventData);
				}
			}
		}

		// Token: 0x06005021 RID: 20513 RVA: 0x00229EF8 File Offset: 0x002280F8
		public override void OnEndDrag(PointerEventData eventData)
		{
			base.OnEndDrag(eventData);
			if (this.habGridCell != null)
			{
				return;
			}
			if (eventData.button == PointerEventData.InputButton.Right)
			{
				return;
			}
			this.moduleTypeIcon.enabled = true;
			this.moduleTierIcon.enabled = true;
			this.moduleConstructionIcon.enabled = true;
			this.moduleDecommissionIcon.enabled = true;
			this.moduleName.enabled = true;
			this.backgroundImage.enabled = true;
			(base.transform as RectTransform).sizeDelta = this.originalSize;
			this.tooltip.enabled = true;
			this.tooltip.ForceHideTooltip();
			TooltipManager.Instance.HideAll();
		}

		// Token: 0x0400339D RID: 13213
		public IHabitatsPreviewer Previewer;

		// Token: 0x0400339E RID: 13214
		[HideInInspector]
		public bool prospective = true;

		// Token: 0x0400339F RID: 13215
		private TooltipTrigger tooltip;

		// Token: 0x040033A0 RID: 13216
		private bool hasInit;

		// Token: 0x040033A1 RID: 13217
		private CanvasGroup group;

		// Token: 0x040033A2 RID: 13218
		private Image moduleIcon;

		// Token: 0x040033A3 RID: 13219
		private Image moduleTypeIcon;

		// Token: 0x040033A4 RID: 13220
		private Image moduleTierIcon;

		// Token: 0x040033A5 RID: 13221
		private Image moduleConstructionIcon;

		// Token: 0x040033A6 RID: 13222
		private Image moduleDecommissionIcon;

		// Token: 0x040033A7 RID: 13223
		private TMP_Text moduleName;

		// Token: 0x040033A8 RID: 13224
		private TIHabModuleTemplate moduleTemplate;

		// Token: 0x040033A9 RID: 13225
		private TIHabModuleState moduleState;

		// Token: 0x040033AA RID: 13226
		private DragDestination dragDestination;

		// Token: 0x040033AB RID: 13227
		[HideInInspector]
		public HabitatsScreenController controller;

		// Token: 0x040033AC RID: 13228
		private Image backgroundImage;

		// Token: 0x040033AD RID: 13229
		public Sprite highlightBackgroundSprite;

		// Token: 0x040033AE RID: 13230
		private Sprite defaultBackgroundSprite;

		// Token: 0x040033AF RID: 13231
		private Vector2 originalSize;

		// Token: 0x040033B0 RID: 13232
		[HideInInspector]
		public HabGridCell habGridCell;
	}
}

using System;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008D1 RID: 2257
	public class BaseSiteListItemController : MonoBehaviour
	{
		// Token: 0x0600568A RID: 22154 RVA: 0x00279268 File Offset: 0x00277468
		public void SetListItem(TIHabSiteState habSite, TIFactionState viewingFaction, bool showSiteName, bool victoryBase, SpaceObjectDetailController controller)
		{
			this.controller = controller;
			this.site = habSite;
			this.victory = victoryBase;
			TISpaceBodyState parentBody = habSite.parentBody;
			if (habSite.hasPlannedOrOperatingBase && GameControl.control.activePlayer.HasIntelOnSpaceAssetLocation(habSite.hab))
			{
				this.statusImage.sprite = habSite.hab.icon;
				this.statusImage.enabled = true;
				this.tip.enabled = true;
				this.statusTipValue = 0;
			}
			else if (viewingFaction.Prospected(parentBody))
			{
				this.statusImage.enabled = true;
				GameControl.assetLoader.LoadAssetForImageAssignment("icons_2d/ICO_probe", this.statusImage);
				this.tip.enabled = true;
				this.statusTipValue = 1;
			}
			else if (viewingFaction.ProspectorEnRoute(parentBody))
			{
				this.statusImage.enabled = true;
				this.tip.enabled = true;
				GameControl.assetLoader.LoadAssetForImageAssignment("icons_2d/ICO_probe_en_route", this.statusImage);
				this.statusTipValue = 2;
			}
			else
			{
				this.statusImage.enabled = false;
				this.statusTipValue = 3;
			}
			this.SetSiteNameText(showSiteName);
			int num = 1;
			int num2 = 7;
			TIHabState hab = habSite.hab;
			if (((hab != null) ? hab.faction : null) == viewingFaction)
			{
				TIHabModuleState mine = habSite.hab.mine;
				if (mine != null && mine.active)
				{
					TIHabModuleTemplate moduleTemplate = habSite.hab.mine.moduleTemplate;
					this.Water.SetText(TIUtilities.FormatBigOrSmallNumber(moduleTemplate.GetMiningIncome_Month(viewingFaction, habSite, FactionResource.Water), num, num2, 0, false, false));
					this.Volatiles.SetText(TIUtilities.FormatBigOrSmallNumber(moduleTemplate.GetMiningIncome_Month(viewingFaction, habSite, FactionResource.Volatiles), num, num2, 0, false, false));
					this.Metals.SetText(TIUtilities.FormatBigOrSmallNumber(moduleTemplate.GetMiningIncome_Month(viewingFaction, habSite, FactionResource.Metals), num, num2, 0, false, false));
					this.Nobles.SetText(TIUtilities.FormatBigOrSmallNumber(moduleTemplate.GetMiningIncome_Month(viewingFaction, habSite, FactionResource.NobleMetals), num, num2, 0, false, false));
					this.Fissiles.SetText(TIUtilities.FormatBigOrSmallNumber(moduleTemplate.GetMiningIncome_Month(viewingFaction, habSite, FactionResource.Fissiles), num, num2, 0, false, false));
					return;
				}
			}
			if (viewingFaction.Prospected(parentBody))
			{
				this.Water.SetText(TIUtilities.FormatBigOrSmallNumber(habSite.GetMonthlyProduction(FactionResource.Water), num, num2, 0, false, false));
				this.Volatiles.SetText(TIUtilities.FormatBigOrSmallNumber(habSite.GetMonthlyProduction(FactionResource.Volatiles), num, num2, 0, false, false));
				this.Metals.SetText(TIUtilities.FormatBigOrSmallNumber(habSite.GetMonthlyProduction(FactionResource.Metals), num, num2, 0, false, false));
				this.Nobles.SetText(TIUtilities.FormatBigOrSmallNumber(habSite.GetMonthlyProduction(FactionResource.NobleMetals), num, num2, 0, false, false));
				this.Fissiles.SetText(TIUtilities.FormatBigOrSmallNumber(habSite.GetMonthlyProduction(FactionResource.Fissiles), num, num2, 0, false, false));
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (FactionResource factionResource in TIResourcesCost.spaceResources)
			{
				float habSiteMinProductivity_month = habSite.GetHabSiteMinProductivity_month(factionResource);
				float habSiteMaxProductivity_month = habSite.GetHabSiteMaxProductivity_month(factionResource);
				if (habSiteMaxProductivity_month > habSiteMinProductivity_month)
				{
					if (habSiteMinProductivity_month == 0f)
					{
						stringBuilder.Append(TIUtilities.FormatBigOrSmallNumber(habSiteMinProductivity_month, 0, 0, 0, false, false));
						stringBuilder.Append("-").Append(TIUtilities.FormatBigOrSmallNumber(habSiteMaxProductivity_month, 0, 1, 0, false, false));
					}
					else
					{
						stringBuilder.Append(TIUtilities.FormatBigOrSmallNumber(habSiteMinProductivity_month, 0, 0, 0, false, false));
						stringBuilder.Append("-").Append(TIUtilities.FormatBigOrSmallNumber(habSiteMaxProductivity_month, 0, 0, 0, false, false));
					}
				}
				else
				{
					stringBuilder.Append(TIUtilities.FormatBigOrSmallNumber(habSiteMinProductivity_month, num, num2, 0, false, false));
				}
				switch (factionResource)
				{
				case FactionResource.Water:
					this.Water.SetText(stringBuilder.ToString());
					break;
				case FactionResource.Volatiles:
					this.Volatiles.SetText(stringBuilder.ToString());
					break;
				case FactionResource.Metals:
					this.Metals.SetText(stringBuilder.ToString());
					break;
				case FactionResource.NobleMetals:
					this.Nobles.SetText(stringBuilder.ToString());
					break;
				case FactionResource.Fissiles:
					this.Fissiles.SetText(stringBuilder.ToString());
					break;
				}
				stringBuilder.Clear();
			}
		}

		// Token: 0x0600568B RID: 22155 RVA: 0x0027968C File Offset: 0x0027788C
		public void SetSiteNameText(bool showSiteName)
		{
			if (this.site == null)
			{
				return;
			}
			string text = this.site.displayName;
			if (!showSiteName && this.site.hasPlannedOrOperatingBase)
			{
				text = this.site.hab.displayName;
				this.tip.SetDelegate("BodyText", () => this.site.hab.BuildShortHabSummary(GameControl.control.activePlayer));
			}
			else
			{
				this.tip.SetDelegate("BodyText", () => this.SetStatusTip(this.statusTipValue));
			}
			StringBuilder stringBuilder = new StringBuilder(text);
			if (this.site.irradiated)
			{
				stringBuilder.Insert(0, TemplateManager.global.irradiatedInlineSpritePath);
			}
			if (this.victory)
			{
				stringBuilder.Append(TIGlobalConfig.globalConfig.victoryItemInlineSpritePath);
			}
			if (this.site.hab != null && (this.site.hab.underAssault || this.site.hab.underBombardment))
			{
				stringBuilder.Append(TIGlobalConfig.globalConfig.armyBattleInlineSpritePath);
			}
			this.SiteName.SetText(stringBuilder.ToString());
		}

		// Token: 0x0600568C RID: 22156 RVA: 0x002797A8 File Offset: 0x002779A8
		public void OnSiteButtonClicked()
		{
			if (GeneralControlsController.CurrentlyTargetingStateType(typeof(TIHabSiteState)))
			{
				SoundEffectController.PlaySelectSound(this.site);
				GameControl.eventManager.TriggerEvent(new HabSiteSelectedEvent(this.site), null, new object[] { this.site });
				TIUtilities.GotoGameState(this.site, true, false, true, true, false, -1f);
				return;
			}
			if (GeneralControlsController.CurrentlyTargetingStateType(typeof(TIHabState)) && this.site.hasPlannedOrOperatingBase)
			{
				SoundEffectController.PlaySelectSound(this.site);
				GameControl.eventManager.TriggerEvent(new HabSelectedEvent(this.site.hab), null, new object[] { this.site.hab });
				TIUtilities.GotoGameState(this.site, true, false, true, true, false, -1f);
				return;
			}
			if (!this.site.hasPlannedOrOperatingBase)
			{
				SoundEffectController.PlaySelectSound(this.site);
				TIUtilities.GotoGameState(this.site, true, true, true, true, false, -1f);
				return;
			}
			SoundEffectController.PlaySelectSound(this.site.hab);
			TIUtilities.GotoGameState(this.site.hab, false, true, true, false, true, -1f);
		}

		// Token: 0x0600568D RID: 22157 RVA: 0x002798D2 File Offset: 0x00277AD2
		public void OnBaseIconClicked()
		{
			if (this.site.hasPlannedOrOperatingBase)
			{
				SoundEffectController.PlaySelectSound(this.site.hab);
				this.controller.HabSelectedFromSiteList(this.site.hab);
				return;
			}
			this.OnSiteButtonClicked();
		}

		// Token: 0x0600568E RID: 22158 RVA: 0x00279910 File Offset: 0x00277B10
		public string SetStatusTip(int setting)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(this.site.displayName);
			switch (setting)
			{
			case 0:
				stringBuilder.AppendLine(this.site.hab.displayName).AppendLine(this.site.hab.description);
				break;
			case 1:
				stringBuilder.AppendLine(Loc.T("UI.Space.Prospected"));
				break;
			case 2:
				stringBuilder.AppendLine(Loc.T("UI.Space.ProspectorEnRoute"));
				break;
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(Loc.T("UI.Space.SitesTooltip"));
			stringBuilder.AppendLine().AppendLine(Loc.T("UI.Space.SolarMultiplier", new object[]
			{
				TemplateManager.global.pathInlineSolarIcon,
				TIUtilities.HighlightLine(TIUtilities.FormatSmallNumber(this.site.solarMultiplier, 7, 1, true, false))
			}));
			if (this.site.IsIrradiated())
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Space.HabSiteHazard", new object[]
				{
					TemplateManager.global.irradiatedInlineSpritePath,
					TIUtilities.RedLine(this.site.irradiatedValue.ToString())
				}));
			}
			if (GameControl.control.activePlayer.CanExplore(this.site.parentBody) && GameControl.control.activePlayer.AlienTerritoryToAvoid(this.site.parentBody))
			{
				stringBuilder.AppendLine(TIUtilities.RedLine(Loc.T("UI.Space.AlienTerritory")));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x04003D9E RID: 15774
		private TIHabSiteState site;

		// Token: 0x04003D9F RID: 15775
		public TMP_Text SiteName;

		// Token: 0x04003DA0 RID: 15776
		public Image statusImage;

		// Token: 0x04003DA1 RID: 15777
		public TMP_Text Water;

		// Token: 0x04003DA2 RID: 15778
		public TMP_Text Volatiles;

		// Token: 0x04003DA3 RID: 15779
		public TMP_Text Metals;

		// Token: 0x04003DA4 RID: 15780
		public TMP_Text Nobles;

		// Token: 0x04003DA5 RID: 15781
		public TMP_Text Fissiles;

		// Token: 0x04003DA6 RID: 15782
		public TooltipTrigger tip;

		// Token: 0x04003DA7 RID: 15783
		public bool victory;

		// Token: 0x04003DA8 RID: 15784
		private int statusTipValue = 3;

		// Token: 0x04003DA9 RID: 15785
		private SpaceObjectDetailController controller;
	}
}

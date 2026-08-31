using System;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200082E RID: 2094
	public class ArmyDetailController : CanvasControllerBase
	{
		// Token: 0x17000EA1 RID: 3745
		// (get) Token: 0x06004B14 RID: 19220 RVA: 0x001F5429 File Offset: 0x001F3629
		// (set) Token: 0x06004B15 RID: 19221 RVA: 0x001F5430 File Offset: 0x001F3630
		public static ArmyDetailController Singleton { get; private set; }

		// Token: 0x06004B16 RID: 19222 RVA: 0x001F5438 File Offset: 0x001F3638
		public override void Initialize()
		{
			ArmyDetailController.Singleton = this;
			base.Initialize();
			GameControl.eventManager.AddListener<ArmyMapItemSelected>(new EventManager.EventDelegate<ArmyMapItemSelected>(this.ArmySelected), null, null, true, false);
			this.myArmyInfoPanel.gameObject.SetActive(true);
			this.myArmyInfoPanel.enabled = false;
			this.otherArmyInfoPanel.gameObject.SetActive(true);
			this.otherArmyInfoPanel.enabled = false;
			base.canvasManager.RegisterInfoPanelDisableOrder(InfoPanel.ArmyDetail, new Action(this.CloseOtherArmyDisplay));
			base.canvasManager.RegisterAssetPanelDisableOrder(AssetPanel.MyArmy, new Action(this.CloseMyArmyDisplay));
			this.SetArmyPanelHeader(this.myArmy, this.myArmyHeaderText);
			this.SetArmyPanelHeader(this.otherArmy, this.otherArmyHeaderText);
			this.myArmyNavalTooltip.SetText("BodyText", Loc.T("UI.Nation.ArmiesTab.NavyPresent"));
			this.otherArmyNavalTooltip.SetText("BodyText", Loc.T("UI.Nation.ArmiesTab.NavyPresent"));
			this.myArmyStandingOrdersTip.SetDelegate("BodyText", () => Loc.T("UI.Army.HuntingXenosTip"));
		}

		// Token: 0x06004B17 RID: 19223 RVA: 0x001F555C File Offset: 0x001F375C
		public override void Show()
		{
			base.Show();
			GameControl.eventManager.AddListener<InfoScreenOpened>(new EventManager.EventDelegate<InfoScreenOpened>(this.OnInfoScreenOpened), null, null, true, false);
			this.myArmyInfoPanel.gameObject.SetActive(true);
			this.otherArmyInfoPanel.gameObject.SetActive(true);
			this.Refresh();
		}

		// Token: 0x06004B18 RID: 19224 RVA: 0x001F55B4 File Offset: 0x001F37B4
		public override void Hide()
		{
			GameControl.eventManager.RemoveListener<InfoScreenOpened>(new EventManager.EventDelegate<InfoScreenOpened>(this.OnInfoScreenOpened), null);
			this.myArmyInfoPanel.gameObject.SetActive(false);
			this.otherArmyInfoPanel.gameObject.SetActive(false);
			base.Hide();
		}

		// Token: 0x06004B19 RID: 19225 RVA: 0x001F5600 File Offset: 0x001F3800
		public override void Refresh()
		{
			if (this.myArmy != null)
			{
				if (this.myArmyDataDirty && this.myArmyInfoPanel.enabled)
				{
					this.UpdateMyArmyDisplay();
				}
				this.myArmyDataDirty = false;
			}
			if (this.otherArmy != null)
			{
				if (this.otherArmyDataDirty && this.otherArmyInfoPanel.enabled)
				{
					this.UpdateOtherArmyDisplay();
				}
				this.otherArmyDataDirty = false;
			}
		}

		// Token: 0x06004B1A RID: 19226 RVA: 0x001F566D File Offset: 0x001F386D
		public void OnClickRename()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			this.nameInputField.text = this.myArmy.GetDisplayName(this.myArmy.faction);
			this.ShowRenameMyArmyPanel();
		}

		// Token: 0x06004B1B RID: 19227 RVA: 0x001F56A2 File Offset: 0x001F38A2
		public void OnClickRevertRename()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.RevertRename();
		}

		// Token: 0x06004B1C RID: 19228 RVA: 0x001F56B6 File Offset: 0x001F38B6
		public void RevertRename()
		{
			this.renameMyArmyPanel.SetActive(false);
			this.nameInputField.text = "";
		}

		// Token: 0x06004B1D RID: 19229 RVA: 0x001F56D4 File Offset: 0x001F38D4
		public void OnClickSaveName()
		{
			if (this.myArmy == null)
			{
				this.RevertRename();
				return;
			}
			this.renameMyArmyPanel.SetActive(false);
			this.myArmy.faction.playerControl.StartAction(new ChangeArmyBio(this.myArmy, this.nameInputField.text, this.nameInputField.text));
			this.UpdateMyArmyDisplay();
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
		}

		// Token: 0x06004B1E RID: 19230 RVA: 0x001F574A File Offset: 0x001F394A
		public void ShowRenameMyArmyPanel()
		{
			this.renameMyArmyPanel.SetActive(true);
			this.nameInputField.Select();
		}

		// Token: 0x06004B1F RID: 19231 RVA: 0x001F5763 File Offset: 0x001F3963
		public void OnSelectInputBox()
		{
			TIInputManager.BlockKeybindings();
		}

		// Token: 0x06004B20 RID: 19232 RVA: 0x001F576A File Offset: 0x001F396A
		public void OnDeSelectInputBox()
		{
			TIInputManager.RestoreKeybindings();
		}

		// Token: 0x06004B21 RID: 19233 RVA: 0x001F5771 File Offset: 0x001F3971
		private void OnMyArmyUpdated(ArmyStatusUpdate e)
		{
			this.myArmyDataDirty = true;
		}

		// Token: 0x06004B22 RID: 19234 RVA: 0x001F577A File Offset: 0x001F397A
		private void OnOtherArmyUpdated(ArmyStatusUpdate e)
		{
			this.otherArmyDataDirty = true;
		}

		// Token: 0x06004B23 RID: 19235 RVA: 0x001F5784 File Offset: 0x001F3984
		private void ArmySelected(TIArmyState army)
		{
			if (army == null)
			{
				this.CheckForCanvasShutdown();
				return;
			}
			if (!this.Visible())
			{
				this.Show();
				this.myArmyInfoPanel.enabled = this.myArmy != null;
				this.otherArmyInfoPanel.enabled = this.otherArmy != null;
			}
			if (army.faction == base.activePlayer)
			{
				this.RemoveMyArmyListeners();
				this.myArmy = army;
				GeneralControlsController.SetUISelectedAssetState(this.myArmy);
				this.AddMyArmyListeners();
				this.UpdateMyArmyDisplay();
				return;
			}
			this.RemoveOtherArmyListeners();
			this.otherArmy = army;
			GeneralControlsController.SetUIOtherSelectedState(this.otherArmy);
			this.AddOtherArmyListeners();
			this.UpdateOtherArmyDisplay();
		}

		// Token: 0x06004B24 RID: 19236 RVA: 0x001F5839 File Offset: 0x001F3A39
		private void ArmySelected(ArmyMapItemSelected e)
		{
			this.ArmySelected(e.army);
		}

		// Token: 0x06004B25 RID: 19237 RVA: 0x001F5847 File Offset: 0x001F3A47
		private void OnInfoScreenOpened(InfoScreenOpened e)
		{
			if (this.Visible())
			{
				this.Hide();
				GameControl.eventManager.AddListener<InfoScreenClosed>(new EventManager.EventDelegate<InfoScreenClosed>(this.RestoreArmyDetailCanvas), null, null, true, false);
			}
		}

		// Token: 0x06004B26 RID: 19238 RVA: 0x001F5871 File Offset: 0x001F3A71
		private void RestoreArmyDetailCanvas(InfoScreenClosed e)
		{
			this.Show();
			GameControl.eventManager.RemoveListener<InfoScreenClosed>(new EventManager.EventDelegate<InfoScreenClosed>(this.RestoreArmyDetailCanvas), null);
		}

		// Token: 0x06004B27 RID: 19239 RVA: 0x001F5890 File Offset: 0x001F3A90
		private void OnMyArmyDestroyed(ArmyMajorStatusUpdate e)
		{
			base.canvasManager.SetActiveAssetPanel(AssetPanel.None, 0f);
		}

		// Token: 0x06004B28 RID: 19240 RVA: 0x001F58A3 File Offset: 0x001F3AA3
		private void OnOtherArmyDestroyed(ArmyMajorStatusUpdate e)
		{
			base.canvasManager.SetActiveInfoPanel(InfoPanel.None, 0f);
		}

		// Token: 0x06004B29 RID: 19241 RVA: 0x001F58B8 File Offset: 0x001F3AB8
		private void AddMyArmyListeners()
		{
			GameControl.eventManager.AddListener<ArmyStatusUpdate>(new EventManager.EventDelegate<ArmyStatusUpdate>(this.OnMyArmyUpdated), this.myArmy.armyStatusUpdateEventName, this.myArmy, true, false);
			GameControl.eventManager.AddListener<ArmyMajorStatusUpdate>(new EventManager.EventDelegate<ArmyMajorStatusUpdate>(this.OnMyArmyDestroyed), null, this.myArmy, false, false);
		}

		// Token: 0x06004B2A RID: 19242 RVA: 0x001F5910 File Offset: 0x001F3B10
		private void RemoveMyArmyListeners()
		{
			if (this.myArmy != null)
			{
				GameControl.eventManager.RemoveListener<ArmyStatusUpdate>(new EventManager.EventDelegate<ArmyStatusUpdate>(this.OnMyArmyUpdated), this.myArmy.armyStatusUpdateEventName);
			}
			GameControl.eventManager.RemoveListener<ArmyMajorStatusUpdate>(new EventManager.EventDelegate<ArmyMajorStatusUpdate>(this.OnMyArmyDestroyed), null);
		}

		// Token: 0x06004B2B RID: 19243 RVA: 0x001F5964 File Offset: 0x001F3B64
		private void AddOtherArmyListeners()
		{
			GameControl.eventManager.AddListener<ArmyStatusUpdate>(new EventManager.EventDelegate<ArmyStatusUpdate>(this.OnOtherArmyUpdated), this.otherArmy.armyStatusUpdateEventName, this.otherArmy, true, false);
			GameControl.eventManager.AddListener<ArmyMajorStatusUpdate>(new EventManager.EventDelegate<ArmyMajorStatusUpdate>(this.OnOtherArmyDestroyed), null, this.otherArmy, false, false);
		}

		// Token: 0x06004B2C RID: 19244 RVA: 0x001F59BC File Offset: 0x001F3BBC
		private void RemoveOtherArmyListeners()
		{
			if (this.otherArmy != null)
			{
				GameControl.eventManager.RemoveListener<ArmyStatusUpdate>(new EventManager.EventDelegate<ArmyStatusUpdate>(this.OnOtherArmyUpdated), this.otherArmy.armyStatusUpdateEventName);
			}
			GameControl.eventManager.RemoveListener<ArmyMajorStatusUpdate>(new EventManager.EventDelegate<ArmyMajorStatusUpdate>(this.OnOtherArmyDestroyed), null);
		}

		// Token: 0x06004B2D RID: 19245 RVA: 0x001F5A0F File Offset: 0x001F3C0F
		private void SetArmyPanelHeader(TIArmyState army, TMP_Text textItem)
		{
			textItem.SetText(Loc.T("UI.Army.Header"));
		}

		// Token: 0x06004B2E RID: 19246 RVA: 0x001F5A21 File Offset: 0x001F3C21
		private void SetIllustration(TIArmyState army, Image illustrationImage)
		{
			if (!string.IsNullOrEmpty(army.illustration))
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(army.illustration, illustrationImage);
				illustrationImage.color = Color.white;
				return;
			}
			illustrationImage.color = Color.black;
		}

		// Token: 0x06004B2F RID: 19247 RVA: 0x001F5A58 File Offset: 0x001F3C58
		private void SetArmyName(TIArmyState army, TMP_Text textItem)
		{
			textItem.SetText(army.GetDisplayName(base.activePlayer));
		}

		// Token: 0x06004B30 RID: 19248 RVA: 0x001F5A6C File Offset: 0x001F3C6C
		private void SetNavalImage(TIArmyState army, Image imageItem)
		{
			imageItem.enabled = army.deploymentType == DeploymentType.Naval;
		}

		// Token: 0x06004B31 RID: 19249 RVA: 0x001F5A7D File Offset: 0x001F3C7D
		private void SetNationFlag(TIArmyState army, Image flagImage, GameObject flagContainer)
		{
			if (army.AlienMegafaunaArmy && !army.faction.IsAlienFaction)
			{
				flagImage.enabled = false;
				flagContainer.SetActive(false);
				return;
			}
			flagImage.sprite = army.homeNation.flag;
			flagContainer.SetActive(true);
		}

		// Token: 0x06004B32 RID: 19250 RVA: 0x001F5ABC File Offset: 0x001F3CBC
		private void SetArmyNationText(TIArmyState army, TMP_Text textItem)
		{
			if (!army.AlienMegafaunaArmy)
			{
				textItem.SetText(army.homeNation.displayName);
				return;
			}
			if (army.faction.IsAlienFaction)
			{
				textItem.SetText(Loc.T("UI.Army.MegafaunaHomeNation"));
				return;
			}
			textItem.SetText(Loc.T("UI.Army.MegafaunaHomeNation1"));
		}

		// Token: 0x06004B33 RID: 19251 RVA: 0x001F5B14 File Offset: 0x001F3D14
		private void SetArmyFactionControlIcon(TIArmyState army, Image factionImage, Image factionGradientImage)
		{
			if (army.faction != null && !army.AlienMegafaunaArmy)
			{
				factionImage.sprite = army.faction.factionIcon64UI;
				factionImage.enabled = true;
				GameControl.assetLoader.LoadAssetForImageAssignment(army.faction.template.gradientPath, factionGradientImage);
				return;
			}
			factionImage.enabled = false;
			GameControl.assetLoader.LoadAssetForImageAssignment(TemplateManager.global.pathUndecidedGradient, factionGradientImage);
		}

		// Token: 0x06004B34 RID: 19252 RVA: 0x001F5B87 File Offset: 0x001F3D87
		private void SetArmyForeground(TIArmyState army, Image foregroundImage)
		{
			foregroundImage.sprite = army.GetForegroundIcon();
		}

		// Token: 0x06004B35 RID: 19253 RVA: 0x001F5B98 File Offset: 0x001F3D98
		private void SetArmyMiltech(TIArmyState army, TMP_Text textItem)
		{
			ArmyType armyType = army.armyType;
			if (armyType - ArmyType.AlienMegafauna <= 1)
			{
				textItem.SetText(army.techLevel.ToString("N1"));
				return;
			}
			textItem.SetText(army.homeNation.GetMilitaryDescriptiveStringAndValue(1));
		}

		// Token: 0x06004B36 RID: 19254 RVA: 0x001F5BE0 File Offset: 0x001F3DE0
		private void SetHQRegionText(TIArmyState army, TMP_Text textItem, Image HQIcon)
		{
			if (army.AlienMegafaunaArmy || army.homeRegion == null)
			{
				HQIcon.enabled = false;
				textItem.enabled = false;
				return;
			}
			HQIcon.enabled = true;
			textItem.SetText(army.homeRegion.displayName);
			textItem.enabled = true;
		}

		// Token: 0x06004B37 RID: 19255 RVA: 0x001F5C34 File Offset: 0x001F3E34
		private void SetStrengthText(TIArmyState army, TMP_Text textItem)
		{
			string text = army.strength.ToPercent("P0");
			if (army.IsFighting(true))
			{
				text = TIUtilities.RedLine(text);
			}
			else if (army.CanHeal())
			{
				text = TIUtilities.GreenLine(text);
			}
			textItem.SetText(text);
		}

		// Token: 0x06004B38 RID: 19256 RVA: 0x001F5C7C File Offset: 0x001F3E7C
		private void SetLocationText(TIArmyState army, TMP_Text textItem)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (army.atSea)
			{
				stringBuilder.Append(Loc.T("UI.Army.AtSea"));
			}
			else
			{
				stringBuilder.Append(army.currentRegion.displayName);
				if (army.currentRegion.terrain == TerrainType.Rugged)
				{
					stringBuilder.Append(" ").Append(TemplateManager.global.ruggedRegionInlineSpritePath);
				}
			}
			textItem.SetText(stringBuilder);
		}

		// Token: 0x06004B39 RID: 19257 RVA: 0x001F5CEC File Offset: 0x001F3EEC
		private string SetHabTooltip(TIArmyState army)
		{
			return Loc.T("UI.Army.HabTooltip", new object[] { army.LEOHabBonus.ToString("N2") ?? "0" });
		}

		// Token: 0x06004B3A RID: 19258 RVA: 0x001F5D28 File Offset: 0x001F3F28
		private void SetHabBonus(TIArmyState army, Image habImage, TMP_Text textItem, TooltipTrigger tooltip)
		{
			if (!army.AlienMegafaunaArmy)
			{
				float leohabBonus = army.LEOHabBonus;
				if (leohabBonus > 0f)
				{
					textItem.SetText(Loc.T("UI.Army.AdviserBonus", new object[] { leohabBonus.ToString("N2") }));
					tooltip.SetDelegate("BodyText", () => this.SetHabTooltip(army));
					textItem.enabled = true;
					habImage.enabled = true;
					tooltip.enabled = true;
					return;
				}
			}
			textItem.enabled = false;
			habImage.enabled = false;
			tooltip.enabled = false;
		}

		// Token: 0x06004B3B RID: 19259 RVA: 0x001F5DD4 File Offset: 0x001F3FD4
		private string SetAdviserTooltip(TIArmyState army)
		{
			string text = "UI.Army.AdviserTooltip";
			object[] array = new object[1];
			int num = 0;
			TINationState homeNation = army.homeNation;
			array[num] = ((homeNation != null) ? homeNation.adviserCommandBonus.ToString("N2") : null) ?? "0";
			return Loc.T(text, array);
		}

		// Token: 0x06004B3C RID: 19260 RVA: 0x001F5E1C File Offset: 0x001F401C
		private void SetAdviserBonus(TIArmyState army, Image adviserImage, TMP_Text textItem, TooltipTrigger tooltip)
		{
			if (!army.AlienMegafaunaArmy)
			{
				TINationState homeNation = army.homeNation;
				float num = ((homeNation != null) ? homeNation.adviserCommandBonus : 0f);
				if (num > 0f)
				{
					GameControl.assetLoader.LoadAssetForImageAssignment(army.homeNation.advisingCouncilors.MaxBy<TICouncilorState, int>((TICouncilorState x) => x.GetAttribute(CouncilorAttribute.Command, true, true, true, false, false, false)).iconResource, adviserImage);
					textItem.SetText(Loc.T("UI.Army.AdviserBonus", new object[] { num.ToString("N2") }));
					tooltip.SetDelegate("BodyText", () => this.SetAdviserTooltip(army));
					textItem.enabled = true;
					adviserImage.enabled = true;
					tooltip.enabled = true;
					return;
				}
			}
			textItem.enabled = false;
			adviserImage.enabled = false;
			tooltip.enabled = false;
		}

		// Token: 0x06004B3D RID: 19261 RVA: 0x001F5F24 File Offset: 0x001F4124
		private void SetOperationData(TIArmyState army, Image opIcon, TMP_Text textItem)
		{
			textItem.SetText(army.OperationDescription());
			if (army.CurrentOperations().Count > 0)
			{
				IOperation operation = army.CurrentOperations()[0].operation;
				GameControl.assetLoader.LoadAssetForImageAssignment(operation.GetOperationIconImagePath_Off(), opIcon);
				opIcon.gameObject.SetActive(true);
				return;
			}
			if (army.InBattleWithArmies())
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(TemplateManager.global.pathArmyCombatIcon, opIcon);
				opIcon.gameObject.SetActive(true);
				return;
			}
			if (army.OccupyingRegion(true))
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(TemplateManager.global.pathOccupationIcon, opIcon);
				opIcon.gameObject.SetActive(true);
				return;
			}
			if (army.huntingXenofauna)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(OperationsManager.operationsLookup[typeof(SetHuntXenoformingOperation)].GetOperationIconImagePath_Off(), opIcon);
				opIcon.gameObject.SetActive(true);
				return;
			}
			opIcon.gameObject.SetActive(false);
		}

		// Token: 0x06004B3E RID: 19262 RVA: 0x001F6018 File Offset: 0x001F4218
		private void UpdateMyArmyDisplay()
		{
			if (this.myArmy != null && !this.myArmy.destroyed && this.myArmy.faction == base.activePlayer)
			{
				if (!this.myArmyInfoPanel.enabled)
				{
					base.canvasManager.SetActiveAssetPanel(AssetPanel.MyArmy, this.myArmyInfoPanel.gameObject.GetComponent<RectTransform>().sizeDelta.y);
					this.myArmyInfoPanel.enabled = true;
				}
				this.SetIllustration(this.myArmy, this.myArmyIllustration);
				this.SetArmyName(this.myArmy, this.myArmyName);
				this.SetNavalImage(this.myArmy, this.myArmyNavalImage);
				this.SetNationFlag(this.myArmy, this.myArmyFlag, this.myArmyFlagContainer);
				this.SetArmyNationText(this.myArmy, this.myArmyNationName);
				this.SetArmyFactionControlIcon(this.myArmy, this.myArmyFaction, this.myArmyFactionGradient);
				this.SetArmyForeground(this.myArmy, this.myArmyImage);
				this.myArmyImageBackground.color = this.myArmy.GetIconBackgroundResourceColor;
				this.SetArmyMiltech(this.myArmy, this.myArmyTechLevel);
				this.SetHQRegionText(this.myArmy, this.myArmyHomeRegion, this.myArmyHomeRegionIcon);
				this.SetStrengthText(this.myArmy, this.myArmyStrength);
				this.SetLocationText(this.myArmy, this.myArmyLocation);
				this.SetAdviserBonus(this.myArmy, this.myArmyAdviserIcon, this.myArmyAdviserBonus, this.myArmyAdviserTooltip);
				this.SetOperationData(this.myArmy, this.myArmyOperationIcon, this.myArmyOperation);
				this.myArmyStandingOrders.gameObject.SetActive(this.myArmy.huntingXenofauna);
				this.SetHabBonus(this.myArmy, this.myArmyHabsIcon, this.myArmyHabsBonus, this.myArmyHabsBonusTip);
				this.RevertRename();
				base.canvasManager.ActiveAssetPanelResized(this.myArmyInfoPanel.gameObject.GetComponent<RectTransform>().sizeDelta.y);
				return;
			}
			base.canvasManager.SetActiveAssetPanel(AssetPanel.None, 0f);
		}

		// Token: 0x06004B3F RID: 19263 RVA: 0x001F623C File Offset: 0x001F443C
		private void UpdateOtherArmyDisplay()
		{
			if (this.otherArmy != null && this.otherArmy.faction != base.activePlayer && !this.otherArmy.destroyed)
			{
				if (!this.otherArmyInfoPanel.enabled)
				{
					this.otherArmyInfoPanel.enabled = true;
					base.canvasManager.SetActiveInfoPanel(InfoPanel.ArmyDetail, this.otherArmyInfoPanel.gameObject.GetComponent<RectTransform>().sizeDelta.y);
				}
				this.SetArmyName(this.otherArmy, this.otherArmyName);
				this.SetIllustration(this.otherArmy, this.otherArmyIllustration);
				this.SetNavalImage(this.otherArmy, this.otherArmyNavalImage);
				this.SetNationFlag(this.otherArmy, this.otherArmyFlag, this.otherArmyFlagContainer);
				this.SetArmyNationText(this.otherArmy, this.otherArmyNationName);
				this.SetArmyFactionControlIcon(this.otherArmy, this.otherArmyFaction, this.otherArmyFactionGradient);
				this.SetArmyForeground(this.otherArmy, this.otherArmyImage);
				this.otherArmyImageBackground.color = this.otherArmy.GetIconBackgroundResourceColor;
				this.SetArmyMiltech(this.otherArmy, this.otherArmyTechLevel);
				this.SetHQRegionText(this.otherArmy, this.otherArmyHomeRegion, this.otherArmyHomeRegionIcon);
				this.SetStrengthText(this.otherArmy, this.otherArmyStrength);
				this.SetLocationText(this.otherArmy, this.otherArmyLocation);
				this.SetAdviserBonus(this.otherArmy, this.otherArmyAdviserIcon, this.otherArmyAdviserBonus, this.otherArmyAdviserTooltip);
				this.SetHabBonus(this.otherArmy, this.otherArmyHabsIcon, this.otherArmyHabsBonus, this.otherArmyHabsBonusTip);
				this.SetOperationData(this.otherArmy, this.otherArmyOperationIcon, this.otherArmyOperation);
				return;
			}
			base.canvasManager.SetActiveInfoPanel(InfoPanel.None, 0f);
		}

		// Token: 0x06004B40 RID: 19264 RVA: 0x001F6418 File Offset: 0x001F4618
		public void OnClickOtherArmyFlag()
		{
			TIUtilities.GotoGameState(this.otherArmy.homeRegion, true, true, true, true, false, -1f);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseLarge", false, false);
		}

		// Token: 0x06004B41 RID: 19265 RVA: 0x001F6440 File Offset: 0x001F4640
		public void OnClickOtherArmyGoto()
		{
			TIUtilities.GotoGameState(this.otherArmy, true, true, true, true, false, -1f);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OtherHumanArmySelect", false, false);
		}

		// Token: 0x06004B42 RID: 19266 RVA: 0x001F6463 File Offset: 0x001F4663
		public void OnClickMyArmyFlag()
		{
			TIUtilities.GotoGameState(this.myArmy.homeRegion, true, true, true, true, false, -1f);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericSelect", false, false);
		}

		// Token: 0x06004B43 RID: 19267 RVA: 0x001F648B File Offset: 0x001F468B
		public void OnClickMyArmyGoto()
		{
			TIUtilities.GotoGameState(this.myArmy, true, true, true, true, false, -1f);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_MyArmySelect", false, false);
		}

		// Token: 0x06004B44 RID: 19268 RVA: 0x001F64AE File Offset: 0x001F46AE
		public void OnClickCloseMyArmyDisplay()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericSelect", false, false);
			base.canvasManager.SetActiveAssetPanel(AssetPanel.None, 0f);
		}

		// Token: 0x06004B45 RID: 19269 RVA: 0x001F64CD File Offset: 0x001F46CD
		public void CloseMyArmyDisplay()
		{
			this.myArmy = null;
			if (this.myArmyInfoPanel != null)
			{
				this.myArmyInfoPanel.enabled = false;
			}
			this.CheckForCanvasShutdown();
		}

		// Token: 0x06004B46 RID: 19270 RVA: 0x001F64F6 File Offset: 0x001F46F6
		public void OnClickExitOtherArmyDisplay()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			base.canvasManager.SetActiveInfoPanel(InfoPanel.None, 0f);
		}

		// Token: 0x06004B47 RID: 19271 RVA: 0x001F6515 File Offset: 0x001F4715
		public void CloseOtherArmyDisplay()
		{
			if (this.otherArmyInfoPanel != null)
			{
				this.otherArmyInfoPanel.enabled = false;
			}
			this.otherArmy = null;
			this.CheckForCanvasShutdown();
		}

		// Token: 0x06004B48 RID: 19272 RVA: 0x001F653E File Offset: 0x001F473E
		public void CheckForCanvasShutdown()
		{
			if (this.myArmyInfoPanel != null && !this.myArmyInfoPanel.enabled && this.otherArmyInfoPanel != null && !this.otherArmyInfoPanel.enabled)
			{
				this.Hide();
			}
		}

		// Token: 0x04002BB8 RID: 11192
		[Header("My Army Data")]
		public TIArmyState myArmy;

		// Token: 0x04002BB9 RID: 11193
		public Canvas myArmyInfoPanel;

		// Token: 0x04002BBA RID: 11194
		public TMP_Text myArmyHeaderText;

		// Token: 0x04002BBB RID: 11195
		public Image myArmyIllustration;

		// Token: 0x04002BBC RID: 11196
		public TMP_Text myArmyName;

		// Token: 0x04002BBD RID: 11197
		public Image myArmyNavalImage;

		// Token: 0x04002BBE RID: 11198
		public TooltipTrigger myArmyNavalTooltip;

		// Token: 0x04002BBF RID: 11199
		public Image myArmyImage;

		// Token: 0x04002BC0 RID: 11200
		public Image myArmyImageBackground;

		// Token: 0x04002BC1 RID: 11201
		public GameObject myArmyFlagContainer;

		// Token: 0x04002BC2 RID: 11202
		public Image myArmyFlag;

		// Token: 0x04002BC3 RID: 11203
		public Image myArmyFaction;

		// Token: 0x04002BC4 RID: 11204
		public Image myArmyFactionGradient;

		// Token: 0x04002BC5 RID: 11205
		public TMP_Text myArmyNationName;

		// Token: 0x04002BC6 RID: 11206
		public TMP_Text myArmyTechLevel;

		// Token: 0x04002BC7 RID: 11207
		public TMP_Text myArmyStrength;

		// Token: 0x04002BC8 RID: 11208
		public Image myArmyHabsIcon;

		// Token: 0x04002BC9 RID: 11209
		public TMP_Text myArmyHabsBonus;

		// Token: 0x04002BCA RID: 11210
		public TooltipTrigger myArmyHabsBonusTip;

		// Token: 0x04002BCB RID: 11211
		public Image myArmyAdviserIcon;

		// Token: 0x04002BCC RID: 11212
		public TMP_Text myArmyAdviserBonus;

		// Token: 0x04002BCD RID: 11213
		public TooltipTrigger myArmyAdviserTooltip;

		// Token: 0x04002BCE RID: 11214
		public Image myArmyHomeRegionIcon;

		// Token: 0x04002BCF RID: 11215
		public TMP_Text myArmyHomeRegion;

		// Token: 0x04002BD0 RID: 11216
		public TMP_Text myArmyLocation;

		// Token: 0x04002BD1 RID: 11217
		public Image myArmyOperationIcon;

		// Token: 0x04002BD2 RID: 11218
		public TMP_Text myArmyOperation;

		// Token: 0x04002BD3 RID: 11219
		public Image myArmyStandingOrders;

		// Token: 0x04002BD4 RID: 11220
		public TooltipTrigger myArmyStandingOrdersTip;

		// Token: 0x04002BD5 RID: 11221
		[Header("Other Army Data")]
		public TIArmyState otherArmy;

		// Token: 0x04002BD6 RID: 11222
		public Canvas otherArmyInfoPanel;

		// Token: 0x04002BD7 RID: 11223
		public TMP_Text otherArmyHeaderText;

		// Token: 0x04002BD8 RID: 11224
		public Image otherArmyIllustration;

		// Token: 0x04002BD9 RID: 11225
		public TMP_Text otherArmyName;

		// Token: 0x04002BDA RID: 11226
		public Image otherArmyNavalImage;

		// Token: 0x04002BDB RID: 11227
		public TooltipTrigger otherArmyNavalTooltip;

		// Token: 0x04002BDC RID: 11228
		public Image otherArmyImage;

		// Token: 0x04002BDD RID: 11229
		public Image otherArmyImageBackground;

		// Token: 0x04002BDE RID: 11230
		public GameObject otherArmyFlagContainer;

		// Token: 0x04002BDF RID: 11231
		public Image otherArmyFlag;

		// Token: 0x04002BE0 RID: 11232
		public Image otherArmyFaction;

		// Token: 0x04002BE1 RID: 11233
		public Image otherArmyFactionGradient;

		// Token: 0x04002BE2 RID: 11234
		public TMP_Text otherArmyNationName;

		// Token: 0x04002BE3 RID: 11235
		public TMP_Text otherArmyTechLevel;

		// Token: 0x04002BE4 RID: 11236
		public TMP_Text otherArmyStrength;

		// Token: 0x04002BE5 RID: 11237
		public Image otherArmyHabsIcon;

		// Token: 0x04002BE6 RID: 11238
		public TMP_Text otherArmyHabsBonus;

		// Token: 0x04002BE7 RID: 11239
		public TooltipTrigger otherArmyHabsBonusTip;

		// Token: 0x04002BE8 RID: 11240
		public Image otherArmyAdviserIcon;

		// Token: 0x04002BE9 RID: 11241
		public TMP_Text otherArmyAdviserBonus;

		// Token: 0x04002BEA RID: 11242
		public TooltipTrigger otherArmyAdviserTooltip;

		// Token: 0x04002BEB RID: 11243
		public TMP_Text otherArmyHomeRegion;

		// Token: 0x04002BEC RID: 11244
		public Image otherArmyHomeRegionIcon;

		// Token: 0x04002BED RID: 11245
		public TMP_Text otherArmyLocation;

		// Token: 0x04002BEE RID: 11246
		public Image otherArmyOperationIcon;

		// Token: 0x04002BEF RID: 11247
		public TMP_Text otherArmyOperation;

		// Token: 0x04002BF0 RID: 11248
		[Header("My Army Customization")]
		public GameObject renameMyArmyPanel;

		// Token: 0x04002BF1 RID: 11249
		public TMP_InputField nameInputField;

		// Token: 0x04002BF2 RID: 11250
		private bool myArmyDataDirty;

		// Token: 0x04002BF3 RID: 11251
		private bool otherArmyDataDirty;
	}
}

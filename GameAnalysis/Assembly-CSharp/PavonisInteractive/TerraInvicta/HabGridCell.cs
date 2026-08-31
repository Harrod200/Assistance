using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000869 RID: 2153
	public abstract class HabGridCell : DragDestination
	{
		// Token: 0x17000ECD RID: 3789
		// (get) Token: 0x06004FC6 RID: 20422 RVA: 0x002272F4 File Offset: 0x002254F4
		// (set) Token: 0x06004FC7 RID: 20423 RVA: 0x002272FC File Offset: 0x002254FC
		public TIHabModuleState habModule { get; protected set; }

		// Token: 0x06004FC8 RID: 20424
		public abstract void SetPreviewer(IHabitatsPreviewer previewer);

		// Token: 0x06004FC9 RID: 20425
		protected abstract void CacheComponents();

		// Token: 0x06004FCA RID: 20426
		protected abstract void AddListeners();

		// Token: 0x06004FCB RID: 20427
		public abstract void SetModule(string imageName, bool playerControlled, bool alien, TIHabModuleState module, TIHabState hab);

		// Token: 0x06004FCC RID: 20428 RVA: 0x00227305 File Offset: 0x00225505
		private void Awake()
		{
			this.Init();
		}

		// Token: 0x06004FCD RID: 20429 RVA: 0x0022730D File Offset: 0x0022550D
		private void Start()
		{
			this.Init();
		}

		// Token: 0x06004FCE RID: 20430 RVA: 0x00227318 File Offset: 0x00225518
		private void Init()
		{
			if (this.hasInit)
			{
				return;
			}
			this.CacheComponents();
			this.AddListeners();
			this.hasInit = true;
			this.cellImage.color = this.hiddenColor;
			this.tooltip = base.gameObject.GetComponent<TooltipTrigger>();
			if (this.tooltip != null)
			{
				this.tooltip.maxTextWidth = 450;
			}
		}

		// Token: 0x06004FCF RID: 20431 RVA: 0x00227384 File Offset: 0x00225584
		protected string SetTooltip()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.isEmpty)
			{
				if (this.habType == HabType.Base)
				{
					if (TIHabState.IsMineSlot(this.sector, this.module, this.habType))
					{
						stringBuilder.AppendLine(Loc.T("UI.Habs.MineSlot"));
					}
					else
					{
						stringBuilder.AppendLine(Loc.T("UI.Habs.EmptyBase"));
					}
				}
				else
				{
					stringBuilder.AppendLine(Loc.T("UI.Habs.EmptyStation"));
				}
			}
			else if (this.connectionSectorOnly)
			{
				stringBuilder.AppendLine("");
			}
			else
			{
				stringBuilder.AppendLine(TIHabModuleState.FullSummary(this.habModule, true));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06004FD0 RID: 20432 RVA: 0x0022742C File Offset: 0x0022562C
		protected void CommonCacheComponents()
		{
			string[] array = base.name.Split(new char[] { '_' });
			if (array[0] == "C")
			{
				this.connectionSectorOnly = true;
				this.sector = (this.module = -1);
			}
			else
			{
				this.sector = int.Parse(array[0].Replace("S", ""));
				this.module = int.Parse(array[1].Replace("M", ""));
			}
			this.gridLayoutGroup = base.GetComponent<GridLayoutGroup>();
			this.cellButton = base.GetComponent<Button>();
			this.cellImage = base.GetComponent<Image>();
			this.moduleImage = base.gameObject.GetComponentOnChild<Image>("ModuleImage");
			this.factionIcon = base.gameObject.GetComponentOnChild<Image>("FactionIcon");
			this.powerIcon = base.gameObject.GetComponentOnChild<Image>("PowerIcon");
			if (this.moduleImage != null)
			{
				this.moduleCellRectTransform = this.moduleImage.rectTransform;
			}
			this.moduleCellConnectorImage = base.gameObject.GetComponentOnChild<Image>("Module");
			this.N2 = base.gameObject.GetComponentOnChild<Image>("N2");
			this.N1 = base.gameObject.GetComponentOnChild<Image>("N1");
			this.W2 = base.gameObject.GetComponentOnChild<Image>("W2");
			this.W1 = base.gameObject.GetComponentOnChild<Image>("W1");
			this.E2 = base.gameObject.GetComponentOnChild<Image>("E2");
			this.E1 = base.gameObject.GetComponentOnChild<Image>("E1");
			this.S2 = base.gameObject.GetComponentOnChild<Image>("S2");
			this.S1 = base.gameObject.GetComponentOnChild<Image>("S1");
			this.dragTarget = base.transform;
			this.dragItemType = DragItemType.HAB;
		}

		// Token: 0x06004FD1 RID: 20433 RVA: 0x00227610 File Offset: 0x00225810
		public virtual void SetInteractable(bool interactable)
		{
			if (this.cellButton != null)
			{
				this.cellButton.interactable = interactable;
			}
		}

		// Token: 0x06004FD2 RID: 20434 RVA: 0x0022762C File Offset: 0x0022582C
		protected void OnSelected()
		{
			if (this.sectorIsPlayerControlled)
			{
				if (!this.Previewer.IsManaging())
				{
					this.Previewer.ManageHab();
				}
				this.Previewer.SelectModule(this);
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_MyHabSelect", false, false);
			}
		}

		// Token: 0x06004FD3 RID: 20435 RVA: 0x00227666 File Offset: 0x00225866
		public virtual void SetGridCellSize(Vector2 size)
		{
			this.gridLayoutGroup.cellSize = size;
			if (this.moduleCellRectTransform != null)
			{
				this.moduleCellRectTransform.sizeDelta = size;
			}
		}

		// Token: 0x06004FD4 RID: 20436 RVA: 0x0022768E File Offset: 0x0022588E
		protected int GetTier(string imageName)
		{
			if (imageName.Contains("T1"))
			{
				return 1;
			}
			if (imageName.Contains("T2"))
			{
				return 2;
			}
			if (imageName.Contains("T3"))
			{
				return 3;
			}
			Debug.Log("cannot parse tier from module imageName " + imageName);
			return 0;
		}

		// Token: 0x06004FD5 RID: 20437 RVA: 0x002276D0 File Offset: 0x002258D0
		public void SetConnectionSprite(Image connectionImage, bool alien)
		{
			if (connectionImage != null)
			{
				if (alien && !connectionImage.sprite.name.Contains("alien") && !connectionImage.sprite.name.Contains("Alien"))
				{
					string text = this.habitatsController.connectorSwaps[connectionImage.sprite.name];
					connectionImage.sprite = this.habitatsController.connectors[text];
					return;
				}
				if ((!alien && connectionImage.sprite.name.Contains("alien")) || connectionImage.sprite.name.Contains("Alien"))
				{
					string key = this.habitatsController.connectorSwaps.FirstOrDefault<KeyValuePair<string, string>>((KeyValuePair<string, string> x) => x.Value == connectionImage.sprite.name).Key;
					connectionImage.sprite = this.habitatsController.connectors[key];
				}
			}
		}

		// Token: 0x06004FD6 RID: 20438 RVA: 0x002277F0 File Offset: 0x002259F0
		public void SetAllConnectionSprites(bool alien)
		{
			this.SetConnectionSprite(this.N1, alien);
			this.SetConnectionSprite(this.N2, alien);
			this.SetConnectionSprite(this.W1, alien);
			this.SetConnectionSprite(this.W2, alien);
			this.SetConnectionSprite(this.E1, alien);
			this.SetConnectionSprite(this.E2, alien);
			this.SetConnectionSprite(this.S1, alien);
			this.SetConnectionSprite(this.S2, alien);
			this.SetConnectionSprite(this.moduleCellConnectorImage, alien);
		}

		// Token: 0x06004FD7 RID: 20439 RVA: 0x00227874 File Offset: 0x00225A74
		public void UpdateConnections(bool hide = false)
		{
			if (this.connectionSectorOnly)
			{
				if (this.N1 != null)
				{
					this.N1.enabled = !hide;
				}
				if (this.N2 != null)
				{
					this.N2.enabled = !hide;
				}
				if (this.W1 != null)
				{
					this.W1.enabled = !hide;
				}
				if (this.W2 != null)
				{
					this.W2.enabled = !hide;
				}
				if (this.S1 != null)
				{
					this.S1.enabled = !hide;
				}
				if (this.S2 != null)
				{
					this.S2.enabled = !hide;
				}
				if (this.E1 != null)
				{
					this.E1.enabled = !hide;
				}
				if (this.E2 != null)
				{
					this.E2.enabled = !hide;
				}
				if (this.moduleCellConnectorImage != null)
				{
					this.moduleCellConnectorImage.enabled = !hide;
					return;
				}
			}
			else
			{
				if (this.N1 != null)
				{
					this.N1.enabled = !hide && this.habModule.N1;
				}
				if (this.N2 != null)
				{
					this.N2.enabled = !hide && this.habModule.N2;
				}
				if (this.W1 != null)
				{
					this.W1.enabled = !hide && this.habModule.W1;
				}
				if (this.W2 != null)
				{
					this.W2.enabled = !hide && this.habModule.W2;
				}
				if (this.S1 != null)
				{
					this.S1.enabled = !hide && this.habModule.S1;
				}
				if (this.S2 != null)
				{
					this.S2.enabled = !hide && this.habModule.S2;
				}
				if (this.E1 != null)
				{
					this.E1.enabled = !hide && this.habModule.E1;
				}
				if (this.E2 != null)
				{
					this.E2.enabled = !hide && this.habModule.E2;
				}
				if (this.moduleCellConnectorImage != null)
				{
					this.moduleCellConnectorImage.enabled = !hide && this.habModule.C0;
				}
			}
		}

		// Token: 0x06004FD8 RID: 20440 RVA: 0x00227B10 File Offset: 0x00225D10
		public void Show()
		{
			this.sectorEnabled = true;
			if (this.isEmpty && !this.sectorIsPlayerControlled)
			{
				this.cellImage.enabled = false;
				this.moduleImage.enabled = false;
			}
			else
			{
				this.moduleImage.enabled = true;
				this.cellImage.enabled = true;
			}
			this.SetPowerIcon(false);
			this.SetDecommissioningVisuals();
			this.UpdateConnections(false);
		}

		// Token: 0x06004FD9 RID: 20441 RVA: 0x00227B7C File Offset: 0x00225D7C
		public void SetDecommissioningVisuals()
		{
			if (this.habModule != null)
			{
				if (this.habModule.decommissioning || (this.habModule.hab != null && this.habModule.hab.decommissioning))
				{
					this.moduleImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
					this.SetPowerIcon(false);
					return;
				}
				this.moduleImage.color = Color.white;
			}
		}

		// Token: 0x06004FDA RID: 20442 RVA: 0x00227C08 File Offset: 0x00225E08
		public void Hide()
		{
			this.sectorEnabled = false;
			this.SetPowerIcon(true);
			this.factionIcon.enabled = false;
			if (this.moduleImage != null)
			{
				this.moduleImage.enabled = false;
			}
			this.cellImage.enabled = false;
			if (this.moduleCellConnectorImage != null)
			{
				this.moduleCellConnectorImage.enabled = false;
			}
			this.UpdateConnections(true);
		}

		// Token: 0x06004FDB RID: 20443 RVA: 0x00227C78 File Offset: 0x00225E78
		public void SetPowerIcon(bool forceOff)
		{
			if (this.habModule == null || this.habModule.empty || this.habModule.active || forceOff)
			{
				if (this.habModule != null && this.habModule.active && (this.habModule.AtrocitiesToDestroy() > 0 || (this.habModule.hab.faction == GameControl.control.activePlayer && this.habModule.AtrocitiesToLose() > 0)))
				{
					GameControl.assetLoader.LoadAssetForImageAssignment(TemplateManager.global.pathLoyaltyIcon, this.powerIcon);
					this.powerIcon.enabled = true;
					return;
				}
				this.powerIcon.enabled = false;
				return;
			}
			else
			{
				if (this.habModule.underConstruction)
				{
					GameControl.assetLoader.LoadAssetForImageAssignment(TemplateManager.global.pathUnderConstructionIcon, this.powerIcon);
					this.powerIcon.enabled = true;
					return;
				}
				if (this.habModule.decommissioning)
				{
					GameControl.assetLoader.LoadAssetForImageAssignment(TemplateManager.global.pathNoneIcon, this.powerIcon);
					this.powerIcon.enabled = true;
					return;
				}
				if (this.habModule.functional && !this.habModule.powered)
				{
					GameControl.assetLoader.LoadAssetForImageAssignment(TemplateManager.global.pathHabPowerAlertIcon, this.powerIcon);
					this.powerIcon.enabled = true;
					return;
				}
				this.powerIcon.enabled = false;
				return;
			}
		}

		// Token: 0x06004FDC RID: 20444 RVA: 0x00227DF6 File Offset: 0x00225FF6
		public override void SetControllerBase(CanvasControllerBase canvasControllerBase)
		{
			this.habitatsController = (HabitatsScreenController)canvasControllerBase;
		}

		// Token: 0x06004FDD RID: 20445 RVA: 0x00227E04 File Offset: 0x00226004
		public override void OnDrop(PointerEventData eventData)
		{
			if (!base.gameObject.activeInHierarchy || !DragManager.canDropCurrentItem)
			{
				return;
			}
			DragItem currentItem = DragManager.currentItem;
			if (currentItem == null)
			{
				return;
			}
			this.habitatsController.StartModulePlacement(((HabModuleListItem)currentItem).GetModuleTemplate(), this.sector, this.module);
			currentItem.Reset();
		}

		// Token: 0x06004FDE RID: 20446 RVA: 0x00227E60 File Offset: 0x00226060
		public override void OnPointerEnter(PointerEventData eventData)
		{
			if (this.sectorIsPlayerControlled)
			{
				this.cellImage.color = this.visibleColor;
				base.OnPointerEnter(eventData);
				if (DragManager.currentItem == null)
				{
					return;
				}
				HabModuleListItem habModuleListItem = DragManager.currentItem as HabModuleListItem;
				if (habModuleListItem != null && habModuleListItem.draggable)
				{
					this.SetInteractable(false);
				}
			}
		}

		// Token: 0x06004FDF RID: 20447 RVA: 0x00227EB8 File Offset: 0x002260B8
		public override void OnPointerExit(PointerEventData eventData)
		{
			this.cellImage.color = this.hiddenColor;
			if (this.sectorIsPlayerControlled)
			{
				base.OnPointerExit(eventData);
				this.SetInteractable(true);
			}
		}

		// Token: 0x06004FE0 RID: 20448 RVA: 0x00227EE1 File Offset: 0x002260E1
		protected override bool CanDropItemHere()
		{
			return base.gameObject.activeSelf && DragManager.currentDragItemType == this.dragItemType && this.sectorIsPlayerControlled;
		}

		// Token: 0x06004FE1 RID: 20449 RVA: 0x00227F05 File Offset: 0x00226105
		protected void OnDestroy()
		{
			this.RemoveListeners();
		}

		// Token: 0x06004FE2 RID: 20450 RVA: 0x00227F0D File Offset: 0x0022610D
		protected void RemoveListeners()
		{
			this.cellButton.onClick.RemoveListener(new UnityAction(this.OnSelected));
		}

		// Token: 0x0400332E RID: 13102
		public bool isEmpty;

		// Token: 0x0400332F RID: 13103
		public int sector;

		// Token: 0x04003330 RID: 13104
		public int module;

		// Token: 0x04003331 RID: 13105
		public IHabitatsPreviewer Previewer;

		// Token: 0x04003332 RID: 13106
		protected HabType habType;

		// Token: 0x04003333 RID: 13107
		protected GridLayoutGroup gridLayoutGroup;

		// Token: 0x04003334 RID: 13108
		protected Button cellButton;

		// Token: 0x04003335 RID: 13109
		protected Image cellImage;

		// Token: 0x04003336 RID: 13110
		protected RectTransform moduleCellRectTransform;

		// Token: 0x04003337 RID: 13111
		protected Image moduleImage;

		// Token: 0x04003338 RID: 13112
		protected Image moduleCellConnectorImage;

		// Token: 0x04003339 RID: 13113
		protected int tier;

		// Token: 0x0400333A RID: 13114
		protected bool sectorEnabled;

		// Token: 0x0400333B RID: 13115
		protected bool sectorIsPlayerControlled;

		// Token: 0x0400333C RID: 13116
		protected bool underConstruction;

		// Token: 0x0400333D RID: 13117
		protected Image factionIcon;

		// Token: 0x0400333E RID: 13118
		protected Image powerIcon;

		// Token: 0x0400333F RID: 13119
		protected Image N2;

		// Token: 0x04003340 RID: 13120
		protected Image N1;

		// Token: 0x04003341 RID: 13121
		protected Image W2;

		// Token: 0x04003342 RID: 13122
		protected Image W1;

		// Token: 0x04003343 RID: 13123
		protected Image E2;

		// Token: 0x04003344 RID: 13124
		protected Image E1;

		// Token: 0x04003345 RID: 13125
		protected Image S2;

		// Token: 0x04003346 RID: 13126
		protected Image S1;

		// Token: 0x04003348 RID: 13128
		public bool connectionSectorOnly;

		// Token: 0x04003349 RID: 13129
		private bool hasInit;

		// Token: 0x0400334A RID: 13130
		private HabitatsScreenController habitatsController;

		// Token: 0x0400334B RID: 13131
		protected Color visibleColor = new Color(255f, 255f, 255f, 255f);

		// Token: 0x0400334C RID: 13132
		protected Color hiddenColor = new Color(255f, 255f, 255f, 0f);

		// Token: 0x0400334D RID: 13133
		public TooltipTrigger tooltip;
	}
}

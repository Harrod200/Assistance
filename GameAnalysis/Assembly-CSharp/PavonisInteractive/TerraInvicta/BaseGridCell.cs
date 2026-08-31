using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000866 RID: 2150
	public class BaseGridCell : HabGridCell
	{
		// Token: 0x06004FAF RID: 20399 RVA: 0x00226B47 File Offset: 0x00224D47
		protected override void CacheComponents()
		{
			base.CommonCacheComponents();
			if (this.moduleCellRectTransform != null)
			{
				this.moduleCellRotation = this.moduleCellRectTransform.localEulerAngles.z;
			}
			this.habType = HabType.Base;
		}

		// Token: 0x06004FB0 RID: 20400 RVA: 0x00226B7C File Offset: 0x00224D7C
		protected override void AddListeners()
		{
			if (this.connectionSectorOnly)
			{
				this.cellButton.interactable = false;
				this.cellImage.color = this.hiddenColor;
				return;
			}
			this.cellButton.onClick.AddListener(new UnityAction(base.OnSelected));
		}

		// Token: 0x06004FB1 RID: 20401 RVA: 0x00226BCB File Offset: 0x00224DCB
		public override void SetPreviewer(IHabitatsPreviewer previewer)
		{
			this.Previewer = previewer;
		}

		// Token: 0x06004FB2 RID: 20402 RVA: 0x00226BD4 File Offset: 0x00224DD4
		public override void SetInteractable(bool interactable)
		{
			if (!this.connectionSectorOnly)
			{
				base.SetInteractable(interactable);
			}
		}

		// Token: 0x06004FB3 RID: 20403 RVA: 0x00226BE8 File Offset: 0x00224DE8
		public override void SetModule(string imageName, bool playerControlled, bool alien, TIHabModuleState moduleState, TIHabState hab)
		{
			if (!this.connectionSectorOnly)
			{
				this.sectorIsPlayerControlled = playerControlled;
				base.habModule = moduleState;
				if (imageName == "blank")
				{
					base.Hide();
					return;
				}
				if (moduleState.moduleTemplate != null)
				{
					this.tier = moduleState.moduleTemplate.tier;
				}
				else
				{
					this.tier = base.GetTier(imageName);
				}
				this.isEmpty = imageName.Contains("Empty");
				if (this.moduleCellConnectorImage != null)
				{
					this.moduleCellConnectorImage.enabled = false;
				}
				if (moduleState.moduleTemplate != null && moduleState.mineLocation)
				{
					if (moduleState.moduleTemplate.tier == 1)
					{
						this.moduleCellRectTransform.localScale = Vector3.one * 5f;
					}
					else
					{
						this.moduleCellRectTransform.localScale = Vector3.one * 5f;
					}
				}
				else
				{
					this.moduleCellRectTransform.localScale = Vector3.one * (float)(this.tier * 2 - 1);
				}
				this.moduleCellRectTransform.localEulerAngles = new Vector3(0f, 0f, this.isEmpty ? (-this.moduleCellRectTransform.parent.localEulerAngles.z) : this.moduleCellRotation);
				if (moduleState.underConstruction)
				{
					string text = moduleState.moduleTemplate.constructionIconResource(HabType.Base);
					GameControl.assetLoader.LoadAssetForImageAssignment(text, this.moduleImage);
				}
				else
				{
					GameControl.assetLoader.LoadAssetForImageAssignment(imageName, this.moduleImage);
				}
				base.SetAllConnectionSprites(hab.IsAlien());
				base.SetPowerIcon(false);
				this.factionIcon.enabled = false;
				if (this.tooltip != null)
				{
					this.tooltip.SetDelegate("BodyText", () => base.SetTooltip());
				}
				base.Show();
			}
		}

		// Token: 0x06004FB4 RID: 20404 RVA: 0x00226DBC File Offset: 0x00224FBC
		public override void SetGridCellSize(Vector2 size)
		{
			if (!this.isEmpty && TIHabState.IsMineSlot(this.sector, this.module, HabType.Base))
			{
				this.gridLayoutGroup.cellSize = size;
				if (this.moduleCellRectTransform != null)
				{
					this.moduleCellRectTransform.sizeDelta = new Vector2(size.x * 4f, size.y);
					return;
				}
			}
			else
			{
				base.SetGridCellSize(size);
			}
		}

		// Token: 0x06004FB5 RID: 20405 RVA: 0x00226E29 File Offset: 0x00225029
		public override void OnPointerEnter(PointerEventData eventData)
		{
			if (!this.connectionSectorOnly)
			{
				base.OnPointerEnter(eventData);
			}
		}

		// Token: 0x06004FB6 RID: 20406 RVA: 0x00226E3A File Offset: 0x0022503A
		public override void OnPointerExit(PointerEventData eventData)
		{
			if (!this.connectionSectorOnly)
			{
				base.OnPointerExit(eventData);
			}
		}

		// Token: 0x06004FB7 RID: 20407 RVA: 0x00226E4B File Offset: 0x0022504B
		protected override bool CanDropItemHere()
		{
			return !this.connectionSectorOnly && base.CanDropItemHere();
		}

		// Token: 0x0400331B RID: 13083
		private float moduleCellRotation;
	}
}

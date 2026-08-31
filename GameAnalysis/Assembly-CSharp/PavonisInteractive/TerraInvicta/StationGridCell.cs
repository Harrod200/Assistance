using System;
using UnityEngine;
using UnityEngine.Events;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000871 RID: 2161
	public class StationGridCell : HabGridCell
	{
		// Token: 0x060050F9 RID: 20729 RVA: 0x00236AE5 File Offset: 0x00234CE5
		protected override void CacheComponents()
		{
			base.CommonCacheComponents();
			this.habType = HabType.Station;
		}

		// Token: 0x060050FA RID: 20730 RVA: 0x00236AF4 File Offset: 0x00234CF4
		protected override void AddListeners()
		{
			this.cellButton.onClick.AddListener(new UnityAction(base.OnSelected));
		}

		// Token: 0x060050FB RID: 20731 RVA: 0x00236B12 File Offset: 0x00234D12
		public override void SetPreviewer(IHabitatsPreviewer previewer)
		{
			this.Previewer = previewer;
		}

		// Token: 0x060050FC RID: 20732 RVA: 0x00236B1C File Offset: 0x00234D1C
		public override void SetModule(string imageName, bool playerControlled, bool alien, TIHabModuleState module, TIHabState hab)
		{
			this.sectorIsPlayerControlled = playerControlled;
			base.habModule = module;
			if (imageName == "blank")
			{
				base.Hide();
				return;
			}
			if (module.moduleTemplate != null)
			{
				this.tier = module.moduleTemplate.tier;
			}
			else
			{
				this.tier = base.GetTier(imageName);
			}
			this.isEmpty = imageName.Contains("Empty");
			this.moduleCellConnectorImage.enabled = false;
			this.moduleCellRectTransform.localScale = Vector3.one * (float)(this.tier * 2 - 1);
			this.moduleCellRectTransform.localEulerAngles = new Vector3(0f, 0f, this.isEmpty ? (-this.moduleCellRectTransform.parent.localEulerAngles.z) : 0f);
			if (module.underConstruction)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(base.habModule.moduleTemplate.constructionIconResource(HabType.Station), this.moduleImage);
			}
			else
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(imageName, this.moduleImage);
			}
			base.SetPowerIcon(false);
			this.factionIcon.enabled = false;
			base.SetAllConnectionSprites(hab.IsAlien());
			if (this.tooltip != null)
			{
				this.tooltip.SetDelegate("BodyText", () => base.SetTooltip());
			}
			base.Show();
		}
	}
}

using System;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008D7 RID: 2263
	public class SectorsListItemController : MonoBehaviour
	{
		// Token: 0x060056A5 RID: 22181 RVA: 0x0027A454 File Offset: 0x00278654
		public void UpdateHeaderItem(TISectorState sector)
		{
			GameControl.assetLoader.LoadAssetForImageAssignment(sector.faction.template.stationIcon, this.firstIcon);
			this.firstIcon.gameObject.SetActive(true);
			this.secondIcon.gameObject.SetActive(false);
			this.primaryText.SetText(sector.shortSectorString);
			this.sectorItem = true;
			this.state = sector;
			this.tip.enabled = false;
		}

		// Token: 0x060056A6 RID: 22182 RVA: 0x0027A4D0 File Offset: 0x002786D0
		public void UpdateListItem(TIHabModuleState module)
		{
			this.firstIcon.gameObject.SetActive(false);
			if (module.underConstruction)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment("icons_2d/ICO_under_construction", this.secondIcon);
				this.secondIcon.gameObject.SetActive(true);
			}
			else if (module.decommissioning)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment("icons_2d/ICO_none", this.secondIcon);
				this.secondIcon.gameObject.SetActive(true);
			}
			else if (!module.powered)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment("icons_2d/ICO_hab_power_alert", this.secondIcon);
				this.secondIcon.gameObject.SetActive(true);
			}
			else
			{
				this.secondIcon.gameObject.SetActive(false);
			}
			if (module.AtrocitiesToDestroy() > 0 || (module.hab.faction == GameControl.control.activePlayer && module.AtrocitiesToLose() > 0))
			{
				this.primaryText.SetText(new StringBuilder(module.moduleTemplate.displayName).Append(TIGlobalConfig.globalConfig.warningInlineSpritePath).ToString());
			}
			else
			{
				this.primaryText.SetText(module.moduleTemplate.displayName);
			}
			this.sectorItem = false;
			this.state = module;
			this.tip.SetDelegate("BodyText", () => TIHabModuleState.FullSummary(module, false));
			this.tip.enabled = true;
		}

		// Token: 0x060056A7 RID: 22183 RVA: 0x0027A674 File Offset: 0x00278874
		public void OnClicked()
		{
			SoundEffectController.PlaySelectSound(this.state.ref_hab);
			if (this.sectorItem)
			{
				if (GeneralControlsController.CurrentlyTargetingStateType(typeof(TISectorState)))
				{
					GameControl.eventManager.TriggerEvent(new SectorSelectedEvent(this.state as TISectorState), null, Array.Empty<object>());
					return;
				}
				GameControl.eventManager.TriggerEvent(new HabDetailRequested(this.state.ref_hab, true), null, Array.Empty<object>());
				return;
			}
			else
			{
				if (GeneralControlsController.CurrentlyTargetingStateType(typeof(TIHabModuleState)))
				{
					GameControl.eventManager.TriggerEvent(new HabModuleSelected(this.state as TIHabModuleState), null, Array.Empty<object>());
					return;
				}
				GameControl.eventManager.TriggerEvent(new HabDetailRequested(this.state.ref_hab, true), null, Array.Empty<object>());
				return;
			}
		}

		// Token: 0x04003DCB RID: 15819
		public Image firstIcon;

		// Token: 0x04003DCC RID: 15820
		public Image secondIcon;

		// Token: 0x04003DCD RID: 15821
		public TMP_Text primaryText;

		// Token: 0x04003DCE RID: 15822
		public bool sectorItem;

		// Token: 0x04003DCF RID: 15823
		public TIGameState state;

		// Token: 0x04003DD0 RID: 15824
		public TooltipTrigger tip;
	}
}

using System;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.SolarSystem;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200059D RID: 1437
	public class HabSiteCouncilorGridItemController : MonoBehaviour
	{
		// Token: 0x060026A9 RID: 9897 RVA: 0x000D26DB File Offset: 0x000D08DB
		public void Init(CouncilorView councilor)
		{
			this.spaceObjectSelection = World.Active.GetExistingManager<SpaceObjectSelection>();
			this.councilor = councilor;
		}

		// Token: 0x060026AA RID: 9898 RVA: 0x000D26F4 File Offset: 0x000D08F4
		public void UpdateGridItem()
		{
			TIFactionState factionCurrent = this.councilor.factionCurrent;
			this.backgroundImage.color = ((factionCurrent == null) ? Color.clear : factionCurrent.template.color);
			string mapIconResourcePathCurrent = this.councilor.mapIconResourcePathCurrent;
			if (mapIconResourcePathCurrent == string.Empty)
			{
				Log.Error("Iconpath empty for councilor " + this.councilor.councilor.displayName, Array.Empty<object>());
			}
			GameControl.assetLoader.LoadAssetForImageAssignment(mapIconResourcePathCurrent, this.councilorIcon);
			this.tooltip.SetDelegate("BodyText", () => this.councilor.displayNameCurrent);
		}

		// Token: 0x060026AB RID: 9899 RVA: 0x000D279D File Offset: 0x000D099D
		public void OnClicked()
		{
			this.spaceObjectSelection.BlockThisFrame = true;
			SoundEffectController.PlaySelectSound(this.councilor.councilor);
			GameControl.eventManager.TriggerEvent(new CouncilorMapItemSelected(this.councilor.councilor), null, Array.Empty<object>());
		}

		// Token: 0x04001CBB RID: 7355
		private CouncilorView councilor;

		// Token: 0x04001CBC RID: 7356
		public Image backgroundImage;

		// Token: 0x04001CBD RID: 7357
		public Image councilorIcon;

		// Token: 0x04001CBE RID: 7358
		public TooltipTrigger tooltip;

		// Token: 0x04001CBF RID: 7359
		private SpaceObjectSelection spaceObjectSelection;
	}
}

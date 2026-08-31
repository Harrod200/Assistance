using System;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.UI;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000876 RID: 2166
	public class IntelAlienEventListItemController : MonoBehaviour
	{
		// Token: 0x0600510B RID: 20747 RVA: 0x00236E78 File Offset: 0x00235078
		public void UpdateListItem(NotificationSummaryItem summary)
		{
			this.item = summary;
			this.eventSummary.SetText(summary.itemSummary);
			if (!string.IsNullOrEmpty(summary.iconResource))
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(summary.iconResource, this.icon);
				this.icon.enabled = true;
			}
			else
			{
				this.icon.enabled = false;
			}
			if (!string.IsNullOrEmpty(summary.iconBackgroundResource))
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(summary.iconBackgroundResource, this.iconBackground);
				this.iconBackground.enabled = true;
				this.iconBackground.color = summary.backgroundColor;
				return;
			}
			this.iconBackground.enabled = false;
		}

		// Token: 0x0600510C RID: 20748 RVA: 0x00236F28 File Offset: 0x00235128
		public void OnClick()
		{
			if (this.item.gotoGameState != null)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
				World.Active.GetExistingManager<CanvasManager>().CloseActiveInfoScreen();
				TIRegionAlienEntityState ref_regionAlienEntity = this.item.gotoGameState.ref_regionAlienEntity;
				if (ref_regionAlienEntity == null || ref_regionAlienEntity.VisibleToFaction(GameControl.control.activePlayer))
				{
					TIUtilities.GotoGameState(this.item.gotoGameState, true, true, true, true, false, -1f);
					return;
				}
				TIUtilities.GotoGameState(this.item.gotoGameState.ref_region, true, true, true, true, false, -1f);
			}
		}

		// Token: 0x040034DD RID: 13533
		public Image icon;

		// Token: 0x040034DE RID: 13534
		public Image iconBackground;

		// Token: 0x040034DF RID: 13535
		public TMP_Text eventSummary;

		// Token: 0x040034E0 RID: 13536
		private NotificationSummaryItem item;
	}
}

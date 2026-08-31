using System;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000855 RID: 2133
	public class ShipConstructionQueueListItemController : MonoBehaviour
	{
		// Token: 0x06004E56 RID: 20054 RVA: 0x0021B5AB File Offset: 0x002197AB
		public void Init(ShipyardGridItemController gridController, Sprite defaultSprite)
		{
			this.gridController = gridController;
			if (this.defaultButtonSprite == null)
			{
				this.defaultButtonSprite = defaultSprite;
			}
		}

		// Token: 0x06004E57 RID: 20055 RVA: 0x0021B5CC File Offset: 0x002197CC
		public void UpdateListItem(ShipConstructionQueueItem item, int position)
		{
			this.item = item;
			if (position == 0)
			{
				this.className.SetText(Loc.T("UI.Fleets.LeadQueueItem", new object[]
				{
					item.shipDesign.fullClassName,
					item.daysToCompletion.ToString("N0")
				}));
			}
			else
			{
				this.className.SetText(Loc.T("UI.Fleets.OtherQueueItem", new object[]
				{
					item.shipDesign.fullClassName,
					item.resourcesCost.completionTime_days.ToString("N0")
				}));
			}
			this.earthImage.enabled = item.resourcesCost.GetSingleCostValue(FactionResource.Boost) > 0f;
			this.HighlightButtonAfterSelection(this.gridController.controller.constructionManagerSelectedQueueItem);
		}

		// Token: 0x06004E58 RID: 20056 RVA: 0x0021B699 File Offset: 0x00219899
		public void OnClickLine()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
			this.gridController.SetSelectedQueueItem(this.item);
		}

		// Token: 0x06004E59 RID: 20057 RVA: 0x0021B6B8 File Offset: 0x002198B8
		public void HighlightButtonAfterSelection(ShipConstructionQueueItem selectedItem)
		{
			this.idleButtonImage.sprite = ((selectedItem == this.item) ? this.button.spriteState.pressedSprite : this.defaultButtonSprite);
		}

		// Token: 0x040031EA RID: 12778
		private ShipyardGridItemController gridController;

		// Token: 0x040031EB RID: 12779
		public Image earthImage;

		// Token: 0x040031EC RID: 12780
		public TMP_Text className;

		// Token: 0x040031ED RID: 12781
		private ShipConstructionQueueItem item;

		// Token: 0x040031EE RID: 12782
		public Image idleButtonImage;

		// Token: 0x040031EF RID: 12783
		private Sprite defaultButtonSprite;

		// Token: 0x040031F0 RID: 12784
		public Button button;
	}
}

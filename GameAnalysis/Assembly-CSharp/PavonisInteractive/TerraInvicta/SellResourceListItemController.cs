using System;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000863 RID: 2147
	public class SellResourceListItemController : MonoBehaviour
	{
		// Token: 0x17000ECC RID: 3788
		// (get) Token: 0x06004FA2 RID: 20386 RVA: 0x00226709 File Offset: 0x00224909
		// (set) Token: 0x06004FA3 RID: 20387 RVA: 0x00226711 File Offset: 0x00224911
		public FactionResource resource { get; private set; }

		// Token: 0x06004FA4 RID: 20388 RVA: 0x0022671A File Offset: 0x0022491A
		public void Initialize(GeneralControlsController controller, FactionResource resource)
		{
			this.controller = controller;
			this.resource = resource;
			this.resourceName.SetText(TIUtilities.GetResourceString(resource));
			GameControl.assetLoader.LoadAssetForImageAssignment(TIUtilities.PathResourceIcon(resource), this.resourceIcon);
		}

		// Token: 0x06004FA5 RID: 20389 RVA: 0x00226754 File Offset: 0x00224954
		public void UpdateListItem(int numberToSell)
		{
			this.numberToSellInput.text = numberToSell.ToString();
			float modifiedResourceMarketValueForSelling = TIGlobalValuesState.GlobalValues.GetModifiedResourceMarketValueForSelling(this.controller.activePlayer, this.resource);
			this.totalSaleValueText.SetText(((float)numberToSell * modifiedResourceMarketValueForSelling).ToString("N2"));
			this.perUnitSaleValueText.SetText(Loc.T("UI.GeneralControls.PerUnit", new object[] { modifiedResourceMarketValueForSelling.ToString(TIUtilities.DecimalPlaces((double)modifiedResourceMarketValueForSelling, 7, 0)) }));
			this.increaseButton.interactable = this.controller.activePlayer.GetCurrentResourceAmount(this.resource) > (float)numberToSell;
			this.decreaseButton.interactable = numberToSell > 0;
		}

		// Token: 0x06004FA6 RID: 20390 RVA: 0x00226810 File Offset: 0x00224A10
		public void OnMinusButtonSelected()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
			int num = -100;
			if (TIInputManager.IsShiftKeyDown)
			{
				if (TIInputManager.IsControlKeyDown)
				{
					num = -10000;
				}
				else if (TIInputManager.IsAltKeyDown)
				{
					num = -10;
				}
				else
				{
					num = -1000;
				}
			}
			else if (TIInputManager.IsAltKeyDown)
			{
				num = -1;
			}
			this.controller.ChangeProposedSale(this.resource, num, true);
		}

		// Token: 0x06004FA7 RID: 20391 RVA: 0x00226874 File Offset: 0x00224A74
		public void OnPlusButtonSelected()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			int num = 100;
			if (TIInputManager.IsShiftKeyDown)
			{
				if (TIInputManager.IsControlKeyDown)
				{
					num = 10000;
				}
				else if (TIInputManager.IsAltKeyDown)
				{
					num = 10;
				}
				else
				{
					num = 1000;
				}
			}
			else if (TIInputManager.IsAltKeyDown)
			{
				num = 1;
			}
			this.controller.ChangeProposedSale(this.resource, num, true);
		}

		// Token: 0x06004FA8 RID: 20392 RVA: 0x002268D8 File Offset: 0x00224AD8
		public void OnAmountChanged()
		{
			string text = this.numberToSellInput.text.Replace("-", "");
			if (text == null || text == string.Empty || this.resource == FactionResource.None)
			{
				return;
			}
			int num = int.Parse(text);
			if ((float)num > this.controller.activePlayer.GetCurrentResourceAmount(this.resource))
			{
				num = (int)this.controller.activePlayer.GetCurrentResourceAmount(this.resource);
				this.numberToSellInput.text = num.ToString();
			}
			this.controller.ChangeProposedSale(this.resource, num, false);
		}

		// Token: 0x04003307 RID: 13063
		private GeneralControlsController controller;

		// Token: 0x04003309 RID: 13065
		public TMP_Text resourceName;

		// Token: 0x0400330A RID: 13066
		public Image resourceIcon;

		// Token: 0x0400330B RID: 13067
		public TMP_InputField numberToSellInput;

		// Token: 0x0400330C RID: 13068
		public TMP_Text numberToSellText;

		// Token: 0x0400330D RID: 13069
		public TMP_Text totalSaleValueText;

		// Token: 0x0400330E RID: 13070
		public TMP_Text perUnitSaleValueText;

		// Token: 0x0400330F RID: 13071
		public Button increaseButton;

		// Token: 0x04003310 RID: 13072
		public Button decreaseButton;
	}
}

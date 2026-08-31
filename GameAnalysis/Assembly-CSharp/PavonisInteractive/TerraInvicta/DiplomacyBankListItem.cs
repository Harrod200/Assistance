using System;
using FullSerializer;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200089F RID: 2207
	public class DiplomacyBankListItem : MonoBehaviour
	{
		// Token: 0x06005340 RID: 21312 RVA: 0x00250D60 File Offset: 0x0024EF60
		public void OnLeftClick()
		{
			if (this.dealTableLink != null)
			{
				DiplomacyTableListItem component = this.dealTableLink.GetComponent<DiplomacyTableListItem>();
				component.Show();
				if (component.itemType == TradeItemType.Treaty)
				{
					((this.diplomacyController.playerTableTreatyItem == component) ? this.diplomacyController.aiTableTreatyItem : this.diplomacyController.playerTableTreatyItem).Show();
					if (component.treaty == TradeOffer.TreatyType.Intel)
					{
						this.diplomacyController.playerBankExchangeIntelItem.button.interactable = false;
						this.diplomacyController.aiBankExchangeIntelItem.button.interactable = false;
					}
				}
				if (component.itemType == TradeItemType.ExchangeIntel)
				{
					((this.diplomacyController.playerTableExchangeIntelItem == component) ? this.diplomacyController.aiTableExchangeIntelItem : this.diplomacyController.playerTableExchangeIntelItem).Show();
					if (this.diplomacyController.playerTableTreatyItem.itemType == TradeItemType.Treaty && this.diplomacyController.playerTableTreatyItem.treaty == TradeOffer.TreatyType.Intel)
					{
						this.diplomacyController.playerBankTreatyItem.button.interactable = false;
						this.diplomacyController.aiBankTreatyItem.button.interactable = false;
					}
				}
				if (component.itemType == TradeItemType.Resource && component.originalValue <= 0)
				{
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Decline", false, false);
					return;
				}
				this.diplomacyController.touchedAIOffer = true;
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
				this.diplomacyController.EvaluateTrade();
			}
		}

		// Token: 0x06005341 RID: 21313 RVA: 0x00250ED0 File Offset: 0x0024F0D0
		public void ShowOrgData(TIOrgState org)
		{
			this.orgTierText.text = org.tierStarsInline;
			this.orgTierText.gameObject.SetActive(true);
			if (org.hasCouncilor)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(org.assignedCouncilor.iconResource, this.orgCouncilorIcon);
				this.orgCouncilorIcon.gameObject.SetActive(true);
				return;
			}
			this.orgCouncilorIcon.gameObject.SetActive(false);
		}

		// Token: 0x06005342 RID: 21314 RVA: 0x00250F45 File Offset: 0x0024F145
		public void HideOrgData()
		{
			this.orgTierText.gameObject.SetActive(false);
			this.orgCouncilorIcon.gameObject.SetActive(false);
		}

		// Token: 0x06005343 RID: 21315 RVA: 0x00250F69 File Offset: 0x0024F169
		private void AddNoAudio()
		{
			if (this.dealTableLink != null)
			{
				this.dealTableLink.GetComponent<DiplomacyTableListItem>().Show();
				this.diplomacyController.touchedAIOffer = true;
				this.diplomacyController.EvaluateTrade();
			}
		}

		// Token: 0x06005344 RID: 21316 RVA: 0x00250FA0 File Offset: 0x0024F1A0
		public void AddToTable(float value, bool playAudio = true)
		{
			this.AddNoAudio();
			this.dealTableLink.GetComponent<DiplomacyTableListItem>().quantitySendInput.text = Mathf.FloorToInt(value).ToString();
		}

		// Token: 0x040038B9 RID: 14521
		public TMP_Text quantityText;

		// Token: 0x040038BA RID: 14522
		public TMP_Text tabText;

		// Token: 0x040038BB RID: 14523
		public Image itemIcon;

		// Token: 0x040038BC RID: 14524
		public GameObject dealTableLink;

		// Token: 0x040038BD RID: 14525
		public TradeItemType itemType;

		// Token: 0x040038BE RID: 14526
		public TooltipTrigger tooltipTrigger;

		// Token: 0x040038BF RID: 14527
		public Button button;

		// Token: 0x040038C0 RID: 14528
		[fsIgnore]
		public DiplomacyController diplomacyController;

		// Token: 0x040038C1 RID: 14529
		[HideInInspector]
		public bool isValid = true;

		// Token: 0x040038C2 RID: 14530
		[Header("Org UI")]
		public TMP_Text orgTierText;

		// Token: 0x040038C3 RID: 14531
		public Image orgCouncilorIcon;
	}
}

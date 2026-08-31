using System;
using FullSerializer;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008A1 RID: 2209
	public class DiplomacyTableListItem : MonoBehaviour
	{
		// Token: 0x0600536E RID: 21358 RVA: 0x002556E0 File Offset: 0x002538E0
		public void Init(DiplomacyController controller, TIFactionState faction, TIFactionState otherFaction, FactionResource resource)
		{
			this.diplomacyController = controller;
			this.itemFaction = faction;
			float num;
			if (resource != FactionResource.None)
			{
				this.factionResource = resource;
				num = this.itemFaction.GetCurrentResourceAmount(resource);
				if (this.itemFaction.player.isAI)
				{
					num = Mathf.Min(num, TradeAI.GetMaximumTradeQuantity(faction, resource));
				}
			}
			else
			{
				num = 1f;
			}
			this.originalValue = Mathf.FloorToInt(num);
			this.quantityOwnedText.text = Mathf.FloorToInt((float)this.originalValue).ToString();
		}

		// Token: 0x0600536F RID: 21359 RVA: 0x0025576C File Offset: 0x0025396C
		public void UpdateResourceLimit()
		{
			if (this.factionResource != FactionResource.None)
			{
				float currentResourceAmount = this.itemFaction.GetCurrentResourceAmount(this.factionResource);
				this.originalValue = Mathf.FloorToInt(currentResourceAmount);
				this.originalValue = Mathf.Clamp(this.originalValue, 0, this.originalValue);
				this.quantityOwnedText.text = Mathf.FloorToInt((float)this.originalValue).ToString();
			}
		}

		// Token: 0x06005370 RID: 21360 RVA: 0x002557D8 File Offset: 0x002539D8
		public void OnRightClick(bool audio = true)
		{
			this.quantitySendInput.text = "0";
			this.DisableGameobject();
			if (this.itemType == TradeItemType.Treaty)
			{
				((this.diplomacyController.playerTableTreatyItem == this) ? this.diplomacyController.aiTableTreatyItem : this.diplomacyController.playerTableTreatyItem).DisableGameobject();
				if (this.treaty == TradeOffer.TreatyType.Intel)
				{
					this.diplomacyController.playerBankExchangeIntelItem.button.interactable = true;
					this.diplomacyController.aiBankExchangeIntelItem.button.interactable = true;
				}
			}
			if (this.itemType == TradeItemType.ExchangeIntel)
			{
				((this.diplomacyController.playerTableExchangeIntelItem == this) ? this.diplomacyController.aiTableExchangeIntelItem : this.diplomacyController.playerTableExchangeIntelItem).DisableGameobject();
				if (this.diplomacyController.playerTableTreatyItem.itemType == TradeItemType.Treaty && this.diplomacyController.playerTableTreatyItem.treaty == TradeOffer.TreatyType.Intel)
				{
					this.diplomacyController.playerBankTreatyItem.button.interactable = true;
					this.diplomacyController.aiBankTreatyItem.button.interactable = true;
				}
			}
			if (audio)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
			}
			this.diplomacyController.touchedAIOffer = true;
			this.diplomacyController.EvaluateTrade();
			this.AllowBothResourcesOnTable();
		}

		// Token: 0x06005371 RID: 21361 RVA: 0x00255924 File Offset: 0x00253B24
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

		// Token: 0x06005372 RID: 21362 RVA: 0x00255999 File Offset: 0x00253B99
		public void HideOrgData()
		{
			this.orgTierText.gameObject.SetActive(false);
			this.orgCouncilorIcon.gameObject.SetActive(false);
		}

		// Token: 0x06005373 RID: 21363 RVA: 0x002559C0 File Offset: 0x00253BC0
		public void Show()
		{
			if (this.itemType == TradeItemType.Resource && this.originalValue <= 0)
			{
				this.DisableGameobject();
				this.quantityOwnedText.gameObject.SetActive(false);
				this.quantitySendInput.gameObject.SetActive(false);
				this.itemDescription.gameObject.SetActive(false);
				return;
			}
			this.EnableGameobject();
			if (this.itemType == TradeItemType.Resource)
			{
				this.quantityOwnedText.gameObject.SetActive(true);
				this.quantitySendInput.gameObject.SetActive(true);
				this.itemDescription.gameObject.SetActive(false);
				this.quantitySendInput.text = "0";
				this.PreventDuplicateResourcesOnTable();
				return;
			}
			this.quantitySendInput.text = "1";
			this.quantityOwnedText.gameObject.SetActive(false);
			this.quantitySendInput.gameObject.SetActive(false);
			this.itemDescription.gameObject.SetActive(true);
		}

		// Token: 0x06005374 RID: 21364 RVA: 0x00255AB4 File Offset: 0x00253CB4
		public void OnValueChanged()
		{
			float num;
			if (float.TryParse(this.quantitySendInput.text, out num))
			{
				if (num <= 0f)
				{
					this.quantitySendInput.text = "0";
				}
				if (num > (float)this.originalValue && this.originalValue > 0)
				{
					num = (float)this.originalValue;
					this.quantitySendInput.text = num.ToString("F0");
				}
				this.quantityOwnedText.text = ((float)this.originalValue - num).ToString("F0");
				this.diplomacyController.touchedAIOffer = true;
				this.diplomacyController.EvaluateTrade();
				return;
			}
			if (this.quantitySendInput.text != "")
			{
				this.quantitySendInput.text = "0";
			}
		}

		// Token: 0x06005375 RID: 21365 RVA: 0x00255B82 File Offset: 0x00253D82
		public void OnDeSelect()
		{
			if (this.quantitySendInput.text == "")
			{
				this.quantitySendInput.text = "0";
			}
		}

		// Token: 0x06005376 RID: 21366 RVA: 0x00255BAB File Offset: 0x00253DAB
		public void EnableGameobject()
		{
			base.gameObject.SetActive(true);
		}

		// Token: 0x06005377 RID: 21367 RVA: 0x00255BB9 File Offset: 0x00253DB9
		public void DisableGameobject()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x06005378 RID: 21368 RVA: 0x00255BC8 File Offset: 0x00253DC8
		private void PreventDuplicateResourcesOnTable()
		{
			if (this == this.diplomacyController.playerTableCashItem)
			{
				this.diplomacyController.aiBankCashItem.GetComponent<Button>().interactable = false;
			}
			if (this == this.diplomacyController.playerTableInfluenceItem)
			{
				this.diplomacyController.aiBankInfluenceItem.GetComponent<Button>().interactable = false;
			}
			if (this == this.diplomacyController.playerTableOpsItem)
			{
				this.diplomacyController.aiBankOpsItem.GetComponent<Button>().interactable = false;
			}
			if (this == this.diplomacyController.playerTableBoostItem)
			{
				this.diplomacyController.aiBankBoostItem.GetComponent<Button>().interactable = false;
			}
			if (this == this.diplomacyController.playerTableWaterItem)
			{
				this.diplomacyController.aiBankWaterItem.GetComponent<Button>().interactable = false;
			}
			if (this == this.diplomacyController.playerTableVolatilesItem)
			{
				this.diplomacyController.aiBankVolatilesItem.GetComponent<Button>().interactable = false;
			}
			if (this == this.diplomacyController.playerTableBaseMetalsItem)
			{
				this.diplomacyController.aiBankBaseMetalsItem.GetComponent<Button>().interactable = false;
			}
			if (this == this.diplomacyController.playerTableNobleMetalsItem)
			{
				this.diplomacyController.aiBankNobleMetalsItem.GetComponent<Button>().interactable = false;
			}
			if (this == this.diplomacyController.playerTableFissilesItem)
			{
				this.diplomacyController.aiBankFissilesItem.GetComponent<Button>().interactable = false;
			}
			if (this == this.diplomacyController.playerTableAntimatterItem)
			{
				this.diplomacyController.aiBankAntimatterItem.GetComponent<Button>().interactable = false;
			}
			if (this == this.diplomacyController.playerTableExoticsItem)
			{
				this.diplomacyController.aiBankExoticsItem.GetComponent<Button>().interactable = false;
			}
			if (this == this.diplomacyController.aiTableCashItem)
			{
				this.diplomacyController.playerBankCashItem.GetComponent<Button>().interactable = false;
			}
			if (this == this.diplomacyController.aiTableInfluenceItem)
			{
				this.diplomacyController.playerBankInfluenceItem.GetComponent<Button>().interactable = false;
			}
			if (this == this.diplomacyController.aiTableOpsItem)
			{
				this.diplomacyController.playerBankOpsItem.GetComponent<Button>().interactable = false;
			}
			if (this == this.diplomacyController.aiTableBoostItem)
			{
				this.diplomacyController.playerBankBoostItem.GetComponent<Button>().interactable = false;
			}
			if (this == this.diplomacyController.aiTableWaterItem)
			{
				this.diplomacyController.playerBankWaterItem.GetComponent<Button>().interactable = false;
			}
			if (this == this.diplomacyController.aiTableVolatilesItem)
			{
				this.diplomacyController.playerBankVolatilesItem.GetComponent<Button>().interactable = false;
			}
			if (this == this.diplomacyController.aiTableBaseMetalsItem)
			{
				this.diplomacyController.playerBankBaseMetalsItem.GetComponent<Button>().interactable = false;
			}
			if (this == this.diplomacyController.aiTableNobleMetalsItem)
			{
				this.diplomacyController.playerBankNobleMetalsItem.GetComponent<Button>().interactable = false;
			}
			if (this == this.diplomacyController.aiTableFissilesItem)
			{
				this.diplomacyController.playerBankFissilesItem.GetComponent<Button>().interactable = false;
			}
			if (this == this.diplomacyController.aiTableAntimatterItem)
			{
				this.diplomacyController.playerBankAntimatterItem.GetComponent<Button>().interactable = false;
			}
			if (this == this.diplomacyController.aiTableExoticsItem)
			{
				this.diplomacyController.playerBankExoticsItem.GetComponent<Button>().interactable = false;
			}
		}

		// Token: 0x06005379 RID: 21369 RVA: 0x00255F5C File Offset: 0x0025415C
		private void AllowBothResourcesOnTable()
		{
			if (this == this.diplomacyController.playerTableCashItem)
			{
				this.diplomacyController.aiBankCashItem.GetComponent<Button>().interactable = true;
			}
			if (this == this.diplomacyController.playerTableInfluenceItem)
			{
				this.diplomacyController.aiBankInfluenceItem.GetComponent<Button>().interactable = true;
			}
			if (this == this.diplomacyController.playerTableOpsItem)
			{
				this.diplomacyController.aiBankOpsItem.GetComponent<Button>().interactable = true;
			}
			if (this == this.diplomacyController.playerTableBoostItem)
			{
				this.diplomacyController.aiBankBoostItem.GetComponent<Button>().interactable = true;
			}
			if (this == this.diplomacyController.playerTableWaterItem)
			{
				this.diplomacyController.aiBankWaterItem.GetComponent<Button>().interactable = true;
			}
			if (this == this.diplomacyController.playerTableVolatilesItem)
			{
				this.diplomacyController.aiBankVolatilesItem.GetComponent<Button>().interactable = true;
			}
			if (this == this.diplomacyController.playerTableBaseMetalsItem)
			{
				this.diplomacyController.aiBankBaseMetalsItem.GetComponent<Button>().interactable = true;
			}
			if (this == this.diplomacyController.playerTableNobleMetalsItem)
			{
				this.diplomacyController.aiBankNobleMetalsItem.GetComponent<Button>().interactable = true;
			}
			if (this == this.diplomacyController.playerTableFissilesItem)
			{
				this.diplomacyController.aiBankFissilesItem.GetComponent<Button>().interactable = true;
			}
			if (this == this.diplomacyController.playerTableAntimatterItem)
			{
				this.diplomacyController.aiBankAntimatterItem.GetComponent<Button>().interactable = true;
			}
			if (this == this.diplomacyController.playerTableExoticsItem)
			{
				this.diplomacyController.aiBankExoticsItem.GetComponent<Button>().interactable = true;
			}
			if (this == this.diplomacyController.aiTableCashItem)
			{
				this.diplomacyController.playerBankCashItem.GetComponent<Button>().interactable = true;
			}
			if (this == this.diplomacyController.aiTableInfluenceItem)
			{
				this.diplomacyController.playerBankInfluenceItem.GetComponent<Button>().interactable = true;
			}
			if (this == this.diplomacyController.aiTableOpsItem)
			{
				this.diplomacyController.playerBankOpsItem.GetComponent<Button>().interactable = true;
			}
			if (this == this.diplomacyController.aiTableBoostItem)
			{
				this.diplomacyController.playerBankBoostItem.GetComponent<Button>().interactable = true;
			}
			if (this == this.diplomacyController.aiTableWaterItem)
			{
				this.diplomacyController.playerBankWaterItem.GetComponent<Button>().interactable = true;
			}
			if (this == this.diplomacyController.aiTableVolatilesItem)
			{
				this.diplomacyController.playerBankVolatilesItem.GetComponent<Button>().interactable = true;
			}
			if (this == this.diplomacyController.aiTableBaseMetalsItem)
			{
				this.diplomacyController.playerBankBaseMetalsItem.GetComponent<Button>().interactable = true;
			}
			if (this == this.diplomacyController.aiTableNobleMetalsItem)
			{
				this.diplomacyController.playerBankNobleMetalsItem.GetComponent<Button>().interactable = true;
			}
			if (this == this.diplomacyController.aiTableFissilesItem)
			{
				this.diplomacyController.playerBankFissilesItem.GetComponent<Button>().interactable = true;
			}
			if (this == this.diplomacyController.aiTableAntimatterItem)
			{
				this.diplomacyController.playerBankAntimatterItem.GetComponent<Button>().interactable = true;
			}
			if (this == this.diplomacyController.aiTableExoticsItem)
			{
				this.diplomacyController.playerBankExoticsItem.GetComponent<Button>().interactable = true;
			}
		}

		// Token: 0x0400392D RID: 14637
		public TMP_Text quantityOwnedText;

		// Token: 0x0400392E RID: 14638
		public TMP_Text itemDescription;

		// Token: 0x0400392F RID: 14639
		public Image itemIcon;

		// Token: 0x04003930 RID: 14640
		public TMP_InputField quantitySendInput;

		// Token: 0x04003931 RID: 14641
		public TradeItemType itemType;

		// Token: 0x04003932 RID: 14642
		public FactionResource factionResource;

		// Token: 0x04003933 RID: 14643
		public TooltipTrigger tooltipTrigger;

		// Token: 0x04003934 RID: 14644
		public TIFactionState itemFaction;

		// Token: 0x04003935 RID: 14645
		[fsIgnore]
		public DiplomacyController diplomacyController;

		// Token: 0x04003936 RID: 14646
		[fsIgnore]
		public TIOrgState orgReference;

		// Token: 0x04003937 RID: 14647
		[fsIgnore]
		public TIHabState habReference;

		// Token: 0x04003938 RID: 14648
		[fsIgnore]
		public TIProjectTemplate projectReference;

		// Token: 0x04003939 RID: 14649
		[fsIgnore]
		public TradeOffer.TreatyType treaty;

		// Token: 0x0400393A RID: 14650
		[Header("Org UI")]
		public TMP_Text orgTierText;

		// Token: 0x0400393B RID: 14651
		public Image orgCouncilorIcon;

		// Token: 0x0400393C RID: 14652
		public int originalValue;
	}
}

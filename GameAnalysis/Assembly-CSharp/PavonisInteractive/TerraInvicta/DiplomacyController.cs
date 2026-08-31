using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008A0 RID: 2208
	public class DiplomacyController : MonoBehaviour
	{
		// Token: 0x17000EF3 RID: 3827
		// (get) Token: 0x06005346 RID: 21318 RVA: 0x00250FE5 File Offset: 0x0024F1E5
		private TIFactionState activePlayer
		{
			get
			{
				return GameControl.control.activePlayer;
			}
		}

		// Token: 0x06005347 RID: 21319 RVA: 0x00250FF4 File Offset: 0x0024F1F4
		public void Setup(TIFactionState targetFaction, NotificationScreenController controller, bool isAIOffer = false)
		{
			this.notificationController = controller;
			this.tradingFaction = targetFaction;
			this.playerResourcesVisible = false;
			this.playerOrgsVisible = false;
			this.playerHabsVisible = false;
			this.playerProjectsVisible = false;
			this.aiResourcesVisible = false;
			this.aiOrgsVisible = false;
			this.aiHabsVisible = false;
			this.aiProjectsVisible = false;
			this.isThisAnAIOffer = false;
			this.ToggleResources(true);
			this.ToggleResources(false);
			this.ToggleTreaties(false);
			this.playerOrgTab.tabText.text = "+";
			this.playerHabsTab.tabText.text = "+";
			this.playerCPsTab.tabText.text = "+";
			this.playerProjectsTab.tabText.text = "+";
			this.aiOrgTab.tabText.text = "+";
			this.aiHabsTab.tabText.text = "+";
			this.aiCPsTab.tabText.text = "+";
			this.aiProjectsTab.tabText.text = "+";
			this.playerBankNAPItem_Info.quantityText.text = Loc.T("UI.Notifications.Diplomacy.NAP");
			this.aiBankNAPItem_Info.quantityText.text = Loc.T("UI.Notifications.Diplomacy.NAP");
			this.playerBankNAPItem_Info.gameObject.SetActive(false);
			this.aiBankNAPItem_Info.gameObject.SetActive(false);
			this.playerBankIntelItem_Info.quantityText.text = Loc.T("UI.Notifications.Diplomacy.IntelSharing");
			this.aiBankIntelItem_Info.quantityText.text = Loc.T("UI.Notifications.Diplomacy.IntelSharing");
			this.playerBankIntelItem_Info.gameObject.SetActive(false);
			this.aiBankIntelItem_Info.gameObject.SetActive(false);
			this.playerBankTruceItem_Info.quantityText.text = Loc.T("UI.Notifications.Diplomacy.Truce");
			this.aiBankTruceItem_Info.quantityText.text = Loc.T("UI.Notifications.Diplomacy.Truce");
			this.playerBankTruceItem_Info.gameObject.SetActive(false);
			this.aiBankTruceItem_Info.gameObject.SetActive(false);
			this.ResetDealTable();
			this.LoadBankValues();
			GameControl.eventManager.AddListener<HabDecommissionStatusChange>(new EventManager.EventDelegate<HabDecommissionStatusChange>(this.OnHabDecommissioned), null, null, true, false);
			GameControl.eventManager.AddListener<HabModuleConstructionStatusChange>(new EventManager.EventDelegate<HabModuleConstructionStatusChange>(this.OnHabModuleConstructionStatusChange), null, null, true, false);
			GameControl.eventManager.AddListener<HabModuleDecommissionStatusChange>(new EventManager.EventDelegate<HabModuleDecommissionStatusChange>(this.OnHabModuleDecommissionStatusChange), null, null, true, false);
			GameControl.eventManager.AddListener<FactionResourcesUpdated>(new EventManager.EventDelegate<FactionResourcesUpdated>(this.OnResourcesUpdated), null, this.activePlayer, false, false);
			GameControl.eventManager.AddListener<CouncilorValuesChanged>(new EventManager.EventDelegate<CouncilorValuesChanged>(this.CouncilorValuesChanged), null, null, true, false);
			this.aiFeedbackDialogText.text = (this.tradingFaction.IsAlienFaction ? Loc.T("UI.Notifications.Diplomacy.TradeValueNone.Alien") : Loc.T("UI.Notifications.Diplomacy.TradeValueNone"));
			TradeAI.PrepareCachesForTrading(this.activePlayer, this.tradingFaction);
			if (isAIOffer || this.testingAITrade)
			{
				AIDailyFactionPlanner.TransferOrgsFromPool(this.tradingFaction);
				TradeOffer.TradeAgreement tradeAgreement = TradeAI.CreateTradeAgreement(this.tradingFaction, this.activePlayer);
				TradeOffer offer = tradeAgreement.GetOffer(this.tradingFaction);
				TradeOffer offer2 = tradeAgreement.GetOffer(this.activePlayer);
				this.PreFillTable(offer, offer2);
				this.touchedAIOffer = false;
				this.isThisAnAIOffer = true;
				this.EvaluateTrade();
			}
			this.playerFactionText.text = this.activePlayer.displayName;
			this.aiFactionText.text = this.tradingFaction.displayName;
			this.playerFactionIcon.sprite = this.activePlayer.factionIcon128UI;
			this.aiFactionIcon.sprite = this.tradingFaction.factionIcon128UI;
			this.playerFactionIconLarge.sprite = this.activePlayer.factionIcon256UI;
			this.aiFactionIconLarge.sprite = this.tradingFaction.factionIcon256UI;
			GameControl.assetLoader.LoadAssetForImageAssignment(this.activePlayer.template.gradientPath, this.playerFactionGradient);
			GameControl.assetLoader.LoadAssetForImageAssignment(this.tradingFaction.template.gradientPath, this.aiFactionGradient);
			if (this.tradingFaction.permanentAlly(this.activePlayer))
			{
				this.aiFactionAttitudeText.text = Loc.T("UI.Notifications.Diplomacy.Attitude_PermanentAlly");
			}
			else if (this.tradingFaction.GetFactionHate(this.activePlayer) <= 0f)
			{
				this.aiFactionAttitudeText.text = Loc.T("UI.Notifications.Diplomacy.Attitude_Tolerant");
			}
			else
			{
				float num = this.tradingFaction.GetFactionHate(this.activePlayer) / 10f;
				num = Mathf.Min(7f, Mathf.Floor(num));
				this.aiFactionAttitudeText.text = Loc.T("UI.Notifications.Diplomacy.Attitude" + num.ToString());
			}
			this.MaximizeTradeWindow();
		}

		// Token: 0x06005348 RID: 21320 RVA: 0x002514AC File Offset: 0x0024F6AC
		public void ResetDealTable()
		{
			for (int i = 0; i < this.playerTableItemsContent.transform.childCount; i++)
			{
				DiplomacyTableListItem component = this.playerTableItemsContent.transform.GetChild(i).GetComponent<DiplomacyTableListItem>();
				component.quantitySendInput.SetTextWithoutNotify("0");
				component.DisableGameobject();
			}
			for (int j = 0; j < this.aiTableItemsContent.transform.childCount; j++)
			{
				DiplomacyTableListItem component2 = this.aiTableItemsContent.transform.GetChild(j).GetComponent<DiplomacyTableListItem>();
				component2.quantitySendInput.SetTextWithoutNotify("0");
				component2.DisableGameobject();
			}
			this.playerBankCashItem.GetComponent<Button>().interactable = true;
			this.playerBankInfluenceItem.GetComponent<Button>().interactable = true;
			this.playerBankOpsItem.GetComponent<Button>().interactable = true;
			this.playerBankBoostItem.GetComponent<Button>().interactable = true;
			this.playerBankWaterItem.GetComponent<Button>().interactable = true;
			this.playerBankVolatilesItem.GetComponent<Button>().interactable = true;
			this.playerBankBaseMetalsItem.GetComponent<Button>().interactable = true;
			this.playerBankNobleMetalsItem.GetComponent<Button>().interactable = true;
			this.playerBankFissilesItem.GetComponent<Button>().interactable = true;
			this.playerBankAntimatterItem.GetComponent<Button>().interactable = true;
			this.playerBankExoticsItem.GetComponent<Button>().interactable = true;
			this.aiBankCashItem.GetComponent<Button>().interactable = true;
			this.aiBankInfluenceItem.GetComponent<Button>().interactable = true;
			this.aiBankOpsItem.GetComponent<Button>().interactable = true;
			this.aiBankBoostItem.GetComponent<Button>().interactable = true;
			this.aiBankWaterItem.GetComponent<Button>().interactable = true;
			this.aiBankVolatilesItem.GetComponent<Button>().interactable = true;
			this.aiBankBaseMetalsItem.GetComponent<Button>().interactable = true;
			this.aiBankNobleMetalsItem.GetComponent<Button>().interactable = true;
			this.aiBankFissilesItem.GetComponent<Button>().interactable = true;
			this.aiBankAntimatterItem.GetComponent<Button>().interactable = true;
			this.aiBankExoticsItem.GetComponent<Button>().interactable = true;
			this.aiTableHateReductionItem.DisableGameobject();
			this.EvaluateTrade();
		}

		// Token: 0x06005349 RID: 21321 RVA: 0x002516CC File Offset: 0x0024F8CC
		private bool IsTableEmpty()
		{
			for (int i = 0; i < this.playerTableItemsContent.transform.childCount; i++)
			{
				GameObject gameObject = this.playerTableItemsContent.transform.GetChild(i).gameObject;
				if (gameObject.activeSelf && gameObject.GetComponent<DiplomacyTableListItem>().itemType == TradeItemType.Resource && gameObject.GetComponent<DiplomacyTableListItem>().quantitySendInput.text != "0")
				{
					return false;
				}
				if (gameObject.activeSelf && gameObject.GetComponent<DiplomacyTableListItem>().itemType != TradeItemType.Resource)
				{
					return false;
				}
			}
			for (int j = 0; j < this.aiTableItemsContent.transform.childCount; j++)
			{
				GameObject gameObject2 = this.aiTableItemsContent.transform.GetChild(j).gameObject;
				if (gameObject2.activeSelf && gameObject2.GetComponent<DiplomacyTableListItem>().itemType == TradeItemType.Resource && gameObject2.GetComponent<DiplomacyTableListItem>().quantitySendInput.text != "0")
				{
					return false;
				}
				if (gameObject2.activeSelf && gameObject2.GetComponent<DiplomacyTableListItem>().itemType != TradeItemType.Resource)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600534A RID: 21322 RVA: 0x002517D2 File Offset: 0x0024F9D2
		public static void LogAITradeDetails(TradeOffer wantOffer, TradeOffer giveOffer)
		{
			Debug.Log(DiplomacyController.TradeOfferDetailString(wantOffer));
			Debug.Log(DiplomacyController.TradeOfferDetailString(giveOffer));
		}

		// Token: 0x0600534B RID: 21323 RVA: 0x002517EC File Offset: 0x0024F9EC
		private static string TradeOfferDetailString(TradeOffer offer)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(offer.offeringFaction.displayName);
			foreach (ResourceValue resourceValue in offer.resourceValues)
			{
				StringBuilder stringBuilder2 = stringBuilder;
				string[] array = new string[3];
				array[0] = resourceValue.resource.ToString();
				array[1] = ", ";
				int num = 2;
				float value = resourceValue.value;
				array[num] = value.ToString();
				stringBuilder2.AppendLine(TIUtilities.CombineStrings(array));
			}
			foreach (TIOrgState tiorgState in offer.orgs)
			{
				stringBuilder.AppendLine(tiorgState.displayName);
			}
			foreach (TIProjectTemplate tiprojectTemplate in offer.projects)
			{
				stringBuilder.AppendLine(tiprojectTemplate.displayName);
			}
			foreach (TIHabState tihabState in offer.habs)
			{
				stringBuilder.AppendLine(tihabState.displayName);
			}
			stringBuilder.AppendLine(offer.treatyType.ToString());
			return stringBuilder.ToString();
		}

		// Token: 0x0600534C RID: 21324 RVA: 0x00251990 File Offset: 0x0024FB90
		public void PreFillTable(TradeOffer aiOffer, TradeOffer playerOffer)
		{
			foreach (ResourceValue resourceValue in playerOffer.resourceValues)
			{
				if (resourceValue.value > 0f)
				{
					switch (resourceValue.resource)
					{
					case FactionResource.Money:
						this.playerBankCashItem.AddToTable(resourceValue.value, false);
						break;
					case FactionResource.Influence:
						this.playerBankInfluenceItem.AddToTable(resourceValue.value, false);
						break;
					case FactionResource.Operations:
						this.playerBankOpsItem.AddToTable(resourceValue.value, false);
						break;
					case FactionResource.Boost:
						this.playerBankBoostItem.AddToTable(resourceValue.value, false);
						break;
					case FactionResource.Water:
						this.playerBankWaterItem.AddToTable(resourceValue.value, false);
						break;
					case FactionResource.Volatiles:
						this.playerBankVolatilesItem.AddToTable(resourceValue.value, false);
						break;
					case FactionResource.Metals:
						this.playerBankBaseMetalsItem.AddToTable(resourceValue.value, false);
						break;
					case FactionResource.NobleMetals:
						this.playerBankNobleMetalsItem.AddToTable(resourceValue.value, false);
						break;
					case FactionResource.Fissiles:
						this.playerBankFissilesItem.AddToTable(resourceValue.value, false);
						break;
					case FactionResource.Antimatter:
						this.playerBankAntimatterItem.AddToTable(resourceValue.value, false);
						break;
					case FactionResource.Exotics:
						this.playerBankExoticsItem.AddToTable(resourceValue.value, false);
						break;
					}
				}
			}
			foreach (TIOrgState tiorgState in playerOffer.orgs)
			{
				foreach (object obj in this.playerBankItemsContent.transform)
				{
					DiplomacyBankListItem component = ((Transform)obj).GetComponent<DiplomacyBankListItem>();
					if (component != null && component.isValid && component.dealTableLink != null && component.dealTableLink.GetComponent<DiplomacyTableListItem>() != null && component.dealTableLink.GetComponent<DiplomacyTableListItem>().orgReference != null && component.dealTableLink.GetComponent<DiplomacyTableListItem>().orgReference == tiorgState)
					{
						component.AddToTable(1f, false);
						break;
					}
				}
			}
			foreach (TIHabState tihabState in playerOffer.habs)
			{
				foreach (object obj2 in this.playerBankItemsContent.transform)
				{
					DiplomacyBankListItem component2 = ((Transform)obj2).GetComponent<DiplomacyBankListItem>();
					if (component2 != null && component2.isValid && component2.dealTableLink != null && component2.dealTableLink.GetComponent<DiplomacyTableListItem>() != null && component2.dealTableLink.GetComponent<DiplomacyTableListItem>().habReference != null && component2.dealTableLink.GetComponent<DiplomacyTableListItem>().habReference == tihabState)
					{
						component2.AddToTable(1f, false);
						break;
					}
				}
			}
			foreach (TIProjectTemplate tiprojectTemplate in playerOffer.projects)
			{
				foreach (object obj3 in this.playerBankItemsContent.transform)
				{
					DiplomacyBankListItem component3 = ((Transform)obj3).GetComponent<DiplomacyBankListItem>();
					if (component3 != null && component3.isValid && component3.dealTableLink != null && component3.dealTableLink.GetComponent<DiplomacyTableListItem>() != null && component3.dealTableLink.GetComponent<DiplomacyTableListItem>().projectReference != null && component3.dealTableLink.GetComponent<DiplomacyTableListItem>().projectReference == tiprojectTemplate)
					{
						component3.AddToTable(1f, false);
						break;
					}
				}
			}
			if (playerOffer.treatyType != TradeOffer.TreatyType.None || aiOffer.treatyType != TradeOffer.TreatyType.None)
			{
				this.playerBankTreatyItem.AddToTable(0f, false);
				this.aiBankTreatyItem.AddToTable(0f, false);
			}
			foreach (ResourceValue resourceValue2 in aiOffer.resourceValues)
			{
				if (resourceValue2.value > 0f)
				{
					switch (resourceValue2.resource)
					{
					case FactionResource.Money:
						if (float.Parse(this.playerTableCashItem.quantitySendInput.text) <= 0f)
						{
							this.aiBankCashItem.AddToTable(resourceValue2.value, false);
						}
						break;
					case FactionResource.Influence:
						if (float.Parse(this.playerTableInfluenceItem.quantitySendInput.text) <= 0f)
						{
							this.aiBankInfluenceItem.AddToTable(resourceValue2.value, false);
						}
						break;
					case FactionResource.Operations:
						if (float.Parse(this.playerTableOpsItem.quantitySendInput.text) <= 0f)
						{
							this.aiBankOpsItem.AddToTable(resourceValue2.value, false);
						}
						break;
					case FactionResource.Boost:
						if (float.Parse(this.playerTableBoostItem.quantitySendInput.text) <= 0f)
						{
							this.aiBankBoostItem.AddToTable(resourceValue2.value, false);
						}
						break;
					case FactionResource.Water:
						if (float.Parse(this.playerTableWaterItem.quantitySendInput.text) <= 0f)
						{
							this.aiBankWaterItem.AddToTable(resourceValue2.value, false);
						}
						break;
					case FactionResource.Volatiles:
						if (float.Parse(this.playerTableVolatilesItem.quantitySendInput.text) <= 0f)
						{
							this.aiBankVolatilesItem.AddToTable(resourceValue2.value, false);
						}
						break;
					case FactionResource.Metals:
						if (float.Parse(this.playerTableBaseMetalsItem.quantitySendInput.text) <= 0f)
						{
							this.aiBankBaseMetalsItem.AddToTable(resourceValue2.value, false);
						}
						break;
					case FactionResource.NobleMetals:
						if (float.Parse(this.playerTableNobleMetalsItem.quantitySendInput.text) <= 0f)
						{
							this.aiBankNobleMetalsItem.AddToTable(resourceValue2.value, false);
						}
						break;
					case FactionResource.Fissiles:
						if (float.Parse(this.playerTableFissilesItem.quantitySendInput.text) <= 0f)
						{
							this.aiBankFissilesItem.AddToTable(resourceValue2.value, false);
						}
						break;
					case FactionResource.Antimatter:
						if (float.Parse(this.playerTableAntimatterItem.quantitySendInput.text) <= 0f)
						{
							this.aiBankAntimatterItem.AddToTable(resourceValue2.value, false);
						}
						break;
					case FactionResource.Exotics:
						if (float.Parse(this.playerTableExoticsItem.quantitySendInput.text) <= 0f)
						{
							this.aiBankExoticsItem.AddToTable(resourceValue2.value, false);
						}
						break;
					}
				}
			}
			foreach (TIOrgState tiorgState2 in aiOffer.orgs)
			{
				foreach (object obj4 in this.aiBankItemsContent.transform)
				{
					DiplomacyBankListItem component4 = ((Transform)obj4).GetComponent<DiplomacyBankListItem>();
					if (component4 != null && component4.isValid && component4.dealTableLink != null && component4.dealTableLink.GetComponent<DiplomacyTableListItem>() != null && component4.dealTableLink.GetComponent<DiplomacyTableListItem>().orgReference != null && component4.dealTableLink.GetComponent<DiplomacyTableListItem>().orgReference == tiorgState2)
					{
						component4.AddToTable(1f, false);
						break;
					}
				}
			}
			foreach (TIHabState tihabState2 in aiOffer.habs)
			{
				foreach (object obj5 in this.aiBankItemsContent.transform)
				{
					DiplomacyBankListItem component5 = ((Transform)obj5).GetComponent<DiplomacyBankListItem>();
					if (component5 != null && component5.isValid && component5.dealTableLink != null && component5.dealTableLink.GetComponent<DiplomacyTableListItem>() != null && component5.dealTableLink.GetComponent<DiplomacyTableListItem>().habReference != null && component5.dealTableLink.GetComponent<DiplomacyTableListItem>().habReference == tihabState2)
					{
						component5.AddToTable(1f, false);
						break;
					}
				}
			}
			foreach (TIProjectTemplate tiprojectTemplate2 in aiOffer.projects)
			{
				foreach (object obj6 in this.aiBankItemsContent.transform)
				{
					DiplomacyBankListItem component6 = ((Transform)obj6).GetComponent<DiplomacyBankListItem>();
					if (component6 != null && component6.isValid && component6.dealTableLink != null && component6.dealTableLink.GetComponent<DiplomacyTableListItem>() != null && component6.dealTableLink.GetComponent<DiplomacyTableListItem>().projectReference != null && component6.dealTableLink.GetComponent<DiplomacyTableListItem>().projectReference == tiprojectTemplate2)
					{
						component6.AddToTable(1f, false);
						break;
					}
				}
			}
		}

		// Token: 0x0600534D RID: 21325 RVA: 0x00252548 File Offset: 0x00250748
		public void OnClickTradeButton()
		{
			this.playerTradeOffer.treatyType = this.aiTradeOffer.treatyType;
			this.playerTradeOffer.intelExchange = this.aiTradeOffer.intelExchange;
			this.activePlayer.playerControl.StartAction(new DiplomacyTradeAction(this.activePlayer, this.tradingFaction, this.playerTradeOffer, this.aiTradeOffer, this.hateModifier));
			this.notificationController.CompletedTrade(this.isThisAnAIOffer);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			this.CleanupListeners();
			if (this.activePlayer.isActivePlayer)
			{
				this.activePlayer.UnlockAchievement("completeTrade");
			}
		}

		// Token: 0x0600534E RID: 21326 RVA: 0x002525F4 File Offset: 0x002507F4
		private void CleanupOldTradeItems()
		{
			for (int i = 0; i < this.playerBankItemsContent.transform.childCount; i++)
			{
				DiplomacyBankListItem component = this.playerBankItemsContent.transform.GetChild(i).GetComponent<DiplomacyBankListItem>();
				if (component.itemType == TradeItemType.Org || component.itemType == TradeItemType.Hab || component.itemType == TradeItemType.Project)
				{
					component.isValid = false;
					global::UnityEngine.Object.Destroy(component.gameObject);
				}
			}
			for (int j = 0; j < this.aiBankItemsContent.transform.childCount; j++)
			{
				DiplomacyBankListItem component2 = this.aiBankItemsContent.transform.GetChild(j).GetComponent<DiplomacyBankListItem>();
				if (component2.itemType == TradeItemType.Org || component2.itemType == TradeItemType.Hab || component2.itemType == TradeItemType.Project)
				{
					component2.isValid = false;
					global::UnityEngine.Object.Destroy(component2.gameObject);
				}
			}
			for (int k = 0; k < this.playerTableItemsContent.transform.childCount; k++)
			{
				DiplomacyTableListItem component3 = this.playerTableItemsContent.transform.GetChild(k).GetComponent<DiplomacyTableListItem>();
				if (component3.itemType == TradeItemType.Org || component3.itemType == TradeItemType.Hab || component3.itemType == TradeItemType.Project)
				{
					global::UnityEngine.Object.Destroy(component3.gameObject);
				}
			}
			for (int l = 0; l < this.aiTableItemsContent.transform.childCount; l++)
			{
				DiplomacyTableListItem component4 = this.aiTableItemsContent.transform.GetChild(l).GetComponent<DiplomacyTableListItem>();
				if (component4.itemType == TradeItemType.Org || component4.itemType == TradeItemType.Hab || component4.itemType == TradeItemType.Project)
				{
					global::UnityEngine.Object.Destroy(component4.gameObject);
				}
			}
		}

		// Token: 0x0600534F RID: 21327 RVA: 0x00252783 File Offset: 0x00250983
		public void OnClickClear()
		{
			this.ResetDealTable();
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Query", false, false);
			this.EvaluateTrade();
		}

		// Token: 0x06005350 RID: 21328 RVA: 0x0025279D File Offset: 0x0025099D
		public void ToggleHabs(bool player)
		{
			this.ToggleTradeItems(player, TradeItemType.Hab);
		}

		// Token: 0x06005351 RID: 21329 RVA: 0x002527A7 File Offset: 0x002509A7
		public void ToggleProjects(bool player)
		{
			this.ToggleTradeItems(player, TradeItemType.Project);
		}

		// Token: 0x06005352 RID: 21330 RVA: 0x002527B1 File Offset: 0x002509B1
		public void ToggleOrgs(bool player)
		{
			this.ToggleTradeItems(player, TradeItemType.Org);
		}

		// Token: 0x06005353 RID: 21331 RVA: 0x002527BB File Offset: 0x002509BB
		public void ToggleResources(bool player)
		{
			this.ToggleTradeItems(player, TradeItemType.Resource);
		}

		// Token: 0x06005354 RID: 21332 RVA: 0x002527C5 File Offset: 0x002509C5
		public void ToggleTreaties(bool player = false)
		{
			this.ToggleTradeItems(player, TradeItemType.Treaty);
		}

		// Token: 0x06005355 RID: 21333 RVA: 0x002527D0 File Offset: 0x002509D0
		private void ToggleTradeItems(bool player, TradeItemType tradeItemType)
		{
			bool flag = false;
			bool flag2 = false;
			DiplomacyBankListItem diplomacyBankListItem = null;
			DiplomacyBankListItem diplomacyBankListItem2 = null;
			switch (tradeItemType)
			{
			case TradeItemType.Resource:
				flag = this.playerResourcesVisible;
				flag2 = this.aiResourcesVisible;
				diplomacyBankListItem = this.playerResourcesTab;
				diplomacyBankListItem2 = this.aiResourcesTab;
				break;
			case TradeItemType.Toggle:
				break;
			case TradeItemType.Org:
				flag = this.playerOrgsVisible;
				flag2 = this.aiOrgsVisible;
				diplomacyBankListItem = this.playerOrgTab;
				diplomacyBankListItem2 = this.aiOrgTab;
				break;
			case TradeItemType.Hab:
				flag = this.playerHabsVisible;
				flag2 = this.aiHabsVisible;
				diplomacyBankListItem = this.playerHabsTab;
				diplomacyBankListItem2 = this.aiHabsTab;
				break;
			case TradeItemType.Project:
				flag = this.playerProjectsVisible;
				flag2 = this.aiProjectsVisible;
				diplomacyBankListItem = this.playerProjectsTab;
				diplomacyBankListItem2 = this.aiProjectsTab;
				break;
			default:
				return;
			}
			if (player)
			{
				if (flag)
				{
					flag = false;
					if (player)
					{
						diplomacyBankListItem.tabText.text = "+";
					}
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
				}
				else
				{
					flag = true;
					if (player)
					{
						diplomacyBankListItem.tabText.text = "-";
					}
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
				}
			}
			else if (flag2)
			{
				flag2 = false;
				if (!player)
				{
					diplomacyBankListItem2.tabText.text = "+";
				}
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
			}
			else
			{
				flag2 = true;
				if (!player)
				{
					diplomacyBankListItem2.tabText.text = "-";
				}
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			}
			if (player)
			{
				for (int i = 0; i < this.playerBankItemsContent.transform.childCount; i++)
				{
					if (this.playerBankItemsContent.transform.GetChild(i).GetComponent<DiplomacyBankListItem>().itemType == tradeItemType)
					{
						if (flag)
						{
							this.playerBankItemsContent.transform.GetChild(i).gameObject.SetActive(true);
						}
						else
						{
							this.playerBankItemsContent.transform.GetChild(i).gameObject.SetActive(false);
						}
					}
				}
			}
			else
			{
				for (int j = 0; j < this.aiBankItemsContent.transform.childCount; j++)
				{
					if (this.aiBankItemsContent.transform.GetChild(j).GetComponent<DiplomacyBankListItem>().itemType == tradeItemType)
					{
						if (flag2)
						{
							this.aiBankItemsContent.transform.GetChild(j).gameObject.SetActive(true);
						}
						else
						{
							this.aiBankItemsContent.transform.GetChild(j).gameObject.SetActive(false);
						}
					}
				}
			}
			switch (tradeItemType)
			{
			case TradeItemType.Resource:
				this.playerResourcesVisible = flag;
				this.aiResourcesVisible = flag2;
				break;
			case TradeItemType.Toggle:
				break;
			case TradeItemType.Org:
				this.playerOrgsVisible = flag;
				this.aiOrgsVisible = flag2;
				break;
			case TradeItemType.Hab:
				this.playerHabsVisible = flag;
				this.aiHabsVisible = flag2;
				break;
			case TradeItemType.Project:
				this.playerProjectsVisible = flag;
				this.aiProjectsVisible = flag2;
				break;
			default:
				return;
			}
			this.HideLockedResources(this.playerResourcesVisible, this.aiResourcesVisible);
		}

		// Token: 0x06005356 RID: 21334 RVA: 0x00252A74 File Offset: 0x00250C74
		private void UpdateImproveRelations()
		{
			if ((from child in this.playerTableItemsContent.transform.Children()
				where child.gameObject.activeSelf && child.parent == this.playerTableItemsContent.transform
				select child).Count<Transform>() > 0 || (from child in this.aiTableItemsContent.transform.Children()
				where child.gameObject.activeSelf && child.parent == this.aiTableItemsContent.transform
				select child).Count<Transform>() > (this.aiTableHateReductionItem.gameObject.activeSelf ? 1 : 0))
			{
				this.aiTableHateReductionItem.EnableGameobject();
				return;
			}
			this.aiTableHateReductionItem.DisableGameobject();
		}

		// Token: 0x06005357 RID: 21335 RVA: 0x00252B00 File Offset: 0x00250D00
		public void EvaluateTrade()
		{
			TradeOffer tradeOffer = this.activePlayer.InitializeTradingOptions(this.tradingFaction);
			tradeOffer.resourceValues.Clear();
			tradeOffer.controlPoints.Clear();
			tradeOffer.habSectors.Clear();
			tradeOffer.orgs.Clear();
			tradeOffer.projects.Clear();
			tradeOffer.resourceValues.Add(new ResourceValue(FactionResource.Money, float.Parse(this.playerTableCashItem.quantitySendInput.text)));
			tradeOffer.resourceValues.Add(new ResourceValue(FactionResource.Influence, float.Parse(this.playerTableInfluenceItem.quantitySendInput.text)));
			tradeOffer.resourceValues.Add(new ResourceValue(FactionResource.Operations, float.Parse(this.playerTableOpsItem.quantitySendInput.text)));
			tradeOffer.resourceValues.Add(new ResourceValue(FactionResource.Boost, float.Parse(this.playerTableBoostItem.quantitySendInput.text)));
			tradeOffer.resourceValues.Add(new ResourceValue(FactionResource.Water, float.Parse(this.playerTableWaterItem.quantitySendInput.text)));
			tradeOffer.resourceValues.Add(new ResourceValue(FactionResource.Volatiles, float.Parse(this.playerTableVolatilesItem.quantitySendInput.text)));
			tradeOffer.resourceValues.Add(new ResourceValue(FactionResource.Metals, float.Parse(this.playerTableBaseMetalsItem.quantitySendInput.text)));
			tradeOffer.resourceValues.Add(new ResourceValue(FactionResource.NobleMetals, float.Parse(this.playerTableNobleMetalsItem.quantitySendInput.text)));
			tradeOffer.resourceValues.Add(new ResourceValue(FactionResource.Fissiles, float.Parse(this.playerTableFissilesItem.quantitySendInput.text)));
			tradeOffer.resourceValues.Add(new ResourceValue(FactionResource.Antimatter, float.Parse(this.playerTableAntimatterItem.quantitySendInput.text)));
			tradeOffer.resourceValues.Add(new ResourceValue(FactionResource.Exotics, float.Parse(this.playerTableExoticsItem.quantitySendInput.text)));
			for (int i = 0; i < this.playerTableItemsContent.transform.childCount; i++)
			{
				GameObject gameObject = this.playerTableItemsContent.transform.GetChild(i).gameObject;
				if (gameObject.activeSelf)
				{
					if (gameObject.GetComponent<DiplomacyTableListItem>().itemType == TradeItemType.Org)
					{
						tradeOffer.orgs.Add(gameObject.GetComponent<DiplomacyTableListItem>().orgReference);
					}
					if (gameObject.GetComponent<DiplomacyTableListItem>().itemType == TradeItemType.Hab)
					{
						tradeOffer.habs.Add(gameObject.GetComponent<DiplomacyTableListItem>().habReference);
					}
					if (gameObject.GetComponent<DiplomacyTableListItem>().itemType == TradeItemType.Project)
					{
						tradeOffer.projects.Add(gameObject.GetComponent<DiplomacyTableListItem>().projectReference);
					}
				}
			}
			TradeOffer tradeOffer2 = this.tradingFaction.InitializeTradingOptions(this.activePlayer);
			tradeOffer2.resourceValues.Clear();
			tradeOffer2.controlPoints.Clear();
			tradeOffer2.habSectors.Clear();
			tradeOffer2.orgs.Clear();
			tradeOffer2.projects.Clear();
			tradeOffer2.resourceValues.Add(new ResourceValue(FactionResource.Money, float.Parse(this.aiTableCashItem.quantitySendInput.text)));
			tradeOffer2.resourceValues.Add(new ResourceValue(FactionResource.Influence, float.Parse(this.aiTableInfluenceItem.quantitySendInput.text)));
			tradeOffer2.resourceValues.Add(new ResourceValue(FactionResource.Operations, float.Parse(this.aiTableOpsItem.quantitySendInput.text)));
			tradeOffer2.resourceValues.Add(new ResourceValue(FactionResource.Boost, float.Parse(this.aiTableBoostItem.quantitySendInput.text)));
			tradeOffer2.resourceValues.Add(new ResourceValue(FactionResource.Water, float.Parse(this.aiTableWaterItem.quantitySendInput.text)));
			tradeOffer2.resourceValues.Add(new ResourceValue(FactionResource.Volatiles, float.Parse(this.aiTableVolatilesItem.quantitySendInput.text)));
			tradeOffer2.resourceValues.Add(new ResourceValue(FactionResource.Metals, float.Parse(this.aiTableBaseMetalsItem.quantitySendInput.text)));
			tradeOffer2.resourceValues.Add(new ResourceValue(FactionResource.NobleMetals, float.Parse(this.aiTableNobleMetalsItem.quantitySendInput.text)));
			tradeOffer2.resourceValues.Add(new ResourceValue(FactionResource.Fissiles, float.Parse(this.aiTableFissilesItem.quantitySendInput.text)));
			tradeOffer2.resourceValues.Add(new ResourceValue(FactionResource.Antimatter, float.Parse(this.aiTableAntimatterItem.quantitySendInput.text)));
			tradeOffer2.resourceValues.Add(new ResourceValue(FactionResource.Exotics, float.Parse(this.aiTableExoticsItem.quantitySendInput.text)));
			if (this.aiTableTreatyItem.gameObject.activeSelf)
			{
				tradeOffer2.treatyType = this.aiTableTreatyItem.treaty;
				tradeOffer.treatyType = this.aiTableTreatyItem.treaty;
			}
			if (this.aiTableExchangeIntelItem.gameObject.activeSelf)
			{
				tradeOffer2.intelExchange = true;
				tradeOffer.intelExchange = true;
			}
			for (int j = 0; j < this.aiTableItemsContent.transform.childCount; j++)
			{
				GameObject gameObject2 = this.aiTableItemsContent.transform.GetChild(j).gameObject;
				if (gameObject2.activeSelf)
				{
					if (gameObject2.GetComponent<DiplomacyTableListItem>().itemType == TradeItemType.Org)
					{
						tradeOffer2.orgs.Add(gameObject2.GetComponent<DiplomacyTableListItem>().orgReference);
					}
					if (gameObject2.GetComponent<DiplomacyTableListItem>().itemType == TradeItemType.Hab)
					{
						tradeOffer2.habs.Add(gameObject2.GetComponent<DiplomacyTableListItem>().habReference);
					}
					if (gameObject2.GetComponent<DiplomacyTableListItem>().itemType == TradeItemType.Project)
					{
						tradeOffer2.projects.Add(gameObject2.GetComponent<DiplomacyTableListItem>().projectReference);
					}
				}
			}
			TradeOffer.TradeAgreement tradeAgreement = new ValueTuple<TradeOffer, TradeOffer>(tradeOffer, tradeOffer2);
			float num;
			bool flag = TradeAI.IsAgreementAcceptable(tradeAgreement, this.tradingFaction, this.activePlayer, out num);
			this.UpdateImproveRelations();
			int num2 = global::UnityEngine.Random.Range(1, TemplateManager.global.tradeAcceptanceTextVariants + 1);
			string text = ((num2 >= 2) ? TIUtilities.CombineStrings(new string[]
			{
				"_",
				num2.ToString()
			}) : "");
			if (flag)
			{
				bool flag2 = TradeAI.ScoreAgreement(tradeAgreement, this.tradingFaction) >= TemplateManager.global.meaningfulTradeThreshold;
				float minimumAgreementFavorability = TradeAI.GetMinimumAgreementFavorability(this.tradingFaction, this.activePlayer);
				bool flag3 = num >= 1f || num - minimumAgreementFavorability >= TemplateManager.global.goodTradeThreshold;
				if (!flag2 || !flag3)
				{
					this.aiFeedbackDialogText.text = (this.tradingFaction.IsAlienFaction ? Loc.T("UI.Notifications.Diplomacy.TradeValueEqual.Alien") : Loc.T(TIUtilities.CombineStrings(new string[] { "UI.Notifications.Diplomacy.TradeValueEqual", text })));
					this.hateModifier = 1f;
				}
				else
				{
					this.aiFeedbackDialogText.text = (this.tradingFaction.IsAlienFaction ? Loc.T("UI.Notifications.Diplomacy.TradeValueHigh.Alien") : Loc.T(TIUtilities.CombineStrings(new string[] { "UI.Notifications.Diplomacy.TradeValueHigh", text })));
					this.hateModifier = 2f;
				}
				this.executeTradeButton.interactable = true;
			}
			else
			{
				if (num != 0f)
				{
					this.aiFeedbackDialogText.text = (this.tradingFaction.IsAlienFaction ? Loc.T("UI.Notifications.Diplomacy.TradeValueLow.Alien") : Loc.T(TIUtilities.CombineStrings(new string[] { "UI.Notifications.Diplomacy.TradeValueLow", text })));
				}
				else
				{
					this.aiFeedbackDialogText.text = (this.tradingFaction.IsAlienFaction ? Loc.T("UI.Notifications.Diplomacy.TradeValueVeryLow.Alien") : Loc.T(TIUtilities.CombineStrings(new string[] { "UI.Notifications.Diplomacy.TradeValueVeryLow", text })));
				}
				this.executeTradeButton.interactable = false;
			}
			if ((this.testingAITrade || this.isThisAnAIOffer) && !this.touchedAIOffer)
			{
				this.aiFeedbackDialogText.text = (this.tradingFaction.IsAlienFaction ? Loc.T("UI.Notifications.Diplomacy.TradeValueOffered.Alien") : Loc.T(TIUtilities.CombineStrings(new string[] { "UI.Notifications.Diplomacy.TradeValueOffered", text })));
				this.executeTradeButton.interactable = true;
			}
			if (this.IsTableEmpty())
			{
				this.aiFeedbackDialogText.text = (this.tradingFaction.IsAlienFaction ? Loc.T("UI.Notifications.Diplomacy.TradeValueNone.Alien") : Loc.T(TIUtilities.CombineStrings(new string[] { "UI.Notifications.Diplomacy.TradeValueNone", text })));
				this.executeTradeButton.interactable = false;
			}
			this.playerTradeOffer = tradeOffer;
			this.aiTradeOffer = tradeOffer2;
		}

		// Token: 0x06005358 RID: 21336 RVA: 0x00253388 File Offset: 0x00251588
		private void LoadBankValues()
		{
			this.CleanupOldTradeItems();
			this.playerBankCashItem.quantityText.text = Mathf.FloorToInt(this.activePlayer.GetCurrentResourceAmount(FactionResource.Money)).ToString();
			this.playerBankInfluenceItem.quantityText.text = Mathf.FloorToInt(this.activePlayer.GetCurrentResourceAmount(FactionResource.Influence)).ToString();
			this.playerBankOpsItem.quantityText.text = Mathf.FloorToInt(this.activePlayer.GetCurrentResourceAmount(FactionResource.Operations)).ToString();
			this.playerBankBoostItem.quantityText.text = Mathf.FloorToInt(this.activePlayer.GetCurrentResourceAmount(FactionResource.Boost)).ToString();
			this.playerBankWaterItem.quantityText.text = Mathf.FloorToInt(this.activePlayer.GetCurrentResourceAmount(FactionResource.Water)).ToString();
			this.playerBankVolatilesItem.quantityText.text = Mathf.FloorToInt(this.activePlayer.GetCurrentResourceAmount(FactionResource.Volatiles)).ToString();
			this.playerBankBaseMetalsItem.quantityText.text = Mathf.FloorToInt(this.activePlayer.GetCurrentResourceAmount(FactionResource.Metals)).ToString();
			this.playerBankNobleMetalsItem.quantityText.text = Mathf.FloorToInt(this.activePlayer.GetCurrentResourceAmount(FactionResource.NobleMetals)).ToString();
			this.playerBankFissilesItem.quantityText.text = Mathf.FloorToInt(this.activePlayer.GetCurrentResourceAmount(FactionResource.Fissiles)).ToString();
			this.playerBankAntimatterItem.quantityText.text = Mathf.FloorToInt(this.activePlayer.GetCurrentResourceAmount(FactionResource.Antimatter)).ToString();
			this.playerBankExoticsItem.quantityText.text = Mathf.FloorToInt(this.activePlayer.GetCurrentResourceAmount(FactionResource.Exotics)).ToString();
			using (List<TIOrgState>.Enumerator enumerator = this.activePlayer.GetAllOrgs().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIOrgState org2 = enumerator.Current;
					if (this.activePlayer.CanTradeOrg(org2, this.tradingFaction))
					{
						GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.bankItemPrefab, this.playerBankItemsContent.transform);
						Loc.SwapFonts(gameObject);
						DiplomacyBankListItem component = gameObject.GetComponent<DiplomacyBankListItem>();
						component.quantityText.text = org2.displayName;
						component.itemIcon.sprite = org2.icon;
						component.itemType = TradeItemType.Org;
						component.ShowOrgData(org2);
						component.tooltipTrigger.enabled = true;
						if (component.tooltipTrigger != null)
						{
							component.tooltipTrigger.SetDelegate("BodyText", () => org2.description(true, this.activePlayer, false, false));
							component.tooltipTrigger.tipPosition = TipPosition.MouseLeftMiddle;
						}
						component.diplomacyController = this;
						gameObject.SetActive(false);
						GameObject gameObject2 = global::UnityEngine.Object.Instantiate<GameObject>(this.tableItemPrefab, this.playerTableItemsContent.transform);
						Loc.SwapFonts(gameObject2);
						DiplomacyTableListItem component2 = gameObject2.GetComponent<DiplomacyTableListItem>();
						component2.itemDescription.text = org2.displayName;
						component2.itemType = TradeItemType.Org;
						component2.ShowOrgData(org2);
						component2.tooltipTrigger.enabled = true;
						component2.tooltipTrigger.SetDelegate("BodyText", () => org2.description(true, this.activePlayer, false, false));
						component2.diplomacyController = this;
						component2.itemIcon.sprite = org2.icon;
						component2.DisableGameobject();
						component2.orgReference = org2;
						component.dealTableLink = gameObject2;
					}
				}
			}
			this.playerHabsTab.transform.SetSiblingIndex(this.playerBankItemsContent.transform.childCount - 1);
			if (!this.activePlayer.IsAlienFaction && !this.tradingFaction.IsAlienFaction)
			{
				using (List<TIHabState>.Enumerator enumerator2 = this.activePlayer.habs.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						TIHabState hab2 = enumerator2.Current;
						if (this.activePlayer.MayTradeAwayHab(hab2, this.tradingFaction))
						{
							GameObject gameObject3 = global::UnityEngine.Object.Instantiate<GameObject>(this.bankItemPrefab, this.playerBankItemsContent.transform);
							Loc.SwapFonts(gameObject3);
							DiplomacyBankListItem component3 = gameObject3.GetComponent<DiplomacyBankListItem>();
							component3.quantityText.text = hab2.displayName;
							component3.itemIcon.sprite = hab2.icon;
							component3.itemType = TradeItemType.Hab;
							component3.HideOrgData();
							component3.tooltipTrigger.enabled = true;
							component3.tooltipTrigger.SetDelegate("BodyText", () => hab2.BuildShortHabSummary(this.activePlayer));
							component3.tooltipTrigger.tipPosition = TipPosition.MouseLeftMiddle;
							component3.diplomacyController = this;
							gameObject3.SetActive(false);
							GameObject gameObject4 = global::UnityEngine.Object.Instantiate<GameObject>(this.tableItemPrefab, this.playerTableItemsContent.transform);
							Loc.SwapFonts(gameObject4);
							DiplomacyTableListItem component4 = gameObject4.GetComponent<DiplomacyTableListItem>();
							component4.itemDescription.text = hab2.displayName;
							component4.itemType = TradeItemType.Hab;
							component4.HideOrgData();
							component4.tooltipTrigger.enabled = true;
							component4.tooltipTrigger.SetDelegate("BodyText", () => hab2.description);
							component4.diplomacyController = this;
							component4.itemIcon.sprite = hab2.icon;
							component4.DisableGameobject();
							component4.habReference = hab2;
							component3.dealTableLink = gameObject4;
						}
					}
				}
				this.playerProjectsTab.transform.SetSiblingIndex(this.playerBankItemsContent.transform.childCount - 1);
				this.playerProjectsTab.gameObject.SetActive(true);
				this.playerHabsTab.gameObject.SetActive(true);
				using (List<TIProjectTemplate>.Enumerator enumerator3 = this.activePlayer.completedProjects.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						TIProjectTemplate project2 = enumerator3.Current;
						if (this.activePlayer.CanTradeProject(project2, this.tradingFaction))
						{
							GameObject gameObject5 = global::UnityEngine.Object.Instantiate<GameObject>(this.bankItemPrefab, this.playerBankItemsContent.transform);
							Loc.SwapFonts(gameObject5);
							DiplomacyBankListItem component5 = gameObject5.GetComponent<DiplomacyBankListItem>();
							component5.quantityText.text = project2.displayName;
							GameControl.assetLoader.LoadAssetForImageAssignment(project2.GetCategoryIconPath(), component5.itemIcon);
							component5.itemType = TradeItemType.Project;
							component5.HideOrgData();
							component5.tooltipTrigger.enabled = true;
							component5.tooltipTrigger.SetDelegate("BodyText", () => project2.GetFullDescription(this.activePlayer, TechBenefitsContext.Prospective, null, false));
							component5.tooltipTrigger.tipPosition = TipPosition.MouseLeftMiddle;
							component5.diplomacyController = this;
							gameObject5.SetActive(false);
							GameObject gameObject6 = global::UnityEngine.Object.Instantiate<GameObject>(this.tableItemPrefab, this.playerTableItemsContent.transform);
							Loc.SwapFonts(gameObject6);
							DiplomacyTableListItem component6 = gameObject6.GetComponent<DiplomacyTableListItem>();
							component6.itemDescription.text = project2.displayName;
							component6.itemType = TradeItemType.Project;
							component6.HideOrgData();
							component6.tooltipTrigger.enabled = true;
							component6.tooltipTrigger.SetDelegate("BodyText", () => project2.GetFullDescription(this.activePlayer, TechBenefitsContext.Prospective, null, false));
							component6.diplomacyController = this;
							GameControl.assetLoader.LoadAssetForImageAssignment(project2.GetCategoryIconPath(), component6.itemIcon);
							component6.DisableGameobject();
							component6.projectReference = project2;
							component5.dealTableLink = gameObject6;
						}
					}
					goto IL_07F2;
				}
			}
			this.playerProjectsTab.gameObject.SetActive(false);
			this.playerHabsTab.gameObject.SetActive(false);
			IL_07F2:
			this.aiBankCashItem.quantityText.text = Mathf.FloorToInt(this.tradingFaction.GetCurrentResourceAmount(FactionResource.Money)).ToString();
			this.aiBankInfluenceItem.quantityText.text = Mathf.FloorToInt(this.tradingFaction.GetCurrentResourceAmount(FactionResource.Influence)).ToString();
			this.aiBankOpsItem.quantityText.text = Mathf.FloorToInt(this.tradingFaction.GetCurrentResourceAmount(FactionResource.Operations)).ToString();
			this.aiBankBoostItem.quantityText.text = Mathf.FloorToInt(this.tradingFaction.GetCurrentResourceAmount(FactionResource.Boost)).ToString();
			this.aiBankWaterItem.quantityText.text = Mathf.FloorToInt(this.tradingFaction.GetCurrentResourceAmount(FactionResource.Water)).ToString();
			this.aiBankVolatilesItem.quantityText.text = Mathf.FloorToInt(this.tradingFaction.GetCurrentResourceAmount(FactionResource.Volatiles)).ToString();
			this.aiBankBaseMetalsItem.quantityText.text = Mathf.FloorToInt(this.tradingFaction.GetCurrentResourceAmount(FactionResource.Metals)).ToString();
			this.aiBankNobleMetalsItem.quantityText.text = Mathf.FloorToInt(this.tradingFaction.GetCurrentResourceAmount(FactionResource.NobleMetals)).ToString();
			this.aiBankFissilesItem.quantityText.text = Mathf.FloorToInt(this.tradingFaction.GetCurrentResourceAmount(FactionResource.Fissiles)).ToString();
			this.aiBankAntimatterItem.quantityText.text = Mathf.FloorToInt(this.tradingFaction.GetCurrentResourceAmount(FactionResource.Antimatter)).ToString();
			this.aiBankExoticsItem.quantityText.text = Mathf.FloorToInt(this.tradingFaction.GetCurrentResourceAmount(FactionResource.Exotics)).ToString();
			this.aiTableHateReductionItem.itemDescription.text = Loc.T("UI.Notifications.Diplomacy.ImproveRelations");
			this.aiTableHateReductionItem.tooltipTrigger.SetDelegate("BodyText", () => Loc.T("UI.Notifications.Diplomacy.ImproveRelationsTooltip"));
			bool flag = this.tradingFaction.CanTradeTruce(this.activePlayer);
			bool flag2 = this.tradingFaction.CanTradeNAP(this.activePlayer);
			bool flag3 = this.tradingFaction.CanTradeIntelSharing(this.activePlayer, false);
			this.aiBankTreatyItem.quantityText.text = "MissingTreatyText";
			if (flag && AIEvaluators.GetWillingnessToTradeTruce(this.tradingFaction, this.activePlayer, false) > 0)
			{
				this.aiBankTreatyItem.gameObject.SetActive(true);
				this.aiBankTreatyItem.button.interactable = true;
				this.aiBankTreatyItem.quantityText.text = Loc.T("UI.Notifications.Diplomacy.Truce");
				this.aiBankTreatyItem.tooltipTrigger.SetDelegate("BodyText", () => Loc.T("UI.Notifications.Diplomacy.TruceDesc", new object[] { 12 }));
				this.aiTableTreatyItem.itemDescription.text = Loc.T("UI.Notifications.Diplomacy.Truce");
				this.aiTableTreatyItem.tooltipTrigger.SetDelegate("BodyText", () => Loc.T("UI.Notifications.Diplomacy.TruceDesc", new object[] { 12 }));
				this.aiTableTreatyItem.treaty = TradeOffer.TreatyType.Truce;
				this.playerBankTreatyItem.gameObject.SetActive(true);
				this.playerBankTreatyItem.button.interactable = true;
				this.playerBankTreatyItem.quantityText.text = Loc.T("UI.Notifications.Diplomacy.Truce");
				this.playerBankTreatyItem.tooltipTrigger.SetDelegate("BodyText", () => Loc.T("UI.Notifications.Diplomacy.TruceDesc", new object[] { 12 }));
				this.playerTableTreatyItem.itemDescription.text = Loc.T("UI.Notifications.Diplomacy.Truce");
				this.playerTableTreatyItem.tooltipTrigger.SetDelegate("BodyText", () => Loc.T("UI.Notifications.Diplomacy.TruceDesc", new object[] { 12 }));
				this.playerTableTreatyItem.treaty = TradeOffer.TreatyType.Truce;
			}
			else if (flag2 && AIEvaluators.GetWillingnessToTradeNAP(this.tradingFaction, this.activePlayer, false) > 0)
			{
				this.aiBankTreatyItem.gameObject.SetActive(true);
				this.aiBankTreatyItem.button.interactable = true;
				this.aiBankTreatyItem.quantityText.text = Loc.T("UI.Notifications.Diplomacy.NAP");
				this.aiBankTreatyItem.tooltipTrigger.SetDelegate("BodyText", () => Loc.T("UI.Notifications.Diplomacy.NAPDesc"));
				this.aiTableTreatyItem.itemDescription.text = Loc.T("UI.Notifications.Diplomacy.NAP");
				this.aiTableTreatyItem.tooltipTrigger.SetDelegate("BodyText", () => Loc.T("UI.Notifications.Diplomacy.NAPDesc"));
				this.aiTableTreatyItem.treaty = TradeOffer.TreatyType.NAP;
				this.playerBankTreatyItem.gameObject.SetActive(true);
				this.playerBankTreatyItem.button.interactable = true;
				this.playerBankTreatyItem.quantityText.text = Loc.T("UI.Notifications.Diplomacy.NAP");
				this.playerBankTreatyItem.tooltipTrigger.SetDelegate("BodyText", () => Loc.T("UI.Notifications.Diplomacy.NAPDesc"));
				this.playerTableTreatyItem.itemDescription.text = Loc.T("UI.Notifications.Diplomacy.NAP");
				this.playerTableTreatyItem.tooltipTrigger.SetDelegate("BodyText", () => Loc.T("UI.Notifications.Diplomacy.NAPDesc"));
				this.playerTableTreatyItem.treaty = TradeOffer.TreatyType.NAP;
				this.playerBankNAPItem_Info.gameObject.SetActive(false);
				this.aiBankNAPItem_Info.gameObject.SetActive(false);
			}
			else if (flag3 && AIEvaluators.GetWillingnessToShareIntel(this.tradingFaction, this.activePlayer, false, false) > 0)
			{
				this.aiBankTreatyItem.gameObject.SetActive(true);
				this.aiBankTreatyItem.button.interactable = true;
				this.aiBankTreatyItem.quantityText.text = Loc.T("UI.Notifications.Diplomacy.IntelSharing");
				this.aiBankTreatyItem.tooltipTrigger.SetDelegate("BodyText", () => Loc.T("UI.Notifications.Diplomacy.IntelSharingDesc"));
				this.aiTableTreatyItem.itemDescription.text = Loc.T("UI.Notifications.Diplomacy.IntelSharing");
				this.aiTableTreatyItem.tooltipTrigger.SetDelegate("BodyText", () => Loc.T("UI.Notifications.Diplomacy.IntelSharingDesc"));
				this.aiTableTreatyItem.treaty = TradeOffer.TreatyType.Intel;
				this.playerBankTreatyItem.gameObject.SetActive(true);
				this.playerBankTreatyItem.button.interactable = true;
				this.playerBankTreatyItem.quantityText.text = Loc.T("UI.Notifications.Diplomacy.IntelSharing");
				this.playerBankTreatyItem.tooltipTrigger.SetDelegate("BodyText", () => Loc.T("UI.Notifications.Diplomacy.IntelSharingDesc"));
				this.playerTableTreatyItem.itemDescription.text = Loc.T("UI.Notifications.Diplomacy.IntelSharing");
				this.playerTableTreatyItem.tooltipTrigger.SetDelegate("BodyText", () => Loc.T("UI.Notifications.Diplomacy.IntelSharingDesc"));
				this.playerTableTreatyItem.treaty = TradeOffer.TreatyType.Intel;
			}
			else
			{
				this.aiBankTreatyItem.gameObject.SetActive(false);
				this.playerBankTreatyItem.gameObject.SetActive(false);
			}
			if (!flag && !this.activePlayer.HasTruce(this.tradingFaction, true) && !this.activePlayer.HasNAP(this.tradingFaction, true) && !this.activePlayer.permanentAlly(this.tradingFaction))
			{
				this.playerBankTruceItem_Info.gameObject.SetActive(true);
				this.aiBankTruceItem_Info.gameObject.SetActive(true);
				this.playerBankTruceItem_Info.tooltipTrigger.SetDelegate("BodyText", () => this.tradingFaction.NoTruceFeedback(this.activePlayer));
				this.aiBankTruceItem_Info.tooltipTrigger.SetDelegate("BodyText", () => this.tradingFaction.NoTruceFeedback(this.activePlayer));
			}
			if (!flag2 && !this.activePlayer.HasNAP(this.tradingFaction, true) && !this.activePlayer.permanentAlly(this.tradingFaction))
			{
				this.playerBankNAPItem_Info.gameObject.SetActive(true);
				this.aiBankNAPItem_Info.gameObject.SetActive(true);
				this.playerBankNAPItem_Info.tooltipTrigger.SetDelegate("BodyText", () => this.tradingFaction.NoNAPTradeFeedback(this.activePlayer, true));
				this.aiBankNAPItem_Info.tooltipTrigger.SetDelegate("BodyText", () => this.tradingFaction.NoNAPTradeFeedback(this.activePlayer, true));
			}
			if (!flag3 && !this.activePlayer.intelSharingFactions.Contains(this.tradingFaction))
			{
				this.SetNoIntelReason();
			}
			if (flag3 && AIEvaluators.GetWillingnessToShareIntel(this.tradingFaction, this.activePlayer, false, false) > 0)
			{
				this.aiBankExchangeIntelItem.gameObject.SetActive(true);
				this.aiBankExchangeIntelItem.quantityText.text = Loc.T("UI.Notifications.Diplomacy.ExchangeIntel");
				this.aiBankExchangeIntelItem.tooltipTrigger.SetDelegate("BodyText", () => Loc.T("UI.Notifications.Diplomacy.ExchangeIntelDesc"));
				this.aiTableExchangeIntelItem.itemDescription.text = Loc.T("UI.Notifications.Diplomacy.ExchangeIntel");
				this.aiTableExchangeIntelItem.tooltipTrigger.SetDelegate("BodyText", () => Loc.T("UI.Notifications.Diplomacy.ExchangeIntelDesc"));
				this.playerBankExchangeIntelItem.gameObject.SetActive(true);
				this.playerBankExchangeIntelItem.quantityText.text = Loc.T("UI.Notifications.Diplomacy.ExchangeIntel");
				this.playerBankExchangeIntelItem.tooltipTrigger.SetDelegate("BodyText", () => Loc.T("UI.Notifications.Diplomacy.ExchangeIntelDesc"));
				this.playerTableExchangeIntelItem.itemDescription.text = Loc.T("UI.Notifications.Diplomacy.ExchangeIntel");
				this.playerTableExchangeIntelItem.tooltipTrigger.SetDelegate("BodyText", () => Loc.T("UI.Notifications.Diplomacy.ExchangeIntelDesc"));
			}
			else
			{
				this.playerBankExchangeIntelItem.gameObject.SetActive(false);
				this.aiBankExchangeIntelItem.gameObject.SetActive(false);
				this.SetNoIntelReason();
			}
			using (List<TIOrgState>.Enumerator enumerator = this.tradingFaction.GetAllOrgs().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIOrgState org = enumerator.Current;
					if (org.IsEligibleForFaction(this.activePlayer))
					{
						TICouncilorState assignedCouncilor = org.assignedCouncilor;
						if (assignedCouncilor == null || assignedCouncilor.CanRemoveOrg(org))
						{
							GameObject gameObject7 = global::UnityEngine.Object.Instantiate<GameObject>(this.bankItemPrefab, this.aiBankItemsContent.transform);
							Loc.SwapFonts(gameObject7);
							DiplomacyBankListItem component7 = gameObject7.GetComponent<DiplomacyBankListItem>();
							component7.quantityText.text = org.displayName;
							component7.itemIcon.sprite = org.icon;
							component7.itemType = TradeItemType.Org;
							component7.ShowOrgData(org);
							component7.tooltipTrigger.enabled = true;
							component7.tooltipTrigger.SetDelegate("BodyText", () => org.description(true, this.activePlayer, false, false));
							component7.tooltipTrigger.tipPosition = TipPosition.MouseRightMiddle;
							component7.diplomacyController = this;
							gameObject7.SetActive(false);
							GameObject gameObject8 = global::UnityEngine.Object.Instantiate<GameObject>(this.tableItemPrefab, this.aiTableItemsContent.transform);
							Loc.SwapFonts(gameObject8);
							DiplomacyTableListItem component8 = gameObject8.GetComponent<DiplomacyTableListItem>();
							component8.itemDescription.text = org.displayName;
							component8.itemType = TradeItemType.Org;
							component8.ShowOrgData(org);
							component8.tooltipTrigger.enabled = true;
							component8.tooltipTrigger.SetDelegate("BodyText", () => org.description(true, this.activePlayer, false, false));
							component8.diplomacyController = this;
							component8.itemIcon.sprite = org.icon;
							component8.DisableGameobject();
							component8.orgReference = org;
							component7.dealTableLink = gameObject8;
						}
					}
				}
			}
			this.aiHabsTab.transform.SetSiblingIndex(this.aiBankItemsContent.transform.childCount - 1);
			if (!this.activePlayer.IsAlienFaction && !this.tradingFaction.IsAlienFaction)
			{
				using (List<TIHabState>.Enumerator enumerator2 = this.tradingFaction.habs.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						TIHabState hab = enumerator2.Current;
						if (this.tradingFaction.MayTradeAwayHab(hab, this.activePlayer))
						{
							GameObject gameObject9 = global::UnityEngine.Object.Instantiate<GameObject>(this.bankItemPrefab, this.aiBankItemsContent.transform);
							Loc.SwapFonts(gameObject9);
							DiplomacyBankListItem component9 = gameObject9.GetComponent<DiplomacyBankListItem>();
							component9.quantityText.text = hab.displayName;
							component9.itemIcon.sprite = hab.icon;
							component9.itemType = TradeItemType.Hab;
							component9.HideOrgData();
							component9.tooltipTrigger.enabled = true;
							component9.tooltipTrigger.SetDelegate("BodyText", () => hab.BuildShortHabSummary(this.activePlayer));
							component9.tooltipTrigger.tipPosition = TipPosition.MouseLeftMiddle;
							component9.diplomacyController = this;
							gameObject9.SetActive(false);
							GameObject gameObject10 = global::UnityEngine.Object.Instantiate<GameObject>(this.tableItemPrefab, this.aiTableItemsContent.transform);
							Loc.SwapFonts(gameObject10);
							DiplomacyTableListItem component10 = gameObject10.GetComponent<DiplomacyTableListItem>();
							component10.itemDescription.text = hab.displayName;
							component10.itemType = TradeItemType.Hab;
							component10.HideOrgData();
							component10.tooltipTrigger.enabled = true;
							component10.tooltipTrigger.SetDelegate("BodyText", () => hab.description);
							component10.diplomacyController = this;
							component10.itemIcon.sprite = hab.icon;
							component10.DisableGameobject();
							component10.habReference = hab;
							component9.dealTableLink = gameObject10;
						}
					}
				}
				this.aiProjectsTab.transform.SetSiblingIndex(this.aiBankItemsContent.transform.childCount - 1);
				this.aiProjectsTab.gameObject.SetActive(true);
				this.aiHabsTab.gameObject.SetActive(true);
				using (List<TIProjectTemplate>.Enumerator enumerator3 = this.tradingFaction.completedProjects.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						TIProjectTemplate project = enumerator3.Current;
						if (this.tradingFaction.CanTradeProject(project, this.activePlayer))
						{
							GameObject gameObject11 = global::UnityEngine.Object.Instantiate<GameObject>(this.bankItemPrefab, this.aiBankItemsContent.transform);
							Loc.SwapFonts(gameObject11);
							DiplomacyBankListItem component11 = gameObject11.GetComponent<DiplomacyBankListItem>();
							component11.quantityText.text = project.displayName;
							GameControl.assetLoader.LoadAssetForImageAssignment(project.GetCategoryIconPath(), component11.itemIcon);
							component11.itemType = TradeItemType.Project;
							component11.HideOrgData();
							component11.tooltipTrigger.enabled = true;
							component11.tooltipTrigger.SetDelegate("BodyText", () => project.GetFullDescription(this.activePlayer, TechBenefitsContext.Prospective, null, false));
							component11.tooltipTrigger.tipPosition = TipPosition.MouseLeftMiddle;
							component11.diplomacyController = this;
							gameObject11.SetActive(false);
							GameObject gameObject12 = global::UnityEngine.Object.Instantiate<GameObject>(this.tableItemPrefab, this.aiTableItemsContent.transform);
							Loc.SwapFonts(gameObject12);
							DiplomacyTableListItem component12 = gameObject12.GetComponent<DiplomacyTableListItem>();
							component12.itemDescription.text = project.displayName;
							component12.itemType = TradeItemType.Project;
							component12.HideOrgData();
							component12.tooltipTrigger.enabled = true;
							component12.tooltipTrigger.SetDelegate("BodyText", () => project.GetFullDescription(this.activePlayer, TechBenefitsContext.Prospective, null, false));
							component12.diplomacyController = this;
							GameControl.assetLoader.LoadAssetForImageAssignment(project.GetCategoryIconPath(), component12.itemIcon);
							component12.DisableGameobject();
							component12.projectReference = project;
							component11.dealTableLink = gameObject12;
						}
					}
					goto IL_18E9;
				}
			}
			this.aiProjectsTab.gameObject.SetActive(false);
			this.aiHabsTab.gameObject.SetActive(false);
			IL_18E9:
			this.playerTableCashItem.Init(this, this.activePlayer, this.tradingFaction, FactionResource.Money);
			this.playerTableInfluenceItem.Init(this, this.activePlayer, this.tradingFaction, FactionResource.Influence);
			this.playerTableOpsItem.Init(this, this.activePlayer, this.tradingFaction, FactionResource.Operations);
			this.playerTableBoostItem.Init(this, this.activePlayer, this.tradingFaction, FactionResource.Boost);
			this.playerTableWaterItem.Init(this, this.activePlayer, this.tradingFaction, FactionResource.Water);
			this.playerTableVolatilesItem.Init(this, this.activePlayer, this.tradingFaction, FactionResource.Volatiles);
			this.playerTableBaseMetalsItem.Init(this, this.activePlayer, this.tradingFaction, FactionResource.Metals);
			this.playerTableNobleMetalsItem.Init(this, this.activePlayer, this.tradingFaction, FactionResource.NobleMetals);
			this.playerTableFissilesItem.Init(this, this.activePlayer, this.tradingFaction, FactionResource.Fissiles);
			this.playerTableAntimatterItem.Init(this, this.activePlayer, this.tradingFaction, FactionResource.Antimatter);
			this.playerTableExoticsItem.Init(this, this.activePlayer, this.tradingFaction, FactionResource.Exotics);
			this.playerTableTreatyItem.Init(this, this.tradingFaction, this.activePlayer, FactionResource.None);
			this.playerTableExchangeIntelItem.Init(this, this.tradingFaction, this.activePlayer, FactionResource.None);
			this.aiTableCashItem.Init(this, this.tradingFaction, this.activePlayer, FactionResource.Money);
			this.aiTableInfluenceItem.Init(this, this.tradingFaction, this.activePlayer, FactionResource.Influence);
			this.aiTableOpsItem.Init(this, this.tradingFaction, this.activePlayer, FactionResource.Operations);
			this.aiTableBoostItem.Init(this, this.tradingFaction, this.activePlayer, FactionResource.Boost);
			this.aiTableWaterItem.Init(this, this.tradingFaction, this.activePlayer, FactionResource.Water);
			this.aiTableVolatilesItem.Init(this, this.tradingFaction, this.activePlayer, FactionResource.Volatiles);
			this.aiTableBaseMetalsItem.Init(this, this.tradingFaction, this.activePlayer, FactionResource.Metals);
			this.aiTableNobleMetalsItem.Init(this, this.tradingFaction, this.activePlayer, FactionResource.NobleMetals);
			this.aiTableFissilesItem.Init(this, this.tradingFaction, this.activePlayer, FactionResource.Fissiles);
			this.aiTableAntimatterItem.Init(this, this.tradingFaction, this.activePlayer, FactionResource.Antimatter);
			this.aiTableExoticsItem.Init(this, this.tradingFaction, this.activePlayer, FactionResource.Exotics);
			this.aiTableTreatyItem.Init(this, this.tradingFaction, this.activePlayer, FactionResource.None);
			this.aiTableHateReductionItem.Init(this, this.tradingFaction, this.activePlayer, FactionResource.None);
			this.aiTableExchangeIntelItem.Init(this, this.tradingFaction, this.activePlayer, FactionResource.None);
			this.HideLockedResources(true, true);
		}

		// Token: 0x06005359 RID: 21337 RVA: 0x00254FCC File Offset: 0x002531CC
		private void SetNoIntelReason()
		{
			this.playerBankIntelItem_Info.gameObject.SetActive(true);
			this.aiBankIntelItem_Info.gameObject.SetActive(true);
			this.playerBankIntelItem_Info.tooltipTrigger.SetDelegate("BodyText", () => this.tradingFaction.NoIntelFeedback(this.activePlayer));
			this.aiBankIntelItem_Info.tooltipTrigger.SetDelegate("BodyText", () => this.tradingFaction.NoIntelFeedback(this.activePlayer));
		}

		// Token: 0x0600535A RID: 21338 RVA: 0x0025503D File Offset: 0x0025323D
		public void OnHabDecommissioned(HabDecommissionStatusChange e)
		{
			this.RevalidateHabsInTrade();
		}

		// Token: 0x0600535B RID: 21339 RVA: 0x00255045 File Offset: 0x00253245
		public void OnHabModuleConstructionStatusChange(HabModuleConstructionStatusChange e)
		{
			this.RevalidateHabsInTrade();
		}

		// Token: 0x0600535C RID: 21340 RVA: 0x0025504D File Offset: 0x0025324D
		public void OnHabModuleDecommissionStatusChange(HabModuleDecommissionStatusChange e)
		{
			this.RevalidateHabsInTrade();
		}

		// Token: 0x0600535D RID: 21341 RVA: 0x00255058 File Offset: 0x00253258
		public void OnResourcesUpdated(FactionResourcesUpdated e)
		{
			for (int i = 0; i < this.playerBankItemsContent.transform.childCount; i++)
			{
				DiplomacyBankListItem component = this.playerBankItemsContent.transform.GetChild(i).GetComponent<DiplomacyBankListItem>();
				DiplomacyTableListItem diplomacyTableListItem = null;
				if (component.dealTableLink != null)
				{
					diplomacyTableListItem = component.dealTableLink.GetComponent<DiplomacyTableListItem>();
				}
				if (component.itemType == TradeItemType.Resource && diplomacyTableListItem != null)
				{
					diplomacyTableListItem.UpdateResourceLimit();
					component.quantityText.text = diplomacyTableListItem.quantityOwnedText.text;
					diplomacyTableListItem.OnValueChanged();
				}
			}
		}

		// Token: 0x0600535E RID: 21342 RVA: 0x002550E8 File Offset: 0x002532E8
		public void CouncilorValuesChanged(CouncilorValuesChanged e)
		{
			List<TIOrgState> list = new List<TIOrgState>();
			list = this.activePlayer.GetAllOrgs();
			for (int i = 0; i < this.playerBankItemsContent.transform.childCount; i++)
			{
				DiplomacyBankListItem component = this.playerBankItemsContent.transform.GetChild(i).GetComponent<DiplomacyBankListItem>();
				DiplomacyTableListItem diplomacyTableListItem = null;
				if (component.dealTableLink != null)
				{
					diplomacyTableListItem = component.dealTableLink.GetComponent<DiplomacyTableListItem>();
				}
				if (component.itemType == TradeItemType.Org && diplomacyTableListItem != null && !list.Contains(diplomacyTableListItem.orgReference))
				{
					diplomacyTableListItem.OnRightClick(false);
					global::UnityEngine.Object.Destroy(component.gameObject);
				}
			}
		}

		// Token: 0x0600535F RID: 21343 RVA: 0x00255188 File Offset: 0x00253388
		private void RevalidateHabsInTrade()
		{
			for (int i = 0; i < this.playerBankItemsContent.transform.childCount; i++)
			{
				DiplomacyBankListItem component = this.playerBankItemsContent.transform.GetChild(i).GetComponent<DiplomacyBankListItem>();
				DiplomacyTableListItem diplomacyTableListItem = null;
				if (component.dealTableLink != null)
				{
					diplomacyTableListItem = component.dealTableLink.GetComponent<DiplomacyTableListItem>();
				}
				if (component.itemType == TradeItemType.Hab && diplomacyTableListItem != null && !this.activePlayer.MayTradeAwayHab(diplomacyTableListItem.habReference, this.tradingFaction))
				{
					diplomacyTableListItem.quantitySendInput.text = "0";
					diplomacyTableListItem.DisableGameobject();
					global::UnityEngine.Object.Destroy(component.gameObject);
				}
			}
			for (int j = 0; j < this.aiBankItemsContent.transform.childCount; j++)
			{
				DiplomacyBankListItem component2 = this.aiBankItemsContent.transform.GetChild(j).GetComponent<DiplomacyBankListItem>();
				DiplomacyTableListItem diplomacyTableListItem2 = null;
				if (component2.dealTableLink != null)
				{
					diplomacyTableListItem2 = component2.dealTableLink.GetComponent<DiplomacyTableListItem>();
				}
				if (component2.itemType == TradeItemType.Hab && diplomacyTableListItem2 != null && !this.tradingFaction.MayTradeAwayHab(diplomacyTableListItem2.habReference, this.activePlayer))
				{
					diplomacyTableListItem2.quantitySendInput.text = "0";
					diplomacyTableListItem2.DisableGameobject();
					global::UnityEngine.Object.Destroy(component2.gameObject);
				}
			}
			this.EvaluateTrade();
		}

		// Token: 0x06005360 RID: 21344 RVA: 0x002552E8 File Offset: 0x002534E8
		private void HideLockedResources(bool playerResourcesVisible = true, bool aiResourcesVisible = true)
		{
			bool flag = !this.activePlayer.UnlockedSpaceResources || !this.tradingFaction.UnlockedSpaceResources;
			bool flag2 = !this.activePlayer.UnlockedAntimatter || !this.tradingFaction.UnlockedAntimatter;
			bool flag3 = !this.activePlayer.UnlockedExotics || !this.tradingFaction.UnlockedExotics;
			this.aiBankCashItem.gameObject.SetActive(aiResourcesVisible);
			this.aiBankInfluenceItem.gameObject.SetActive(aiResourcesVisible);
			this.aiBankOpsItem.gameObject.SetActive(aiResourcesVisible);
			this.aiBankBoostItem.gameObject.SetActive(aiResourcesVisible);
			this.playerBankWaterItem.gameObject.SetActive(playerResourcesVisible && !flag);
			this.playerBankVolatilesItem.gameObject.SetActive(playerResourcesVisible && !flag);
			this.playerBankBaseMetalsItem.gameObject.SetActive(playerResourcesVisible && !flag);
			this.playerBankNobleMetalsItem.gameObject.SetActive(playerResourcesVisible && !flag);
			this.playerBankFissilesItem.gameObject.SetActive(playerResourcesVisible && !flag);
			this.aiBankWaterItem.gameObject.SetActive(aiResourcesVisible && !flag);
			this.aiBankVolatilesItem.gameObject.SetActive(aiResourcesVisible && !flag);
			this.aiBankBaseMetalsItem.gameObject.SetActive(aiResourcesVisible && !flag);
			this.aiBankNobleMetalsItem.gameObject.SetActive(aiResourcesVisible && !flag);
			this.aiBankFissilesItem.gameObject.SetActive(aiResourcesVisible && !flag);
			this.playerBankAntimatterItem.gameObject.SetActive(playerResourcesVisible && !flag2);
			this.aiBankAntimatterItem.gameObject.SetActive(!aiResourcesVisible && !flag2);
			this.playerBankExoticsItem.gameObject.SetActive(playerResourcesVisible && !flag3);
			this.aiBankExoticsItem.gameObject.SetActive(!aiResourcesVisible && !flag3);
		}

		// Token: 0x06005361 RID: 21345 RVA: 0x00255502 File Offset: 0x00253702
		public void MinimizeTradeWindowPressed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.tradeCanvas.sortingOrder = 9;
			this.tradeBodyObject.SetActive(!this.tradeBodyObject.activeSelf);
			this.UpdateTradeWindowMinimizeStatus();
		}

		// Token: 0x06005362 RID: 21346 RVA: 0x0025553C File Offset: 0x0025373C
		private void MaximizeTradeWindow()
		{
			this.tradeCanvas.sortingOrder = 16;
			this.tradeBodyObject.SetActive(true);
			this.UpdateTradeWindowMinimizeStatus();
		}

		// Token: 0x06005363 RID: 21347 RVA: 0x0025555D File Offset: 0x0025375D
		private void UpdateTradeWindowMinimizeStatus()
		{
			TIUtilities.UpdateButtonSpritesPlusMinus(this.minimizeTradeWindowButton, !this.tradeBodyObject.activeSelf, false);
		}

		// Token: 0x06005364 RID: 21348 RVA: 0x0025557C File Offset: 0x0025377C
		public void CleanupListeners()
		{
			GameControl.eventManager.RemoveListener<HabDecommissionStatusChange>(new EventManager.EventDelegate<HabDecommissionStatusChange>(this.OnHabDecommissioned), null);
			GameControl.eventManager.RemoveListener<HabModuleConstructionStatusChange>(new EventManager.EventDelegate<HabModuleConstructionStatusChange>(this.OnHabModuleConstructionStatusChange), null);
			GameControl.eventManager.RemoveListener<HabModuleDecommissionStatusChange>(new EventManager.EventDelegate<HabModuleDecommissionStatusChange>(this.OnHabModuleDecommissionStatusChange), null);
			GameControl.eventManager.RemoveListener<FactionResourcesUpdated>(new EventManager.EventDelegate<FactionResourcesUpdated>(this.OnResourcesUpdated), null);
			GameControl.eventManager.RemoveListener<CouncilorValuesChanged>(new EventManager.EventDelegate<CouncilorValuesChanged>(this.CouncilorValuesChanged), null);
		}

		// Token: 0x040038C4 RID: 14532
		[Header("Bank Player")]
		public DiplomacyBankListItem playerBankCashItem;

		// Token: 0x040038C5 RID: 14533
		public DiplomacyBankListItem playerBankInfluenceItem;

		// Token: 0x040038C6 RID: 14534
		public DiplomacyBankListItem playerBankOpsItem;

		// Token: 0x040038C7 RID: 14535
		public DiplomacyBankListItem playerBankBoostItem;

		// Token: 0x040038C8 RID: 14536
		public DiplomacyBankListItem playerBankWaterItem;

		// Token: 0x040038C9 RID: 14537
		public DiplomacyBankListItem playerBankVolatilesItem;

		// Token: 0x040038CA RID: 14538
		public DiplomacyBankListItem playerBankBaseMetalsItem;

		// Token: 0x040038CB RID: 14539
		public DiplomacyBankListItem playerBankNobleMetalsItem;

		// Token: 0x040038CC RID: 14540
		public DiplomacyBankListItem playerBankFissilesItem;

		// Token: 0x040038CD RID: 14541
		public DiplomacyBankListItem playerBankAntimatterItem;

		// Token: 0x040038CE RID: 14542
		public DiplomacyBankListItem playerBankExoticsItem;

		// Token: 0x040038CF RID: 14543
		public DiplomacyBankListItem playerBankTreatyItem;

		// Token: 0x040038D0 RID: 14544
		public DiplomacyBankListItem playerBankNAPItem_Info;

		// Token: 0x040038D1 RID: 14545
		public DiplomacyBankListItem playerBankIntelItem_Info;

		// Token: 0x040038D2 RID: 14546
		public DiplomacyBankListItem playerBankTruceItem_Info;

		// Token: 0x040038D3 RID: 14547
		public DiplomacyBankListItem playerBankExchangeIntelItem;

		// Token: 0x040038D4 RID: 14548
		[Header("Bank AI")]
		public DiplomacyBankListItem aiBankCashItem;

		// Token: 0x040038D5 RID: 14549
		public DiplomacyBankListItem aiBankInfluenceItem;

		// Token: 0x040038D6 RID: 14550
		public DiplomacyBankListItem aiBankOpsItem;

		// Token: 0x040038D7 RID: 14551
		public DiplomacyBankListItem aiBankBoostItem;

		// Token: 0x040038D8 RID: 14552
		public DiplomacyBankListItem aiBankWaterItem;

		// Token: 0x040038D9 RID: 14553
		public DiplomacyBankListItem aiBankVolatilesItem;

		// Token: 0x040038DA RID: 14554
		public DiplomacyBankListItem aiBankBaseMetalsItem;

		// Token: 0x040038DB RID: 14555
		public DiplomacyBankListItem aiBankNobleMetalsItem;

		// Token: 0x040038DC RID: 14556
		public DiplomacyBankListItem aiBankFissilesItem;

		// Token: 0x040038DD RID: 14557
		public DiplomacyBankListItem aiBankAntimatterItem;

		// Token: 0x040038DE RID: 14558
		public DiplomacyBankListItem aiBankExoticsItem;

		// Token: 0x040038DF RID: 14559
		public DiplomacyBankListItem aiBankTreatyItem;

		// Token: 0x040038E0 RID: 14560
		public DiplomacyBankListItem aiBankNAPItem_Info;

		// Token: 0x040038E1 RID: 14561
		public DiplomacyBankListItem aiBankIntelItem_Info;

		// Token: 0x040038E2 RID: 14562
		public DiplomacyBankListItem aiBankTruceItem_Info;

		// Token: 0x040038E3 RID: 14563
		public DiplomacyBankListItem aiBankExchangeIntelItem;

		// Token: 0x040038E4 RID: 14564
		[Header("DealTable Player")]
		public DiplomacyTableListItem playerTableCashItem;

		// Token: 0x040038E5 RID: 14565
		public DiplomacyTableListItem playerTableInfluenceItem;

		// Token: 0x040038E6 RID: 14566
		public DiplomacyTableListItem playerTableOpsItem;

		// Token: 0x040038E7 RID: 14567
		public DiplomacyTableListItem playerTableBoostItem;

		// Token: 0x040038E8 RID: 14568
		public DiplomacyTableListItem playerTableWaterItem;

		// Token: 0x040038E9 RID: 14569
		public DiplomacyTableListItem playerTableVolatilesItem;

		// Token: 0x040038EA RID: 14570
		public DiplomacyTableListItem playerTableBaseMetalsItem;

		// Token: 0x040038EB RID: 14571
		public DiplomacyTableListItem playerTableNobleMetalsItem;

		// Token: 0x040038EC RID: 14572
		public DiplomacyTableListItem playerTableFissilesItem;

		// Token: 0x040038ED RID: 14573
		public DiplomacyTableListItem playerTableAntimatterItem;

		// Token: 0x040038EE RID: 14574
		public DiplomacyTableListItem playerTableExoticsItem;

		// Token: 0x040038EF RID: 14575
		public DiplomacyTableListItem playerTableTreatyItem;

		// Token: 0x040038F0 RID: 14576
		public DiplomacyTableListItem playerTableExchangeIntelItem;

		// Token: 0x040038F1 RID: 14577
		[Header("DealTable AI")]
		public DiplomacyTableListItem aiTableCashItem;

		// Token: 0x040038F2 RID: 14578
		public DiplomacyTableListItem aiTableInfluenceItem;

		// Token: 0x040038F3 RID: 14579
		public DiplomacyTableListItem aiTableOpsItem;

		// Token: 0x040038F4 RID: 14580
		public DiplomacyTableListItem aiTableBoostItem;

		// Token: 0x040038F5 RID: 14581
		public DiplomacyTableListItem aiTableWaterItem;

		// Token: 0x040038F6 RID: 14582
		public DiplomacyTableListItem aiTableVolatilesItem;

		// Token: 0x040038F7 RID: 14583
		public DiplomacyTableListItem aiTableBaseMetalsItem;

		// Token: 0x040038F8 RID: 14584
		public DiplomacyTableListItem aiTableNobleMetalsItem;

		// Token: 0x040038F9 RID: 14585
		public DiplomacyTableListItem aiTableFissilesItem;

		// Token: 0x040038FA RID: 14586
		public DiplomacyTableListItem aiTableAntimatterItem;

		// Token: 0x040038FB RID: 14587
		public DiplomacyTableListItem aiTableExoticsItem;

		// Token: 0x040038FC RID: 14588
		public DiplomacyTableListItem aiTableTreatyItem;

		// Token: 0x040038FD RID: 14589
		public DiplomacyTableListItem aiTableHateReductionItem;

		// Token: 0x040038FE RID: 14590
		public DiplomacyTableListItem aiTableExchangeIntelItem;

		// Token: 0x040038FF RID: 14591
		[Header("Main")]
		public GameObject tradeBodyObject;

		// Token: 0x04003900 RID: 14592
		public GameObject playerTableItemsContent;

		// Token: 0x04003901 RID: 14593
		public GameObject aiTableItemsContent;

		// Token: 0x04003902 RID: 14594
		public GameObject playerBankItemsContent;

		// Token: 0x04003903 RID: 14595
		public GameObject aiBankItemsContent;

		// Token: 0x04003904 RID: 14596
		public GameObject bankItemPrefab;

		// Token: 0x04003905 RID: 14597
		public GameObject tableItemPrefab;

		// Token: 0x04003906 RID: 14598
		public Image playerFactionIcon;

		// Token: 0x04003907 RID: 14599
		public Image aiFactionIcon;

		// Token: 0x04003908 RID: 14600
		public Image playerFactionIconLarge;

		// Token: 0x04003909 RID: 14601
		public Image aiFactionIconLarge;

		// Token: 0x0400390A RID: 14602
		public Image playerFactionGradient;

		// Token: 0x0400390B RID: 14603
		public Image aiFactionGradient;

		// Token: 0x0400390C RID: 14604
		public TMP_Text playerFactionText;

		// Token: 0x0400390D RID: 14605
		public TMP_Text aiFactionText;

		// Token: 0x0400390E RID: 14606
		public TMP_Text aiFactionAttitudeText;

		// Token: 0x0400390F RID: 14607
		public DiplomacyBankListItem playerResourcesTab;

		// Token: 0x04003910 RID: 14608
		public DiplomacyBankListItem aiResourcesTab;

		// Token: 0x04003911 RID: 14609
		public DiplomacyBankListItem playerOrgTab;

		// Token: 0x04003912 RID: 14610
		public DiplomacyBankListItem aiOrgTab;

		// Token: 0x04003913 RID: 14611
		public DiplomacyBankListItem playerHabsTab;

		// Token: 0x04003914 RID: 14612
		public DiplomacyBankListItem aiHabsTab;

		// Token: 0x04003915 RID: 14613
		public DiplomacyBankListItem playerCPsTab;

		// Token: 0x04003916 RID: 14614
		public DiplomacyBankListItem aiCPsTab;

		// Token: 0x04003917 RID: 14615
		public DiplomacyBankListItem playerProjectsTab;

		// Token: 0x04003918 RID: 14616
		public DiplomacyBankListItem aiProjectsTab;

		// Token: 0x04003919 RID: 14617
		public Canvas tradeCanvas;

		// Token: 0x0400391A RID: 14618
		public TMP_Text aiFeedbackDialogText;

		// Token: 0x0400391B RID: 14619
		public Button executeTradeButton;

		// Token: 0x0400391C RID: 14620
		public Button minimizeTradeWindowButton;

		// Token: 0x0400391D RID: 14621
		private float hateModifier = 1f;

		// Token: 0x0400391E RID: 14622
		private bool playerOrgsVisible;

		// Token: 0x0400391F RID: 14623
		private bool aiOrgsVisible;

		// Token: 0x04003920 RID: 14624
		private bool playerResourcesVisible = true;

		// Token: 0x04003921 RID: 14625
		private bool aiResourcesVisible = true;

		// Token: 0x04003922 RID: 14626
		private bool playerHabsVisible;

		// Token: 0x04003923 RID: 14627
		private bool aiHabsVisible;

		// Token: 0x04003924 RID: 14628
		private bool playerProjectsVisible;

		// Token: 0x04003925 RID: 14629
		private bool aiProjectsVisible;

		// Token: 0x04003926 RID: 14630
		private TradeOffer playerTradeOffer;

		// Token: 0x04003927 RID: 14631
		private TradeOffer aiTradeOffer;

		// Token: 0x04003928 RID: 14632
		public bool touchedAIOffer;

		// Token: 0x04003929 RID: 14633
		private bool isThisAnAIOffer;

		// Token: 0x0400392A RID: 14634
		private NotificationScreenController notificationController;

		// Token: 0x0400392B RID: 14635
		public TIFactionState tradingFaction;

		// Token: 0x0400392C RID: 14636
		private bool testingAITrade;
	}
}

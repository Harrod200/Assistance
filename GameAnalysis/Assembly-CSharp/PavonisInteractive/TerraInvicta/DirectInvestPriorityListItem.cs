using System;
using System.Collections.Generic;
using System.Linq;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000890 RID: 2192
	public class DirectInvestPriorityListItem : MonoBehaviour
	{
		// Token: 0x17000EE7 RID: 3815
		// (get) Token: 0x060051EC RID: 20972 RVA: 0x002401A1 File Offset: 0x0023E3A1
		// (set) Token: 0x060051ED RID: 20973 RVA: 0x002401A9 File Offset: 0x0023E3A9
		public PriorityType priority { get; protected set; }

		// Token: 0x060051EE RID: 20974 RVA: 0x002401B2 File Offset: 0x0023E3B2
		public void Init(NationInfoController controller, PriorityType priority)
		{
			this.controller = controller;
			this.priority = priority;
			this.priorityName.SetText(TIUtilities.GetPriorityString(priority, true));
		}

		// Token: 0x060051EF RID: 20975 RVA: 0x002401D4 File Offset: 0x0023E3D4
		public void SetListItem(TINationState nation)
		{
			this.nation = nation;
			this.priorityQuickDescription.SetText(NationInfoController.PrioritySummaryString(this.priority, nation, false));
			this.DITooltip.SetDelegate("BodyText", () => PriorityListItemController.priorityTipStr(GameControl.control.activePlayer, nation, this.priority, TIUtilities.GetPriorityString(this.priority, true)));
			this.UpdateDIData();
		}

		// Token: 0x060051F0 RID: 20976 RVA: 0x00240240 File Offset: 0x0023E440
		public void OnEditValue()
		{
			if (this.controller == null || !this.controller.enabled)
			{
				return;
			}
			string text = this.inputFieldIPs.text.Replace("-", "");
			if (text == null || text == string.Empty)
			{
				this.inputFieldIPs.SetTextWithoutNotify("0");
				return;
			}
			int num = int.Parse(text);
			using (List<PriorityType>.Enumerator enumerator = this.controller.plannedDirectInvestments.Keys.ToList<PriorityType>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == this.priority)
					{
						this.controller.plannedDirectInvestments[this.priority] = 0f;
					}
				}
			}
			this.controller.IncreaseDirectInvestment(this.priority, num);
			this.UpdateDIData();
		}

		// Token: 0x060051F1 RID: 20977 RVA: 0x00240334 File Offset: 0x0023E534
		public void OnSelectInputField()
		{
			TIInputManager.BlockKeybindings();
		}

		// Token: 0x060051F2 RID: 20978 RVA: 0x0024033B File Offset: 0x0023E53B
		public void OnDeSelectInputField()
		{
			TIInputManager.RestoreKeybindings();
		}

		// Token: 0x060051F3 RID: 20979 RVA: 0x00240342 File Offset: 0x0023E542
		public void ClearValue()
		{
			this.inputFieldIPs.text = "0";
		}

		// Token: 0x060051F4 RID: 20980 RVA: 0x00240354 File Offset: 0x0023E554
		public void OnPressIncrease()
		{
			int num = 1;
			if (TIInputManager.IsShiftKeyDown)
			{
				if (TIInputManager.IsControlKeyDown)
				{
					num *= 100;
				}
				else
				{
					num *= 10;
				}
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.controller.IncreaseDirectInvestment(this.priority, num);
			this.UpdateDIData();
		}

		// Token: 0x060051F5 RID: 20981 RVA: 0x002403A4 File Offset: 0x0023E5A4
		public void OnPressDecrease()
		{
			int num = 1;
			if (Input.GetKey(KeyCode.LeftShift))
			{
				if (Input.GetKey(KeyCode.LeftControl))
				{
					num *= 100;
				}
				else
				{
					num *= 10;
				}
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.controller.DecreaseDirectInvestment(this.priority, (float)num);
			this.UpdateDIData();
		}

		// Token: 0x060051F6 RID: 20982 RVA: 0x002403FC File Offset: 0x0023E5FC
		private void UpdateDIData()
		{
			TIResourcesCost tiresourcesCost = this.nation.InvestmentPointDirectPurchasePrice(this.priority, this.controller.activePlayer);
			FactionResource factionResource = FactionResource.None;
			if (tiresourcesCost.GetSingleCostValue(FactionResource.Money) > 0f)
			{
				factionResource = FactionResource.Money;
			}
			else if (tiresourcesCost.GetSingleCostValue(FactionResource.Influence) > 0f)
			{
				factionResource = FactionResource.Influence;
			}
			else if (tiresourcesCost.GetSingleCostValue(FactionResource.Operations) > 0f)
			{
				factionResource = FactionResource.Operations;
			}
			this.inputFieldIPs.text = this.controller.plannedDirectInvestments[this.priority].ToString();
			this.perIPCost.SetText(tiresourcesCost.ToString("Relevant", false, false, this.controller.activePlayer, false, factionResource));
			TIResourcesCost tiresourcesCost2 = this.controller.CurrentSingleDirectInvestmentCost(this.priority);
			this.resourcesToSpend.SetText(tiresourcesCost2.ToString("Relevant", false, false, null, false, factionResource));
			this.UpdateDIButtonsInteractable();
		}

		// Token: 0x060051F7 RID: 20983 RVA: 0x002404E0 File Offset: 0x0023E6E0
		public void UpdateDIButtonsInteractable()
		{
			TIResourcesCost tiresourcesCost = this.nation.InvestmentPointDirectPurchasePrice(this.priority, this.controller.activePlayer);
			int num;
			this.inputFieldIPs.interactable = this.nation.CanDirectInvest(this.controller.activePlayer, this.priority, out num);
			int num2;
			this.increaseButton.interactable = this.nation.CanDirectInvest(this.controller.activePlayer, this.priority, out num2) && this.controller.ProspectiveDirectInvestmentCosts(tiresourcesCost).CanAfford(this.controller.activePlayer, 1f, null, float.PositiveInfinity) && (float)num2 - this.controller.plannedDirectInvestments[this.priority] > 0f;
			this.decreaseButton.interactable = this.controller.plannedDirectInvestments[this.priority] > 0f;
		}

		// Token: 0x0400369B RID: 13979
		private NationInfoController controller;

		// Token: 0x0400369C RID: 13980
		private TINationState nation;

		// Token: 0x0400369E RID: 13982
		public TMP_Text priorityName;

		// Token: 0x0400369F RID: 13983
		public TMP_Text perIPCost;

		// Token: 0x040036A0 RID: 13984
		public TMP_Text resourcesToSpend;

		// Token: 0x040036A1 RID: 13985
		public TMP_Text priorityQuickDescription;

		// Token: 0x040036A2 RID: 13986
		public TMP_InputField inputFieldIPs;

		// Token: 0x040036A3 RID: 13987
		public Button increaseButton;

		// Token: 0x040036A4 RID: 13988
		public Button decreaseButton;

		// Token: 0x040036A5 RID: 13989
		public TooltipTrigger DITooltip;
	}
}

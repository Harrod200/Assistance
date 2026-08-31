using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.SpaceCombat.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008BE RID: 2238
	public class STOFighterNationListItemController : MonoBehaviour
	{
		// Token: 0x17000F13 RID: 3859
		// (get) Token: 0x06005577 RID: 21879 RVA: 0x0026DBF0 File Offset: 0x0026BDF0
		// (set) Token: 0x06005578 RID: 21880 RVA: 0x0026DBF8 File Offset: 0x0026BDF8
		public TINationState nation { get; protected set; }

		// Token: 0x06005579 RID: 21881 RVA: 0x0026DC04 File Offset: 0x0026BE04
		public void SetListItem(TINationState nation, PrecombatController controller, List<TIShipWeaponTemplate> allowedMissiles)
		{
			this.controller = controller;
			this.nation = nation;
			this.flag.sprite = nation.flag;
			this.nationName.SetText(nation.displayName);
			this.missileDropdown.ClearOptions();
			this.missileList = new Dictionary<int, TIShipWeaponTemplate>();
			int num = 0;
			foreach (TIShipWeaponTemplate tishipWeaponTemplate in allowedMissiles)
			{
				this.missileDropdown.options.Add(new TMP_Dropdown.OptionData
				{
					text = tishipWeaponTemplate.displayName
				});
				this.missileList.Add(num, tishipWeaponTemplate);
				num++;
			}
			this.missileDropdownScrollrect.enabled = true;
			this.copyLoadoutButtonTip.SetDelegate("BodyText", () => Loc.T("UI.Precombat.STOCopyLoadoutTip"));
			this.copyLoadoutButtonTip.enabled = true;
			this.ExternalMissileChange(allowedMissiles.MaxBy<TIShipWeaponTemplate, float>((TIShipWeaponTemplate x) => x.BaseDamageAtRange_points(500f, false)));
			this.fighterReadoutTip.SetDelegate("BodyText", () => this.currentDesign.quickSummary(false, null, false, true, false));
			this.fighterReadoutTip.enabled = true;
			this.SetNumberFighters(controller.STOFighterPlan[nation].count);
		}

		// Token: 0x0600557A RID: 21882 RVA: 0x0026DD78 File Offset: 0x0026BF78
		public void SetWeapon(TIShipWeaponTemplate missile)
		{
			this.currentDesign = GameControl.control.activePlayer.DesignSTOFighter(this.nation, missile);
			this.missileTip.SetDelegate("BodyText", () => new StringBuilder().AppendLine(missile.displayName).AppendLine(missile.GetDescriptionData(null, null, true, ShipModuleSlotType.HullHardPoint, false)).ToString());
			this.missileTip.enabled = true;
			this.controller.STOFighterPlan[this.nation].SetDesign(this.currentDesign);
			if (this.controller.STOFighterPlan[this.nation].count > 0 && this.controller.AvailableBoostWithFighterPlan(GameControl.control.activePlayer) < 0f)
			{
				float num = Mathf.Abs(this.controller.AvailableBoostWithFighterPlan(GameControl.control.activePlayer)) / this.controller.STOFighterPlan[this.nation].singleFighterBoostCost;
				if ((double)num != Math.Truncate((double)num))
				{
					num += 1f;
				}
				num = (float)((int)Math.Truncate((double)num));
				this.SetNumberFighters(this.controller.STOFighterPlan[this.nation].count - (int)num);
			}
			else
			{
				this.SetNumberFighters(this.controller.STOFighterPlan[this.nation].count);
			}
			GameControl.assetLoader.LoadAssetForImageAssignment(missile.iconResource, this.copyLoadoutButton);
			this.controller.UpdateSTOFighterTotals();
		}

		// Token: 0x0600557B RID: 21883 RVA: 0x0026DEFC File Offset: 0x0026C0FC
		public void SetNumberFighters(int value)
		{
			this.controller.STOFighterPlan[this.nation].SetCount(Mathf.Clamp(value, 0, this.nation.availableSTOFighters));
			this.numFighters.SetText(Loc.T("UI.Precombat.SlashValue", new object[]
			{
				value.ToString("N0"),
				this.nation.availableSTOFighters.ToString("N0")
			}));
			if (value == 0)
			{
				this.boostCost.SetText(Loc.T("UI.Precombat.ParensValue", new object[] { TIUtilities.RedLine(this.controller.STOFighterPlan[this.nation].singleFighterBoostCost.ToString("N1")) }));
			}
			else
			{
				this.boostCost.SetText(this.controller.STOFighterPlan[this.nation].boostCost.ToString("N1"));
			}
			this.SetButtons();
			this.controller.UpdateSTOFighterTotals();
		}

		// Token: 0x0600557C RID: 21884 RVA: 0x0026E010 File Offset: 0x0026C210
		public void SetButtons()
		{
			this.minusButton.interactable = this.controller.STOFighterPlan[this.nation].count > 0;
			this.plusButton.interactable = this.controller.STOFighterPlan[this.nation].count < this.nation.availableSTOFighters && this.controller.AvailableBoostWithFighterPlan(GameControl.control.activePlayer) >= this.controller.STOFighterPlan[this.nation].singleFighterBoostCost;
		}

		// Token: 0x0600557D RID: 21885 RVA: 0x0026E0B1 File Offset: 0x0026C2B1
		public void OnMissileDropdownChanged()
		{
			this.SetWeapon(this.missileList[this.missileDropdown.value]);
		}

		// Token: 0x0600557E RID: 21886 RVA: 0x0026E0CF File Offset: 0x0026C2CF
		public void OnHitPlus()
		{
			this.SetNumberFighters(this.controller.STOFighterPlan[this.nation].count + 1);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
		}

		// Token: 0x0600557F RID: 21887 RVA: 0x0026E100 File Offset: 0x0026C300
		public void OnHitMinus()
		{
			this.SetNumberFighters(this.controller.STOFighterPlan[this.nation].count - 1);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
		}

		// Token: 0x06005580 RID: 21888 RVA: 0x0026E134 File Offset: 0x0026C334
		public void OnHitApplyMissileToAll()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			using (IEnumerator<object> enumerator = this.controller.STONationsLaunchList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (STOFighterNationListItemController.<>o__26.<>p__0 == null)
					{
						STOFighterNationListItemController.<>o__26.<>p__0 = CallSite<Func<CallSite, object, STOFighterNationListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(STOFighterNationListItemController), typeof(STOFighterNationListItemController)));
					}
					STOFighterNationListItemController stofighterNationListItemController = STOFighterNationListItemController.<>o__26.<>p__0.Target(STOFighterNationListItemController.<>o__26.<>p__0, enumerator.Current);
					if (stofighterNationListItemController.nation != this.nation)
					{
						stofighterNationListItemController.ExternalMissileChange(this.missileList[this.missileDropdown.value]);
					}
				}
			}
		}

		// Token: 0x06005581 RID: 21889 RVA: 0x0026E1FC File Offset: 0x0026C3FC
		public void ExternalMissileChange(TIShipWeaponTemplate newMissile)
		{
			this.SetWeapon(newMissile);
			this.missileDropdown.SetValueWithoutNotify(this.missileList.FirstOrDefault<KeyValuePair<int, TIShipWeaponTemplate>>((KeyValuePair<int, TIShipWeaponTemplate> x) => x.Value == newMissile).Key);
			this.missileDropdown.captionText.text = newMissile.displayName;
			this.controller.UpdateSTOFighterTotals();
		}

		// Token: 0x06005582 RID: 21890 RVA: 0x0026E272 File Offset: 0x0026C472
		public void ExternalFighterCountChange(int newCount)
		{
			this.SetNumberFighters(newCount);
			this.controller.UpdateSTOFighterTotals();
		}

		// Token: 0x04003BCF RID: 15311
		private PrecombatController controller;

		// Token: 0x04003BD1 RID: 15313
		public Image flag;

		// Token: 0x04003BD2 RID: 15314
		public TMP_Text nationName;

		// Token: 0x04003BD3 RID: 15315
		public TMP_Text numFighters;

		// Token: 0x04003BD4 RID: 15316
		public TMP_Text boostCost;

		// Token: 0x04003BD5 RID: 15317
		public TMP_Dropdown missileDropdown;

		// Token: 0x04003BD6 RID: 15318
		public ScrollRect missileDropdownScrollrect;

		// Token: 0x04003BD7 RID: 15319
		public Image copyLoadoutButton;

		// Token: 0x04003BD8 RID: 15320
		public TooltipTrigger copyLoadoutButtonTip;

		// Token: 0x04003BD9 RID: 15321
		public TooltipTrigger fighterReadoutTip;

		// Token: 0x04003BDA RID: 15322
		public TooltipTrigger missileTip;

		// Token: 0x04003BDB RID: 15323
		public Button plusButton;

		// Token: 0x04003BDC RID: 15324
		public Button minusButton;

		// Token: 0x04003BDD RID: 15325
		private Dictionary<int, TIShipWeaponTemplate> missileList;

		// Token: 0x04003BDE RID: 15326
		private TISpaceShipTemplate currentDesign;
	}
}

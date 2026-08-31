using System;
using System.Text;
using ModelShark;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta.SpaceCombat.UI
{
	// Token: 0x02000A0A RID: 2570
	public class SpaceCombatDamageGridItemController : MonoBehaviour
	{
		// Token: 0x1700110F RID: 4367
		// (get) Token: 0x060063D3 RID: 25555 RVA: 0x002F2B73 File Offset: 0x002F0D73
		// (set) Token: 0x060063D4 RID: 25556 RVA: 0x002F2B7B File Offset: 0x002F0D7B
		[HideInInspector]
		public ShipSystem attachedSystem { get; private set; }

		// Token: 0x17001110 RID: 4368
		// (get) Token: 0x060063D5 RID: 25557 RVA: 0x002F2B84 File Offset: 0x002F0D84
		// (set) Token: 0x060063D6 RID: 25558 RVA: 0x002F2B8C File Offset: 0x002F0D8C
		[HideInInspector]
		public ModuleDataEntry attachedModule { get; private set; }

		// Token: 0x060063D7 RID: 25559 RVA: 0x002F2B95 File Offset: 0x002F0D95
		private string damageIconStr(float damagePct)
		{
			if (damagePct <= 0f)
			{
				return "ui_spacecombat/ICO_battle_comp_A";
			}
			if (damagePct >= 1f)
			{
				return "ui_spacecombat/ICO_battle_comp_C";
			}
			return "ui_spacecombat/ICO_battle_comp_B";
		}

		// Token: 0x060063D8 RID: 25560 RVA: 0x002F2BB8 File Offset: 0x002F0DB8
		private string SetTooltip()
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = this.which;
			float num2;
			if (num == 1)
			{
				num2 = this.ship.GetSystemFunction(this.attachedSystem);
				stringBuilder.AppendLine(Loc.T(new StringBuilder("UI.SpaceCombat.").Append(this.attachedSystem.ToString()).ToString()));
				stringBuilder.AppendLine(num2.ToPercent(TIUtilities.DecimalPlaces_P((double)num2, 1, 0)));
				stringBuilder.AppendLine(Loc.T(new StringBuilder("UI.SpaceCombat.Description.").Append(this.attachedSystem.ToString()).ToString()));
				switch (this.attachedSystem)
				{
				case ShipSystem.NoseStructure:
				{
					stringBuilder.AppendLine(Loc.T("UI.SpaceCombat.NoseArmorIntegrity"));
					float armorIntegrity = this.ship.armor[ArmorFacing.Nose].GetArmorIntegrity();
					stringBuilder.AppendLine(armorIntegrity.ToPercent(TIUtilities.DecimalPlaces_P((double)armorIntegrity, 7, 0)));
					stringBuilder.AppendLine(Loc.T("UI.SpaceCombat.ArmorValue", new object[] { this.ship.armor[ArmorFacing.Nose].armorValue }));
					stringBuilder.AppendLine(Loc.T("UI.SpaceCombat.Description.ArmorIntegrity"));
					break;
				}
				case ShipSystem.CentralStructure:
				{
					stringBuilder.AppendLine(Loc.T("UI.SpaceCombat.PortArmorIntegrity"));
					float armorIntegrity2 = this.ship.armor[ArmorFacing.Left].GetArmorIntegrity();
					stringBuilder.AppendLine(armorIntegrity2.ToPercent(TIUtilities.DecimalPlaces_P((double)armorIntegrity2, 7, 0)));
					stringBuilder.AppendLine(Loc.T("UI.SpaceCombat.ArmorValue", new object[] { this.ship.armor[ArmorFacing.Left].armorValue }));
					stringBuilder.AppendLine(Loc.T("UI.SpaceCombat.StarboardArmorIntegrity"));
					float armorIntegrity3 = this.ship.armor[ArmorFacing.Right].GetArmorIntegrity();
					stringBuilder.AppendLine(armorIntegrity3.ToPercent(TIUtilities.DecimalPlaces_P((double)armorIntegrity3, 7, 0)));
					stringBuilder.AppendLine(Loc.T("UI.SpaceCombat.ArmorValue", new object[] { this.ship.armor[ArmorFacing.Right].armorValue }));
					stringBuilder.AppendLine(Loc.T("UI.SpaceCombat.Description.ArmorIntegrity"));
					break;
				}
				case ShipSystem.TailStructure:
				{
					stringBuilder.AppendLine(Loc.T("UI.SpaceCombat.TailArmorIntegrity"));
					float armorIntegrity4 = this.ship.armor[ArmorFacing.Tail].GetArmorIntegrity();
					stringBuilder.AppendLine(armorIntegrity4.ToPercent(TIUtilities.DecimalPlaces_P((double)armorIntegrity4, 7, 0)));
					stringBuilder.AppendLine(Loc.T("UI.SpaceCombat.ArmorValue", new object[] { this.ship.armor[ArmorFacing.Tail].armorValue }));
					stringBuilder.AppendLine(Loc.T("UI.SpaceCombat.Description.ArmorIntegrity"));
					break;
				}
				}
				return stringBuilder.ToString();
			}
			if (num != 2)
			{
				return string.Empty;
			}
			num2 = this.ship.GetPartFunction(this.attachedModule);
			stringBuilder.AppendLine(this.attachedModule.moduleTemplate.displayName);
			stringBuilder.Append(num2.ToPercent(TIUtilities.DecimalPlaces_P((double)num2, 1, 0)));
			if (!this.ship.FireControlActive() && this.attachedModule.weaponTemplate != null)
			{
				stringBuilder.Append(" (").Append(Loc.T("UI.SpaceCombat.FireControl")).Append(")");
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060063D9 RID: 25561 RVA: 0x002F2F3E File Offset: 0x002F113E
		public void PreInitialize(Vector2Int coordinates)
		{
			this.coordinates = coordinates;
		}

		// Token: 0x060063DA RID: 25562 RVA: 0x002F2F47 File Offset: 0x002F1147
		public void Initialize()
		{
			this.Clear();
			if (this.recentDamageAnimatior != null)
			{
				GameControl.eventManager.AddListener<GameTimeSpeedChanged>(new EventManager.EventDelegate<GameTimeSpeedChanged>(this.OnGameTimeSpeedChanged), null, null, true, false);
			}
		}

		// Token: 0x060063DB RID: 25563 RVA: 0x002F2F78 File Offset: 0x002F1178
		public void Clear()
		{
			this.damageIcon.enabled = false;
			this.damConIcon.enabled = false;
			this.damConDisabledIcon.enabled = false;
			this.damageDetailText.enabled = false;
			this.which = 0;
			if (this.recentDamageAnimatior != null)
			{
				this.recentDamageAnimatior.Play("Idle");
			}
			GameControl.eventManager.RemoveListener<GameTimeSpeedChanged>(new EventManager.EventDelegate<GameTimeSpeedChanged>(this.OnGameTimeSpeedChanged), null);
		}

		// Token: 0x060063DC RID: 25564 RVA: 0x002F2FF4 File Offset: 0x002F11F4
		public void Initialize(TISpaceShipState ship, ShipSystem attachedSystem)
		{
			this.attachedSystem = attachedSystem;
			this.ship = ship;
			this.damageIcon.enabled = true;
			this.damConIcon.enabled = false;
			this.damConDisabledIcon.enabled = false;
			this.damageDetailText.enabled = true;
			this.damageDetailText.SetDelegate("BodyText", () => this.SetTooltip());
			this.which = 1;
			this.UpdateListItem();
			if (this.recentDamageAnimatior != null)
			{
				GameControl.eventManager.AddListener<GameTimeSpeedChanged>(new EventManager.EventDelegate<GameTimeSpeedChanged>(this.OnGameTimeSpeedChanged), null, null, true, false);
			}
		}

		// Token: 0x060063DD RID: 25565 RVA: 0x002F3090 File Offset: 0x002F1290
		public void Initialize(TISpaceShipState ship, ModuleDataEntry attachedModule)
		{
			this.attachedModule = attachedModule;
			this.ship = ship;
			this.damageIcon.enabled = true;
			this.damConIcon.enabled = false;
			this.damageDetailText.enabled = true;
			this.damageDetailText.SetDelegate("BodyText", () => this.SetTooltip());
			this.which = 2;
			this.UpdateListItem();
			if (this.recentDamageAnimatior != null)
			{
				GameControl.eventManager.AddListener<GameTimeSpeedChanged>(new EventManager.EventDelegate<GameTimeSpeedChanged>(this.OnGameTimeSpeedChanged), null, null, true, false);
			}
		}

		// Token: 0x060063DE RID: 25566 RVA: 0x002F3120 File Offset: 0x002F1320
		public void UpdateListItem()
		{
			int num = this.which;
			if (num == 1)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(this.damageIconStr(this.ship.GetSystemDamage(this.attachedSystem)), this.damageIcon);
				return;
			}
			if (num != 2)
			{
				return;
			}
			GameControl.assetLoader.LoadAssetForImageAssignment(this.damageIconStr(this.ship.GetPartDamage(this.attachedModule)), this.damageIcon);
		}

		// Token: 0x060063DF RID: 25567 RVA: 0x002F318C File Offset: 0x002F138C
		public void SetRepairStatus(bool value, bool isDamConSuspended)
		{
			this.damConIcon.enabled = value;
			if (value)
			{
				this.damConDisabledIcon.enabled = isDamConSuspended;
			}
		}

		// Token: 0x060063E0 RID: 25568 RVA: 0x002F31A9 File Offset: 0x002F13A9
		public void TookDamage(bool repaired)
		{
			if (repaired)
			{
				return;
			}
			if (this.recentDamageAnimatior != null)
			{
				this.recentDamageAnimatior.SetTrigger("Damaged");
			}
		}

		// Token: 0x060063E1 RID: 25569 RVA: 0x002F31D0 File Offset: 0x002F13D0
		private void OnGameTimeSpeedChanged(GameTimeSpeedChanged e)
		{
			if (TIGlobalValuesState.isSpaceCombatEnabled)
			{
				if (GameControl.spaceCombat.combatHUD.clockController.IsPaused)
				{
					this.recentDamageAnimatior.speed = 0f;
					return;
				}
				if (this.recentDamageAnimatior.speed == 0f)
				{
					this.recentDamageAnimatior.speed = 1f;
				}
			}
		}

		// Token: 0x060063E2 RID: 25570 RVA: 0x002F322D File Offset: 0x002F142D
		private void OnDestroy()
		{
			GameControl.eventManager.RemoveListener<GameTimeSpeedChanged>(new EventManager.EventDelegate<GameTimeSpeedChanged>(this.OnGameTimeSpeedChanged), null);
		}

		// Token: 0x040046A3 RID: 18083
		public Image damageIcon;

		// Token: 0x040046A4 RID: 18084
		public Image damConIcon;

		// Token: 0x040046A5 RID: 18085
		public Image damConDisabledIcon;

		// Token: 0x040046A6 RID: 18086
		public TooltipTrigger damageDetailText;

		// Token: 0x040046A7 RID: 18087
		public TMP_Text overlayText;

		// Token: 0x040046A8 RID: 18088
		public Animator recentDamageAnimatior;

		// Token: 0x040046A9 RID: 18089
		private TISpaceShipState ship;

		// Token: 0x040046AC RID: 18092
		private int which;

		// Token: 0x040046AD RID: 18093
		public Vector2Int coordinates;
	}
}

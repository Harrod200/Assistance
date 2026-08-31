using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Ship;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta.SpaceCombat.UI
{
	// Token: 0x02000A08 RID: 2568
	public class ShipWeaponUIController : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x17001108 RID: 4360
		// (get) Token: 0x06006323 RID: 25379 RVA: 0x002EB2C0 File Offset: 0x002E94C0
		private TIShipWeaponTemplate weaponTemplate
		{
			get
			{
				return this.weapon.weaponTemplate;
			}
		}

		// Token: 0x06006324 RID: 25380 RVA: 0x002EB2D0 File Offset: 0x002E94D0
		public void Initialize(Weapon weapon, SpaceCombatCanvasController controller)
		{
			this.weapon = weapon;
			this.controller = controller;
			this.ship = weapon.combatant.WeaponCarrierState as TISpaceShipState;
			GameControl.assetLoader.LoadAssetForImageAssignment(this.weaponTemplate.combatIconResource, this.weaponIcon);
			this.weaponName.SetText(this.weaponTemplate.displayName);
			this.energyUsagePanel.SetActive(!weapon.weaponTemplate.selfPowered);
			this.ammoPanel.SetActive(weapon.weaponTemplate.hasMagazine());
			if (this.weaponTemplate.ref_projectileWeapon != null)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(this.weaponTemplate.ref_projectileWeapon.ammoIconPath, this.ammoIcon);
			}
			this.tooltip.SetDelegate("BodyText", new ParameterizedTextField.BuildStringOnTooltipHover(this.UpdateTooltip));
			if (!this._addedCombatSecondListener)
			{
				GameControl.eventManager.AddListener<CombatSecond>(new EventManager.EventDelegate<CombatSecond>(this.UpdateStatus), null, null, true, false);
				this._addedCombatSecondListener = true;
			}
			this.SetGridItem();
			foreach (CombatShipController combatShipController in GameControl.spaceCombat.ships)
			{
				if (combatShipController.ShipState == this.ship)
				{
					this.shipUIController = combatShipController.visualizationController.UIController;
					this.cShipController = combatShipController;
				}
			}
			this._showingBollixed = false;
		}

		// Token: 0x06006325 RID: 25381 RVA: 0x002EB450 File Offset: 0x002E9650
		public void UpdateGridItem()
		{
			this.UpdateAmmoText();
			this.UpdateStatus();
			this.UpdateFireMode();
		}

		// Token: 0x06006326 RID: 25382 RVA: 0x002EB464 File Offset: 0x002E9664
		public void UpdateAmmoText()
		{
			if (this.weapon.weaponTemplate.hasMagazine())
			{
				this.ammo.SetText(this.ship.ammo[this.weapon.weaponData].ToString());
			}
		}

		// Token: 0x06006327 RID: 25383 RVA: 0x002EB4B1 File Offset: 0x002E96B1
		public void UpdateStatus(CombatSecond e)
		{
			this.UpdateStatus();
		}

		// Token: 0x06006328 RID: 25384 RVA: 0x002EB4BC File Offset: 0x002E96BC
		public void UpdateStatus()
		{
			if (this == null || base.gameObject == null || this.button == null || this.primaryFrame == null)
			{
				return;
			}
			if (this.ship.WeaponDamaged(this.weapon.weaponData) || !this.ship.FireControlActive())
			{
				this._weaponWasDamaged = true;
				this.primaryFrame.color = Color.red;
				this.button.interactable = false;
				GameControl.assetLoader.LoadAssetForImageAssignment("ui_spaceCombat/BUT_mode_idle", this.weaponStatusIcon);
			}
			else if (!this.ship.WeaponIsOperable(this.weapon.weaponData))
			{
				this.primaryFrame.color = new Color(1f, 0.647f, 0f);
				this.button.interactable = !this.ship.combatAIControl;
			}
			else if (this.weapon.OnCooldown(this.controller.gameTime.currentTime.ExportTime()))
			{
				if (this.weapon.bollixed)
				{
					this.primaryFrame.color = Color.blue;
					if (!this._showingBollixed)
					{
						GameControl.assetLoader.LoadAssetForImageAssignment("ui_spaceCombat/ICO_weaponBollixed", this.weaponIcon);
						this._showingBollixed = true;
					}
				}
				else
				{
					this.primaryFrame.color = Color.yellow;
				}
				this.button.interactable = !this.ship.combatAIControl;
			}
			else
			{
				this.primaryFrame.color = Color.white;
				this.button.interactable = !this.ship.combatAIControl;
				if (this._weaponWasDamaged)
				{
					this.UpdateFireMode();
					this._weaponWasDamaged = false;
				}
				if (this._showingBollixed)
				{
					GameControl.assetLoader.LoadAssetForImageAssignment(this.weaponTemplate.combatIconResource, this.weaponIcon);
					this._showingBollixed = false;
				}
			}
			if ((float)TIFrameCounter.FrameCount - this.lastStatusUpdate > 120f)
			{
				this.tooltip.ForceRefreshTooltipIfOpen();
				this.lastStatusUpdate = (float)TIFrameCounter.FrameCount;
			}
		}

		// Token: 0x06006329 RID: 25385 RVA: 0x002EB6D8 File Offset: 0x002E98D8
		public void UpdateFireMode()
		{
			if (!(this.weaponStatusIcon != null) || !(this.ship != null))
			{
				this.weaponModeText.SetText("");
				return;
			}
			GameControl.assetLoader.LoadAssetForImageAssignment(this.weapon.currentFireMode.iconPath, this.weaponStatusIcon);
			if (this.weapon.bollixed)
			{
				this.weaponModeText.SetText(Loc.T("UI.SpaceCombat.Jammed"));
				return;
			}
			if (this.ship.WeaponDamaged(this.weapon.weaponData))
			{
				this.weaponModeText.SetText(Loc.T("UI.SpaceCombat.Damaged"));
				return;
			}
			if (!this.ship.FireControlActive() || !this.ship.WeaponHasPower(this.weapon.weaponData))
			{
				this.weaponModeText.SetText(Loc.T("UI.SpaceCombat.Offline"));
				return;
			}
			this.weaponModeText.SetText(this.weapon.currentFireMode.displayName);
		}

		// Token: 0x0600632A RID: 25386 RVA: 0x002EB7E0 File Offset: 0x002E99E0
		private string UpdateTooltip()
		{
			StringBuilder stringBuilder = new StringBuilder(this.weapon.weaponTemplate.displayName).AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.Append(this.weapon.weaponTemplate.SpecificDescriptionData());
			stringBuilder.AppendLine(this.weapon.weaponTemplate.GetLocalizedTargetingRange());
			if (this.weapon.weaponTemplate.defenseMode)
			{
				stringBuilder.AppendLine(this.weapon.weaponTemplate.GetLocalizedDefenseTargetingRange(this.ship.template));
			}
			if (this.weapon.weaponTemplate.hasMagazine())
			{
				stringBuilder.AppendLine(this.weapon.weaponTemplate.GetLocalizedMagazineMaxAmmoCount(this.ship.template));
			}
			if (this.weapon.weaponTemplate.EnergyUsage_GJ(0f) > 0f)
			{
				stringBuilder.AppendLine(this.weapon.weaponTemplate.GetLocalizedEnergyUsage());
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(this.weapon.currentFireMode.displayName);
			stringBuilder.AppendLine(this.weapon.currentFireMode.description);
			if (!this.ship.FireControlActive())
			{
				stringBuilder.AppendLine(TIUtilities.RedLine(Loc.T("UI.SpaceCombat.Weapon.FireControlDamage")));
			}
			else if (this.ship.WeaponDamaged(this.weapon.weaponData))
			{
				stringBuilder.AppendLine(TIUtilities.RedLine(Loc.T("UI.SpaceCombat.Weapon.Damaged")));
			}
			else if (!this.ship.WeaponHasAmmo(this.weapon.weaponData))
			{
				stringBuilder.AppendLine(TIUtilities.RedLine(Loc.T("UI.SpaceCombat.Weapon.NoAmmo")));
			}
			else if (!this.ship.WeaponHasPower(this.weapon.weaponData))
			{
				stringBuilder.AppendLine(TIUtilities.RedLine(Loc.T("UI.SpaceCombat.Weapon.NoPower")));
			}
			else if (this.ship.WeaponFireExceedsHeatCapacity(this.weapon.weaponData))
			{
				stringBuilder.AppendLine(TIUtilities.RedLine(Loc.T("UI.SpaceCombat.Weapon.HeatProblem")));
			}
			else if (this.weapon.OnCooldown(this.controller.gameTime.currentTime.ExportTime()))
			{
				if (this.weapon.bollixed)
				{
					stringBuilder.AppendLine(TIUtilities.HighlightLine(Loc.T("UI.SpaceCombat.Weapon.Bollixed")));
				}
				else
				{
					stringBuilder.AppendLine(TIUtilities.HighlightLine(Loc.T("UI.SpaceCombat.Weapon.Cooldown")));
				}
			}
			stringBuilder.Append(TIUtilities.InlineMouseClickStr(0)).Append(TIUtilities.InlineMouseClickStr(1)).Append(Loc.T("UI.SpaceCombat.WeaponManager"))
				.AppendLine();
			stringBuilder.Append(TIUtilities.InlineKeyboardModifierStr(KeyCode.LeftShift)).Append(Loc.T("UI.SpaceCombat.WeaponManagerShift")).AppendLine();
			if (this.controller.groupSelectedFriendlyShips.Count > 1)
			{
				stringBuilder.Append(TIUtilities.InlineKeyboardModifierStr(KeyCode.LeftControl)).Append(TIUtilities.InlineKeyboardModifierStr(KeyCode.LeftShift)).Append(Loc.T("UI.SpaceCombat.WeaponManagerControlShift"))
					.AppendLine();
			}
			stringBuilder.Append(TIUtilities.InlineKeyboardModifierStr(KeyCode.LeftAlt)).Append(Loc.T("UI.SpaceCombat.WeaponManagerAlt")).AppendLine();
			stringBuilder.Append(TIUtilities.InlineKeyboardModifierStr(KeyCode.LeftControl)).Append(TIUtilities.InlineKeyboardModifierStr(KeyCode.LeftAlt)).Append(Loc.T("UI.SpaceCombat.WeaponManagerControlAlt"))
				.AppendLine();
			stringBuilder.Append(TIUtilities.InlineKeyboardModifierStr(KeyCode.LeftControl)).Append(Loc.T("UI.SpaceCombat.WeaponManagerControl")).AppendLine();
			return stringBuilder.ToString();
		}

		// Token: 0x0600632B RID: 25387 RVA: 0x002EBB77 File Offset: 0x002E9D77
		public void SetGridItem()
		{
			this.UpdateFireMode();
			this.UpdateStatus();
			this.UpdateAmmoText();
		}

		// Token: 0x0600632C RID: 25388 RVA: 0x002EBB8C File Offset: 0x002E9D8C
		private ShipWeaponUIController.ChangeWeaponClickMode CheckKeysDownForBatchChange(out SpaceCombatCanvasController.ChangeCommandScopeMode mode)
		{
			bool isShiftKeyDown = TIInputManager.IsShiftKeyDown;
			bool isControlKeyDown = TIInputManager.IsControlKeyDown;
			bool isAltKeyDown = TIInputManager.IsAltKeyDown;
			if (isControlKeyDown)
			{
				if (isShiftKeyDown && !isAltKeyDown)
				{
					mode = SpaceCombatCanvasController.ChangeCommandScopeMode.AllShipsInGroup;
					return ShipWeaponUIController.ChangeWeaponClickMode.AllWeaponsOfTypeOnShipsInGroup;
				}
				if (isAltKeyDown)
				{
					mode = SpaceCombatCanvasController.ChangeCommandScopeMode.AllShipsOfClass;
					return ShipWeaponUIController.ChangeWeaponClickMode.AllWeaponsOfTypeOnShipsInClass;
				}
				mode = SpaceCombatCanvasController.ChangeCommandScopeMode.AllShipsInFleet;
				return ShipWeaponUIController.ChangeWeaponClickMode.AllWeaponsOfTypeOnShipsInFleet;
			}
			else
			{
				if (isShiftKeyDown && !isAltKeyDown)
				{
					mode = SpaceCombatCanvasController.ChangeCommandScopeMode.JustThisShip;
					return ShipWeaponUIController.ChangeWeaponClickMode.AllWeaponsOfTypeOnShip;
				}
				if (isAltKeyDown)
				{
					mode = SpaceCombatCanvasController.ChangeCommandScopeMode.AllShipsOfClass;
					return ShipWeaponUIController.ChangeWeaponClickMode.JustWeaponOnShipsInClass;
				}
				mode = SpaceCombatCanvasController.ChangeCommandScopeMode.JustThisShip;
				return ShipWeaponUIController.ChangeWeaponClickMode.JustThisWeapon;
			}
		}

		// Token: 0x0600632D RID: 25389 RVA: 0x002EBBDC File Offset: 0x002E9DDC
		private void ChangeFireMode(IFireMode newFireMode)
		{
			SpaceCombatCanvasController.ChangeCommandScopeMode changeCommandScopeMode;
			ShipWeaponUIController.ChangeWeaponClickMode changeWeaponClickMode = this.CheckKeysDownForBatchChange(out changeCommandScopeMode);
			switch (changeWeaponClickMode)
			{
			case ShipWeaponUIController.ChangeWeaponClickMode.JustThisWeapon:
				this.ship.faction.playerControl.StartAction(new SetWeaponModeAction(this.ship, this.weapon, newFireMode.mode));
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CombatWeaponCycle", false, false);
				return;
			case ShipWeaponUIController.ChangeWeaponClickMode.AllWeaponsOfTypeOnShip:
				foreach (IWeapon weapon in this.cShipController.hull.IterateByClass<IWeapon>())
				{
					Weapon weapon2 = weapon as Weapon;
					if (weapon2.weaponTemplate.dataName == this.weaponTemplate.dataName)
					{
						this.ship.faction.playerControl.StartAction(new SetWeaponModeAction(this.ship, weapon2, newFireMode.mode));
					}
				}
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CombatWeaponCycle", false, false);
				return;
			case ShipWeaponUIController.ChangeWeaponClickMode.AllWeaponsOfTypeOnShipsInGroup:
			case ShipWeaponUIController.ChangeWeaponClickMode.AllWeaponsOfTypeOnShipsInClass:
			case ShipWeaponUIController.ChangeWeaponClickMode.AllWeaponsOfTypeOnShipsInFleet:
			{
				List<TISpaceShipState> batchofShips = this.controller.GetBatchofShips(this.ship, changeCommandScopeMode);
				foreach (TISpaceShipState tispaceShipState in batchofShips)
				{
					foreach (IWeapon weapon3 in this.controller.combatMgr.combatantLookup[tispaceShipState].ref_shipController.hull.IterateByClass<IWeapon>())
					{
						Weapon weapon4 = weapon3 as Weapon;
						if (weapon4.weaponTemplate.dataName == this.weaponTemplate.dataName)
						{
							this.ship.faction.playerControl.StartAction(new SetWeaponModeAction(tispaceShipState, weapon4, newFireMode.mode));
						}
					}
				}
				if (changeWeaponClickMode == ShipWeaponUIController.ChangeWeaponClickMode.AllWeaponsOfTypeOnShipsInGroup && batchofShips.Count == 0)
				{
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
					return;
				}
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CombatWeaponCycle", false, false);
				return;
			}
			case ShipWeaponUIController.ChangeWeaponClickMode.JustWeaponOnShipsInClass:
				foreach (TISpaceShipState tispaceShipState2 in this.controller.GetBatchofShips(this.ship, changeCommandScopeMode))
				{
					IWeapon weapon5 = this.controller.combatMgr.combatantLookup[tispaceShipState2].ref_shipController.hull.IterateByClass<IWeapon>().First<IWeapon>((IWeapon x) => (x as Weapon).weaponData.slotIndex == this.weapon.weaponData.slotIndex);
					tispaceShipState2.faction.playerControl.StartAction(new SetWeaponModeAction(tispaceShipState2, weapon5 as Weapon, newFireMode.mode));
				}
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CombatWeaponCycle", false, false);
				return;
			default:
				return;
			}
		}

		// Token: 0x0600632E RID: 25390 RVA: 0x002EBEB4 File Offset: 0x002EA0B4
		public void OnButtonPressed()
		{
			int num = this.weapon.fireModes.IndexOf(this.weapon.currentFireMode);
			num++;
			if (num >= this.weapon.fireModes.Count)
			{
				num = 0;
			}
			IFireMode fireMode = this.weapon.fireModes[num];
			this.ChangeFireMode(fireMode);
			this.tooltip.ForceRefreshTooltipIfOpen();
		}

		// Token: 0x0600632F RID: 25391 RVA: 0x002EBF1C File Offset: 0x002EA11C
		public void OnRightButtonPressed()
		{
			int num = this.weapon.fireModes.IndexOf(this.weapon.currentFireMode);
			num--;
			if (num < 0)
			{
				num = this.weapon.fireModes.Count - 1;
			}
			IFireMode fireMode = this.weapon.fireModes[num];
			this.ChangeFireMode(fireMode);
			this.tooltip.ForceRefreshTooltipIfOpen();
		}

		// Token: 0x06006330 RID: 25392 RVA: 0x002EBF84 File Offset: 0x002EA184
		public void OnPointerEnter(PointerEventData eventData)
		{
			float num = SpaceCombatManager.km_to_scale(this.weaponTemplate.targetingRange_km);
			float num2 = 1f / (100f * GameControl.spaceCombat.modelScalingFactor);
			if (this.weaponTemplate.pivotRange_deg < 180f)
			{
				float num3 = this.weaponTemplate.pivotRange_deg * 2f;
				if (this.shipUIController.weaponRangeCone != null)
				{
					this.shipUIController.weaponRangeCone.SetActive(true);
					this.shipUIController.weaponRangeCone.transform.GetChild(0).localScale = new Vector3(num * 120f * num2, num * 120f * num2, num * 120f * num2);
					if (this.shipUIController.weaponRangeCone.transform.GetChild(0).GetComponent<MeshFilter>().mesh.vertexCount == 0)
					{
						this.shipUIController.weaponRangeCone.transform.GetChild(0).GetComponent<NoseWeaponCone>().CreateCone(num3);
					}
					this.shipUIController.cone_Material.SetFloat("_MaxDistance", num);
					this.shipUIController.cone_Material.SetVector("_Target", this.cShipController.gameObject.transform.position);
				}
			}
			this.shipUIController.weaponRangeSphere.SetActive(true);
			this.shipUIController.weaponRangeSphere.transform.GetChild(0).localScale = new Vector3(num * 200f * num2, num * 200f * num2, num * 200f * num2);
			foreach (CombatShipController combatShipController in GameControl.spaceCombat.activeShips)
			{
				if (combatShipController.faction != GameControl.control.activePlayer && Vector3.Distance(combatShipController.transform.position, this.cShipController.transform.position) < num)
				{
					combatShipController.visualizationController.UIController.UIcanvas.enabled = true;
					combatShipController.visualizationController.UIController.SetShipDamageImages();
					this.reticlesToHide.Add(combatShipController.visualizationController.UIController);
				}
			}
			if (this.weaponTemplate.attackMode && this.weaponTemplate.defenseMode)
			{
				this.shipUIController.sphere_Material.SetFloat("_InnerRingIntensity", 0.15f);
				return;
			}
			this.shipUIController.sphere_Material.SetFloat("_InnerRingIntensity", 0f);
		}

		// Token: 0x06006331 RID: 25393 RVA: 0x002EC230 File Offset: 0x002EA430
		public void OnPointerExit(PointerEventData eventData)
		{
			this.shipUIController.weaponRangeSphere.SetActive(false);
			this.shipUIController.weaponRangeCone.SetActive(false);
			foreach (ShipUIController shipUIController in this.reticlesToHide)
			{
				if (shipUIController != null)
				{
					shipUIController.UIcanvas.enabled = false;
				}
			}
			this.reticlesToHide.Clear();
		}

		// Token: 0x06006332 RID: 25394 RVA: 0x002EC2C0 File Offset: 0x002EA4C0
		private void OnDestroy()
		{
			GameControl.eventManager.RemoveListener<CombatSecond>(new EventManager.EventDelegate<CombatSecond>(this.UpdateStatus), null);
			this._addedCombatSecondListener = false;
		}

		// Token: 0x040045FC RID: 17916
		public Image primaryFrame;

		// Token: 0x040045FD RID: 17917
		public Image weaponIcon;

		// Token: 0x040045FE RID: 17918
		public Image weaponStatusIcon;

		// Token: 0x040045FF RID: 17919
		public Image ammoIcon;

		// Token: 0x04004600 RID: 17920
		public TMP_Text weaponName;

		// Token: 0x04004601 RID: 17921
		public TMP_Text ammo;

		// Token: 0x04004602 RID: 17922
		public TMP_Text weaponModeText;

		// Token: 0x04004603 RID: 17923
		public TooltipTrigger tooltip;

		// Token: 0x04004604 RID: 17924
		public Button button;

		// Token: 0x04004605 RID: 17925
		public GameObject energyUsagePanel;

		// Token: 0x04004606 RID: 17926
		public GameObject ammoPanel;

		// Token: 0x04004607 RID: 17927
		private Weapon weapon;

		// Token: 0x04004608 RID: 17928
		private TISpaceShipState ship;

		// Token: 0x04004609 RID: 17929
		private SpaceCombatCanvasController controller;

		// Token: 0x0400460A RID: 17930
		private ShipUIController shipUIController;

		// Token: 0x0400460B RID: 17931
		private CombatShipController cShipController;

		// Token: 0x0400460C RID: 17932
		private List<ShipUIController> reticlesToHide = new List<ShipUIController>();

		// Token: 0x0400460D RID: 17933
		private bool _addedCombatSecondListener;

		// Token: 0x0400460E RID: 17934
		private bool _weaponWasDamaged;

		// Token: 0x0400460F RID: 17935
		private bool _showingBollixed;

		// Token: 0x04004610 RID: 17936
		private float lastStatusUpdate;

		// Token: 0x020013A6 RID: 5030
		public enum ChangeWeaponClickMode
		{
			// Token: 0x04007268 RID: 29288
			JustThisWeapon,
			// Token: 0x04007269 RID: 29289
			AllWeaponsOfTypeOnShip,
			// Token: 0x0400726A RID: 29290
			AllWeaponsOfTypeOnShipsInGroup,
			// Token: 0x0400726B RID: 29291
			JustWeaponOnShipsInClass,
			// Token: 0x0400726C RID: 29292
			AllWeaponsOfTypeOnShipsInClass,
			// Token: 0x0400726D RID: 29293
			AllWeaponsOfTypeOnShipsInFleet
		}
	}
}

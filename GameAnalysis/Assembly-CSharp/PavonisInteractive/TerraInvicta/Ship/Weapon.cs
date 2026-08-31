using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Ship
{
	// Token: 0x02000980 RID: 2432
	public abstract class Weapon : BaseComponent, IWeapon, IComponent
	{
		// Token: 0x17000FDC RID: 4060
		// (get) Token: 0x06005C7A RID: 23674 RVA: 0x002C12CB File Offset: 0x002BF4CB
		// (set) Token: 0x06005C7B RID: 23675 RVA: 0x002C12D3 File Offset: 0x002BF4D3
		public CombatantController combatant { get; private set; }

		// Token: 0x17000FDD RID: 4061
		// (get) Token: 0x06005C7C RID: 23676 RVA: 0x002C12DC File Offset: 0x002BF4DC
		// (set) Token: 0x06005C7D RID: 23677 RVA: 0x002C12E4 File Offset: 0x002BF4E4
		public Transform combatantTransform { get; private set; }

		// Token: 0x17000FDE RID: 4062
		// (get) Token: 0x06005C7E RID: 23678 RVA: 0x002C12ED File Offset: 0x002BF4ED
		// (set) Token: 0x06005C7F RID: 23679 RVA: 0x002C12F5 File Offset: 0x002BF4F5
		public IList<IFireMode> fireModes { get; private set; }

		// Token: 0x17000FDF RID: 4063
		// (get) Token: 0x06005C80 RID: 23680 RVA: 0x002C12FE File Offset: 0x002BF4FE
		// (set) Token: 0x06005C81 RID: 23681 RVA: 0x002C1306 File Offset: 0x002BF506
		public ShipWeaponVisController weaponVisualization { get; protected set; }

		// Token: 0x17000FE0 RID: 4064
		// (get) Token: 0x06005C82 RID: 23682 RVA: 0x002C130F File Offset: 0x002BF50F
		// (set) Token: 0x06005C83 RID: 23683 RVA: 0x002C1317 File Offset: 0x002BF517
		public ShipWeaponVisController altWeaponVisualization { get; protected set; }

		// Token: 0x17000FE1 RID: 4065
		// (get) Token: 0x06005C84 RID: 23684 RVA: 0x002C1320 File Offset: 0x002BF520
		// (set) Token: 0x06005C85 RID: 23685 RVA: 0x002C1328 File Offset: 0x002BF528
		public ModuleDataEntry weaponData { get; protected set; }

		// Token: 0x17000FE2 RID: 4066
		// (get) Token: 0x06005C86 RID: 23686 RVA: 0x002C1331 File Offset: 0x002BF531
		// (set) Token: 0x06005C87 RID: 23687 RVA: 0x002C1339 File Offset: 0x002BF539
		public TIShipWeaponTemplate weaponTemplate { get; protected set; }

		// Token: 0x17000FE3 RID: 4067
		// (get) Token: 0x06005C88 RID: 23688 RVA: 0x002C1344 File Offset: 0x002BF544
		public Vector3 position
		{
			get
			{
				ShipWeaponVisController shipWeaponVisController = this.SelectWeaponVisualization(this.targetedPosition);
				if (shipWeaponVisController != null)
				{
					return shipWeaponVisController.firePoint.transform.position;
				}
				return this.combatant.position;
			}
		}

		// Token: 0x17000FE4 RID: 4068
		// (get) Token: 0x06005C89 RID: 23689 RVA: 0x002C1383 File Offset: 0x002BF583
		private int weaponSlot
		{
			get
			{
				return this.weaponData.slotIndex;
			}
		}

		// Token: 0x17000FE5 RID: 4069
		// (get) Token: 0x06005C8A RID: 23690 RVA: 0x002C1390 File Offset: 0x002BF590
		// (set) Token: 0x06005C8B RID: 23691 RVA: 0x002C1398 File Offset: 0x002BF598
		public IFireMode currentFireMode { get; set; }

		// Token: 0x17000FE6 RID: 4070
		// (get) Token: 0x06005C8C RID: 23692 RVA: 0x002C13A1 File Offset: 0x002BF5A1
		// (set) Token: 0x06005C8D RID: 23693 RVA: 0x002C13A9 File Offset: 0x002BF5A9
		private protected DateTime lastFiredAt { protected get; private set; }

		// Token: 0x17000FE7 RID: 4071
		// (get) Token: 0x06005C8E RID: 23694 RVA: 0x002C13B2 File Offset: 0x002BF5B2
		// (set) Token: 0x06005C8F RID: 23695 RVA: 0x002C13BA File Offset: 0x002BF5BA
		public bool bollixed { get; protected set; }

		// Token: 0x17000FE8 RID: 4072
		// (get) Token: 0x06005C90 RID: 23696 RVA: 0x002C13C3 File Offset: 0x002BF5C3
		// (set) Token: 0x06005C91 RID: 23697 RVA: 0x002C13CB File Offset: 0x002BF5CB
		public IDamageable target { get; protected set; }

		// Token: 0x06005C92 RID: 23698 RVA: 0x002C13D4 File Offset: 0x002BF5D4
		public Weapon(CombatantShipController ship, ModuleDataEntry weaponData)
			: this(ship, weaponData, ComponentMap.single)
		{
		}

		// Token: 0x06005C93 RID: 23699 RVA: 0x002C13E4 File Offset: 0x002BF5E4
		public Weapon(CombatantShipController ship, ModuleDataEntry weaponData, ComponentMap map)
			: base(map)
		{
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			this.combatant = ship;
			this.combatantTransform = this.combatant.transform;
			this.weaponData = weaponData;
			this.weaponTemplate = weaponData.moduleTemplate.ref_weapon;
			this.SetFireModes(this.weaponTemplate, ship.ShipState.template, ship.ShipState.faction);
			this.lastFiredAt = this.gameTime.currentTime.ExportTime();
			if (ship.ModelController != null)
			{
				if (this.weaponTemplate.noseWeapon)
				{
					this.weaponVisualization = ship.ModelController.noseWeaponControllers[ship.ModelController.SlotToWeaponMountIndex(this.weaponSlot, this.weaponTemplate.mount)];
				}
				else
				{
					this.weaponVisualization = ship.ModelController.dorsalHullWeaponControllers[ship.ModelController.SlotToWeaponMountIndex(this.weaponSlot, this.weaponTemplate.mount)];
					this.altWeaponVisualization = ship.ModelController.ventralHullWeaponControllers[ship.ModelController.SlotToWeaponMountIndex(this.weaponSlot, this.weaponTemplate.mount)];
				}
			}
			GameControl.eventManager.AddListener<ShipSystemDamageChange>(new EventManager.EventDelegate<ShipSystemDamageChange>(this.OnShipSystemDamaged), null, ship.ShipState, true, false);
			GameControl.eventManager.AddListener<ShipOfficerKilled>(new EventManager.EventDelegate<ShipOfficerKilled>(this.OnShipOfficerKilled), null, ship.ShipState, true, false);
			this.UpdateCooldownValues();
		}

		// Token: 0x06005C94 RID: 23700 RVA: 0x002C156C File Offset: 0x002BF76C
		public Weapon(CombatHabModuleController habModule, ModuleDataEntry weaponData, int slot)
		{
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			this.combatant = habModule;
			this.combatantTransform = this.combatant.transform;
			this.weaponData = weaponData;
			this.weaponTemplate = weaponData.moduleTemplate.ref_weapon;
			this.SetFireModes(this.weaponTemplate, null, habModule.habModule.GetFaction());
			this.lastFiredAt = this.gameTime.currentTime.ExportTime();
			this.weaponVisualization = habModule.dorsalWeaponControllers[slot];
			this.altWeaponVisualization = habModule.ventralWeaponControllers[slot];
			this.UpdateCooldownValues();
		}

		// Token: 0x06005C95 RID: 23701 RVA: 0x002C1618 File Offset: 0x002BF818
		public Weapon(TIGameState surfaceState, ModuleDataEntry weaponData)
		{
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			this.weaponData = weaponData;
			this.weaponTemplate = weaponData.moduleTemplate.ref_weapon;
			this.lastFiredAt = this.gameTime.currentTime.ExportTime();
		}

		// Token: 0x17000FE9 RID: 4073
		// (get) Token: 0x06005C96 RID: 23702 RVA: 0x002C166C File Offset: 0x002BF86C
		private bool OnTarget
		{
			get
			{
				ShipWeaponVisController shipWeaponVisController = this.SelectWeaponVisualization(this.targetedPosition);
				return !(shipWeaponVisController != null) || shipWeaponVisController.OnTarget();
			}
		}

		// Token: 0x06005C97 RID: 23703 RVA: 0x002C1698 File Offset: 0x002BF898
		private void SetFireModes(TIShipWeaponTemplate weaponTemplate, TISpaceShipTemplate shipTemplate, TIFactionState faction)
		{
			this.fireModes = new List<IFireMode>
			{
				new IdleFireMode(this)
			};
			this.currentFireMode = this.fireModes[0];
			bool flag = false;
			List<FireMode> actualFireModes = weaponTemplate.GetActualFireModes(false);
			FireModeDataTemplateEntry? fireModeDataTemplateEntry = ((shipTemplate != null) ? new FireModeDataTemplateEntry?(shipTemplate.GetFireModeDataEntryFromSlot(this.weaponSlot)) : null);
			if (!flag)
			{
				int? num = ((fireModeDataTemplateEntry != null) ? new int?(fireModeDataTemplateEntry.GetValueOrDefault().slot) : null);
				int num2 = this.weaponSlot;
				if ((((num.GetValueOrDefault() == num2) & (num != null)) && fireModeDataTemplateEntry != null && fireModeDataTemplateEntry.GetValueOrDefault().fireMode == FireMode.Idle) || weaponTemplate.DefaultFireMode == FireMode.Idle)
				{
					flag = true;
					this.currentFireMode = this.fireModes[0];
				}
			}
			if (actualFireModes.Contains(FireMode.Focus))
			{
				FocusFireMode focusFireMode = new FocusFireMode(this);
				this.fireModes.Add(focusFireMode);
				if (!flag)
				{
					int? num = ((fireModeDataTemplateEntry != null) ? new int?(fireModeDataTemplateEntry.GetValueOrDefault().slot) : null);
					int num2 = this.weaponSlot;
					if ((num.GetValueOrDefault() == num2) & (num != null))
					{
						FireMode? fireMode = ((fireModeDataTemplateEntry != null) ? new FireMode?(fireModeDataTemplateEntry.GetValueOrDefault().fireMode) : null);
						FireMode fireMode2 = focusFireMode.mode;
						if ((fireMode.GetValueOrDefault() == fireMode2) & (fireMode != null))
						{
							goto IL_0195;
						}
					}
					if (weaponTemplate.DefaultFireMode != focusFireMode.mode)
					{
						goto IL_019F;
					}
					IL_0195:
					flag = true;
					this.currentFireMode = focusFireMode;
				}
			}
			IL_019F:
			if (actualFireModes.Contains(FireMode.Offense))
			{
				OffenseFireMode offenseFireMode = new OffenseFireMode(this);
				this.fireModes.Add(offenseFireMode);
				if (!flag)
				{
					int? num = ((fireModeDataTemplateEntry != null) ? new int?(fireModeDataTemplateEntry.GetValueOrDefault().slot) : null);
					int num2 = this.weaponSlot;
					if ((num.GetValueOrDefault() == num2) & (num != null))
					{
						FireMode? fireMode = ((fireModeDataTemplateEntry != null) ? new FireMode?(fireModeDataTemplateEntry.GetValueOrDefault().fireMode) : null);
						FireMode fireMode2 = offenseFireMode.mode;
						if ((fireMode.GetValueOrDefault() == fireMode2) & (fireMode != null))
						{
							goto IL_0260;
						}
					}
					if (weaponTemplate.DefaultFireMode != offenseFireMode.mode)
					{
						goto IL_026A;
					}
					IL_0260:
					flag = true;
					this.currentFireMode = offenseFireMode;
				}
			}
			IL_026A:
			if (actualFireModes.Contains(FireMode.Defense))
			{
				DefenseFireMode defenseFireMode = new DefenseFireMode(this);
				this.fireModes.Add(defenseFireMode);
				if (!flag)
				{
					int? num = ((fireModeDataTemplateEntry != null) ? new int?(fireModeDataTemplateEntry.GetValueOrDefault().slot) : null);
					int num2 = this.weaponSlot;
					if ((num.GetValueOrDefault() == num2) & (num != null))
					{
						FireMode? fireMode = ((fireModeDataTemplateEntry != null) ? new FireMode?(fireModeDataTemplateEntry.GetValueOrDefault().fireMode) : null);
						FireMode fireMode2 = defenseFireMode.mode;
						if ((fireMode.GetValueOrDefault() == fireMode2) & (fireMode != null))
						{
							goto IL_032B;
						}
					}
					if (weaponTemplate.DefaultFireMode != defenseFireMode.mode)
					{
						goto IL_0335;
					}
					IL_032B:
					flag = true;
					this.currentFireMode = defenseFireMode;
				}
			}
			IL_0335:
			if (actualFireModes.Contains(FireMode.Guardian) && !weaponTemplate.isMissileWeapon)
			{
				GuardianFireMode guardianFireMode = new GuardianFireMode(this, !faction.isActivePlayer);
				this.fireModes.Add(guardianFireMode);
				if (!flag)
				{
					int? num = ((fireModeDataTemplateEntry != null) ? new int?(fireModeDataTemplateEntry.GetValueOrDefault().slot) : null);
					int num2 = this.weaponSlot;
					if ((num.GetValueOrDefault() == num2) & (num != null))
					{
						FireMode? fireMode = ((fireModeDataTemplateEntry != null) ? new FireMode?(fireModeDataTemplateEntry.GetValueOrDefault().fireMode) : null);
						FireMode fireMode2 = guardianFireMode.mode;
						if ((fireMode.GetValueOrDefault() == fireMode2) & (fireMode != null))
						{
							goto IL_040A;
						}
					}
					if (weaponTemplate.DefaultFireMode != guardianFireMode.mode)
					{
						goto IL_0414;
					}
					IL_040A:
					flag = true;
					this.currentFireMode = guardianFireMode;
				}
			}
			IL_0414:
			if (actualFireModes.Contains(FireMode.Salvo) && weaponTemplate.isMissileWeapon)
			{
				SalvoFireMode salvoFireMode = new SalvoFireMode(this);
				this.fireModes.Add(salvoFireMode);
				if (!flag)
				{
					int? num = ((fireModeDataTemplateEntry != null) ? new int?(fireModeDataTemplateEntry.GetValueOrDefault().slot) : null);
					int num2 = this.weaponSlot;
					if ((num.GetValueOrDefault() == num2) & (num != null))
					{
						FireMode? fireMode = ((fireModeDataTemplateEntry != null) ? new FireMode?(fireModeDataTemplateEntry.GetValueOrDefault().fireMode) : null);
						FireMode fireMode2 = salvoFireMode.mode;
						if ((fireMode.GetValueOrDefault() == fireMode2) & (fireMode != null))
						{
							goto IL_04E0;
						}
					}
					if (weaponTemplate.DefaultFireMode != salvoFireMode.mode)
					{
						goto IL_04EA;
					}
					IL_04E0:
					flag = true;
					this.currentFireMode = salvoFireMode;
				}
			}
			IL_04EA:
			if (actualFireModes.Contains(FireMode.Bracket) && weaponTemplate.isGunTypeWeapon && !weaponTemplate.isPlasmaWeapon)
			{
				BracketFireMode bracketFireMode = new BracketFireMode(this);
				this.fireModes.Add(bracketFireMode);
				if (!flag)
				{
					int? num = ((fireModeDataTemplateEntry != null) ? new int?(fireModeDataTemplateEntry.GetValueOrDefault().slot) : null);
					int num2 = this.weaponSlot;
					if ((num.GetValueOrDefault() == num2) & (num != null))
					{
						FireMode? fireMode = ((fireModeDataTemplateEntry != null) ? new FireMode?(fireModeDataTemplateEntry.GetValueOrDefault().fireMode) : null);
						FireMode fireMode2 = bracketFireMode.mode;
						if ((fireMode.GetValueOrDefault() == fireMode2) & (fireMode != null))
						{
							goto IL_05C1;
						}
					}
					if (weaponTemplate.DefaultFireMode != bracketFireMode.mode)
					{
						return;
					}
					IL_05C1:
					this.currentFireMode = bracketFireMode;
				}
			}
		}

		// Token: 0x06005C98 RID: 23704 RVA: 0x002C1C70 File Offset: 0x002BFE70
		public void OnShipSystemDamaged(ShipSystemDamageChange e)
		{
			if (e.system == ShipSystem.FireControl)
			{
				this.UpdateCooldownValues();
			}
		}

		// Token: 0x06005C99 RID: 23705 RVA: 0x002C1C81 File Offset: 0x002BFE81
		public void OnShipOfficerKilled(ShipOfficerKilled e)
		{
			this.UpdateCooldownValues();
		}

		// Token: 0x06005C9A RID: 23706 RVA: 0x002C1C8C File Offset: 0x002BFE8C
		public static void GetAdjustedCooldownValues(CombatWeaponCarrierState combatantState, TIShipWeaponTemplate weaponTemplate, out float adjustedCooldownDuration_s, out float adjustedDefensiveCooldownDuration_s, out float adjustedIntraSalvoCooldownDuration_s)
		{
			float num = combatantState.FireControlFunction();
			if (num <= 0f)
			{
				num = 0.01f;
			}
			float num2 = weaponTemplate.cooldown_s / num;
			if (combatantState.isShip())
			{
				num2 += combatantState.ref_shipCarrier().SumOfficerEffectsModifiers(OfficerEffectType.GlobalWeaponCooldown, num2);
				WeaponClass weaponClass = weaponTemplate.weaponClass;
				if (weaponClass - WeaponClass.Laser > 1)
				{
					if (weaponClass == WeaponClass.Magnetic)
					{
						num2 += combatantState.ref_shipCarrier().SumOfficerEffectsModifiers(OfficerEffectType.MagWeaponCooldown, num2);
					}
				}
				else
				{
					num2 += combatantState.ref_shipCarrier().SumOfficerEffectsModifiers(OfficerEffectType.BeamCooldown, num2);
				}
			}
			float num3 = num2 * 1f;
			if (combatantState.isShip())
			{
				num3 += combatantState.ref_shipCarrier().SumOfficerEffectsModifiers(OfficerEffectType.PointDefenseCooldown, num3);
				if (!weaponTemplate.attackMode && weaponTemplate.defenseMode)
				{
					num2 += combatantState.ref_shipCarrier().SumOfficerEffectsModifiers(OfficerEffectType.PointDefenseCooldown, num2);
				}
			}
			float num4 = weaponTemplate.intraSalvoCooldown_s / num;
			adjustedCooldownDuration_s = num2;
			adjustedDefensiveCooldownDuration_s = num3;
			adjustedIntraSalvoCooldownDuration_s = num4;
		}

		// Token: 0x06005C9B RID: 23707 RVA: 0x002C1D60 File Offset: 0x002BFF60
		public void UpdateCooldownValues()
		{
			float num;
			float num2;
			float num3;
			Weapon.GetAdjustedCooldownValues(this.combatant.WeaponCarrierState, this.weaponTemplate, out num, out num2, out num3);
			this.cooldownDuration_s = TimeSpan.FromSeconds((double)num);
			this.defensiveCooldown_s = TimeSpan.FromSeconds((double)num2);
			this.intraSalvoCooldownDuration_s = TimeSpan.FromSeconds((double)num3);
		}

		// Token: 0x06005C9C RID: 23708 RVA: 0x002C1DB0 File Offset: 0x002BFFB0
		public bool OnCooldown(DateTime currentTime)
		{
			return currentTime < this.lastFiredAt + this.currentCooldownDuration_s;
		}

		// Token: 0x06005C9D RID: 23709 RVA: 0x002C1DCC File Offset: 0x002BFFCC
		public bool InArc(Vector3 targetPosition, Vector3 targetVelocity, Vector3 targetAcceleration)
		{
			if (!this.weaponTemplate.noseWeapon)
			{
				return true;
			}
			if (this.weaponTemplate.isLaserWeapon || this.weaponTemplate.isParticleWeapon)
			{
				return Vector3.Angle(targetPosition - this.position, this.combatantTransform.forward) <= this.weaponTemplate.pivotRange_deg;
			}
			if (targetAcceleration == Vector3.zero)
			{
				if (this.weaponTemplate.isMissileWeapon)
				{
					bool flag;
					Vector3 vector = TISpaceCombatProjectileState.FirstOrderInterceptPosition(this.position, this.combatant.velocityVector, SpaceCombatManager.km_to_scale(this.weaponTemplate.ref_missileWeapon.deltaV_kps), targetPosition, targetVelocity, out flag);
					return !flag && Vector3.Angle(vector - this.position, this.combatantTransform.forward) <= this.weaponTemplate.pivotRange_deg;
				}
				bool flag2;
				Vector3 vector2 = TISpaceCombatProjectileState.FirstOrderInterceptPosition(this.position, this.combatant.velocityVector, SpaceCombatManager.km_to_scale(this.weaponTemplate.ref_gunWeapon.muzzleVelocity_kps), targetPosition, targetVelocity, out flag2);
				return !flag2 && Vector3.Angle(vector2 - this.position, this.combatantTransform.forward) <= this.weaponTemplate.pivotRange_deg;
			}
			else
			{
				if (this.weaponTemplate.isMissileWeapon)
				{
					bool flag3;
					Vector3 vector3 = TISpaceCombatProjectileState.SecondOrderInterceptPosition(this.position, this.combatant.velocityVector, SpaceCombatManager.km_to_scale(this.weaponTemplate.ref_missileWeapon.deltaV_kps), targetPosition, targetVelocity, targetAcceleration, this.weaponTemplate.cooldown_s, out flag3);
					return !flag3 && Vector3.Angle(vector3 - this.position, this.combatantTransform.forward) <= this.weaponTemplate.pivotRange_deg;
				}
				bool flag4;
				Vector3 vector4 = TISpaceCombatProjectileState.SecondOrderInterceptPosition(this.position, this.combatant.velocityVector, SpaceCombatManager.km_to_scale(this.weaponTemplate.ref_gunWeapon.muzzleVelocity_kps), targetPosition, targetVelocity, targetAcceleration, this.weaponTemplate.cooldown_s, out flag4);
				return !flag4 && Vector3.Angle(vector4 - this.position, this.combatantTransform.forward) <= this.weaponTemplate.pivotRange_deg;
			}
		}

		// Token: 0x06005C9E RID: 23710 RVA: 0x002C2004 File Offset: 0x002C0204
		public void EnterCooldown(bool downFired = false, bool cooldownByBollix = false, bool cooldownByDisable = false, int overrideDuration_s = 0)
		{
			DateTime dateTime = this.gameTime.currentTime.ExportTime();
			if (cooldownByBollix)
			{
				float num = 0f;
				if (this.OnCooldown(dateTime))
				{
					num = Mathf.Max(0f, (float)(dateTime - this.lastFiredAt).TotalSeconds);
				}
				this.currentCooldownDuration_s = new TimeSpan(0, 0, (int)Mathf.Max(num, (float)overrideDuration_s));
				this.bollixed = (float)overrideDuration_s >= num;
				this.lastFiredAt = dateTime;
			}
			else if (cooldownByDisable)
			{
				float num2 = 0f;
				if (this.OnCooldown(dateTime))
				{
					num2 = Mathf.Max(0f, (float)(dateTime - this.lastFiredAt).TotalSeconds);
				}
				this.currentCooldownDuration_s = new TimeSpan(0, 0, (int)((float)overrideDuration_s + num2));
				this.lastFiredAt = dateTime;
			}
			else
			{
				if (this.weaponTemplate.salvo_shots > 1 && this.shotsFiredThisSalvo > 0)
				{
					double num3 = (dateTime - this.lastFiredAt).TotalSeconds - (double)this.weaponTemplate.intraSalvoCooldown_s;
					if (num3 > 0.0)
					{
						num3 *= (double)((float)this.weaponTemplate.salvo_shots / this.weaponTemplate.cooldown_s);
						this.shotsFiredThisSalvo -= Mathf.Min((int)Math.Truncate(num3), this.shotsFiredThisSalvo);
					}
				}
				this.shotsFiredThisSalvo++;
				if (downFired)
				{
					if (this.shotsFiredThisSalvo == this.weaponTemplate.salvo_shots || this.weaponTemplate.salvo_shots <= 1)
					{
						if (this.defensiveCooldown_s < this.cooldownDuration_s)
						{
							this.currentCooldownDuration_s = this.defensiveCooldown_s;
						}
						else
						{
							this.currentCooldownDuration_s = this.cooldownDuration_s;
						}
					}
					else if (this.defensiveCooldown_s < this.intraSalvoCooldownDuration_s)
					{
						this.currentCooldownDuration_s = this.defensiveCooldown_s;
					}
					else
					{
						this.currentCooldownDuration_s = this.intraSalvoCooldownDuration_s;
					}
				}
				else if (this.shotsFiredThisSalvo == this.weaponTemplate.salvo_shots || this.weaponTemplate.salvo_shots <= 1)
				{
					this.currentCooldownDuration_s = this.cooldownDuration_s;
				}
				else
				{
					this.currentCooldownDuration_s = this.intraSalvoCooldownDuration_s;
				}
			}
			this.lastFiredAt = dateTime.AddSeconds((double)TIUtilities.RandomRange(Mathf.Max(-1f, -this.weaponTemplate.intraSalvoCooldown_s * 0.25f), Mathf.Min(1f, this.weaponTemplate.intraSalvoCooldown_s * 0.25f)));
		}

		// Token: 0x06005C9F RID: 23711 RVA: 0x002C2274 File Offset: 0x002C0474
		public ShipWeaponVisController SelectWeaponVisualization(Vector3 targetedPosition)
		{
			if (this.weaponTemplate.noseWeapon)
			{
				return this.weaponVisualization;
			}
			if (!this.weaponTemplate.hullWeapon)
			{
				return null;
			}
			if (Vector3.Dot(targetedPosition - this.combatant.position, this.combatantTransform.up) >= 0f)
			{
				return this.weaponVisualization;
			}
			return this.altWeaponVisualization;
		}

		// Token: 0x06005CA0 RID: 23712 RVA: 0x002C22D9 File Offset: 0x002C04D9
		public void SetTarget_Strategy(IDamageable newTarget, Vector3d position)
		{
			this.target = newTarget;
			this.targetedPosition = (Vector3)position;
		}

		// Token: 0x06005CA1 RID: 23713 RVA: 0x002C22F0 File Offset: 0x002C04F0
		public float TargetChance(CombatTargetableState target, TIShipWeaponTemplate weapon, float distance_km, int ECMDefeats)
		{
			float num = Mathf.Min(1f, distance_km / weapon.targetingRange_km);
			float num2 = 1f + this.combatant.WeaponCarrierState.TargetingBonus(weapon, this.combatant.combatMgr.combatState.AlliedHab(this.combatant.WeaponCarrierState)) + (float)ECMDefeats * TIGlobalConfig.globalConfig.attackBonusPerTargetECMDefeat;
			float num3 = target.ECMValue(this.combatant.WeaponCarrierState.GetFaction(), this.combatant.combatMgr.combatState.AlliedHab(target)) * num;
			return num2 - num3;
		}

		// Token: 0x06005CA2 RID: 23714 RVA: 0x002C2388 File Offset: 0x002C0588
		public bool AcquireTarget(DateTime currentTime)
		{
			if (this.OnCooldown(currentTime))
			{
				return false;
			}
			if (this.bollixed)
			{
				this.bollixed = false;
				this.combatant.ECMDefeats.Add(this.target);
			}
			IDamageable target = this.target;
			float num;
			this.target = this.currentFireMode.AcquireTarget(currentTime, out this.targetedPosition, out num);
			if (this.target != null)
			{
				bool flag = this.combatant.ref_shipController != null && (this.target.damageableType == IDamageableType.Ship || this.target.damageableType == IDamageableType.Missile) && TIUtilities.RandomFloatValue() > this.combatant.ref_shipController.ShipState.GetSystemFunction(ShipSystem.Sensors);
				if (this.target != target || flag)
				{
					int num2 = this.combatant.ECMDefeats.Count<IDamageable>((IDamageable x) => x == this.target);
					float num3 = this.TargetChance(this.target.combatTargetableState, this.weaponTemplate, num, num2);
					float num4 = TIUtilities.RandomFloatValue();
					if (num4 > num3)
					{
						float num5 = 100f * (num4 - num3);
						int num6 = (int)((this.weaponTemplate.isMissileWeapon ? TIGlobalConfig.globalConfig.ECM_SecondsBollixedPerPointMissed_Missile : TIGlobalConfig.globalConfig.ECM_SecondsBollixedPerPointMissed) * num5 / (float)(num2 + 1));
						this.EnterCooldown(false, true, false, num6);
					}
				}
				this.SelectWeaponVisualization(this.targetedPosition).SetTarget(this.target, this.targetedPosition);
				return true;
			}
			this.weaponVisualization.ClearTarget();
			if (!this.weaponTemplate.noseWeapon)
			{
				this.altWeaponVisualization.ClearTarget();
			}
			return false;
		}

		// Token: 0x06005CA3 RID: 23715 RVA: 0x002C2525 File Offset: 0x002C0725
		public virtual Vector3 GetPositionToTarget(IDamageable targetToCheck, out bool impossible)
		{
			if (targetToCheck == null)
			{
				impossible = true;
				return Vector3.zero;
			}
			impossible = false;
			return targetToCheck.position;
		}

		// Token: 0x06005CA4 RID: 23716 RVA: 0x002C253C File Offset: 0x002C073C
		public bool TryFireCommon(DateTime currentTime)
		{
			if (this.OnCooldown(currentTime) || this.target == null || !this.combatant.WeaponCarrierState.WeaponCanFire(this.weaponData))
			{
				return false;
			}
			if (this.shotsFiredThisSalvo == this.weaponTemplate.salvo_shots)
			{
				this.shotsFiredThisSalvo = 0;
			}
			return this.OnTarget;
		}

		// Token: 0x06005CA5 RID: 23717
		public abstract bool TryFire(DateTime currentTime);

		// Token: 0x06005CA6 RID: 23718 RVA: 0x002C259C File Offset: 0x002C079C
		public int GetSTOShotCount(float shooterLongitude, TISpaceShipState target, float secondsSinceLastFiring)
		{
			float num = (shooterLongitude * 0.017453292f + 6.2831855f) % 6.2831855f;
			float num2 = target.fleet.longitude * 0.017453292f;
			float num3 = (float)(target.fleet.semiMajorAxis_km * 6.2831854820251465);
			float num4 = (float)(target.fleet.meanVelocity_mps / 1000.0) / num3 * 6.2831855f;
			float num5 = (float)(6.2831854820251465 / target.ref_spaceBody.rotationperiod_s);
			float num6 = num4 - num5;
			float num7 = num6 * secondsSinceLastFiring;
			float num8 = this.weaponTemplate.cooldown_s * num6;
			if (num8 == 0f)
			{
				Log.Error("radiansBetweenShots is zero. Cannot compute shot count.", Array.Empty<object>());
				return 0;
			}
			float num9 = num2;
			float num10 = num2 - num7 + num8;
			int num11 = 0;
			while ((num8 >= 0f) ? (num9 > num10) : (num9 < num10))
			{
				float num12 = (num - num9 + 62.831856f) % 6.2831855f;
				if (num12 < 0f)
				{
					num12 += 6.2831855f;
				}
				if (num12 < 1.5707964f || num12 > 4.712389f)
				{
					Vector3 vector = Quaternion.AngleAxis(num * 57.29578f, Vector3.up) * Vector3.right * (float)target.ref_spaceBody.meanRadius_km;
					Vector3 vector2 = Quaternion.AngleAxis(num9 * 57.29578f, Vector3.up) * Vector3.right * ((float)target.ref_spaceBody.meanRadius_km + target.fleet.bombardmentAltitude_km) - vector;
					if (Vector3.Angle(vector, vector2) < 90f)
					{
						num11++;
					}
				}
				num9 -= num8;
			}
			return Mathf.Max(1, num11);
		}

		// Token: 0x040041EB RID: 16875
		private TimeSpan currentCooldownDuration_s;

		// Token: 0x040041EC RID: 16876
		private TimeSpan cooldownDuration_s;

		// Token: 0x040041ED RID: 16877
		private TimeSpan intraSalvoCooldownDuration_s;

		// Token: 0x040041EE RID: 16878
		private TimeSpan defensiveCooldown_s;

		// Token: 0x040041EF RID: 16879
		protected GameTimeManager gameTime;

		// Token: 0x040041F7 RID: 16887
		protected int shotsFiredThisSalvo;

		// Token: 0x040041F8 RID: 16888
		public Vector3 targetedPosition;

		// Token: 0x040041FD RID: 16893
		public const float defensiveFireCooldownModifier = 1f;

		// Token: 0x040041FE RID: 16894
		private const bool FleetECMDefeatSharing = false;
	}
}

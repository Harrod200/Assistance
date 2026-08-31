using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Ship;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009E1 RID: 2529
	public class CombatHabModuleController : CombatantController, IDamageable
	{
		// Token: 0x17001065 RID: 4197
		// (get) Token: 0x06005F59 RID: 24409 RVA: 0x002D296C File Offset: 0x002D0B6C
		// (set) Token: 0x06005F5A RID: 24410 RVA: 0x002D2974 File Offset: 0x002D0B74
		public TIHabModuleState habModule { get; private set; }

		// Token: 0x06005F5B RID: 24411 RVA: 0x002D297D File Offset: 0x002D0B7D
		public override IDamageableType GetCombatantType()
		{
			return IDamageableType.StationModule;
		}

		// Token: 0x17001066 RID: 4198
		// (get) Token: 0x06005F5C RID: 24412 RVA: 0x002D2980 File Offset: 0x002D0B80
		// (set) Token: 0x06005F5D RID: 24413 RVA: 0x002D2988 File Offset: 0x002D0B88
		public float baseHitPoints { get; private set; }

		// Token: 0x17001067 RID: 4199
		// (get) Token: 0x06005F5E RID: 24414 RVA: 0x002D2991 File Offset: 0x002D0B91
		// (set) Token: 0x06005F5F RID: 24415 RVA: 0x002D2999 File Offset: 0x002D0B99
		public TIShipArmorTemplate armorTemplate { get; private set; }

		// Token: 0x17001068 RID: 4200
		// (get) Token: 0x06005F60 RID: 24416 RVA: 0x002D29A2 File Offset: 0x002D0BA2
		// (set) Token: 0x06005F61 RID: 24417 RVA: 0x002D29AA File Offset: 0x002D0BAA
		public List<SphereCollider> combatHitColliders { get; private set; }

		// Token: 0x17001069 RID: 4201
		// (get) Token: 0x06005F62 RID: 24418 RVA: 0x002D29B3 File Offset: 0x002D0BB3
		// (set) Token: 0x06005F63 RID: 24419 RVA: 0x002D29BB File Offset: 0x002D0BBB
		public List<ModuleDataEntry> weaponDataEntries { get; private set; }

		// Token: 0x1700106A RID: 4202
		// (get) Token: 0x06005F64 RID: 24420 RVA: 0x002D29C4 File Offset: 0x002D0BC4
		// (set) Token: 0x06005F65 RID: 24421 RVA: 0x002D29CC File Offset: 0x002D0BCC
		public List<IWeapon> weapons { get; private set; }

		// Token: 0x1700106B RID: 4203
		// (get) Token: 0x06005F66 RID: 24422 RVA: 0x002D29D5 File Offset: 0x002D0BD5
		// (set) Token: 0x06005F67 RID: 24423 RVA: 0x002D29DD File Offset: 0x002D0BDD
		public List<ShipWeaponVisController> dorsalWeaponControllers { get; private set; }

		// Token: 0x1700106C RID: 4204
		// (get) Token: 0x06005F68 RID: 24424 RVA: 0x002D29E6 File Offset: 0x002D0BE6
		// (set) Token: 0x06005F69 RID: 24425 RVA: 0x002D29EE File Offset: 0x002D0BEE
		public List<ShipWeaponVisController> ventralWeaponControllers { get; private set; }

		// Token: 0x1700106D RID: 4205
		// (get) Token: 0x06005F6A RID: 24426 RVA: 0x002D29F7 File Offset: 0x002D0BF7
		public override CombatHabModuleController ref_habModuleController
		{
			get
			{
				return this;
			}
		}

		// Token: 0x06005F6B RID: 24427 RVA: 0x002D29FA File Offset: 0x002D0BFA
		public override SpaceCombatAssetUIController UIController()
		{
			return this.habModuleController.UIController;
		}

		// Token: 0x06005F6C RID: 24428 RVA: 0x002D2A07 File Offset: 0x002D0C07
		public override CombatTargetableState GetCombatantState()
		{
			return this.habModule;
		}

		// Token: 0x1700106E RID: 4206
		// (get) Token: 0x06005F6D RID: 24429 RVA: 0x002D2A0F File Offset: 0x002D0C0F
		public override CombatTargetableState combatTargetableState
		{
			get
			{
				return this.habModule;
			}
		}

		// Token: 0x1700106F RID: 4207
		// (get) Token: 0x06005F6E RID: 24430 RVA: 0x002D2A17 File Offset: 0x002D0C17
		TISpaceCombatProjectileState IDamageable.ref_projectile
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001070 RID: 4208
		// (get) Token: 0x06005F6F RID: 24431 RVA: 0x002D2A1A File Offset: 0x002D0C1A
		public override Vector3 velocityVector_kps
		{
			get
			{
				return Vector3.zero;
			}
		}

		// Token: 0x17001071 RID: 4209
		// (get) Token: 0x06005F70 RID: 24432 RVA: 0x002D2A21 File Offset: 0x002D0C21
		public override IDamageableType damageableType
		{
			get
			{
				return IDamageableType.StationModule;
			}
		}

		// Token: 0x06005F71 RID: 24433 RVA: 0x002D2A24 File Offset: 0x002D0C24
		public override Vector3 positionAtTime(DateTime currentTime)
		{
			return base.position;
		}

		// Token: 0x17001072 RID: 4210
		// (get) Token: 0x06005F72 RID: 24434 RVA: 0x002D2A2C File Offset: 0x002D0C2C
		public override Vector3 accelerationVector
		{
			get
			{
				return Vector3.zero;
			}
		}

		// Token: 0x17001073 RID: 4211
		// (get) Token: 0x06005F73 RID: 24435 RVA: 0x002D2A33 File Offset: 0x002D0C33
		public override Vector3 accelerationVector_kps
		{
			get
			{
				return Vector3.zero;
			}
		}

		// Token: 0x06005F74 RID: 24436 RVA: 0x002D2A3A File Offset: 0x002D0C3A
		public override float GetCrossSectionalArea_m2(float angle = -3.4028235E+38f)
		{
			return this.habModule.GetCrossSectionalArea_m2(angle);
		}

		// Token: 0x06005F75 RID: 24437 RVA: 0x002D2A48 File Offset: 0x002D0C48
		public void InitializeForCombat(TIHabModuleState habModule, HabModuleController habModuleController)
		{
			base.WeaponCarrierState = habModule;
			base.combatMgr = GameControl.spaceCombat;
			this.habModule = habModule;
			this.habModuleController = habModuleController;
			this.habModelController = habModuleController.habModelController;
			base.combatantTransform = base.transform;
			this.baseHitPoints = (this.hitPoints = (float)habModule.moduleTemplate.BaseStationModuleHitPoints(habModule.ref_faction, habModule.hab));
			this.armorTemplate = habModule.armorTemplate;
			this.armor = habModule.StationModuleArmorPoints;
			TIShipWeaponTemplate pointDefenseWeaponTemplate = habModule.PointDefenseWeaponTemplate;
			TIShipWeaponTemplate defenseWeaponTemplate = habModule.defenseWeaponTemplate;
			TIShipWeaponTemplate defenseWeaponTemplate_gun = habModule.defenseWeaponTemplate_gun;
			TIShipWeaponTemplate tishipWeaponTemplate = null;
			this.weaponDataEntries = new List<ModuleDataEntry>
			{
				new ModuleDataEntry(pointDefenseWeaponTemplate, 0),
				new ModuleDataEntry(defenseWeaponTemplate, 1),
				new ModuleDataEntry(defenseWeaponTemplate_gun, 2)
			};
			if (habModule.moduleTemplate.weaponMounts >= 4)
			{
				tishipWeaponTemplate = habModule.defenseWeaponTemplate_plasma;
				if (tishipWeaponTemplate != null)
				{
					this.weaponDataEntries.Add(new ModuleDataEntry(tishipWeaponTemplate, 3));
				}
			}
			if (base.combatMgr.combatState.autoresolve)
			{
				return;
			}
			base.gameObject.transform.SetLayer(LayerMask.NameToLayer("HurtBox"), false);
			SphereCollider sphereCollider = base.gameObject.AddComponent<SphereCollider>();
			sphereCollider.radius = (float)(habModule.moduleTemplate.tier * 480) * GameControl.spaceCombat.modelScalingFactor;
			this.combatHitColliders = new List<SphereCollider> { sphereCollider };
			this.genericColliders = new List<Collider>();
			foreach (Collider collider in base.gameObject.GetComponents<Collider>())
			{
				this.genericColliders.Add(collider);
			}
			this.alliedCombatants = new List<CombatantController>();
			this.enemyCombatants = new List<CombatantController>();
			this.dorsalWeaponControllers = new List<ShipWeaponVisController>();
			this.ventralWeaponControllers = new List<ShipWeaponVisController>();
			GameObject gameObject = GameControl.assetLoader.LoadAsset<GameObject>(habModule.moduleTemplate.stationModelResource);
			GameObject gameObject2 = GameControl.assetLoader.LoadAsset<GameObject>(pointDefenseWeaponTemplate.modelResource);
			GameObject gameObject3 = GameControl.assetLoader.LoadAsset<GameObject>(defenseWeaponTemplate.modelResource);
			GameObject gameObject4 = GameControl.assetLoader.LoadAsset<GameObject>(defenseWeaponTemplate_gun.modelResource);
			GameObject gameObject5 = global::UnityEngine.Object.Instantiate<GameObject>(gameObject2, base.transform);
			GameObject gameObject6 = global::UnityEngine.Object.Instantiate<GameObject>(gameObject3, base.transform);
			GameObject gameObject7 = global::UnityEngine.Object.Instantiate<GameObject>(gameObject4, base.transform);
			GameObject gameObject8 = global::UnityEngine.Object.Instantiate<GameObject>(gameObject2, base.transform);
			GameObject gameObject9 = global::UnityEngine.Object.Instantiate<GameObject>(gameObject3, base.transform);
			GameObject gameObject10 = global::UnityEngine.Object.Instantiate<GameObject>(gameObject4, base.transform);
			gameObject5.transform.localPosition = gameObject.transform.GetChild(0).localPosition;
			gameObject5.transform.localRotation = gameObject.transform.GetChild(0).localRotation;
			gameObject6.transform.localPosition = gameObject.transform.GetChild(1).localPosition;
			gameObject6.transform.localRotation = gameObject.transform.GetChild(1).localRotation;
			gameObject7.transform.localPosition = gameObject.transform.GetChild(2).localPosition;
			gameObject7.transform.localRotation = gameObject.transform.GetChild(2).localRotation;
			gameObject8.transform.localPosition = gameObject.transform.GetChild(3).localPosition;
			gameObject8.transform.localRotation = gameObject.transform.GetChild(3).localRotation;
			gameObject9.transform.localPosition = gameObject.transform.GetChild(4).localPosition;
			gameObject9.transform.localRotation = gameObject.transform.GetChild(4).localRotation;
			gameObject10.transform.localPosition = gameObject.transform.GetChild(5).localPosition;
			gameObject10.transform.localRotation = gameObject.transform.GetChild(5).localRotation;
			ShipWeaponVisController shipWeaponVisController = gameObject5.AddComponent<ShipWeaponVisController>();
			this.dorsalWeaponControllers.Add(shipWeaponVisController);
			shipWeaponVisController.Initialize(this, gameObject5, pointDefenseWeaponTemplate, 0);
			ShipWeaponVisController shipWeaponVisController2 = gameObject6.AddComponent<ShipWeaponVisController>();
			this.dorsalWeaponControllers.Add(shipWeaponVisController2);
			shipWeaponVisController2.Initialize(this, gameObject6, defenseWeaponTemplate, 1);
			ShipWeaponVisController shipWeaponVisController3 = gameObject7.AddComponent<ShipWeaponVisController>();
			this.dorsalWeaponControllers.Add(shipWeaponVisController3);
			shipWeaponVisController3.Initialize(this, gameObject7, defenseWeaponTemplate_gun, 2);
			ShipWeaponVisController shipWeaponVisController4 = gameObject8.AddComponent<ShipWeaponVisController>();
			this.ventralWeaponControllers.Add(shipWeaponVisController4);
			shipWeaponVisController4.Initialize(this, gameObject8, pointDefenseWeaponTemplate, 0);
			ShipWeaponVisController shipWeaponVisController5 = gameObject9.AddComponent<ShipWeaponVisController>();
			this.ventralWeaponControllers.Add(shipWeaponVisController5);
			shipWeaponVisController5.Initialize(this, gameObject9, defenseWeaponTemplate, 1);
			ShipWeaponVisController shipWeaponVisController6 = gameObject10.AddComponent<ShipWeaponVisController>();
			this.ventralWeaponControllers.Add(shipWeaponVisController6);
			shipWeaponVisController6.Initialize(this, gameObject10, defenseWeaponTemplate_gun, 2);
			if (tishipWeaponTemplate != null)
			{
				GameObject gameObject11 = GameControl.assetLoader.LoadAsset<GameObject>(tishipWeaponTemplate.modelResource);
				GameObject gameObject12 = global::UnityEngine.Object.Instantiate<GameObject>(gameObject11, base.transform);
				GameObject gameObject13 = global::UnityEngine.Object.Instantiate<GameObject>(gameObject11, base.transform);
				gameObject12.transform.localPosition = gameObject.transform.GetChild(6).localPosition;
				gameObject12.transform.localRotation = gameObject.transform.GetChild(6).localRotation;
				gameObject13.transform.localPosition = gameObject.transform.GetChild(7).localPosition;
				gameObject13.transform.localRotation = gameObject.transform.GetChild(7).localRotation;
				ShipWeaponVisController shipWeaponVisController7 = gameObject12.AddComponent<ShipWeaponVisController>();
				this.dorsalWeaponControllers.Add(shipWeaponVisController7);
				shipWeaponVisController7.Initialize(this, gameObject12, tishipWeaponTemplate, 3);
				ShipWeaponVisController shipWeaponVisController8 = gameObject13.AddComponent<ShipWeaponVisController>();
				this.ventralWeaponControllers.Add(shipWeaponVisController8);
				shipWeaponVisController8.Initialize(this, gameObject13, tishipWeaponTemplate, 3);
			}
			List<ShipWeaponVisController> dorsalWeaponControllers = this.dorsalWeaponControllers;
			if (dorsalWeaponControllers != null)
			{
				dorsalWeaponControllers.ForEach(delegate(ShipWeaponVisController x)
				{
					x.gameObject.SetActive(true);
				});
			}
			List<ShipWeaponVisController> ventralWeaponControllers = this.ventralWeaponControllers;
			if (ventralWeaponControllers != null)
			{
				ventralWeaponControllers.ForEach(delegate(ShipWeaponVisController x)
				{
					x.gameObject.SetActive(true);
				});
			}
			this.weapons = this.weaponDataEntries.Select<ModuleDataEntry, IWeapon>(delegate(ModuleDataEntry weaponEntry)
			{
				if (weaponEntry.moduleTemplate is TIBeamWeaponTemplate)
				{
					return new BeamWeapon(this, weaponEntry, weaponEntry.slotIndex);
				}
				if (weaponEntry.moduleTemplate is TIGunTypeWeaponTemplate)
				{
					return new ProjectileWeapon(this, weaponEntry, weaponEntry.slotIndex);
				}
				if (weaponEntry.moduleTemplate.ref_missileWeapon != null)
				{
					return new MissileWeapon(this, weaponEntry, weaponEntry.slotIndex);
				}
				return null;
			}).ToList<IWeapon>();
		}

		// Token: 0x17001074 RID: 4212
		// (get) Token: 0x06005F76 RID: 24438 RVA: 0x002D3063 File Offset: 0x002D1263
		// (set) Token: 0x06005F77 RID: 24439 RVA: 0x002D306A File Offset: 0x002D126A
		public override Vector3 velocityVector
		{
			get
			{
				return Vector3.zero;
			}
			protected set
			{
				new Vector3(0f, 0f, 0f);
			}
		}

		// Token: 0x17001075 RID: 4213
		// (get) Token: 0x06005F78 RID: 24440 RVA: 0x002D3081 File Offset: 0x002D1281
		// (set) Token: 0x06005F79 RID: 24441 RVA: 0x002D3089 File Offset: 0x002D1289
		public override List<Collider> hitColliders
		{
			get
			{
				return this.genericColliders;
			}
			protected set
			{
			}
		}

		// Token: 0x06005F7A RID: 24442 RVA: 0x002D308C File Offset: 0x002D128C
		public void DestroyHabModule(TIFactionState destroyer)
		{
			base.destructionTriggered = true;
			GameObject gameObject;
			AssetCacheManager.destructionSequencePrefabs.TryGetValue(this.habModule.moduleTemplate.stationDestructionResource, out gameObject);
			GameObject gameObject2 = global::UnityEngine.Object.Instantiate<GameObject>(gameObject, base.transform);
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localRotation = Quaternion.identity;
			base.StartCoroutine(this.DestroyModuleDelayed(destroyer, this.habModule));
		}

		// Token: 0x06005F7B RID: 24443 RVA: 0x002D30FC File Offset: 0x002D12FC
		private IEnumerator DestroyModuleDelayed(TIFactionState destroyer, TIHabModuleState state)
		{
			yield return this.delay;
			this.habModule.hab.DestroyModule(destroyer, state, true, true, true, 0f, true, false);
			List<ShipWeaponVisController> dorsalWeaponControllers = this.dorsalWeaponControllers;
			if (dorsalWeaponControllers != null)
			{
				dorsalWeaponControllers.ForEach(delegate(ShipWeaponVisController x)
				{
					x.gameObject.SetActive(false);
				});
			}
			List<ShipWeaponVisController> ventralWeaponControllers = this.ventralWeaponControllers;
			if (ventralWeaponControllers != null)
			{
				ventralWeaponControllers.ForEach(delegate(ShipWeaponVisController x)
				{
					x.gameObject.SetActive(false);
				});
			}
			using (List<CombatShipController>.Enumerator enumerator = base.combatMgr.activeShips.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CombatShipController combatShipController = enumerator.Current;
					if (combatShipController.primaryTarget == this)
					{
						combatShipController.ShipState.faction.playerControl.StartAction(new ClearPrimaryTargetAction(combatShipController.ShipState));
						foreach (IWeapon weapon in combatShipController.hull.IterateByClass<IWeapon>())
						{
							if (weapon.currentFireMode.mode == FireMode.Focus)
							{
								if (weapon.fireModes.Any<IFireMode>((IFireMode x) => x.mode == FireMode.Offense))
								{
									combatShipController.ShipState.faction.playerControl.StartAction(new SetWeaponModeAction(combatShipController.ShipState, weapon as Weapon, FireMode.Offense));
								}
							}
						}
						GameControl.eventManager.TriggerEvent(new ShipPrimaryTargetDestroyed(combatShipController.ShipState), null, new object[] { combatShipController.ShipState });
					}
				}
				yield break;
			}
			yield break;
		}

		// Token: 0x06005F7C RID: 24444 RVA: 0x002D311C File Offset: 0x002D131C
		public override float ApplyDamage(DamageSource source)
		{
			float num2;
			float num = CombatHabModuleController.ApplyDamage(this.habModule.moduleTemplate, source, this.armorTemplate, this.baseHitPoints, this.habModule.hab.irradiatedMultiplier, ref this.armor, ref this.hitPoints, out num2, this.weapons);
			if (this.hitPoints <= 0f && !this.habModule.destroyed)
			{
				if (!base.destructionTriggered)
				{
					base.destructionTriggered = true;
					TIFactionState faction = source.attacker.GetFaction();
					if (!base.combatMgr.combatState.autoresolve)
					{
						base.combatMgr.DestroyHabModule(faction, this);
					}
					GameControl.eventManager.TriggerEvent(new HabModuleDestroyedInCombat(this.habModule, source), null, Array.Empty<object>());
				}
			}
			else if (source.damage.amount > 0f)
			{
				GameControl.eventManager.TriggerEvent(new HabModuleDamagedInCombat(this.habModule, source.damage.weapon, source.damage.amount, num2), null, new object[] { this.habModule });
			}
			return num;
		}

		// Token: 0x06005F7D RID: 24445 RVA: 0x002D3238 File Offset: 0x002D1438
		public static float ApplyDamage(TIHabModuleTemplate moduleTemplate, DamageSource source, TIShipArmorTemplate armorTemplate, float baseHitPoints, float irradiatedMultiplier, ref float armor, ref float hitPoints, out float absorbedDamage, IEnumerable<IWeapon> weapons = null)
		{
			float num = source.damage.amount;
			absorbedDamage = 0f;
			float num2 = armor;
			TIShipWeaponTemplate weapon = source.damage.weapon;
			if (weapon.isLaserWeapon)
			{
				num2 = weapon.ref_laserWeapon.ModifyArmorValueForLaserShot(source.damage.range_km, num2, -1f);
			}
			if (source.damage.type == DamageType.ParticleBeam)
			{
				float num3 = num * weapon.ref_particleWeapon.heatFraction;
				float num4 = num * weapon.ref_particleWeapon.xRayFraction;
				float num5 = num * weapon.ref_particleWeapon.baryonFraction;
				float num6 = 0.0625f * irradiatedMultiplier;
				num4 = Mathf.Max(0f, num4 * Mathf.Min(num6, Mathf.Pow(0.5f, armorTemplate.armor_section_thickness_m(armor) * 100f / armorTemplate.xRayHalfValue_cm)));
				num5 = Mathf.Max(0f, num5 * Mathf.Min(num6, Mathf.Pow(0.5f, armorTemplate.armor_section_thickness_m(armor) * 100f / armorTemplate.baryonicHalfValue_cm))) * 5f;
				float num7 = num4 + num5;
				if (weapons != null && num7 >= TIUtilities.RandomFloatValue())
				{
					Weapon weapon2 = weapons.SelectRandomItem<IWeapon>() as Weapon;
					if (weapon2 != null)
					{
						weapon2.EnterCooldown(false, false, true, 2 + (int)num7);
					}
				}
				num = num3;
			}
			if (TIUtilities.RandomFloatValue() < hitPoints / baseHitPoints)
			{
				if (num2 > num)
				{
					num2 = num;
				}
				num -= num2;
				absorbedDamage = num2;
			}
			hitPoints -= num;
			return num;
		}

		// Token: 0x06005F7E RID: 24446 RVA: 0x002D33AB File Offset: 0x002D15AB
		public void UpdateHab()
		{
			this.UpdateIsMissileSaturated();
		}

		// Token: 0x06005F7F RID: 24447 RVA: 0x002D33B4 File Offset: 0x002D15B4
		public float GetHabModuleEffectiveScaledCombatRange()
		{
			float num = 0f;
			int num2 = 0;
			foreach (ShipWeaponVisController shipWeaponVisController in this.dorsalWeaponControllers)
			{
				if (shipWeaponVisController.weaponTemplate != null && base.WeaponCarrierState.WeaponIsOperable(shipWeaponVisController.weaponModuleData) && (shipWeaponVisController.weaponTemplate.GetActualFireModes(false).Contains(FireMode.Focus) || shipWeaponVisController.weaponTemplate.GetActualFireModes(false).Contains(FireMode.Offense)))
				{
					if (shipWeaponVisController.weaponTemplate.isLaserWeapon && shipWeaponVisController.weaponTemplate.GetActualFireModes(false).Contains(FireMode.Offense))
					{
						num += shipWeaponVisController.weaponTemplate.targetingRange_km * 0.6f;
					}
					else
					{
						num += shipWeaponVisController.weaponTemplate.targetingRange_km * 0.75f;
					}
					num2++;
				}
			}
			return SpaceCombatManager.km_to_scale(num / (float)num2);
		}

		// Token: 0x06005F80 RID: 24448 RVA: 0x002D34AC File Offset: 0x002D16AC
		protected void UpdateIsMissileSaturated()
		{
			List<MissileController> allMissilesTargetingMe = this.GetAllMissilesTargetingMe();
			base.isMissileSaturated = this.EstimateShipKillDamageThreshold() < this.EstimatedIncomingMissileDamage(allMissilesTargetingMe);
		}

		// Token: 0x06005F81 RID: 24449 RVA: 0x002D34D5 File Offset: 0x002D16D5
		private float EstimateShipKillDamageThreshold()
		{
			return this.baseHitPoints * 2f * (1f + (float)this.habModule.tier * 5f / 50f);
		}

		// Token: 0x06005F82 RID: 24450 RVA: 0x002D3504 File Offset: 0x002D1704
		public int EstimatedMaxProjectilesPointDefenseCanHandle()
		{
			int num = 0;
			foreach (IWeapon weapon in this.weapons)
			{
				if (weapon.currentFireMode is DefenseFireMode || weapon.currentFireMode is GuardianFireMode)
				{
					Weapon weapon2 = weapon as Weapon;
					num += Mathf.CeilToInt(60f / weapon2.weaponTemplate.cooldown_s);
				}
			}
			float num2 = 1f;
			return ((float)num * num2 * 1.2f).Round();
		}

		// Token: 0x06005F83 RID: 24451 RVA: 0x002D35A4 File Offset: 0x002D17A4
		public float EstimatedIncomingMissileDamage(List<MissileController> incomingMissiles)
		{
			float num = 0f;
			int num2 = this.EstimatedMaxProjectilesPointDefenseCanHandle();
			foreach (MissileController missileController in incomingMissiles.OrderBy<MissileController, float>((MissileController x) => TIUtilities.RandomFloatValue()))
			{
				if (num2 > 0)
				{
					num2--;
				}
				else
				{
					num += missileController.GetEstimatedDamage_Points();
				}
			}
			return num;
		}

		// Token: 0x06005F84 RID: 24452 RVA: 0x002D362C File Offset: 0x002D182C
		private List<MissileController> GetAllMissilesTargetingMe()
		{
			List<MissileController> list = new List<MissileController>();
			foreach (ProjectileController projectileController in GameControl.spaceCombat._projectiles.Values)
			{
				if (!(projectileController == null) && projectileController.isMissile && !(projectileController.projectileState.shootingFaction == base.faction) && !projectileController.hasHit && !projectileController.beenDestroyed)
				{
					MissileController missileController = projectileController as MissileController;
					if (missileController.target == this && TIUtilities.MovingTowardsTarget(base.position, this.velocityVector, projectileController.position, projectileController.velocityVector))
					{
						list.Add(missileController);
					}
				}
			}
			return list;
		}

		// Token: 0x06005F86 RID: 24454 RVA: 0x002D3710 File Offset: 0x002D1910
		Transform IDamageable.get_transform()
		{
			return base.transform;
		}

		// Token: 0x040043D8 RID: 17368
		private HabModuleController habModuleController;

		// Token: 0x040043D9 RID: 17369
		private HabModelController habModelController;

		// Token: 0x040043DB RID: 17371
		public float hitPoints;

		// Token: 0x040043DC RID: 17372
		public float armor;

		// Token: 0x040043DF RID: 17375
		private List<Collider> genericColliders;

		// Token: 0x040043E4 RID: 17380
		private readonly WaitForSeconds delay = new WaitForSeconds(1.9f);
	}
}

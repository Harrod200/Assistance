using System;
using FMOD.Studio;
using FMODUnity;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Ship;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200070E RID: 1806
	public class ShipWeaponVisController : MonoBehaviour
	{
		// Token: 0x170005DE RID: 1502
		// (get) Token: 0x06002AEB RID: 10987 RVA: 0x000E8BC7 File Offset: 0x000E6DC7
		[HideInInspector]
		public bool hasTarget
		{
			get
			{
				return (this.target != null && !this.target.isDestroyed) || this.stratLayerTarget != null;
			}
		}

		// Token: 0x06002AEC RID: 10988 RVA: 0x000E8BEC File Offset: 0x000E6DEC
		private void SetPrefabs()
		{
			if (this.createdPrefabs || this.weaponTemplate == null)
			{
				return;
			}
			TIBeamWeaponTemplate tibeamWeaponTemplate = this.weaponTemplate as TIBeamWeaponTemplate;
			TIGunTypeWeaponTemplate tigunTypeWeaponTemplate = this.weaponTemplate as TIGunTypeWeaponTemplate;
			TIMissileTemplate ref_missileWeapon = this.weaponTemplate.ref_missileWeapon;
			if (!string.IsNullOrEmpty(this.weaponTemplate.effectResource))
			{
				this.shotEffectPath = this.weaponTemplate.effectResource;
			}
			else if (tibeamWeaponTemplate != null)
			{
				this.shotEffectPath = TemplateManager.global.pathFallbackLaserVFX;
			}
			else if (tigunTypeWeaponTemplate != null)
			{
				this.shotEffectPath = TemplateManager.global.pathFallbackMuzzleFlashVFX;
			}
			if (!string.IsNullOrEmpty(this.shotEffectPath))
			{
				this.shotEffectInstance = TIVFXManager.GetVFX(this.shotEffectPath, this.firePoint.transform);
				this.shotEffectInstance.transform.localEulerAngles = Vector3.zero;
				this.shotEffectInstance.transform.localPosition = Vector3.zero;
				this.shotEffectInstance.transform.localScale = Vector3.one;
				this.shotEffectInstance.SetActive(false);
				this.beamController = this.shotEffectInstance.GetComponent<BeamWeaponController>();
			}
			if (tigunTypeWeaponTemplate != null)
			{
				if (!string.IsNullOrEmpty(tigunTypeWeaponTemplate.shotModelResource))
				{
					this.projectileResource = tigunTypeWeaponTemplate.shotModelResource;
				}
				else
				{
					this.projectileResource = TemplateManager.global.pathFallbackProjectileVFX;
				}
				this.projectilePrefab = GameControl.assetLoader.LoadAsset<GameObject>(this.projectileResource);
			}
			if (ref_missileWeapon != null)
			{
				if (!string.IsNullOrEmpty(ref_missileWeapon.shotModelResource))
				{
					this.projectileResource = ref_missileWeapon.shotModelResource;
				}
				else
				{
					this.projectileResource = TemplateManager.global.pathFallbackProjectileVFX;
				}
				this.projectilePrefab = GameControl.assetLoader.LoadAsset<GameObject>(this.projectileResource);
			}
			this.muzzleFlashes = base.gameObject.GetComponentsInChildren<ShipWeaponMuzzleFlashController>(true);
			for (int i = 0; i < this.muzzleFlashes.Length; i++)
			{
				this.muzzleFlashes[i].transform.parent.gameObject.SetActive(true);
				this.muzzleFlashes[i].transform.parent.gameObject.transform.localScale = new Vector3(50f, 50f, 50f);
			}
			this.createdPrefabs = true;
		}

		// Token: 0x06002AED RID: 10989 RVA: 0x000E8E08 File Offset: 0x000E7008
		private void InitializeCommon(bool createPrefabs = true)
		{
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			if (!string.IsNullOrEmpty(this.weaponTemplate.fireSoundFXResource))
			{
				this.eventPath = this.weaponTemplate.fireSoundFXResource;
			}
			this.turretTrainingRate_degsec = 270f / (float)this.weaponTemplate.internalSize;
			if (createPrefabs)
			{
				this.SetPrefabs();
			}
			this.RotateToFoward();
			this._initialBaseObjectRotation = new GameObject("Base Orientation").transform;
			this._initialBaseObjectRotation.SetParent(this.baseObjectTransform.parent, false);
			this._initialBaseObjectRotation.position = this.baseObjectTransform.position;
			this._initialBaseObjectRotation.localRotation = this.baseObjectTransform.localRotation;
			this.baseObject.transform.SetParent(this._initialBaseObjectRotation);
			this.baseObject.transform.localRotation = Quaternion.identity;
			this._originalBaseMaterial = this.baseObject.GetComponent<MeshRenderer>().material;
			this._originalWeaponMaterial = this.weaponObject.GetComponent<MeshRenderer>().material;
			this._destroyedMaterial = GameControl.assetLoader.LoadAsset<Material>("spacecombat/MAT_TEX_Weapon_Destroyed");
		}

		// Token: 0x06002AEE RID: 10990 RVA: 0x000E8F34 File Offset: 0x000E7134
		public void Initialize(CombatHabModuleController controller, GameObject baseObject, TIShipWeaponTemplate weaponTemplate, int slot)
		{
			this.myTransform = base.transform;
			this.baseObject = baseObject;
			this.baseObjectTransform = baseObject.transform;
			this.weaponObject = this.baseObjectTransform.GetChild(0).gameObject;
			this.weaponObjectTransform = this.weaponObject.transform;
			this.firePoint = this.weaponObjectTransform.GetChild(0).gameObject;
			this.firePointTransform = this.firePoint.transform;
			this.weaponTemplate = weaponTemplate;
			this.combatHabModuleController = controller;
			this.weaponModuleData = new ModuleDataEntry(weaponTemplate, slot);
			this.weaponCarrierState = controller.habModule;
			this.InitializeCommon(true);
		}

		// Token: 0x06002AEF RID: 10991 RVA: 0x000E8FE0 File Offset: 0x000E71E0
		public void Initialize(ShipVisController controller, ModuleDataEntry moduleData, bool forVisualizationOnly)
		{
			this.myTransform = base.transform;
			this.baseObjectTransform = this.baseObject.transform;
			this.weaponObjectTransform = this.weaponObject.transform;
			this.firePointTransform = this.firePoint.transform;
			this.shipVisController = controller;
			this.weaponModuleData = moduleData;
			this.weaponTemplate = this.weaponModuleData.moduleTemplate.ref_weapon;
			this.weaponCarrierState = controller.shipState;
			this.UIVisualizationOnly = forVisualizationOnly;
			this.InitializeCommon(false);
			if (this.weaponTemplate.bombardmentValue > 0f && !forVisualizationOnly)
			{
				GameControl.eventManager.AddListener<EndBombardment>(new EventManager.EventDelegate<EndBombardment>(this.EndFireMissionOrder), null, controller.shipState.fleet, false, false);
			}
		}

		// Token: 0x06002AF0 RID: 10992 RVA: 0x000E90A4 File Offset: 0x000E72A4
		public void OnDestroy()
		{
			TIShipWeaponTemplate tishipWeaponTemplate = this.weaponTemplate;
			if (tishipWeaponTemplate != null && tishipWeaponTemplate.bombardmentValue > 0f)
			{
				GameControl.eventManager.RemoveListener<EndBombardment>(new EventManager.EventDelegate<EndBombardment>(this.EndFireMissionOrder), null);
			}
			this.CeaseBeamFire();
			this.CeaseGunFire();
			this.eventInstance.Stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
			this.eventInstance.Release();
			GameControl.eventManager.RemoveListener<ShipDestroyedWeaponExplosion>(new EventManager.EventDelegate<ShipDestroyedWeaponExplosion>(this.OnWeaponDestroyedExplosion), null);
			if (this.shotEffectInstance != null)
			{
				TIVFXManager.ReturnVFX(this.shotEffectPath, this.shotEffectInstance);
			}
		}

		// Token: 0x06002AF1 RID: 10993 RVA: 0x000E9140 File Offset: 0x000E7340
		public void OnEnable()
		{
			TIShipWeaponTemplate tishipWeaponTemplate = this.weaponTemplate;
			if (tishipWeaponTemplate != null && tishipWeaponTemplate.bombardmentValue > 0f && !this.UIVisualizationOnly && this.shipVisController != null)
			{
				GameControl.eventManager.AddListener<FireMissionOrder>(new EventManager.EventDelegate<FireMissionOrder>(this.OnFireMissionOrder), null, this.shipVisController.shipState, false, false);
			}
			if (this.shipVisController != null && !this.shipVisController.UIVisualizationOnly)
			{
				GameControl.eventManager.AddListener<ShipDestroyedWeaponExplosion>(new EventManager.EventDelegate<ShipDestroyedWeaponExplosion>(this.OnWeaponDestroyedExplosion), null, this.shipVisController.shipState, false, false);
			}
		}

		// Token: 0x06002AF2 RID: 10994 RVA: 0x000E91E4 File Offset: 0x000E73E4
		public void OnDisable()
		{
			TIShipWeaponTemplate tishipWeaponTemplate = this.weaponTemplate;
			if (tishipWeaponTemplate != null && tishipWeaponTemplate.bombardmentValue > 0f && !this.UIVisualizationOnly)
			{
				GameControl.eventManager.RemoveListener<FireMissionOrder>(new EventManager.EventDelegate<FireMissionOrder>(this.OnFireMissionOrder), null);
			}
			GameControl.eventManager.RemoveListener<ShipDestroyedWeaponExplosion>(new EventManager.EventDelegate<ShipDestroyedWeaponExplosion>(this.OnWeaponDestroyedExplosion), null);
		}

		// Token: 0x06002AF3 RID: 10995 RVA: 0x000E9242 File Offset: 0x000E7442
		public void CreatePrefabs()
		{
			this.SetPrefabs();
		}

		// Token: 0x06002AF4 RID: 10996 RVA: 0x000E924A File Offset: 0x000E744A
		public void SetTarget(IDamageable target, Vector3 targetPosition)
		{
			this.target = target;
			this.targetPosition = targetPosition;
			this.stratLayerTarget = null;
		}

		// Token: 0x06002AF5 RID: 10997 RVA: 0x000E9264 File Offset: 0x000E7464
		private Vector3 GetTargetPosition(TIDateTime time = null)
		{
			if (this.target != null)
			{
				return this.targetPosition;
			}
			if (!TIGameState.Valid(this.stratLayerTarget))
			{
				return Vector3.forward;
			}
			if (this.stratLayerTarget.ref_habSite != null)
			{
				return this.targetParentSpaceBody.rotation * Quaternion.AngleAxis(this.targetLongitude, -Vector3.up) * Quaternion.AngleAxis(this.targetLatitude, -Vector3.right) * Vector3.forward * this.stratLayerTarget.ref_habSite.radius_gameUnits + ((time == null) ? this.targetParentSpaceBody.position : CameraManager.Singleton.ScaledPosition_DoNotTouchCache(this.stratLayerTarget.ref_spaceBody.GetGlobalPositionAtTime(time)));
			}
			return this.targetParentSpaceBody.rotation * Quaternion.AngleAxis(this.targetLongitude, -Vector3.up) * Quaternion.AngleAxis(this.targetLatitude, -Vector3.right) * Vector3.forward * this.stratLayerTarget.ref_spaceBody.controller.radius_gameUnits + ((time == null) ? this.targetParentSpaceBody.position : CameraManager.Singleton.ScaledPosition_DoNotTouchCache(this.stratLayerTarget.ref_spaceBody.GetGlobalPositionAtTime(time)));
		}

		// Token: 0x06002AF6 RID: 10998 RVA: 0x000E93E4 File Offset: 0x000E75E4
		public void SetStratLayerTarget(TIGameState target, Vector3 targetPosition, float targetLongitude, float targetLatitude, Transform parentSpaceBody)
		{
			this.target = null;
			this.stratLayerTarget = target;
			this.targetLongitude = targetLongitude;
			this.targetLatitude = targetLatitude;
			this.targetParentSpaceBody = parentSpaceBody;
		}

		// Token: 0x06002AF7 RID: 10999 RVA: 0x000E940B File Offset: 0x000E760B
		public void ClearStratLayerTarget()
		{
			this.stratLayerTarget = null;
			this.RotateToFoward();
		}

		// Token: 0x06002AF8 RID: 11000 RVA: 0x000E941A File Offset: 0x000E761A
		public void ClearTarget()
		{
			this.target = null;
		}

		// Token: 0x06002AF9 RID: 11001 RVA: 0x000E9424 File Offset: 0x000E7624
		public bool OnTarget()
		{
			if (this.hasTarget && !this.weaponTemplate.staticLauncher)
			{
				Vector3 vector = this.GetTargetPosition(null);
				return Vector3.Angle(this.weaponObjectTransform.forward, (vector - this.weaponObjectTransform.position).normalized) < 1f;
			}
			return true;
		}

		// Token: 0x06002AFA RID: 11002 RVA: 0x000E9480 File Offset: 0x000E7680
		public bool BombardmentTargetInLineOfSight(TIDateTime time)
		{
			return TISpaceShipState.BombardmentTargetInLineOfSight(this.weaponCarrierState.ref_shipCarrier(), this.weaponCarrierState.ref_shipCarrier().fleet.bombardmentTarget, time);
		}

		// Token: 0x06002AFB RID: 11003 RVA: 0x000E94A8 File Offset: 0x000E76A8
		private void RotateToFoward()
		{
			this.RotateToTarget(true);
		}

		// Token: 0x06002AFC RID: 11004 RVA: 0x000E94B4 File Offset: 0x000E76B4
		public void RotateToTarget(bool forceToForward = false)
		{
			TIShipWeaponTemplate tishipWeaponTemplate = this.weaponTemplate;
			if (tishipWeaponTemplate != null && !tishipWeaponTemplate.staticLauncher)
			{
				CombatWeaponCarrierState combatWeaponCarrierState = this.weaponCarrierState;
				if (combatWeaponCarrierState != null && combatWeaponCarrierState.WeaponIsOperable(this.weaponModuleData))
				{
					if (this.updateTime == null)
					{
						this.updateTime = new TIDateTime(this.gameTime.currentTime);
					}
					float num = (float)this.gameTime.currentTime.DifferenceInSeconds(this.updateTime);
					this.updateTime.CopyDateTime(this.gameTime.currentTime);
					if (!forceToForward)
					{
						Vector3 vector = this.GetTargetPosition(null);
						if (Vector3.Angle(this.baseObjectTransform.up, (vector - base.transform.position).normalized) <= this.weaponTemplate.pivotRange_deg)
						{
							Vector3 vector2 = this._initialBaseObjectRotation.rotation * Vector3.up;
							float num2 = Vector3.Dot(vector2, vector - this.baseObjectTransform.position);
							Vector3 normalized = (vector - vector2 * num2 - this.baseObjectTransform.position).normalized;
							float num3 = Vector3.Angle(normalized, this._initialBaseObjectRotation.rotation * Vector3.forward);
							if (Vector3.Dot(normalized, this._initialBaseObjectRotation.rotation * Vector3.right) < 0f)
							{
								num3 *= -1f;
							}
							if (!Mathf.Approximately(num3, 0f))
							{
								Quaternion quaternion = Quaternion.AngleAxis(num3, Vector3.up);
								if (Mathf.Abs(num3) > Mathf.Abs(this.turretTrainingRate_degsec) * num)
								{
									num3 = this.turretTrainingRate_degsec * num;
								}
								this.baseObjectTransform.localRotation = Quaternion.RotateTowards(this.baseObjectTransform.localRotation, quaternion, Mathf.Abs(num3));
							}
							Vector3 right = this.weaponObjectTransform.right;
							float num4 = Vector3.Dot(right, vector - this.baseObjectTransform.position);
							Vector3 normalized2 = (vector - right * num4 - this.weaponObjectTransform.position).normalized;
							float num5 = Vector3.SignedAngle(this.weaponObjectTransform.forward, normalized2, this.weaponObjectTransform.right);
							if (!Mathf.Approximately(num5, 0f))
							{
								Quaternion quaternion2 = this.weaponObjectTransform.localRotation * Quaternion.Euler(num5, 0f, 0f);
								if (Mathf.Abs(num5) > Mathf.Abs(this.turretTrainingRate_degsec) * num)
								{
									num5 = this.turretTrainingRate_degsec * num;
								}
								this.weaponObjectTransform.localRotation = Quaternion.RotateTowards(this.weaponObjectTransform.localRotation, quaternion2, Mathf.Abs(num5));
							}
						}
					}
				}
			}
		}

		// Token: 0x06002AFD RID: 11005 RVA: 0x000E9774 File Offset: 0x000E7974
		private bool Bombarding()
		{
			if (!TIGlobalValuesState.isSpaceCombatEnabled && this.shipVisController != null)
			{
				TISpaceShipState shipState = this.shipVisController.shipState;
				TIGameState tigameState;
				if (shipState == null)
				{
					tigameState = null;
				}
				else
				{
					TISpaceFleetState fleet = shipState.fleet;
					tigameState = ((fleet != null) ? fleet.bombardmentTarget : null);
				}
				return tigameState != null;
			}
			return false;
		}

		// Token: 0x06002AFE RID: 11006 RVA: 0x000E97C4 File Offset: 0x000E79C4
		public void Fire(bool truncated, TIDateTime time = null)
		{
			if (!this.createdPrefabs)
			{
				this.SetPrefabs();
			}
			if (!TIGlobalValuesState.isSpaceCombatEnabled && this.shipVisController != null)
			{
				if (this.shipVisController.fleetVisController == null)
				{
					return;
				}
				if (!this.shipVisController.gameObject.activeInHierarchy && this.beamController != null)
				{
					this.beamController.transform.SetParent(this.shipVisController.fleetVisController.container.transform, false);
				}
			}
			if (!TIGlobalValuesState.isSpaceCombatEnabled || this.OnTarget())
			{
				switch (this.weaponTemplate.weaponClass)
				{
				case WeaponClass.NavalGun:
				case WeaponClass.Magnetic:
				case WeaponClass.Plasma:
					this.shotEffectInstance.transform.localPosition = Vector3.zero;
					this.shotEffectInstance.SetActive(true);
					this.shotEffectInstance.transform.localRotation = this.firePoint.transform.localRotation;
					foreach (ShipWeaponMuzzleFlashController shipWeaponMuzzleFlashController in this.muzzleFlashes)
					{
						shipWeaponMuzzleFlashController.gameObject.SetActive(true);
						shipWeaponMuzzleFlashController.Flash();
					}
					break;
				case WeaponClass.Laser:
				case WeaponClass.Particle:
					if (this.beamController != null)
					{
						this.shotEffectInstance.SetActive(true);
						this.beamController.enabled = true;
						if (this.combatHabModuleController != null)
						{
							this.shotEffectInstance.transform.localScale = this.habBeamScaling;
						}
						else
						{
							this.shotEffectInstance.transform.localScale = Vector3.one;
						}
						LineRenderer component = this.shotEffectInstance.GetComponent<LineRenderer>();
						if (!this.Bombarding())
						{
							this.beamController.Initialize(this.target);
							this.ceaseBeamFireTime = new TIDateTime(this.gameTime.currentTime, 2.0);
						}
						else
						{
							this.beamController.Initialize(this.weaponCarrierState.GetTargetableState(), this.stratLayerTarget, time, LayerMask.NameToLayer("Solar System"));
							component.endWidth = 0f;
							base.Invoke("CeaseBeamFire", (this.gameTime.currentSpeedIndex <= 1) ? 2f : 1f);
						}
					}
					break;
				}
				this.eventPath = this.weaponTemplate.fireSoundFXResource;
				if (this.eventPath != null)
				{
					if (!this.eventInstance.isValid())
					{
						this.eventInstance = AudioManager.CreateFMODInstance(this.eventPath);
					}
					if (TIGlobalValuesState.isSpaceCombatEnabled)
					{
						this.eventInstance.SetDistance(AudioManager.GetCombatAudioMaxDistance(this.eventInstance), 1f);
					}
					if (TIGlobalValuesState.isSpaceCombatEnabled || (!TIGlobalValuesState.isSpaceCombatEnabled && this.shipVisController.gameObject.activeInHierarchy))
					{
						this.eventInstance.set3DAttributes(base.gameObject.transform.To3DAttributes());
						this.eventInstance.Play(base.gameObject);
					}
				}
			}
		}

		// Token: 0x06002AFF RID: 11007 RVA: 0x000E9AB0 File Offset: 0x000E7CB0
		public void CeaseBeamFire()
		{
			if (this.beamController != null)
			{
				if (this.firePoint != null)
				{
					this.beamController.transform.SetParent(this.firePoint.transform, false);
				}
				this.beamController.DisableLaser();
			}
		}

		// Token: 0x06002B00 RID: 11008 RVA: 0x000E9B00 File Offset: 0x000E7D00
		public void CeaseGunFire()
		{
			if (this.shotEffectInstance != null)
			{
				this.shotEffectInstance.SetActive(false);
			}
		}

		// Token: 0x06002B01 RID: 11009 RVA: 0x000E9B1C File Offset: 0x000E7D1C
		private void OnFireMissionOrder(FireMissionOrder e)
		{
			if (e.doNotVisualize)
			{
				return;
			}
			if (e.target.ref_naturalSpaceObject == null || e.target.ref_naturalSpaceObject.controller == null || e.target.ref_naturalSpaceObject.controller.modelLink == null || !e.target.ref_naturalSpaceObject.controller.modelLink.activeInHierarchy)
			{
				return;
			}
			if (e.ship == this.shipVisController.shipState && e.moduleData.slotIndex == this.weaponModuleData.slotIndex)
			{
				this.SetStratLayerTarget(e.target, e.targetDisplayPosition, e.targetLongitude, e.targetLatitude, e.parentSpaceBody);
				this.Fire(false, e.time);
			}
		}

		// Token: 0x06002B02 RID: 11010 RVA: 0x000E9BF8 File Offset: 0x000E7DF8
		private void EndFireMissionOrder(EndBombardment e)
		{
			this.ClearStratLayerTarget();
			this.CeaseBeamFire();
			this.CeaseGunFire();
		}

		// Token: 0x06002B03 RID: 11011 RVA: 0x000E9C0C File Offset: 0x000E7E0C
		private void OnWeaponDestroyedExplosion(ShipDestroyedWeaponExplosion e)
		{
			if ((TIGlobalValuesState.isSpaceCombatEnabled || (!this.UIVisualizationOnly && this.shipVisController != null && this.shipVisController.gameObject.activeInHierarchy)) && TIGameState.Valid(e.ship))
			{
				ModuleDataEntry partExploding = e.partExploding;
				int? num = ((partExploding != null) ? new int?(partExploding.slotIndex) : null);
				ModuleDataEntry moduleDataEntry = this.weaponModuleData;
				int? num2 = ((moduleDataEntry != null) ? new int?(moduleDataEntry.slotIndex) : null);
				if ((num.GetValueOrDefault() == num2.GetValueOrDefault()) & (num != null == (num2 != null)))
				{
					ModuleDataEntry partExploding2 = e.partExploding;
					string text = ((partExploding2 != null) ? partExploding2.moduleTemplateName : null);
					ModuleDataEntry moduleDataEntry2 = this.weaponModuleData;
					if (text == ((moduleDataEntry2 != null) ? moduleDataEntry2.moduleTemplateName : null))
					{
						GameObject gameObject = GameControl.assetLoader.LoadAsset<GameObject>(TemplateManager.global.pathWeaponExplosion);
						this.weaponExplosionInstance = global::UnityEngine.Object.Instantiate<GameObject>(gameObject, this.weaponObjectTransform.position, new Quaternion(0f, 0f, 0f, 1f), base.transform);
						this._originalWeaponMaterial = this.weaponObject.GetComponent<MeshRenderer>().material;
						this._originalBaseMaterial = this.weaponObject.GetComponent<MeshRenderer>().material;
						this.weaponObject.GetComponent<MeshRenderer>().material = this._destroyedMaterial;
						this.baseObject.GetComponent<MeshRenderer>().material = this._destroyedMaterial;
					}
				}
			}
		}

		// Token: 0x06002B04 RID: 11012 RVA: 0x000E9D98 File Offset: 0x000E7F98
		public void OnWeaponRepaired()
		{
			this.weaponObject.GetComponent<MeshRenderer>().material = this._originalWeaponMaterial;
			this.baseObject.GetComponent<MeshRenderer>().material = this._originalBaseMaterial;
		}

		// Token: 0x06002B05 RID: 11013 RVA: 0x000E9DC6 File Offset: 0x000E7FC6
		public void OnGameTimePlay()
		{
			if (this.weaponExplosionInstance != null)
			{
				this.weaponExplosionInstance.GetComponent<ParticleSystem>().Play();
			}
		}

		// Token: 0x06002B06 RID: 11014 RVA: 0x000E9DE6 File Offset: 0x000E7FE6
		public void OnGameTimePause()
		{
			if (this.weaponExplosionInstance != null)
			{
				this.weaponExplosionInstance.GetComponent<ParticleSystem>().Pause();
			}
		}

		// Token: 0x06002B07 RID: 11015 RVA: 0x000E9E08 File Offset: 0x000E8008
		public void Update()
		{
			if (!this.UIVisualizationOnly && (TIGlobalValuesState.isSpaceCombatEnabled || this.Bombarding()))
			{
				if (this.gameTime == null)
				{
					this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
				}
				if (this.gameTime.currentSpeed == 0f)
				{
					return;
				}
				if (this.hasTarget)
				{
					this.RotateToTarget(false);
				}
				if (this.gameTime.currentTime > this.ceaseBeamFireTime && this.beamController.enabled)
				{
					this.CeaseBeamFire();
				}
			}
		}

		// Token: 0x040020D9 RID: 8409
		protected GameTimeManager gameTime;

		// Token: 0x040020DA RID: 8410
		public GameObject baseObject;

		// Token: 0x040020DB RID: 8411
		public Transform baseObjectTransform;

		// Token: 0x040020DC RID: 8412
		public GameObject weaponObject;

		// Token: 0x040020DD RID: 8413
		public Transform weaponObjectTransform;

		// Token: 0x040020DE RID: 8414
		public GameObject firePoint;

		// Token: 0x040020DF RID: 8415
		public Transform firePointTransform;

		// Token: 0x040020E0 RID: 8416
		public Transform myTransform;

		// Token: 0x040020E1 RID: 8417
		[HideInInspector]
		public ModuleDataEntry weaponModuleData;

		// Token: 0x040020E2 RID: 8418
		[HideInInspector]
		public TIShipWeaponTemplate weaponTemplate;

		// Token: 0x040020E3 RID: 8419
		[HideInInspector]
		public ShipVisController shipVisController;

		// Token: 0x040020E4 RID: 8420
		[HideInInspector]
		public CombatHabModuleController combatHabModuleController;

		// Token: 0x040020E5 RID: 8421
		[HideInInspector]
		private CombatWeaponCarrierState weaponCarrierState;

		// Token: 0x040020E6 RID: 8422
		[HideInInspector]
		public IDamageable target;

		// Token: 0x040020E7 RID: 8423
		[HideInInspector]
		public TIGameState stratLayerTarget;

		// Token: 0x040020E8 RID: 8424
		private Vector3 targetPosition;

		// Token: 0x040020E9 RID: 8425
		private float targetLongitude;

		// Token: 0x040020EA RID: 8426
		private float targetLatitude;

		// Token: 0x040020EB RID: 8427
		private Transform targetParentSpaceBody;

		// Token: 0x040020EC RID: 8428
		private EventInstance eventInstance;

		// Token: 0x040020ED RID: 8429
		protected string eventPath;

		// Token: 0x040020EE RID: 8430
		protected GameObject shotEffectInstance;

		// Token: 0x040020EF RID: 8431
		public GameObject weaponExplosionInstance;

		// Token: 0x040020F0 RID: 8432
		private TIDateTime updateTime;

		// Token: 0x040020F1 RID: 8433
		private bool UIVisualizationOnly;

		// Token: 0x040020F2 RID: 8434
		[Header("Beam Weapons")]
		protected BeamWeaponController beamController;

		// Token: 0x040020F3 RID: 8435
		protected RaycastHit shotData;

		// Token: 0x040020F4 RID: 8436
		[Header("Projectile Weapons")]
		protected ShipWeaponMuzzleFlashController[] muzzleFlashes;

		// Token: 0x040020F5 RID: 8437
		[HideInInspector]
		public string projectileResource;

		// Token: 0x040020F6 RID: 8438
		[HideInInspector]
		public GameObject projectilePrefab;

		// Token: 0x040020F7 RID: 8439
		private const float baselineTurretTrainingRate_degSec = 270f;

		// Token: 0x040020F8 RID: 8440
		private float turretTrainingRate_degsec = 270f;

		// Token: 0x040020F9 RID: 8441
		private Transform _initialBaseObjectRotation;

		// Token: 0x040020FA RID: 8442
		private Material _destroyedMaterial;

		// Token: 0x040020FB RID: 8443
		private Material _originalBaseMaterial;

		// Token: 0x040020FC RID: 8444
		private Material _originalWeaponMaterial;

		// Token: 0x040020FD RID: 8445
		private bool createdPrefabs;

		// Token: 0x040020FE RID: 8446
		private string shotEffectPath = "";

		// Token: 0x040020FF RID: 8447
		private const string CeaseBeamFireStr = "CeaseBeamFire";

		// Token: 0x04002100 RID: 8448
		private TIDateTime ceaseBeamFireTime;

		// Token: 0x04002101 RID: 8449
		private const float combatBeamOnTime = 2f;

		// Token: 0x04002102 RID: 8450
		private readonly Vector3 habBeamScaling = new Vector3(0.1f, 0.1f, 0.1f);

		// Token: 0x04002103 RID: 8451
		public GameObject TargetIndicator;

		// Token: 0x04002104 RID: 8452
		public GameObject UpIndicator;

		// Token: 0x04002105 RID: 8453
		public GameObject InitialBaseForwardIndicator;

		// Token: 0x04002106 RID: 8454
		public GameObject BaseForwardIndicator;

		// Token: 0x04002107 RID: 8455
		public GameObject WeaponForwardIndicator;

		// Token: 0x04002108 RID: 8456
		public bool ShowDebugVisualization;
	}
}

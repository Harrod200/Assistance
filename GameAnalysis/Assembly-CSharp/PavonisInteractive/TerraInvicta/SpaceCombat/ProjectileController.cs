using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Jobs;
using PavonisInteractive.TerraInvicta.Ship;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009FD RID: 2557
	public abstract class ProjectileController : MonoBehaviour, IDamageable
	{
		// Token: 0x170010D5 RID: 4309
		// (get) Token: 0x060061FB RID: 25083 RVA: 0x002DF8AD File Offset: 0x002DDAAD
		public virtual IDamageableType damageableType
		{
			get
			{
				return IDamageableType.BallisticProjectile;
			}
		}

		// Token: 0x170010D6 RID: 4310
		// (get) Token: 0x060061FC RID: 25084 RVA: 0x002DF8B0 File Offset: 0x002DDAB0
		// (set) Token: 0x060061FD RID: 25085 RVA: 0x002DF8B8 File Offset: 0x002DDAB8
		public TISpaceCombatProjectileState projectileState { get; private set; }

		// Token: 0x170010D7 RID: 4311
		// (get) Token: 0x060061FE RID: 25086 RVA: 0x002DF8C1 File Offset: 0x002DDAC1
		public CombatTargetableState combatTargetableState
		{
			get
			{
				return this.projectileState;
			}
		}

		// Token: 0x170010D8 RID: 4312
		// (get) Token: 0x060061FF RID: 25087 RVA: 0x002DF8C9 File Offset: 0x002DDAC9
		// (set) Token: 0x06006200 RID: 25088 RVA: 0x002DF8D1 File Offset: 0x002DDAD1
		public Transform projectileTransform { get; protected set; }

		// Token: 0x170010D9 RID: 4313
		// (get) Token: 0x06006201 RID: 25089 RVA: 0x002DF8DA File Offset: 0x002DDADA
		// (set) Token: 0x06006202 RID: 25090 RVA: 0x002DF8E2 File Offset: 0x002DDAE2
		public List<Collider> hitColliders { get; protected set; }

		// Token: 0x170010DA RID: 4314
		// (get) Token: 0x06006203 RID: 25091 RVA: 0x002DF8EB File Offset: 0x002DDAEB
		// (set) Token: 0x06006204 RID: 25092 RVA: 0x002DF8F3 File Offset: 0x002DDAF3
		public ShipWeaponVisController weaponController { get; protected set; }

		// Token: 0x170010DB RID: 4315
		// (get) Token: 0x06006205 RID: 25093 RVA: 0x002DF8FC File Offset: 0x002DDAFC
		// (set) Token: 0x06006206 RID: 25094 RVA: 0x002DF904 File Offset: 0x002DDB04
		public bool clearedLauncher { get; protected set; }

		// Token: 0x170010DC RID: 4316
		// (get) Token: 0x06006207 RID: 25095 RVA: 0x002DF90D File Offset: 0x002DDB0D
		// (set) Token: 0x06006208 RID: 25096 RVA: 0x002DF915 File Offset: 0x002DDB15
		public bool hasHit { get; protected set; }

		// Token: 0x170010DD RID: 4317
		// (get) Token: 0x06006209 RID: 25097 RVA: 0x002DF91E File Offset: 0x002DDB1E
		// (set) Token: 0x0600620A RID: 25098 RVA: 0x002DF926 File Offset: 0x002DDB26
		public bool beenDestroyed { get; protected set; }

		// Token: 0x170010DE RID: 4318
		// (get) Token: 0x0600620B RID: 25099 RVA: 0x002DF92F File Offset: 0x002DDB2F
		// (set) Token: 0x0600620C RID: 25100 RVA: 0x002DF937 File Offset: 0x002DDB37
		public Vector3 velocityVector { get; protected set; }

		// Token: 0x170010DF RID: 4319
		// (get) Token: 0x0600620D RID: 25101 RVA: 0x002DF940 File Offset: 0x002DDB40
		public Vector3 accelerationVector
		{
			get
			{
				return this.v3_accelerationVector;
			}
		}

		// Token: 0x170010E0 RID: 4320
		// (get) Token: 0x0600620E RID: 25102 RVA: 0x002DF948 File Offset: 0x002DDB48
		public Vector3 accelerationVector_kps
		{
			get
			{
				return this.v3_accelerationVector_kps;
			}
		}

		// Token: 0x170010E1 RID: 4321
		// (get) Token: 0x0600620F RID: 25103 RVA: 0x002DF950 File Offset: 0x002DDB50
		public Vector3 v3_accelerationVector_kps
		{
			get
			{
				return this.v3_accelerationVector / 0.05f;
			}
		}

		// Token: 0x06006210 RID: 25104 RVA: 0x002DF962 File Offset: 0x002DDB62
		public float GetCrossSectionalArea_m2(float angle)
		{
			return this.projectileState.CrossSectionalArea_m2(angle);
		}

		// Token: 0x170010E2 RID: 4322
		// (get) Token: 0x06006211 RID: 25105 RVA: 0x002DF970 File Offset: 0x002DDB70
		protected TIProjectileWeaponTemplate weaponTemplate
		{
			get
			{
				return this.projectileState.originWeapon;
			}
		}

		// Token: 0x170010E3 RID: 4323
		// (get) Token: 0x06006212 RID: 25106 RVA: 0x002DF97D File Offset: 0x002DDB7D
		public float warheadMass_kg
		{
			get
			{
				return this.weaponTemplate.warheadMass_kg;
			}
		}

		// Token: 0x06006213 RID: 25107 RVA: 0x002DF98A File Offset: 0x002DDB8A
		public Vector3 positionAtTime(DateTime currentTime)
		{
			return this.projectileState.ProjectedLinearPositionAtTime_FromCurrent(currentTime);
		}

		// Token: 0x170010E4 RID: 4324
		// (get) Token: 0x06006214 RID: 25108 RVA: 0x002DF998 File Offset: 0x002DDB98
		public TISpaceCombatProjectileState ref_projectile
		{
			get
			{
				return this.projectileState;
			}
		}

		// Token: 0x170010E5 RID: 4325
		// (get) Token: 0x06006215 RID: 25109 RVA: 0x002DF9A0 File Offset: 0x002DDBA0
		Vector3 IDamageable.velocityVector
		{
			get
			{
				return this.velocityVector;
			}
		}

		// Token: 0x170010E6 RID: 4326
		// (get) Token: 0x06006216 RID: 25110 RVA: 0x002DF9A8 File Offset: 0x002DDBA8
		Transform IDamageable.damageableTransform
		{
			get
			{
				return this.projectileTransform;
			}
		}

		// Token: 0x170010E7 RID: 4327
		// (get) Token: 0x06006217 RID: 25111 RVA: 0x002DF9B0 File Offset: 0x002DDBB0
		public Vector3 velocityVector_kps
		{
			get
			{
				return this.projectileState.velocityVector_kps;
			}
		}

		// Token: 0x170010E8 RID: 4328
		// (get) Token: 0x06006218 RID: 25112 RVA: 0x002DF9BD File Offset: 0x002DDBBD
		public Vector3 position
		{
			get
			{
				return this.projectileTransform.position;
			}
		}

		// Token: 0x170010E9 RID: 4329
		// (get) Token: 0x06006219 RID: 25113 RVA: 0x002DF9CA File Offset: 0x002DDBCA
		public bool isDestroyed
		{
			get
			{
				return this.beenDestroyed || this.hasHit;
			}
		}

		// Token: 0x170010EA RID: 4330
		// (get) Token: 0x0600621A RID: 25114 RVA: 0x002DF9DC File Offset: 0x002DDBDC
		public virtual bool isMissile
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600621B RID: 25115
		public abstract void Fire(Vector3 originPosition, Vector3 targetPosition, IDamageable target = null);

		// Token: 0x0600621C RID: 25116 RVA: 0x002DF9E0 File Offset: 0x002DDBE0
		public virtual bool ThreateningEnemyCombatant(List<CombatantController> combatantList)
		{
			foreach (CombatantController combatantController in combatantList)
			{
				if (this.projectileState.WillHitSphere(combatantController.position, combatantController.velocityVector, combatantController.damageableType, combatantController))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600621D RID: 25117 RVA: 0x002DFA50 File Offset: 0x002DDC50
		public void Awake()
		{
			this._collisionMask = 1 << LayerMask.NameToLayer("HurtBox");
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			this.projectileParticleSystem = base.gameObject.GetComponentInChildren<ParticleSystem>();
			if (this.projectileParticleSystem)
			{
				this.projectileParticleTransform = this.projectileParticleSystem.transform;
				this.projectileParticleDefaultScale = this.projectileParticleTransform.localScale;
			}
			if (this.hitColliders == null)
			{
				this.hitColliders = new List<Collider>();
			}
			else
			{
				this.hitColliders.Clear();
			}
			if (this.projectileCollider != null)
			{
				this.hitColliders.Add(this.projectileCollider);
				return;
			}
			GameObject gameObject = new GameObject("Hitbox");
			this.projectileCollider = gameObject.AddComponent<BoxCollider>();
			(this.projectileCollider as BoxCollider).size = new Vector3(1f, 1f, 1f * GameControl.spaceCombat.projectileScalingFactor);
			this.hitColliders.Add(this.projectileCollider);
			gameObject.transform.SetParent(base.transform, false);
		}

		// Token: 0x0600621E RID: 25118 RVA: 0x002DFB70 File Offset: 0x002DDD70
		private void Update()
		{
			if (this.gameTime.currentSpeed == 0f && !this.isPaused)
			{
				this.OnPause();
			}
			else if (this.isPaused && this.gameTime.currentSpeed != 0f)
			{
				this.OnUnpause();
			}
			if (this.gameTime.Paused)
			{
				return;
			}
			if (this.beenDestroyed)
			{
				this.timeUntilDestroy -= Time.deltaTime;
				if (this.timeUntilDestroy <= 0f)
				{
					this.Destroy();
				}
			}
		}

		// Token: 0x0600621F RID: 25119
		public abstract void UpdateController();

		// Token: 0x06006220 RID: 25120
		protected abstract void OnPause();

		// Token: 0x06006221 RID: 25121
		protected abstract void OnUnpause();

		// Token: 0x06006222 RID: 25122 RVA: 0x002DFBFC File Offset: 0x002DDDFC
		public void Initialize(ProjectileJobContainer container, ShipWeaponVisController weaponController, TISpaceCombatProjectileState projectileState)
		{
			this._container = container;
			this.projectileTransform = base.transform;
			this.weaponController = weaponController;
			TIProjectileWeaponTemplate ref_projectileWeapon = weaponController.weaponTemplate.ref_projectileWeapon;
			this.SetImpactPrefab(ref_projectileWeapon);
			this.SetDestructionPrefab((ref_projectileWeapon.ammoMass_kg < 10f) ? "spaceCombat/TinyExplosion" : "spaceCombat/SmallExplosion");
			if (ref_projectileWeapon.isMissileWeapon)
			{
				projectileState.deltaV = ref_projectileWeapon.ref_missileWeapon.deltaV_kps;
			}
			this.projectileState = projectileState;
		}

		// Token: 0x06006223 RID: 25123 RVA: 0x002DFC78 File Offset: 0x002DDE78
		private void SetImpactPrefab(TIProjectileWeaponTemplate weaponTemplate)
		{
			string impactVisualFXResource = weaponTemplate.impactVisualFXResource;
			if (this.currentImpactPrefabResource != impactVisualFXResource)
			{
				GameObject gameObject = GameControl.assetLoader.LoadAsset<GameObject>(impactVisualFXResource);
				if (gameObject != null)
				{
					this.ReturnImpactVFXObject();
					this.currentImpactPrefabResource = impactVisualFXResource;
					this.impactObject = TIVFXManager.GetVFX(impactVisualFXResource, base.transform);
					this.impactParticleSystem = this.impactObject.GetComponent<ParticleSystem>();
					this.impactObject.transform.localPosition = Vector3.zero;
					this.impactObject.transform.localScale = GameControl.spaceCombat.projectileScalingFactor * 20f * gameObject.transform.localScale;
					if (weaponTemplate.isMissileWeapon)
					{
						WarheadClass warheadClass = weaponTemplate.ref_missileWeapon.warheadClass;
						switch (warheadClass)
						{
						case WarheadClass.Nuclear:
						case WarheadClass.Antimatter:
						{
							float num = weaponTemplate.ref_missileWeapon.RangeAtOneDamage_km(warheadClass);
							float num2 = SpaceCombatManager.km_to_scale(num);
							num2 /= this.projectileTransform.localScale.x;
							num2 *= 0.8f;
							this.impactObject.transform.localScale = new Vector3(num2, num2, num2);
							break;
						}
						case WarheadClass.ShapedNuclear:
						{
							float num = weaponTemplate.ref_missileWeapon.RangeAtOneDamage_km(warheadClass);
							num /= this.projectileTransform.localScale.x;
							num *= 2f;
							this.impactObject.GetComponent<CasabaSizeByShape>().SetEffectRange(num, weaponTemplate.ref_missileWeapon.shapedChargeAngle);
							break;
						}
						}
					}
				}
			}
			this.impactObject.SetActive(false);
			string impactSoundFXResource = weaponTemplate.impactSoundFXResource;
			if (this.currentImpactSoundResource != impactSoundFXResource)
			{
				if (this.eventInstance.isValid())
				{
					this.eventInstance.Stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
					this.eventInstance.Release();
				}
				this.eventInstance = AudioManager.CreateFMODInstance(impactSoundFXResource);
				if (TIGlobalValuesState.isSpaceCombatEnabled)
				{
					this.eventInstance.SetDistance(AudioManager.GetCombatAudioMaxDistance(this.eventInstance), 1f);
					this.eventInstance.set3DAttributes(base.gameObject.transform.To3DAttributes());
				}
			}
		}

		// Token: 0x06006224 RID: 25124 RVA: 0x002DFE8C File Offset: 0x002DE08C
		private void SetDestructionPrefab(string resource)
		{
			if (this.currentDestructionPrefabResource != resource)
			{
				GameObject gameObject = GameControl.assetLoader.LoadAsset<GameObject>(resource);
				if (gameObject != null)
				{
					this.ReturnDestructionVFXObject();
					this.currentDestructionPrefabResource = resource;
					this.destructionObject = TIVFXManager.GetVFX(resource, base.transform);
					this.destructionParticleSystem = this.destructionObject.GetComponent<ParticleSystem>();
					this.destructionObject.transform.localPosition = Vector3.zero;
					this.destructionObject.transform.localScale = gameObject.transform.localScale;
					this.destructionParticleSystem.transform.localScale = 2f * GameControl.spaceCombat.projectileScalingFactor * Vector3.one;
					this.destructionObject.SetActive(false);
				}
			}
		}

		// Token: 0x06006225 RID: 25125 RVA: 0x002DFF58 File Offset: 0x002DE158
		private void ReturnImpactVFXObject()
		{
			if (!string.IsNullOrEmpty(this.currentImpactPrefabResource) && this.impactObject != null)
			{
				TIVFXManager.ReturnVFX(this.currentImpactPrefabResource, this.impactObject);
			}
		}

		// Token: 0x06006226 RID: 25126 RVA: 0x002DFF86 File Offset: 0x002DE186
		private void ReturnDestructionVFXObject()
		{
			if (!string.IsNullOrEmpty(this.currentDestructionPrefabResource) && this.destructionObject != null)
			{
				TIVFXManager.ReturnVFX(this.currentDestructionPrefabResource, this.destructionObject);
			}
		}

		// Token: 0x06006227 RID: 25127 RVA: 0x002DFFB4 File Offset: 0x002DE1B4
		private void OnDisable()
		{
		}

		// Token: 0x06006228 RID: 25128 RVA: 0x002DFFB6 File Offset: 0x002DE1B6
		private void OnDestroy()
		{
			if (this.eventInstance.isValid())
			{
				this.eventInstance.Stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
				this.eventInstance.Release();
			}
			this.ReturnImpactVFXObject();
			this.ReturnDestructionVFXObject();
		}

		// Token: 0x06006229 RID: 25129 RVA: 0x002DFFEC File Offset: 0x002DE1EC
		private void Destroy()
		{
			if (this.eventInstance.isValid())
			{
				this.eventInstance.Stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
				this.eventInstance.Release();
			}
			base.gameObject.SetActive(false);
			this.destructionObject.SetActive(false);
			this.projectileState.OnDestroyed();
		}

		// Token: 0x0600622A RID: 25130 RVA: 0x002E0044 File Offset: 0x002DE244
		public virtual float ApplyDamage(DamageSource source)
		{
			WeaponClass weaponClass = this.projectileState.originWeapon.weaponClass;
			if (weaponClass != WeaponClass.NavalGun)
			{
				if (weaponClass != WeaponClass.Magnetic)
				{
					this.Destruct(false);
				}
				else
				{
					TIShipWeaponTemplate weapon = source.damage.weapon;
					if (weapon != null && weapon.isParticleWeapon)
					{
						this.projectileState.massDamage_kg += Mathf.Max(0f, source.damage.amount * source.damage.weapon.ref_particleWeapon.heatFraction * 10f);
						base.GetComponent<MagBulletParticleController>().UpdateMass(this.projectileState.effectiveMass_kg, this.warheadMass_kg);
						if (this.projectileState.effectiveMass_kg <= 0f)
						{
							this.Destruct(false);
						}
						else if (TIUtilities.RandomFloatValue() < source.damage.amount / 100f)
						{
							this.Destruct(false);
						}
					}
					else
					{
						this.projectileState.massDamage_kg += Mathf.Max(new float[]
						{
							0f,
							source.damage.amount * 10f,
							(float)(source.damage.shreddingAmount * 10)
						});
						base.GetComponent<MagBulletParticleController>().UpdateMass(this.projectileState.effectiveMass_kg, this.warheadMass_kg);
						if (this.projectileState.effectiveMass_kg <= 0f)
						{
							this.Destruct(false);
						}
					}
				}
			}
			else
			{
				TIShipWeaponTemplate weapon2 = source.damage.weapon;
				if (weapon2 != null && weapon2.isParticleWeapon)
				{
					if (source.damage.amount * source.damage.weapon.ref_particleWeapon.heatFraction >= 1f)
					{
						this.Destruct(false);
					}
				}
				else
				{
					this.Destruct(false);
				}
			}
			return source.damage.amount;
		}

		// Token: 0x0600622B RID: 25131 RVA: 0x002E0234 File Offset: 0x002DE434
		public virtual void Destruct(bool isAlreadyRemovedFromLiveProjectiles = false)
		{
			this._container.RemoveProjectile(this.projectileTransform);
			if (!isAlreadyRemovedFromLiveProjectiles)
			{
				this.projectileState.RemoveFromLiveProjectiles();
			}
			this.beenDestroyed = true;
			this.destructionObject.SetActive(true);
			if (this.projectileParticleSystem != null)
			{
				this.projectileParticleSystem.Stop();
			}
			this.timeUntilDestroy = Mathf.Max(this.destructionParticleSystem.main.duration, this.impactParticleSystem.main.duration);
		}

		// Token: 0x0600622C RID: 25132 RVA: 0x002E02C0 File Offset: 0x002DE4C0
		public void Impact(Vector3 position, Vector3 eulerAngles)
		{
			if (this.impactObject != null)
			{
				if (this.eventInstance.isValid())
				{
					RuntimeManager.AttachInstanceToGameObject(this.eventInstance, this.impactObject.GetComponent<Transform>(), this.impactObject.GetComponent<Rigidbody>());
					this.eventInstance.Play();
					this.eventInstance.Release();
				}
				this.impactObject.transform.position = position;
				this.impactObject.transform.forward = eulerAngles;
				this.impactObject.SetActive(true);
			}
		}

		// Token: 0x0600622D RID: 25133 RVA: 0x002E0350 File Offset: 0x002DE550
		protected RaycastHit FilterRaycastsForHits(List<RaycastHit> raycastHitsList, out IDamageable hitDamageable)
		{
			float num = float.PositiveInfinity;
			RaycastHit raycastHit = default(RaycastHit);
			hitDamageable = null;
			foreach (RaycastHit raycastHit2 in raycastHitsList)
			{
				GameObject gameObject = raycastHit2.collider.gameObject;
				CombatantController componentInParent = gameObject.GetComponentInParent<CombatantController>();
				if (raycastHit2.distance <= num)
				{
					if (componentInParent != null)
					{
						if (!(componentInParent.faction == this.projectileState.shootingFaction) && (!(componentInParent.ref_shipController != null) || !componentInParent.ref_shipController.ModelController.mainHullDestroyed) && (!(componentInParent.ref_habModuleController != null) || !componentInParent.ref_habModuleController.destructionTriggered))
						{
							num = raycastHit2.distance;
							raycastHit = raycastHit2;
							hitDamageable = componentInParent;
						}
					}
					else
					{
						ProjectileController componentInParent2 = gameObject.GetComponentInParent<ProjectileController>();
						if (!(componentInParent2 != null) || (!(this.projectileState.shootingFaction == componentInParent2.projectileState.shootingFaction) && !componentInParent2.beenDestroyed))
						{
							num = raycastHit2.distance;
							raycastHit = raycastHit2;
							hitDamageable = componentInParent2;
						}
					}
				}
			}
			return raycastHit;
		}

		// Token: 0x0600622F RID: 25135 RVA: 0x002E04A3 File Offset: 0x002DE6A3
		Transform IDamageable.get_transform()
		{
			return base.transform;
		}

		// Token: 0x040044DD RID: 17629
		protected LayerMask _collisionMask;

		// Token: 0x040044DE RID: 17630
		protected GameTimeManager gameTime;

		// Token: 0x040044DF RID: 17631
		public Collider projectileCollider;

		// Token: 0x040044E2 RID: 17634
		protected ParticleSystem projectileParticleSystem;

		// Token: 0x040044E3 RID: 17635
		protected Transform projectileParticleTransform;

		// Token: 0x040044E4 RID: 17636
		protected Vector3 projectileParticleDefaultScale;

		// Token: 0x040044E5 RID: 17637
		protected string currentImpactPrefabResource;

		// Token: 0x040044E6 RID: 17638
		protected string currentDestructionPrefabResource;

		// Token: 0x040044E7 RID: 17639
		protected string currentImpactSoundResource;

		// Token: 0x040044E8 RID: 17640
		protected GameObject impactObject;

		// Token: 0x040044E9 RID: 17641
		protected ParticleSystem impactParticleSystem;

		// Token: 0x040044EA RID: 17642
		protected EventInstance eventInstance;

		// Token: 0x040044EB RID: 17643
		protected GameObject destructionObject;

		// Token: 0x040044EC RID: 17644
		protected ParticleSystem destructionParticleSystem;

		// Token: 0x040044ED RID: 17645
		protected float timeUntilDestroy;

		// Token: 0x040044EE RID: 17646
		protected float DestroyTime;

		// Token: 0x040044EF RID: 17647
		protected ProjectileJobContainer _container;

		// Token: 0x040044F0 RID: 17648
		protected RaycastHit[] raycastHitsArray;

		// Token: 0x040044F4 RID: 17652
		protected Vector3 originPosition;

		// Token: 0x040044F6 RID: 17654
		public Vector3 v3_accelerationVector = Vector3.zero;

		// Token: 0x040044F7 RID: 17655
		protected const float CLEAR_LAUNCHER_DELAY_s = 5f;

		// Token: 0x040044F8 RID: 17656
		public bool isPaused;
	}
}

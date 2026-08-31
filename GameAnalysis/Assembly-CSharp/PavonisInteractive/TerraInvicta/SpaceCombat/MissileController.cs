using System;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Jobs;
using PavonisInteractive.TerraInvicta.Ship;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009E2 RID: 2530
	public class MissileController : ProjectileController
	{
		// Token: 0x17001076 RID: 4214
		// (get) Token: 0x06005F88 RID: 24456 RVA: 0x002D3777 File Offset: 0x002D1977
		// (set) Token: 0x06005F89 RID: 24457 RVA: 0x002D377F File Offset: 0x002D197F
		public IDamageable target { get; private set; }

		// Token: 0x17001077 RID: 4215
		// (get) Token: 0x06005F8A RID: 24458 RVA: 0x002D3788 File Offset: 0x002D1988
		public override IDamageableType damageableType
		{
			get
			{
				return IDamageableType.Missile;
			}
		}

		// Token: 0x17001078 RID: 4216
		// (get) Token: 0x06005F8B RID: 24459 RVA: 0x002D378B File Offset: 0x002D198B
		private float modScale
		{
			get
			{
				return 0.8f / SpaceCombatManager.GetScalingAdjustmentFactor();
			}
		}

		// Token: 0x17001079 RID: 4217
		// (get) Token: 0x06005F8C RID: 24460 RVA: 0x002D3798 File Offset: 0x002D1998
		private float maxDisplaySize
		{
			get
			{
				return 3f / SpaceCombatManager.GetScalingAdjustmentFactor();
			}
		}

		// Token: 0x06005F8D RID: 24461 RVA: 0x002D37A5 File Offset: 0x002D19A5
		public new void Awake()
		{
			base.Awake();
			this.FindMissingReferences();
		}

		// Token: 0x06005F8E RID: 24462 RVA: 0x002D37B4 File Offset: 0x002D19B4
		private void FindMissingReferences()
		{
			if (this.missileRenderer == null)
			{
				this.missileRenderer = base.GetComponentInChildren<MeshRenderer>();
			}
			if (this.trackingDisplayObject == null)
			{
				foreach (Transform transform in base.transform.GetChildren())
				{
					if (transform.name.Contains("TrackingUI"))
					{
						this.trackingDisplayObject = transform.gameObject;
						this.trackingDisplayRenderer = transform.GetComponent<SpriteRenderer>();
					}
				}
			}
		}

		// Token: 0x06005F8F RID: 24463 RVA: 0x002D3854 File Offset: 0x002D1A54
		public void UpdateTrackingDisplay()
		{
			if (this.mainCamT == null)
			{
				this.mainCamT = GameControl.spaceCombat.mainCameraTransform;
			}
			if (this.trackingDisplayTransform == null)
			{
				return;
			}
			if (GameControl.spaceCombat.combatHUD.debugHideUI)
			{
				this.trackingDisplayObject.SetActive(false);
				return;
			}
			this.trackingDisplayTransform.LookAt(this.mainCamT.position);
			float sqrMagnitude = (this.mainCamT.position - this.trackingDisplayTransform.position).sqrMagnitude;
			if (sqrMagnitude < this.trackingDisplayEnvelope * this.trackingDisplayEnvelope)
			{
				this.trackingDisplayObject.SetActive(false);
			}
			else
			{
				this.trackingDisplayObject.SetActive(true);
			}
			float num = Mathf.Clamp(sqrMagnitude * this.modScale * GameControl.spaceCombat.projectileScalingFactor, this.minDisplaySize, this.maxDisplaySize);
			this.trackingDisplayTransform.localScale = new Vector3(num, num, 1f);
		}

		// Token: 0x1700107A RID: 4218
		// (get) Token: 0x06005F90 RID: 24464 RVA: 0x002D394A File Offset: 0x002D1B4A
		public override bool isMissile
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06005F91 RID: 24465 RVA: 0x002D3950 File Offset: 0x002D1B50
		public override bool ThreateningEnemyCombatant(List<CombatantController> combatantList)
		{
			foreach (CombatantController combatantController in combatantList)
			{
				if (base.projectileState.thrustersEnabled)
				{
					return true;
				}
				if (base.projectileState.WillHitSphere(combatantController.position, combatantController.velocityVector, combatantController.damageableType, combatantController))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005F92 RID: 24466 RVA: 0x002D39D0 File Offset: 0x002D1BD0
		public override void Fire(Vector3 originPosition, Vector3 targetPosition, IDamageable target)
		{
			this.FindMissingReferences();
			this.missileRenderer.enabled = true;
			this.missileRenderer.gameObject.transform.localScale = Vector3.one * GameControl.spaceCombat.projectileScalingFactor;
			base.gameObject.SetActive(true);
			base.projectileTransform = base.transform;
			this.missileTemplate = base.weaponTemplate.ref_missileWeapon;
			base.projectileTransform.position = originPosition;
			base.projectileTransform.LookAt(targetPosition);
			if (this.projectileParticleSystem != null)
			{
				this.projectileParticleSystem.Clear();
			}
			this.destructionParticleSystem.Clear();
			base.hasHit = false;
			base.beenDestroyed = false;
			base.clearedLauncher = false;
			this.removedFromLiveProjectiles = false;
			this.maxRunTime_s = this.missileTemplate.targetingRange_km * this.missileTemplate.deltaV_kps * 0.5f;
			this.SetNewTargetData(target);
			float num = SpaceCombatManager.kps2_to_scale(this.missileTemplate.acceleration_g * 0.00980665f);
			float num2 = SpaceCombatManager.kps2_to_scale(this.missileTemplate.deltaV_kps) * 0.5f;
			base.velocityVector = SpaceCombatManager.vector_km_to_scale(base.projectileState.velocityVector_kps);
			this._container.AddProjectile(this, base.transform, ProjectileJobData.MovementType.Missile, base.projectileState, this.missileTemplate.deltaV_kps, num, num2, base.velocityVector, originPosition, target, this.missileTemplate.maneuver_angle, this.missileTemplate.thrustRamp_s, this.missileTemplate.turnRamp_s, this.missileTemplate.rotation_degps);
			this.trackingDisplayTransform = this.trackingDisplayObject.transform;
			this.trackingDisplayRenderer.color = base.projectileState.shootingFaction.template.color;
			this.trackingDisplayRenderer.enabled = true;
		}

		// Token: 0x06005F93 RID: 24467 RVA: 0x002D3BA0 File Offset: 0x002D1DA0
		public void SetNewTargetData(IDamageable newTarget)
		{
			this._container.SetProjectileTarget(base.transform, newTarget);
			this.target = newTarget;
			this.targetCombatant = this.target as CombatantController;
			this.targetProjectile = this.target as ProjectileController;
			this.prevTargetPosition = newTarget.transform.position;
		}

		// Token: 0x06005F94 RID: 24468 RVA: 0x002D3BFC File Offset: 0x002D1DFC
		public IDamageable Retarget()
		{
			List<IDamageable> list = new List<IDamageable>();
			IDamageable damageable = null;
			SpaceCombatManager combatMgr = GameControl.spaceCombat;
			if (this.missileTemplate.attackMode)
			{
				List<CombatantController> list2 = combatMgr.combatantLookup.Values.Where<CombatantController>((CombatantController x) => !x.destructionTriggered && x.faction != this.projectileState.shootingFaction).ToList<CombatantController>();
				list.AddRange(list2);
			}
			if (this.missileTemplate.defenseMode)
			{
				List<ProjectileController> list3 = combatMgr._reverseProjectiles.Keys.Where<ProjectileController>((ProjectileController x) => x.gameObject.activeSelf && combatMgr._reverseProjectiles[x].shootingFaction != this.projectileState.shootingFaction).ToList<ProjectileController>();
				list.AddRange(list3);
			}
			float num = float.MaxValue;
			foreach (IDamageable damageable2 in list)
			{
				if (!damageable2.isDestroyed)
				{
					float sqrMagnitude = (damageable2.position - this.prevTargetPosition).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						num = sqrMagnitude;
						damageable = damageable2;
					}
				}
			}
			return damageable;
		}

		// Token: 0x06005F95 RID: 24469 RVA: 0x002D3D18 File Offset: 0x002D1F18
		public override float ApplyDamage(DamageSource source)
		{
			base.beenDestroyed = true;
			if ((source is BeamWeapon.Beam || source is MissileController.BurstDamage) && this.missileTemplate.AOEWeapon && this.IsAnEnemyWithinAOE(source.hitPosition))
			{
				this.DoExplosionAOEDamage(null, source.hitPosition);
				this.MissileImpactVFX(this.projectileParticleTransform.position, base.projectileTransform.forward * -1f);
			}
			else
			{
				this.missileRenderer.enabled = false;
				this.Destruct(this.removedFromLiveProjectiles);
			}
			return source.damage.amount;
		}

		// Token: 0x06005F96 RID: 24470 RVA: 0x002D3DB2 File Offset: 0x002D1FB2
		public float GetEstimatedDamage_Points()
		{
			return this.missileTemplate.EstimatedBaseDamageAtRange_points(20f, false);
		}

		// Token: 0x06005F97 RID: 24471 RVA: 0x002D3DC8 File Offset: 0x002D1FC8
		private bool HasHitTarget()
		{
			float num = 0.023f + (base.velocityVector.magnitude + base.accelerationVector.magnitude) * Time.deltaTime * this.gameTime.currentSpeed * 3.5f;
			this.raycastHitsArray = Physics.RaycastAll(base.projectileTransform.position, base.velocityVector, num, this._collisionMask);
			if (this.raycastHitsArray.Length == 0)
			{
				return false;
			}
			IDamageable damageable;
			RaycastHit raycastHit = base.FilterRaycastsForHits(this.raycastHitsArray.ToList<RaycastHit>(), out damageable);
			if (damageable == null)
			{
				return false;
			}
			DamageSource damageSource = new MissileController.MissileDamage(base.velocityVector_kps, raycastHit.point, base.projectileState.origin, damageable, this.missileTemplate, base.projectileState.shootingFaction, base.projectileState.effectiveMass_kg);
			damageable.ApplyDamage(damageSource);
			if (this.missileTemplate.AOEWeapon)
			{
				this.DoExplosionAOEDamage(damageable, raycastHit.point);
				if (damageable.damageableType == IDamageableType.StationModule)
				{
					GameControl.spaceCombat.habModelController.HabHitByNukeInCombat(base.projectileState.shootingFaction, raycastHit.point);
				}
			}
			this.MissileImpactVFX(raycastHit.point, raycastHit.normal);
			if (damageable.damageableType == IDamageableType.Missile)
			{
				(damageable as MissileController).HitByShooter(raycastHit);
			}
			return true;
		}

		// Token: 0x06005F98 RID: 24472 RVA: 0x002D3F14 File Offset: 0x002D2114
		public void HitByShooter(RaycastHit hit)
		{
			TISpaceCombatProjectileState ref_projectile = base.ref_projectile;
			TIMissileTemplate ref_missileWeapon = ref_projectile.originWeapon.ref_missileWeapon;
			if (ref_missileWeapon.AOEWeapon)
			{
				DamageSource damageSource = new MissileController.MissileDamage(ref_projectile.velocityVector_kps, hit.point, ref_projectile.origin, this, ref_missileWeapon, ref_projectile.shootingFaction, ref_projectile.effectiveMass_kg);
				this.ApplyDamage(damageSource);
				this.DoExplosionAOEDamage(this, hit.point);
				this.MissileImpactVFX(hit.point, ((IDamageable)this).damageableTransform.forward * -1f);
			}
		}

		// Token: 0x06005F99 RID: 24473 RVA: 0x002D3F9C File Offset: 0x002D219C
		private bool IsAnEnemyWithinAOE(Vector3 pointOfImpact)
		{
			IEnumerable<CombatantController> enumerable = GameControl.spaceCombat.combatantLookup.Values.Where<CombatantController>((CombatantController x) => x.faction != base.projectileState.shootingFaction);
			new List<RaycastHit>();
			List<IDamageable> list = new List<IDamageable>();
			if (this.missileTemplate.warheadClass == WarheadClass.ShapedNuclear)
			{
				float num = SpaceCombatManager.km_to_scale(this.missileTemplate.RangeAtOneDamage_km(WarheadClass.ShapedNuclear));
				foreach (RaycastHit raycastHit in TIUtilities.SimpleConeCastAll(base.projectileTransform.position, base.projectileTransform.forward, base.projectileTransform.right, 5, this.missileTemplate.shapedChargeAngle / 2f, num, this._collisionMask))
				{
					IDamageable damageable = raycastHit.collider.gameObject.GetComponentInParent<CombatantController>();
					if (damageable == null)
					{
						damageable = raycastHit.collider.gameObject.GetComponentInParent<ProjectileController>();
					}
					if (damageable != null && !list.Contains(damageable))
					{
						list.Add(damageable);
					}
				}
			}
			foreach (CombatantController combatantController in enumerable)
			{
				if (!combatantController.isDestroyed)
				{
					float num2 = SpaceCombatManager.scale_to_km(Vector3.Distance(pointOfImpact, combatantController.position)) * 1000f;
					float num3 = 0f;
					switch (this.missileTemplate.warheadClass)
					{
					case WarheadClass.Nuclear:
					case WarheadClass.Antimatter:
						num3 = base.weaponTemplate.flatDamage_MJ / 12.566371f / (num2 * num2);
						break;
					case WarheadClass.ShapedNuclear:
						if (Vector3.Angle(base.projectileTransform.forward, combatantController.position - base.projectileTransform.position) < this.missileTemplate.shapedChargeAngle / 2f || list.Contains(combatantController))
						{
							float num4 = Mathf.Tan(0.017453292f * this.missileTemplate.shapedChargeAngle);
							num3 = this.missileTemplate.flatDamage_MJ * 0.1f / (3.1415927f * num4 * num2 * num4 * num2);
						}
						break;
					}
					if (combatantController.ref_projectile == null)
					{
						num3 /= 20f;
					}
					if (num3 >= 1f)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06005F9A RID: 24474 RVA: 0x002D422C File Offset: 0x002D242C
		private void DoExplosionAOEDamage(IDamageable alreadyHitDamageable, Vector3 pointOfImpact)
		{
			List<IDamageable> list = GameControl.spaceCombat.combatantLookup.Values.ToList<IDamageable>();
			list.AddRange(GameControl.spaceCombat._reverseProjectiles.Keys.ToList<ProjectileController>());
			new List<RaycastHit>();
			List<IDamageable> list2 = new List<IDamageable>();
			if (this.missileTemplate.warheadClass == WarheadClass.ShapedNuclear)
			{
				float num = SpaceCombatManager.km_to_scale(this.missileTemplate.RangeAtOneDamage_km(WarheadClass.ShapedNuclear));
				using (List<RaycastHit>.Enumerator enumerator = TIUtilities.SimpleConeCastAll(base.projectileTransform.position, base.projectileTransform.forward, base.projectileTransform.right, 5, this.missileTemplate.shapedChargeAngle / 2f, num, this._collisionMask).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						RaycastHit raycastHit = enumerator.Current;
						IDamageable damageable = raycastHit.collider.gameObject.GetComponentInParent<CombatantController>();
						if (damageable == null)
						{
							damageable = raycastHit.collider.gameObject.GetComponentInParent<ProjectileController>();
						}
						if (damageable != null && !list2.Contains(damageable))
						{
							list2.Add(damageable);
						}
					}
					goto IL_0141;
				}
			}
			if ((this.missileTemplate.warheadClass == WarheadClass.Nuclear || this.missileTemplate.warheadClass == WarheadClass.Antimatter) && GameControl.spaceCombat.habModelController != null)
			{
				this.CheckIfHabCoreIsCaughtInNukeBlast(pointOfImpact);
			}
			IL_0141:
			foreach (IDamageable damageable2 in list)
			{
				if (damageable2 != alreadyHitDamageable && !damageable2.isDestroyed)
				{
					float num2 = SpaceCombatManager.scale_to_km(Vector3.Distance(pointOfImpact, damageable2.position)) * 1000f;
					float num3 = 0f;
					switch (this.missileTemplate.warheadClass)
					{
					case WarheadClass.Nuclear:
					case WarheadClass.Antimatter:
						num3 = base.weaponTemplate.flatDamage_MJ / 12.566371f / (num2 * num2);
						break;
					case WarheadClass.ShapedNuclear:
						if (Vector3.Angle(base.projectileTransform.forward, damageable2.position - base.projectileTransform.position) < this.missileTemplate.shapedChargeAngle / 2f || list2.Contains(damageable2))
						{
							float num4 = Mathf.Tan(0.017453292f * this.missileTemplate.shapedChargeAngle);
							num3 = this.missileTemplate.flatDamage_MJ * 0.1f / (3.1415927f * num4 * num2 * num4 * num2);
						}
						break;
					}
					if (damageable2.ref_projectile == null)
					{
						num3 /= 20f;
					}
					if (num3 >= 1f)
					{
						DamageSource damageSource = new MissileController.BurstDamage(pointOfImpact, damageable2.position, base.projectileState.origin, damageable2, this.missileTemplate, base.projectileState.shootingFaction, num3);
						damageable2.ApplyDamage(damageSource);
					}
				}
			}
		}

		// Token: 0x06005F9B RID: 24475 RVA: 0x002D4538 File Offset: 0x002D2738
		private void CheckIfHabCoreIsCaughtInNukeBlast(Vector3 pointOfImpact)
		{
			Vector3 position = GameControl.spaceCombat.habModelController.transform.position;
			float num = SpaceCombatManager.scale_to_km(Vector3.Distance(pointOfImpact, position)) * 1000f;
			if (base.weaponTemplate.flatDamage_MJ / 12.566371f / (num * num) / 20f >= 1f)
			{
				GameControl.spaceCombat.habModelController.HabHitByNukeInCombat(base.projectileState.shootingFaction, pointOfImpact);
			}
		}

		// Token: 0x06005F9C RID: 24476 RVA: 0x002D45AC File Offset: 0x002D27AC
		public override void UpdateController()
		{
			this.UpdateTrackingDisplay();
			if (this.gameTime.Paused)
			{
				return;
			}
			if (base.hasHit)
			{
				return;
			}
			if (!this.removedFromLiveProjectiles && (base.projectileState.deltaV < 0f || Mathf.Approximately(base.projectileState.deltaV, 0f)))
			{
				this.removedFromLiveProjectiles = true;
				base.projectileState.RemoveFromLiveProjectiles();
			}
			float num = (float)this.gameTime.currentTime.DifferenceInSeconds(base.projectileState.launchTime);
			if ((this.removedFromLiveProjectiles && num > this.maxRunTime_s / 2f) || num > this.maxRunTime_s)
			{
				if (this.missileRenderer != null)
				{
					this.missileRenderer.enabled = false;
				}
				this.Destruct(this.removedFromLiveProjectiles);
			}
			if (base.projectileTransform == null)
			{
				return;
			}
			if (!base.beenDestroyed && !base.hasHit && !this.HasHitTarget() && num >= 2f)
			{
				if (!base.clearedLauncher && num > 5f)
				{
					base.clearedLauncher = true;
				}
				this.projectileParticleTransform.localScale = this.projectileParticleDefaultScale * base.projectileState.thrustAmount;
				if (!this.projectileParticleSystem.isPlaying && base.projectileState.thrustersEnabled)
				{
					this.projectileParticleSystem.Play();
				}
				else if (this.projectileParticleSystem.isPlaying && !base.projectileState.thrustersEnabled)
				{
					this.projectileParticleSystem.Stop();
				}
			}
			if ((this.target == null || (this.targetCombatant != null && (!this.targetCombatant.isActiveAndEnabled || this.targetCombatant.destructionTriggered)) || (this.targetProjectile != null && (!this.targetProjectile.isActiveAndEnabled || this.targetProjectile.beenDestroyed))) && !base.beenDestroyed)
			{
				IDamageable damageable = this.Retarget();
				if (damageable == null)
				{
					this.missileRenderer.enabled = false;
					this.projectileParticleSystem.Stop();
					this.Destruct(this.removedFromLiveProjectiles);
					return;
				}
				this.SetNewTargetData(damageable);
			}
			base.velocityVector = base.projectileState.velocityVector_kps * 0.05f;
			if (this.targetCombatant != null)
			{
				this.prevTargetPosition = this.targetCombatant.transform.position;
				return;
			}
			if (this.targetProjectile != null)
			{
				this.prevTargetPosition = this.targetProjectile.transform.position;
			}
		}

		// Token: 0x06005F9D RID: 24477 RVA: 0x002D483D File Offset: 0x002D2A3D
		protected override void OnPause()
		{
			this.isPaused = true;
			if (this.destructionObject.activeInHierarchy)
			{
				this.destructionParticleSystem.Pause();
			}
			if (this.impactObject.activeInHierarchy)
			{
				this.impactParticleSystem.Pause();
			}
		}

		// Token: 0x06005F9E RID: 24478 RVA: 0x002D4876 File Offset: 0x002D2A76
		protected override void OnUnpause()
		{
			this.isPaused = false;
			if (this.destructionObject.activeInHierarchy)
			{
				this.destructionParticleSystem.Play();
			}
			if (this.impactObject.activeInHierarchy)
			{
				this.impactParticleSystem.Play();
			}
		}

		// Token: 0x06005F9F RID: 24479 RVA: 0x002D48B0 File Offset: 0x002D2AB0
		private void MissileImpactVFX(Vector3 pointOfImpact, Vector3 normalOfPointOfImpact)
		{
			Vector3 vector = Vector3.Reflect(pointOfImpact - base.projectileTransform.position, normalOfPointOfImpact);
			if (this.missileTemplate.warheadClass == WarheadClass.ShapedNuclear)
			{
				vector = base.projectileTransform.forward;
			}
			base.projectileTransform.position = pointOfImpact;
			base.velocityVector = Vector3.zero;
			this.projectileParticleSystem.Stop();
			if (this.eventInstance.isValid())
			{
				this.eventInstance.Stop(STOP_MODE.IMMEDIATE);
			}
			this.missileRenderer.enabled = false;
			base.hasHit = true;
			base.Impact(pointOfImpact, vector);
			this.Destruct(this.removedFromLiveProjectiles);
			if (this.missileTemplate.AOEWeapon)
			{
				Mood.TriggerEvent(Mood.Event.SDKL_Explosion);
			}
		}

		// Token: 0x06005FA0 RID: 24480 RVA: 0x002D4965 File Offset: 0x002D2B65
		public override void Destruct(bool isAlreadyRemovedFromLiveProjectiles = false)
		{
			this.trackingDisplayRenderer.enabled = false;
			base.Destruct(isAlreadyRemovedFromLiveProjectiles);
		}

		// Token: 0x06005FA1 RID: 24481 RVA: 0x002D497C File Offset: 0x002D2B7C
		public void OnDrawGizmos()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			float num = 0.023f + (base.velocityVector.magnitude + base.accelerationVector.magnitude) * Time.deltaTime * this.gameTime.currentSpeed * 1.05f;
			Gizmos.DrawLine(base.projectileTransform.position, base.projectileTransform.position + base.velocityVector * num);
		}

		// Token: 0x040043E5 RID: 17381
		protected TIMissileTemplate missileTemplate;

		// Token: 0x040043E7 RID: 17383
		private const float ignitionDelay_s = 2f;

		// Token: 0x040043E8 RID: 17384
		private const float DV_cheat = 0.5f;

		// Token: 0x040043E9 RID: 17385
		public Renderer missileRenderer;

		// Token: 0x040043EA RID: 17386
		private float maxRunTime_s;

		// Token: 0x040043EB RID: 17387
		private bool removedFromLiveProjectiles;

		// Token: 0x040043EC RID: 17388
		private CombatantController targetCombatant;

		// Token: 0x040043ED RID: 17389
		private ProjectileController targetProjectile;

		// Token: 0x040043EE RID: 17390
		private Vector3 prevTargetPosition;

		// Token: 0x040043EF RID: 17391
		[Header("Tracking UI")]
		public GameObject trackingDisplayObject;

		// Token: 0x040043F0 RID: 17392
		public SpriteRenderer trackingDisplayRenderer;

		// Token: 0x040043F1 RID: 17393
		private float trackingDisplayEnvelope = 2f;

		// Token: 0x040043F2 RID: 17394
		private float minDisplaySize;

		// Token: 0x040043F3 RID: 17395
		private Transform mainCamT;

		// Token: 0x040043F4 RID: 17396
		public Transform trackingDisplayTransform;

		// Token: 0x02001382 RID: 4994
		public class MissileDamage : ProjectileDamageSource
		{
			// Token: 0x06009160 RID: 37216 RVA: 0x003473F8 File Offset: 0x003455F8
			public MissileDamage(Vector3 inboundVelocityVector_kps, Vector3 hitPosition, CombatWeaponCarrierState attacker, IDamageable target, TIMissileTemplate missileTemplate, TIFactionState launchingFaction, float warheadMass_kg)
			{
				base.attacker = attacker;
				base.hitPosition = hitPosition;
				base.warheadMass_kg = warheadMass_kg;
				Vector3 vector = inboundVelocityVector_kps - target.velocityVector_kps;
				DamageType damageType = missileTemplate.GetDamageType();
				CombatantShipController combatantShipController = target as CombatantShipController;
				if (combatantShipController != null)
				{
					float num = TIUtilities.RandomFloatValue();
					if (TIGameState.Valid(attacker.ref_shipCarrier()))
					{
						num += Mathf.Max(0f, attacker.ref_shipCarrier().TargetingBonus(missileTemplate, GameControl.spaceCombat.combatState.AlliedHab(attacker)));
					}
					if (!missileTemplate.AOEWeapon && num < combatantShipController.ShipState.ECMValue(launchingFaction, GameControl.spaceCombat.combatState.AlliedHab(combatantShipController.WeaponCarrierState)))
					{
						base.damage = new Damage(missileTemplate, 0f, damageType, 0f, 0f, 0, launchingFaction);
						return;
					}
				}
				base.damage = missileTemplate.GetComplexDamage(0f, target.damageableType, target.GetCrossSectionalArea_m2(float.MaxValue), vector.magnitude, attacker, launchingFaction, -1f);
			}
		}

		// Token: 0x02001383 RID: 4995
		public class BurstDamage : DamageSource
		{
			// Token: 0x06009161 RID: 37217 RVA: 0x00347508 File Offset: 0x00345708
			public BurstDamage(Vector3 origin, Vector3 hitPosition, CombatWeaponCarrierState attacker, IDamageable target, TIMissileTemplate missileTemplate, TIFactionState launchingFaction, float damageValue)
			{
				base.attacker = attacker;
				base.hitPosition = hitPosition;
				if (target.damageableType == IDamageableType.StationModule)
				{
					base.damage = new Damage(missileTemplate, 0f, DamageType.Thermal, damageValue * 3f, 0f, 0, launchingFaction);
					return;
				}
				if (missileTemplate.warheadClass == WarheadClass.ShapedNuclear && missileTemplate.shapedChargeAngle < 1f)
				{
					base.damage = new Damage(missileTemplate, 0f, DamageType.Thermal, damageValue, 0f, 0, launchingFaction);
					return;
				}
				base.damage = new Damage(missileTemplate, 0f, DamageType.Thermal, 0f, 0f, (int)damageValue, launchingFaction);
			}
		}
	}
}

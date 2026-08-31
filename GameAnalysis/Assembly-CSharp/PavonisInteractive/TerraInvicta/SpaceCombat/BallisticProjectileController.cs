using System;
using System.Linq;
using FMOD.Studio;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Jobs;
using PavonisInteractive.TerraInvicta.Ship;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009E0 RID: 2528
	public class BallisticProjectileController : ProjectileController
	{
		// Token: 0x17001064 RID: 4196
		// (get) Token: 0x06005F53 RID: 24403 RVA: 0x002D2560 File Offset: 0x002D0760
		protected TIGunTypeWeaponTemplate gunTemplate
		{
			get
			{
				return base.weaponTemplate as TIGunTypeWeaponTemplate;
			}
		}

		// Token: 0x06005F54 RID: 24404 RVA: 0x002D2570 File Offset: 0x002D0770
		public override void Fire(Vector3 originPosition, Vector3 targetPosition, IDamageable target = null)
		{
			base.gameObject.SetActive(false);
			base.gameObject.SetActive(true);
			base.projectileTransform = base.transform;
			base.projectileTransform.localScale = Vector3.one * GameControl.spaceCombat.projectileScalingFactor;
			base.velocityVector = SpaceCombatManager.vector_km_to_scale(base.projectileState.velocityVector_kps);
			this.originPosition = originPosition;
			base.projectileTransform.position = originPosition;
			base.projectileTransform.LookAt(targetPosition);
			this.projectileParticleSystem.Clear();
			this.destructionParticleSystem.Clear();
			this.projectileParticleSystem.Play();
			base.hasHit = false;
			base.beenDestroyed = false;
			base.clearedLauncher = false;
			this.velocityVector_Magnitude = base.velocityVector.magnitude;
			this.maximumDistance = Mathf.Max(base.weaponController.weaponTemplate.targetingRange_km * 1.25f, base.weaponController.weaponTemplate.targetingRange_km * 1.25f * (this.velocityVector_Magnitude / 0.05f) / this.gunTemplate.muzzleVelocity_kps);
			if (base.weaponTemplate.weaponClass == WeaponClass.Magnetic)
			{
				base.projectileTransform.localScale *= Mathf.Log10(base.warheadMass_kg);
				base.GetComponent<MagBulletParticleController>().UpdateMass(base.projectileState.effectiveMass_kg, base.warheadMass_kg);
			}
			this._container.AddProjectile(this, base.projectileTransform, ProjectileJobData.MovementType.Ballistic, base.projectileState, 0f, float.MaxValue, float.MaxValue, base.velocityVector, originPosition, target, 0f, 0f, 0f, 0f);
		}

		// Token: 0x06005F55 RID: 24405 RVA: 0x002D2720 File Offset: 0x002D0920
		public override void UpdateController()
		{
			if (!base.hasHit && !this.gameTime.Paused && !base.beenDestroyed)
			{
				if (Vector3.Distance(this.originPosition, base.projectileTransform.position) / 0.05f > this.maximumDistance)
				{
					this.Destruct(false);
				}
				if (!base.clearedLauncher && (float)this.gameTime.currentTime.DifferenceInSeconds(base.projectileState.launchTime) > 5f)
				{
					base.clearedLauncher = true;
				}
				float num = this.velocityVector_Magnitude * 1.2f * Time.deltaTime * this.gameTime.currentSpeed;
				this.raycastHitsArray = Physics.RaycastAll(base.projectileTransform.position, base.projectileTransform.forward, num, this._collisionMask);
				if (this.raycastHitsArray.Length == 0)
				{
					return;
				}
				IDamageable damageable;
				RaycastHit raycastHit = base.FilterRaycastsForHits(this.raycastHitsArray.ToList<RaycastHit>(), out damageable);
				if (damageable != null)
				{
					DamageSource damageSource = new BallisticProjectileController.ProjectileDamage(base.velocityVector, raycastHit.point, base.projectileState.origin, damageable, this.gunTemplate, base.projectileState.shootingFaction, base.projectileState.effectiveMass_kg);
					damageable.ApplyDamage(damageSource);
					Vector3 vector = Vector3.Reflect(raycastHit.point - base.projectileTransform.position, raycastHit.normal);
					base.projectileTransform.position = raycastHit.point;
					this.projectileParticleSystem.Stop();
					if (this.eventInstance.isValid())
					{
						this.eventInstance.Stop(STOP_MODE.IMMEDIATE);
					}
					base.hasHit = true;
					base.Impact(raycastHit.point, vector);
					this.Destruct(false);
					if (damageable.damageableType == IDamageableType.Missile)
					{
						(damageable as MissileController).HitByShooter(raycastHit);
					}
					return;
				}
			}
		}

		// Token: 0x06005F56 RID: 24406 RVA: 0x002D28F2 File Offset: 0x002D0AF2
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

		// Token: 0x06005F57 RID: 24407 RVA: 0x002D292B File Offset: 0x002D0B2B
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

		// Token: 0x040043D4 RID: 17364
		private float maximumDistance;

		// Token: 0x040043D5 RID: 17365
		private float velocityVector_Magnitude;

		// Token: 0x040043D6 RID: 17366
		private RaycastHit hit;

		// Token: 0x0200137F RID: 4991
		public class ProjectileDamage : ProjectileDamageSource
		{
			// Token: 0x06009151 RID: 37201 RVA: 0x003470C4 File Offset: 0x003452C4
			public ProjectileDamage(Vector3 inboundVelocityVector, Vector3 hitPosition, CombatWeaponCarrierState attacker, IDamageable target, TIGunTypeWeaponTemplate weaponTemplate, TIFactionState attackerFaction, float projectileMass_kg)
			{
				base.attacker = attacker;
				base.hitPosition = hitPosition;
				base.warheadMass_kg = projectileMass_kg;
				Vector3 vector = inboundVelocityVector - target.velocityVector;
				if (TIUtilities.IsInCombatMode)
				{
					vector /= 0.05f;
				}
				base.damage = weaponTemplate.GetComplexDamage(0f, target.damageableType, target.GetCrossSectionalArea_m2(float.MaxValue), vector.magnitude, attacker, attackerFaction, base.warheadMass_kg);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Ship
{
	// Token: 0x0200096B RID: 2411
	public class DefenseFireMode : IFireMode
	{
		// Token: 0x17000F99 RID: 3993
		// (get) Token: 0x06005BE6 RID: 23526 RVA: 0x002BFB57 File Offset: 0x002BDD57
		// (set) Token: 0x06005BE7 RID: 23527 RVA: 0x002BFB5F File Offset: 0x002BDD5F
		public IWeapon weapon { get; private set; }

		// Token: 0x17000F9A RID: 3994
		// (get) Token: 0x06005BE8 RID: 23528 RVA: 0x002BFB68 File Offset: 0x002BDD68
		public FireMode mode
		{
			get
			{
				return FireMode.Defense;
			}
		}

		// Token: 0x17000F9B RID: 3995
		// (get) Token: 0x06005BE9 RID: 23529 RVA: 0x002BFB6B File Offset: 0x002BDD6B
		public string displayName
		{
			get
			{
				if (!this.weaponTemplate.CanOnlyDefensivelyTargetMissiles())
				{
					return Loc.T("UI.SpaceCombat.Defense");
				}
				return Loc.T("UI.SpaceCombat.MissileDefense");
			}
		}

		// Token: 0x17000F9C RID: 3996
		// (get) Token: 0x06005BEA RID: 23530 RVA: 0x002BFB8F File Offset: 0x002BDD8F
		public string description
		{
			get
			{
				if (!this.weaponTemplate.CanOnlyDefensivelyTargetMissiles())
				{
					return Loc.T("UI.SpaceCombat.Defense.description");
				}
				return Loc.T("UI.SpaceCombat.MissileDefense.description");
			}
		}

		// Token: 0x17000F9D RID: 3997
		// (get) Token: 0x06005BEB RID: 23531 RVA: 0x002BFBB3 File Offset: 0x002BDDB3
		public string iconPath
		{
			get
			{
				if (!this.weaponTemplate.CanOnlyDefensivelyTargetMissiles())
				{
					return "ui_spacecombat/BUT_mode_defense";
				}
				return "ui_spacecombat/BUT_mode_missiledefense";
			}
		}

		// Token: 0x06005BEC RID: 23532 RVA: 0x002BFBD0 File Offset: 0x002BDDD0
		public DefenseFireMode(IWeapon weapon)
		{
			this.weapon = weapon;
			this.weaponAsset = weapon as Weapon;
			this.weaponTemplate = this.weaponAsset.weaponTemplate;
			this.combatantTransform = weapon.combatant.combatantTransform;
			this.weaponClass = this.weaponTemplate.weaponClass;
			this.firingState = this.weaponAsset.combatant.WeaponCarrierState;
			this.saturatedTargetableProjectiles = new List<DefenseFireMode.TargetingData>();
			this.faction = this.firingState.GetFaction();
			float num = SpaceCombatManager.km_to_scale(this.weaponTemplate.targetingRange_km);
			this.effectiveRange_u = Mathf.Min(num, SpaceCombatManager.km_to_scale(this.weaponTemplate.EffectiveRangeAgainstProjectiles_km()));
			if (this.firingState.isShip())
			{
				this.effectiveRange_u += this.firingState.ref_shipCarrier().hull.length_m / 2f * GameControl.spaceCombat.modelScalingFactor;
			}
			else
			{
				float num2 = 50f;
				this.effectiveRange_u += num2 / 2f * GameControl.spaceCombat.modelScalingFactor;
			}
			this.saturationValue = (this.SaturationValues.ContainsKey(this.weaponClass) ? this.SaturationValues[this.weaponClass] : (-1));
		}

		// Token: 0x06005BED RID: 23533 RVA: 0x002BFD48 File Offset: 0x002BDF48
		public bool SufficientDamageToFire(float distance_km, float minDamage)
		{
			WeaponClass weaponClass = this.weaponClass;
			if (weaponClass != WeaponClass.Laser)
			{
				if (weaponClass != WeaponClass.Particle)
				{
					return true;
				}
				if (this.firingState.isShip() && this.firingState.ref_shipCarrier() != null)
				{
					return distance_km <= this.firingState.ref_shipCarrier().effectiveBeamWeaponRange_km[minDamage][this.weaponTemplate.dataName];
				}
				return distance_km <= this.weaponTemplate.ref_particleWeapon.RangeToDoDamage_km(minDamage, null);
			}
			else
			{
				if (this.firingState.isShip() && this.firingState.ref_shipCarrier() != null)
				{
					return distance_km <= this.firingState.ref_shipCarrier().effectiveBeamWeaponRange_km[minDamage][this.weaponTemplate.dataName];
				}
				return distance_km <= this.weaponTemplate.ref_laserWeapon.RangeToDoDamage_km(minDamage, null);
			}
		}

		// Token: 0x06005BEE RID: 23534 RVA: 0x002BFE38 File Offset: 0x002BE038
		public float GetExpectedDamage(float distance_km, IDamageable target)
		{
			switch (this.weaponClass)
			{
			case WeaponClass.Laser:
				return this.weaponTemplate.ref_laserWeapon.EstimatedDamageAtRange_MJ(distance_km, target.GetCrossSectionalArea_m2(float.MaxValue), this.firingState) / 20f;
			case WeaponClass.Particle:
				return this.weaponTemplate.DamageAtRange_points(distance_km, target.GetCrossSectionalArea_m2(float.MaxValue), this.firingState, 0f, 0f, false).directDamage_Points;
			}
			return this.weaponTemplate.BaseDamageAtRange_points(distance_km, false);
		}

		// Token: 0x06005BEF RID: 23535 RVA: 0x002BFED4 File Offset: 0x002BE0D4
		public IDamageable AcquireTarget(DateTime currentTime, out Vector3 targetPosition, out float distanceToTarget_km)
		{
			targetPosition = Vector3.zero;
			IDamageable damageable = null;
			float num = float.MaxValue;
			distanceToTarget_km = float.MaxValue;
			this.saturatedTargetableProjectiles.Clear();
			foreach (ProjectileController projectileController in GameControl.spaceCombat._projectiles.Values)
			{
				if (!(projectileController == null) && !(projectileController.projectileState.shootingFaction == this.faction) && (projectileController.isMissile || (!this.weaponTemplate.isMissileWeapon && (!this.weaponTemplate.isParticleWeapon || this.weaponTemplate.ref_particleWeapon.dispersionModel != ParticleBeamDispersionModel.Charged))) && !projectileController.hasHit && !projectileController.beenDestroyed && projectileController.weaponController.weaponTemplate.isPointDefenseTargetable)
				{
					bool flag;
					Vector3 positionToTarget = this.weaponAsset.GetPositionToTarget(projectileController, out flag);
					if (!flag && this.weaponAsset.InArc(positionToTarget, projectileController.velocityVector, projectileController.accelerationVector))
					{
						float num2 = Vector3.Distance(this.combatantTransform.position, positionToTarget);
						distanceToTarget_km = SpaceCombatManager.scale_to_km(num2);
						if (num2 < this.effectiveRange_u && num2 < num && this.SufficientDamageToFire(distanceToTarget_km, projectileController.projectileState.originWeapon.minDamageForPDToFire) && projectileController.ThreateningEnemyCombatant(this.weapon.combatant.alliedCombatants))
						{
							if (this.saturationValue > 0 && projectileController.projectileState.enemiesTargetingMe.Count >= this.saturationValue)
							{
								this.saturatedTargetableProjectiles.Add(new DefenseFireMode.TargetingData(projectileController, num2, positionToTarget, distanceToTarget_km));
							}
							else
							{
								num = num2;
								damageable = projectileController;
								targetPosition = positionToTarget;
							}
						}
					}
				}
			}
			if (damageable == null && this.saturatedTargetableProjectiles.Count > 0)
			{
				foreach (DefenseFireMode.TargetingData targetingData in this.saturatedTargetableProjectiles)
				{
					if (targetingData.scaledDistance < num && (TIGlobalConfig.globalConfig.alwaysFireAtSaturated || targetingData.distance_km < (float)(100 * this.saturationValue) || targetingData.possibleTargetProjectile.projectileState.enemiesTargetingMe.Count < this.saturationValue * 2))
					{
						num = targetingData.scaledDistance;
						damageable = targetingData.possibleTargetProjectile;
						targetPosition = targetingData.candidateTargetPosition;
						distanceToTarget_km = targetingData.distance_km;
					}
				}
			}
			if (damageable != null)
			{
				distanceToTarget_km = SpaceCombatManager.scale_to_km(num);
			}
			return damageable;
		}

		// Token: 0x040041B7 RID: 16823
		private Weapon weaponAsset;

		// Token: 0x040041B8 RID: 16824
		private TIShipWeaponTemplate weaponTemplate;

		// Token: 0x040041B9 RID: 16825
		private readonly float effectiveRange_u;

		// Token: 0x040041BA RID: 16826
		private WeaponClass weaponClass;

		// Token: 0x040041BB RID: 16827
		private CombatWeaponCarrierState firingState;

		// Token: 0x040041BC RID: 16828
		private Transform combatantTransform;

		// Token: 0x040041BD RID: 16829
		private TIFactionState faction;

		// Token: 0x040041BE RID: 16830
		private int saturationValue = -1;

		// Token: 0x040041BF RID: 16831
		private List<DefenseFireMode.TargetingData> saturatedTargetableProjectiles;

		// Token: 0x040041C0 RID: 16832
		private readonly Dictionary<WeaponClass, int> SaturationValues = new Dictionary<WeaponClass, int>
		{
			{
				WeaponClass.Missile,
				1
			},
			{
				WeaponClass.NavalGun,
				4
			},
			{
				WeaponClass.Magnetic,
				2
			}
		};

		// Token: 0x02001332 RID: 4914
		private struct TargetingData
		{
			// Token: 0x0600908C RID: 37004 RVA: 0x00344E02 File Offset: 0x00343002
			public TargetingData(ProjectileController possibleTargetProjectile, float scaledDistance, Vector3 candidateTargetPosition, float distance_km)
			{
				this.possibleTargetProjectile = possibleTargetProjectile;
				this.scaledDistance = scaledDistance;
				this.candidateTargetPosition = candidateTargetPosition;
				this.distance_km = distance_km;
			}

			// Token: 0x04006F63 RID: 28515
			public ProjectileController possibleTargetProjectile;

			// Token: 0x04006F64 RID: 28516
			public float scaledDistance;

			// Token: 0x04006F65 RID: 28517
			public Vector3 candidateTargetPosition;

			// Token: 0x04006F66 RID: 28518
			public float distance_km;
		}
	}
}

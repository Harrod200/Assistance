using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Ship
{
	// Token: 0x02000981 RID: 2433
	public class BeamWeapon : Weapon
	{
		// Token: 0x06005CA8 RID: 23720 RVA: 0x002C2759 File Offset: 0x002C0959
		public BeamWeapon(CombatantShipController ship, ModuleDataEntry weaponData)
			: base(ship, weaponData)
		{
			this.beamWeapon = weaponData.moduleTemplate as TIBeamWeaponTemplate;
		}

		// Token: 0x06005CA9 RID: 23721 RVA: 0x002C2774 File Offset: 0x002C0974
		public BeamWeapon(CombatHabModuleController module, ModuleDataEntry weaponData, int slot)
			: base(module, weaponData, slot)
		{
			this.beamWeapon = weaponData.moduleTemplate as TIBeamWeaponTemplate;
		}

		// Token: 0x06005CAA RID: 23722 RVA: 0x002C2790 File Offset: 0x002C0990
		public BeamWeapon(TIGameState dummy, ModuleDataEntry weaponData)
			: base(dummy, weaponData)
		{
			this.beamWeapon = weaponData.moduleTemplate as TIBeamWeaponTemplate;
		}

		// Token: 0x06005CAB RID: 23723 RVA: 0x002C27AC File Offset: 0x002C09AC
		public override bool TryFire(DateTime currentTime)
		{
			if (!base.TryFireCommon(currentTime))
			{
				return false;
			}
			bool flag = base.target.damageableType == IDamageableType.BallisticProjectile || base.target.damageableType == IDamageableType.Missile;
			base.EnterCooldown(TISpaceShipState.LaserDownfiring(base.weaponTemplate, flag ? base.target.ref_projectile : null), false, false, 0);
			base.combatant.WeaponCarrierState.FireWeapon(base.weaponData, flag ? base.target.ref_projectile : null);
			ShipWeaponVisController shipWeaponVisController = base.SelectWeaponVisualization(this.targetedPosition);
			shipWeaponVisController.Fire(flag, null);
			base.target.ApplyDamage(new BeamWeapon.Beam(base.target, shipWeaponVisController.transform.position, this.targetedPosition, this.beamWeapon, base.combatant.WeaponCarrierState));
			return true;
		}

		// Token: 0x06005CAC RID: 23724 RVA: 0x002C287F File Offset: 0x002C0A7F
		public BeamWeapon.Beam GetDamageSource(CombatWeaponCarrierState attacker, float distance_km)
		{
			return new BeamWeapon.Beam(base.target, distance_km, this.beamWeapon, attacker);
		}

		// Token: 0x040041FF RID: 16895
		public TIBeamWeaponTemplate beamWeapon;

		// Token: 0x02001339 RID: 4921
		public class Beam : DamageSource
		{
			// Token: 0x060090A3 RID: 37027 RVA: 0x00345128 File Offset: 0x00343328
			public Beam(IDamageable target, Vector3 start, Vector3 end, TIBeamWeaponTemplate weaponTemplate, CombatWeaponCarrierState attacker)
			{
				base.attacker = attacker;
				Ray ray = new Ray(start, end - start);
				float num = Vector3.Distance(end, start);
				using (List<Collider>.Enumerator enumerator = target.hitColliders.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						RaycastHit raycastHit;
						if (enumerator.Current.Raycast(ray, out raycastHit, num))
						{
							base.hitPosition = raycastHit.point;
							break;
						}
					}
				}
				float num2 = SpaceCombatManager.scale_to_km(num);
				DamageBreakdown damageBreakdown = weaponTemplate.DamageAtRange_points(num2, target.GetCrossSectionalArea_m2(float.MaxValue), attacker, 0f, 0f, true);
				base.damage = new Damage(weaponTemplate, num2, weaponTemplate.GetDamageType(), damageBreakdown.directDamage_Points, damageBreakdown.chippingDamage_Points, 0, attacker.GetFaction());
			}

			// Token: 0x060090A4 RID: 37028 RVA: 0x00345208 File Offset: 0x00343408
			public Beam(IDamageable target, float distance_km, Vector3 hitPosition_, TIBeamWeaponTemplate weaponTemplate, CombatWeaponCarrierState attacker)
			{
				base.attacker = attacker;
				base.hitPosition = hitPosition_;
				DamageBreakdown damageBreakdown = weaponTemplate.DamageAtRange_points(distance_km, target.GetCrossSectionalArea_m2(float.MaxValue), attacker, 0f, 0f, true);
				base.damage = new Damage(weaponTemplate, distance_km, weaponTemplate.GetDamageType(), damageBreakdown.directDamage_Points, damageBreakdown.chippingDamage_Points, 0, (attacker != null) ? attacker.GetFaction() : null);
			}

			// Token: 0x060090A5 RID: 37029 RVA: 0x0034527C File Offset: 0x0034347C
			public Beam(IDamageable target, float distance_km, TIBeamWeaponTemplate weaponTemplate, CombatWeaponCarrierState attacker)
			{
				base.attacker = attacker;
				base.hitPosition = target.transform.position + target.transform.forward;
				DamageBreakdown damageBreakdown = weaponTemplate.DamageAtRange_points(distance_km, target.GetCrossSectionalArea_m2(float.MaxValue), attacker, 0f, 0f, true);
				base.damage = new Damage(weaponTemplate, distance_km, weaponTemplate.GetDamageType(), damageBreakdown.directDamage_Points, damageBreakdown.chippingDamage_Points, 0, attacker.GetFaction());
			}
		}
	}
}

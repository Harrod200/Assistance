using System;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Ship
{
	// Token: 0x02000982 RID: 2434
	public class ProjectileWeapon : Weapon
	{
		// Token: 0x06005CAD RID: 23725 RVA: 0x002C2894 File Offset: 0x002C0A94
		public ProjectileWeapon(CombatantShipController ship, ModuleDataEntry weaponData)
			: base(ship, weaponData)
		{
		}

		// Token: 0x06005CAE RID: 23726 RVA: 0x002C289E File Offset: 0x002C0A9E
		public ProjectileWeapon(CombatHabModuleController habModule, ModuleDataEntry weaponData, int slot)
			: base(habModule, weaponData, slot)
		{
		}

		// Token: 0x06005CAF RID: 23727 RVA: 0x002C28AC File Offset: 0x002C0AAC
		public override Vector3 GetPositionToTarget(IDamageable targetToCheck, out bool impossible)
		{
			if (targetToCheck == null)
			{
				impossible = true;
				return Vector3.zero;
			}
			IDamageableType damageableType = targetToCheck.damageableType;
			if (damageableType == IDamageableType.Ship || damageableType == IDamageableType.Missile)
			{
				return TISpaceCombatProjectileState.SecondOrderInterceptPosition(base.position, base.combatant.velocityVector, SpaceCombatManager.km_to_scale(base.weaponTemplate.ref_gunWeapon.muzzleVelocity_kps), targetToCheck.position, targetToCheck.velocityVector, targetToCheck.accelerationVector, base.weaponTemplate.cooldown_s, out impossible);
			}
			return TISpaceCombatProjectileState.FirstOrderInterceptPosition(base.position, base.combatant.velocityVector, SpaceCombatManager.km_to_scale(base.weaponTemplate.ref_gunWeapon.muzzleVelocity_kps), targetToCheck.position, targetToCheck.velocityVector, out impossible);
		}

		// Token: 0x06005CB0 RID: 23728 RVA: 0x002C2958 File Offset: 0x002C0B58
		public override bool TryFire(DateTime currentTime)
		{
			if (!base.TryFireCommon(currentTime))
			{
				return false;
			}
			ShipWeaponVisController shipWeaponVisController = base.SelectWeaponVisualization(this.targetedPosition);
			ProjectileController projectileController = base.combatant.combatMgr.SetProjectile(shipWeaponVisController);
			base.combatant.combatMgr._reverseProjectiles[projectileController].Fire(base.combatant.WeaponCarrierState, base.weaponTemplate.ref_gunWeapon, this.gameTime.currentTime, shipWeaponVisController.firePoint.transform.position, this.targetedPosition, base.combatant.velocityVector_kps);
			projectileController.Fire(shipWeaponVisController.firePoint.transform.position, this.targetedPosition, null);
			base.EnterCooldown(false, false, false, 0);
			TISpaceCombatProjectileState ref_projectile = base.target.ref_projectile;
			base.combatant.WeaponCarrierState.FireWeapon(base.weaponData, ref_projectile);
			shipWeaponVisController.Fire(ref_projectile != null, null);
			return true;
		}
	}
}

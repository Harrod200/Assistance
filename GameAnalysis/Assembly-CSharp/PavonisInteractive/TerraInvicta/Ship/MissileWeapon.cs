using System;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Ship
{
	// Token: 0x02000983 RID: 2435
	public class MissileWeapon : Weapon
	{
		// Token: 0x06005CB1 RID: 23729 RVA: 0x002C2A45 File Offset: 0x002C0C45
		public MissileWeapon(CombatantShipController ship, ModuleDataEntry weaponData)
			: base(ship, weaponData)
		{
			this.missileTemplate = base.weaponTemplate.ref_missileWeapon;
		}

		// Token: 0x06005CB2 RID: 23730 RVA: 0x002C2A60 File Offset: 0x002C0C60
		public MissileWeapon(CombatHabModuleController habModule, ModuleDataEntry weaponData, int slot)
			: base(habModule, weaponData, slot)
		{
			this.missileTemplate = base.weaponTemplate.ref_missileWeapon;
		}

		// Token: 0x06005CB3 RID: 23731 RVA: 0x002C2A7C File Offset: 0x002C0C7C
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
				return TISpaceCombatProjectileState.SecondOrderInterceptPosition(base.position, base.combatant.velocityVector, SpaceCombatManager.km_to_scale(this.missileTemplate.deltaV_kps), targetToCheck.position, targetToCheck.velocityVector, targetToCheck.accelerationVector, base.weaponTemplate.cooldown_s, out impossible);
			}
			return TISpaceCombatProjectileState.FirstOrderInterceptPosition(base.position, base.combatant.velocityVector, SpaceCombatManager.km_to_scale(this.missileTemplate.deltaV_kps), targetToCheck.position, targetToCheck.velocityVector, out impossible);
		}

		// Token: 0x06005CB4 RID: 23732 RVA: 0x002C2B1C File Offset: 0x002C0D1C
		public override bool TryFire(DateTime currentTime)
		{
			if (!base.TryFireCommon(currentTime))
			{
				return false;
			}
			ShipWeaponVisController shipWeaponVisController = base.SelectWeaponVisualization(this.targetedPosition);
			ProjectileController projectileController = base.combatant.combatMgr.SetProjectile(shipWeaponVisController);
			base.combatant.combatMgr._reverseProjectiles[projectileController].Fire(base.combatant.WeaponCarrierState, base.weaponTemplate.ref_missileWeapon, this.gameTime.currentTime, shipWeaponVisController.firePoint.transform.position, this.targetedPosition, base.combatant.velocityVector_kps);
			projectileController.Fire(shipWeaponVisController.firePoint.transform.position, this.targetedPosition, base.target);
			base.EnterCooldown(false, false, false, 0);
			TISpaceCombatProjectileState ref_projectile = base.target.ref_projectile;
			base.combatant.WeaponCarrierState.FireWeapon(base.weaponData, ref_projectile);
			base.SelectWeaponVisualization(this.targetedPosition).Fire(ref_projectile != null, null);
			return true;
		}

		// Token: 0x04004200 RID: 16896
		public TIMissileTemplate missileTemplate;
	}
}

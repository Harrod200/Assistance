using System;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Ship
{
	// Token: 0x0200097D RID: 2429
	public abstract class TIAttackFireMode
	{
		// Token: 0x17000FD3 RID: 4051
		// (get) Token: 0x06005C66 RID: 23654 RVA: 0x002C0D08 File Offset: 0x002BEF08
		// (set) Token: 0x06005C67 RID: 23655 RVA: 0x002C0D10 File Offset: 0x002BEF10
		public IWeapon weapon { get; protected set; }

		// Token: 0x06005C68 RID: 23656 RVA: 0x002C0D1C File Offset: 0x002BEF1C
		public float GetExpectedDamage(float distance_km, IDamageable target)
		{
			switch (this.weaponClass)
			{
			case WeaponClass.Laser:
				return this.weaponTemplate.ref_laserWeapon.EstimatedDamageAtRange_MJ(distance_km, target.GetCrossSectionalArea_m2(float.MaxValue), this.combatant.WeaponCarrierState) / 20f;
			case WeaponClass.Particle:
				return this.weaponTemplate.DamageAtRange_points(distance_km, target.GetCrossSectionalArea_m2(float.MaxValue), this.combatant.WeaponCarrierState, 0f, 0f, true).directDamage_Points;
			}
			return this.weaponTemplate.BaseDamageAtRange_points(distance_km, false);
		}

		// Token: 0x06005C69 RID: 23657 RVA: 0x002C0DB8 File Offset: 0x002BEFB8
		public float GetEfficientWeaponRange_km()
		{
			return this.weaponTemplate.targetingRange_km;
		}

		// Token: 0x06005C6A RID: 23658 RVA: 0x002C0DC8 File Offset: 0x002BEFC8
		protected float GetMinimumExpectedDamageToFire(CombatantController target)
		{
			CombatShipController combatShipController = target as CombatShipController;
			if (combatShipController != null && this.ship != null)
			{
				float num;
				ArmorFacing armorFacing = (this.ship.hull as Hull).BearingFacing(this.combatant.combatantTransform, combatShipController.combatantTransform, out num);
				TISpaceShipState.ArmorData armorData = combatShipController.ShipState.armor[armorFacing];
				if (armorData.armorValue == 0 || armorData.chippedPct > 0.2f)
				{
					return 0.15f;
				}
			}
			return 1f;
		}

		// Token: 0x040041E2 RID: 16866
		protected TIShipWeaponTemplate weaponTemplate;

		// Token: 0x040041E3 RID: 16867
		protected WeaponClass weaponClass;

		// Token: 0x040041E4 RID: 16868
		protected CombatantController combatant;

		// Token: 0x040041E5 RID: 16869
		protected Transform combatantTransform;

		// Token: 0x040041E6 RID: 16870
		protected Weapon weaponAsset;

		// Token: 0x040041E7 RID: 16871
		protected CombatShipController ship;

		// Token: 0x040041E8 RID: 16872
		protected float scaledTargetingRange;
	}
}

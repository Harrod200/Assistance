using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Ship
{
	// Token: 0x02000984 RID: 2436
	public class CollisionImpact : DamageSource
	{
		// Token: 0x06005CB5 RID: 23733 RVA: 0x002C2C1C File Offset: 0x002C0E1C
		public CollisionImpact(Vector3 hitPosition, CombatantController combatant1, CombatantController combatant2)
		{
			base.attacker = combatant1.WeaponCarrierState;
			base.hitPosition = hitPosition;
			Vector3 vector = SpaceCombatManager.scale_to_km_vec3(combatant1.velocityVector - combatant2.velocityVector);
			float num = 1f;
			float num2;
			if (combatant1.WeaponCarrierState.ref_shipCarrier() != null)
			{
				num2 = combatant1.WeaponCarrierState.ref_shipCarrier().currentMass_kg;
				if (combatant2.WeaponCarrierState.ref_shipCarrier() != null)
				{
					num = combatant1.WeaponCarrierState.ref_shipCarrier().hull.volume_m3 / combatant2.WeaponCarrierState.ref_shipCarrier().hull.volume_m3;
				}
			}
			else
			{
				TIHabState ref_hab = combatant1.WeaponCarrierState.ref_habModuleCarrier().ref_hab;
				num2 = combatant1.WeaponCarrierState.ref_habModuleCarrier().moduleTemplate.Mass_tons(ref_hab.irradiatedMultiplier, ref_hab.ref_spaceBody, ref_hab.ref_naturalSpaceObject, ref_hab.faction) * 1000f;
			}
			num = Mathf.Min(num, 1f);
			if (num < 0.5f)
			{
				num *= num;
			}
			if (num < 0.03f)
			{
				num *= num;
			}
			float num3 = CollisionImpact.KineticEnergyDamage_MJ(num2, vector.magnitude) / 20f;
			base.damage = new Damage(null, 0f, DamageType.Kinetic, num3 * 0.05f * num, num3 * 0.05f * num, 0, base.attacker.GetFaction());
		}

		// Token: 0x06005CB6 RID: 23734 RVA: 0x002C2D76 File Offset: 0x002C0F76
		public static float KineticEnergyDamage_MJ(float shipMass_kg, float finalVelocity_kps)
		{
			return 0.5f * shipMass_kg * Mathf.Pow(finalVelocity_kps * 1000f, 2f) * 1E-06f;
		}
	}
}

using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Ship
{
	// Token: 0x0200097E RID: 2430
	public class OffenseFireMode : TIAttackFireMode, IFireMode
	{
		// Token: 0x17000FD4 RID: 4052
		// (get) Token: 0x06005C6C RID: 23660 RVA: 0x002C0E50 File Offset: 0x002BF050
		public FireMode mode
		{
			get
			{
				return FireMode.Offense;
			}
		}

		// Token: 0x17000FD5 RID: 4053
		// (get) Token: 0x06005C6D RID: 23661 RVA: 0x002C0E53 File Offset: 0x002BF053
		public string displayName
		{
			get
			{
				if (!this.weaponTemplate.isMissileWeapon)
				{
					return Loc.T("UI.SpaceCombat.Offense");
				}
				return Loc.T("UI.SpaceCombat.MissileOffense");
			}
		}

		// Token: 0x17000FD6 RID: 4054
		// (get) Token: 0x06005C6E RID: 23662 RVA: 0x002C0E77 File Offset: 0x002BF077
		public string description
		{
			get
			{
				if (!this.weaponTemplate.isMissileWeapon)
				{
					return Loc.T("UI.SpaceCombat.Offense.description");
				}
				return Loc.T("UI.SpaceCombat.MissileOffense.description");
			}
		}

		// Token: 0x17000FD7 RID: 4055
		// (get) Token: 0x06005C6F RID: 23663 RVA: 0x002C0E9B File Offset: 0x002BF09B
		public string iconPath
		{
			get
			{
				if (!this.weaponTemplate.isMissileWeapon)
				{
					return "ui_spacecombat/BUT_mode_attack_red";
				}
				return "ui_spacecombat/BUT_mode_missileattack";
			}
		}

		// Token: 0x06005C70 RID: 23664 RVA: 0x002C0EB8 File Offset: 0x002BF0B8
		public OffenseFireMode(IWeapon weapon)
		{
			base.weapon = weapon;
			this.weaponAsset = weapon as Weapon;
			this.weaponTemplate = this.weaponAsset.weaponTemplate;
			this.weaponClass = this.weaponTemplate.weaponClass;
			this.combatant = weapon.combatant;
			this.combatantTransform = this.combatant.transform;
			if (this.combatant.GetCombatantType() == IDamageableType.Ship)
			{
				this.ship = this.combatant.ref_shipController;
			}
			this.scaledTargetingRange = SpaceCombatManager.km_to_scale(this.weaponTemplate.targetingRange_km);
		}

		// Token: 0x06005C71 RID: 23665 RVA: 0x002C0F54 File Offset: 0x002BF154
		public virtual IDamageable AcquireTarget(DateTime currentTime, out Vector3 targetPosition, out float distanceToTarget_km)
		{
			IDamageable damageable = null;
			targetPosition = Vector3.zero;
			CombatantController combatantController = null;
			distanceToTarget_km = float.MaxValue;
			if (this.ship != null)
			{
				combatantController = this.ship.primaryTarget;
			}
			Vector3 position = this.combatantTransform.position;
			if (combatantController != null && !combatantController.destructionTriggered)
			{
				bool flag;
				Vector3 positionToTarget = this.weaponAsset.GetPositionToTarget(combatantController, out flag);
				if (!flag && this.weaponAsset.InArc(positionToTarget, combatantController.velocityVector, combatantController.accelerationVector))
				{
					float num = Vector3.Distance(position, positionToTarget);
					distanceToTarget_km = SpaceCombatManager.scale_to_km(num);
					float expectedDamage = base.GetExpectedDamage(distanceToTarget_km, combatantController);
					float minimumExpectedDamageToFire = base.GetMinimumExpectedDamageToFire(combatantController);
					if (num <= this.scaledTargetingRange && expectedDamage >= minimumExpectedDamageToFire)
					{
						targetPosition = positionToTarget;
						return combatantController;
					}
				}
			}
			float num2 = float.MaxValue;
			foreach (CombatantController combatantController2 in this.combatant.enemyCombatants)
			{
				if (!combatantController2.destructionTriggered && (!this.weaponTemplate.isMissileWeapon || !combatantController2.isMissileSaturated))
				{
					bool flag2;
					Vector3 positionToTarget2 = this.weaponAsset.GetPositionToTarget(combatantController2, out flag2);
					if (!flag2 && this.weaponAsset.InArc(positionToTarget2, combatantController2.velocityVector, combatantController2.accelerationVector))
					{
						float num3 = Vector3.Distance(position, positionToTarget2);
						distanceToTarget_km = SpaceCombatManager.scale_to_km(num3);
						if (num3 <= this.scaledTargetingRange && num3 < num2 && base.GetExpectedDamage(distanceToTarget_km, combatantController2) >= base.GetMinimumExpectedDamageToFire(combatantController2))
						{
							num2 = num3;
							damageable = combatantController2;
							targetPosition = positionToTarget2;
						}
					}
				}
			}
			if (damageable != null)
			{
				distanceToTarget_km = SpaceCombatManager.scale_to_km(num2);
			}
			return damageable;
		}
	}
}

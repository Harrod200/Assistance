using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Ship
{
	// Token: 0x0200096D RID: 2413
	public class FocusFireMode : TIAttackFireMode, IFireMode
	{
		// Token: 0x17000FA0 RID: 4000
		// (get) Token: 0x06005BFC RID: 23548 RVA: 0x002C032E File Offset: 0x002BE52E
		public virtual string displayName
		{
			get
			{
				return Loc.T("UI.SpaceCombat.Focus");
			}
		}

		// Token: 0x17000FA1 RID: 4001
		// (get) Token: 0x06005BFD RID: 23549 RVA: 0x002C033A File Offset: 0x002BE53A
		public virtual string description
		{
			get
			{
				return Loc.T("UI.SpaceCombat.Focus.description");
			}
		}

		// Token: 0x17000FA2 RID: 4002
		// (get) Token: 0x06005BFE RID: 23550 RVA: 0x002C0346 File Offset: 0x002BE546
		public virtual string iconPath
		{
			get
			{
				return "ui_spacecombat/BUT_mode_focus_fire";
			}
		}

		// Token: 0x17000FA3 RID: 4003
		// (get) Token: 0x06005BFF RID: 23551 RVA: 0x002C034D File Offset: 0x002BE54D
		public virtual FireMode mode
		{
			get
			{
				return FireMode.Focus;
			}
		}

		// Token: 0x06005C00 RID: 23552 RVA: 0x002C0350 File Offset: 0x002BE550
		public FocusFireMode(IWeapon weapon)
		{
			base.weapon = weapon;
			this.combatant = weapon.combatant;
			this.weaponAsset = weapon as Weapon;
			this.weaponTemplate = this.weaponAsset.weaponTemplate;
			this.weaponClass = this.weaponTemplate.weaponClass;
			if (weapon.combatant.GetCombatantType() == IDamageableType.Ship)
			{
				this.ship = weapon.combatant.ref_shipController;
			}
			this.scaledTargetingRange = SpaceCombatManager.km_to_scale(this.weaponTemplate.targetingRange_km);
		}

		// Token: 0x06005C01 RID: 23553 RVA: 0x002C03D8 File Offset: 0x002BE5D8
		public IDamageable AcquireTarget(DateTime currentTime, out Vector3 targetPosition, out float distance_km)
		{
			IDamageable damageable = null;
			targetPosition = Vector3.zero;
			CombatantController combatantController = null;
			distance_km = float.MaxValue;
			if (this.ship != null)
			{
				combatantController = this.ship.primaryTarget;
			}
			if (combatantController != null && !combatantController.destructionTriggered)
			{
				bool flag;
				Vector3 positionToTarget = this.weaponAsset.GetPositionToTarget(combatantController, out flag);
				if (!flag && this.weaponAsset.InArc(positionToTarget, combatantController.velocityVector, combatantController.accelerationVector))
				{
					float num = Vector3.Distance(base.weapon.combatant.transform.position, positionToTarget);
					distance_km = SpaceCombatManager.scale_to_km(num);
					if (num <= this.scaledTargetingRange && base.GetExpectedDamage(distance_km, combatantController) >= base.GetMinimumExpectedDamageToFire(combatantController))
					{
						damageable = combatantController;
						targetPosition = positionToTarget;
					}
				}
			}
			return damageable;
		}
	}
}

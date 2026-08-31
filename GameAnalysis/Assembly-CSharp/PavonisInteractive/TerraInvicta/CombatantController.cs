using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta.Ship;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005B5 RID: 1461
	public abstract class CombatantController : MonoBehaviour, IDamageable
	{
		// Token: 0x06002791 RID: 10129
		public abstract SpaceCombatAssetUIController UIController();

		// Token: 0x17000582 RID: 1410
		// (get) Token: 0x06002792 RID: 10130 RVA: 0x000D8674 File Offset: 0x000D6874
		public bool isDestroyed
		{
			get
			{
				return this.destructionTriggered;
			}
		}

		// Token: 0x17000583 RID: 1411
		// (get) Token: 0x06002793 RID: 10131 RVA: 0x000D867C File Offset: 0x000D687C
		public Vector3 position
		{
			get
			{
				return this.GetDamageableTransform.position;
			}
		}

		// Token: 0x17000584 RID: 1412
		// (get) Token: 0x06002794 RID: 10132 RVA: 0x000D8689 File Offset: 0x000D6889
		public Vector3 localPosition
		{
			get
			{
				return this.GetDamageableTransform.localPosition;
			}
		}

		// Token: 0x06002795 RID: 10133
		public abstract CombatTargetableState GetCombatantState();

		// Token: 0x06002796 RID: 10134
		public abstract IDamageableType GetCombatantType();

		// Token: 0x06002797 RID: 10135
		public abstract Vector3 positionAtTime(DateTime currentTime);

		// Token: 0x06002798 RID: 10136
		public abstract float ApplyDamage(DamageSource source);

		// Token: 0x17000585 RID: 1413
		// (get) Token: 0x06002799 RID: 10137
		// (set) Token: 0x0600279A RID: 10138
		public abstract List<Collider> hitColliders { get; protected set; }

		// Token: 0x17000586 RID: 1414
		// (get) Token: 0x0600279B RID: 10139
		// (set) Token: 0x0600279C RID: 10140
		public abstract Vector3 velocityVector { get; protected set; }

		// Token: 0x17000587 RID: 1415
		// (get) Token: 0x0600279D RID: 10141
		public abstract Vector3 velocityVector_kps { get; }

		// Token: 0x17000588 RID: 1416
		// (get) Token: 0x0600279E RID: 10142
		public abstract IDamageableType damageableType { get; }

		// Token: 0x17000589 RID: 1417
		// (get) Token: 0x0600279F RID: 10143 RVA: 0x000D8696 File Offset: 0x000D6896
		Transform IDamageable.damageableTransform
		{
			get
			{
				return this.GetDamageableTransform;
			}
		}

		// Token: 0x1700058A RID: 1418
		// (get) Token: 0x060027A0 RID: 10144 RVA: 0x000D869E File Offset: 0x000D689E
		public TISpaceCombatProjectileState ref_projectile
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700058B RID: 1419
		// (get) Token: 0x060027A2 RID: 10146 RVA: 0x000D86AA File Offset: 0x000D68AA
		// (set) Token: 0x060027A1 RID: 10145 RVA: 0x000D86A1 File Offset: 0x000D68A1
		public CombatWeaponCarrierState WeaponCarrierState { get; protected set; }

		// Token: 0x1700058C RID: 1420
		// (get) Token: 0x060027A3 RID: 10147 RVA: 0x000D86B2 File Offset: 0x000D68B2
		public TIFactionState faction
		{
			get
			{
				CombatWeaponCarrierState weaponCarrierState = this.WeaponCarrierState;
				if (weaponCarrierState == null)
				{
					return null;
				}
				return weaponCarrierState.GetFaction();
			}
		}

		// Token: 0x1700058D RID: 1421
		// (get) Token: 0x060027A4 RID: 10148 RVA: 0x000D86C5 File Offset: 0x000D68C5
		// (set) Token: 0x060027A5 RID: 10149 RVA: 0x000D86CD File Offset: 0x000D68CD
		[HideInInspector]
		public bool destructionTriggered { get; protected set; }

		// Token: 0x060027A6 RID: 10150 RVA: 0x000D86D6 File Offset: 0x000D68D6
		public bool IsFriendlyTo(CombatantController combatant)
		{
			return this.alliedCombatants.Contains(combatant);
		}

		// Token: 0x1700058E RID: 1422
		// (get) Token: 0x060027A7 RID: 10151 RVA: 0x000D86E4 File Offset: 0x000D68E4
		// (set) Token: 0x060027A8 RID: 10152 RVA: 0x000D86EC File Offset: 0x000D68EC
		public Transform combatantTransform { get; protected set; }

		// Token: 0x1700058F RID: 1423
		// (get) Token: 0x060027A9 RID: 10153 RVA: 0x000D86F5 File Offset: 0x000D68F5
		// (set) Token: 0x060027AA RID: 10154 RVA: 0x000D86FD File Offset: 0x000D68FD
		public SpaceCombatManager combatMgr { get; protected set; }

		// Token: 0x17000590 RID: 1424
		// (get) Token: 0x060027AB RID: 10155 RVA: 0x000D8706 File Offset: 0x000D6906
		public virtual CombatShipController ref_shipController
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000591 RID: 1425
		// (get) Token: 0x060027AC RID: 10156 RVA: 0x000D8709 File Offset: 0x000D6909
		public virtual CombatHabModuleController ref_habModuleController
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000592 RID: 1426
		// (get) Token: 0x060027AD RID: 10157
		public abstract Vector3 accelerationVector { get; }

		// Token: 0x17000593 RID: 1427
		// (get) Token: 0x060027AE RID: 10158
		public abstract Vector3 accelerationVector_kps { get; }

		// Token: 0x17000594 RID: 1428
		// (get) Token: 0x060027AF RID: 10159
		public abstract CombatTargetableState combatTargetableState { get; }

		// Token: 0x17000595 RID: 1429
		// (get) Token: 0x060027B0 RID: 10160 RVA: 0x000D870C File Offset: 0x000D690C
		public virtual Transform GetDamageableTransform
		{
			get
			{
				return this.combatantTransform;
			}
		}

		// Token: 0x060027B1 RID: 10161
		public abstract float GetCrossSectionalArea_m2(float angle);

		// Token: 0x17000596 RID: 1430
		// (get) Token: 0x060027B2 RID: 10162 RVA: 0x000D8714 File Offset: 0x000D6914
		// (set) Token: 0x060027B3 RID: 10163 RVA: 0x000D871C File Offset: 0x000D691C
		public bool isMissileSaturated { get; protected set; }

		// Token: 0x060027B5 RID: 10165 RVA: 0x000D8738 File Offset: 0x000D6938
		Transform IDamageable.get_transform()
		{
			return base.transform;
		}

		// Token: 0x04001D71 RID: 7537
		[HideInInspector]
		public List<CombatantController> alliedCombatants;

		// Token: 0x04001D72 RID: 7538
		[HideInInspector]
		public List<CombatantController> enemyCombatants;

		// Token: 0x04001D76 RID: 7542
		public List<IDamageable> ECMDefeats = new List<IDamageable>();
	}
}

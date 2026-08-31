using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Ship
{
	// Token: 0x02000971 RID: 2417
	public interface IDamageable
	{
		// Token: 0x17000FAD RID: 4013
		// (get) Token: 0x06005C15 RID: 23573
		Vector3 position { get; }

		// Token: 0x17000FAE RID: 4014
		// (get) Token: 0x06005C16 RID: 23574
		bool isDestroyed { get; }

		// Token: 0x06005C17 RID: 23575
		Vector3 positionAtTime(DateTime timeToProject);

		// Token: 0x17000FAF RID: 4015
		// (get) Token: 0x06005C18 RID: 23576
		List<Collider> hitColliders { get; }

		// Token: 0x06005C19 RID: 23577
		float ApplyDamage(DamageSource source);

		// Token: 0x17000FB0 RID: 4016
		// (get) Token: 0x06005C1A RID: 23578
		CombatTargetableState combatTargetableState { get; }

		// Token: 0x17000FB1 RID: 4017
		// (get) Token: 0x06005C1B RID: 23579
		TISpaceCombatProjectileState ref_projectile { get; }

		// Token: 0x17000FB2 RID: 4018
		// (get) Token: 0x06005C1C RID: 23580
		Vector3 accelerationVector { get; }

		// Token: 0x17000FB3 RID: 4019
		// (get) Token: 0x06005C1D RID: 23581
		Vector3 accelerationVector_kps { get; }

		// Token: 0x17000FB4 RID: 4020
		// (get) Token: 0x06005C1E RID: 23582
		Vector3 velocityVector { get; }

		// Token: 0x17000FB5 RID: 4021
		// (get) Token: 0x06005C1F RID: 23583
		Vector3 velocityVector_kps { get; }

		// Token: 0x17000FB6 RID: 4022
		// (get) Token: 0x06005C20 RID: 23584
		IDamageableType damageableType { get; }

		// Token: 0x06005C21 RID: 23585
		float GetCrossSectionalArea_m2(float angle = 3.4028235E+38f);

		// Token: 0x17000FB7 RID: 4023
		// (get) Token: 0x06005C22 RID: 23586
		Transform damageableTransform { get; }

		// Token: 0x17000FB8 RID: 4024
		// (get) Token: 0x06005C23 RID: 23587
		Transform transform { get; }
	}
}

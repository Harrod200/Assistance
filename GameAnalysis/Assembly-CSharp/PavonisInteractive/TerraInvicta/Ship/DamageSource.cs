using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Ship
{
	// Token: 0x0200096F RID: 2415
	public abstract class DamageSource
	{
		// Token: 0x17000FA9 RID: 4009
		// (get) Token: 0x06005C0B RID: 23563 RVA: 0x002C062E File Offset: 0x002BE82E
		// (set) Token: 0x06005C0C RID: 23564 RVA: 0x002C0636 File Offset: 0x002BE836
		public CombatWeaponCarrierState attacker { get; protected set; }

		// Token: 0x17000FAA RID: 4010
		// (get) Token: 0x06005C0D RID: 23565 RVA: 0x002C063F File Offset: 0x002BE83F
		// (set) Token: 0x06005C0E RID: 23566 RVA: 0x002C0647 File Offset: 0x002BE847
		public Vector3 hitPosition { get; protected set; }

		// Token: 0x17000FAB RID: 4011
		// (get) Token: 0x06005C0F RID: 23567 RVA: 0x002C0650 File Offset: 0x002BE850
		// (set) Token: 0x06005C10 RID: 23568 RVA: 0x002C0658 File Offset: 0x002BE858
		public Damage damage { get; protected set; }

		// Token: 0x06005C11 RID: 23569 RVA: 0x002C0661 File Offset: 0x002BE861
		public DamageSource()
		{
		}
	}
}

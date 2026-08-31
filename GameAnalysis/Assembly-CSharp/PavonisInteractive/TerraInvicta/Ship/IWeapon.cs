using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Ship
{
	// Token: 0x02000973 RID: 2419
	public interface IWeapon : IComponent
	{
		// Token: 0x17000FBA RID: 4026
		// (get) Token: 0x06005C26 RID: 23590
		CombatantController combatant { get; }

		// Token: 0x17000FBB RID: 4027
		// (get) Token: 0x06005C27 RID: 23591
		Vector3 position { get; }

		// Token: 0x17000FBC RID: 4028
		// (get) Token: 0x06005C28 RID: 23592
		IList<IFireMode> fireModes { get; }

		// Token: 0x17000FBD RID: 4029
		// (get) Token: 0x06005C29 RID: 23593
		// (set) Token: 0x06005C2A RID: 23594
		IFireMode currentFireMode { get; set; }

		// Token: 0x17000FBE RID: 4030
		// (get) Token: 0x06005C2B RID: 23595
		IDamageable target { get; }

		// Token: 0x06005C2C RID: 23596
		bool TryFire(DateTime currentTime);

		// Token: 0x06005C2D RID: 23597
		bool AcquireTarget(DateTime currentTime);
	}
}

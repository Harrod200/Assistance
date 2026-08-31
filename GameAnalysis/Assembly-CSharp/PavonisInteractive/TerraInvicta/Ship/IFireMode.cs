using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Ship
{
	// Token: 0x0200097B RID: 2427
	public interface IFireMode
	{
		// Token: 0x17000FC9 RID: 4041
		// (get) Token: 0x06005C58 RID: 23640
		IWeapon weapon { get; }

		// Token: 0x06005C59 RID: 23641
		IDamageable AcquireTarget(DateTime currentTime, out Vector3 positionToTarget, out float distanceToTarget_km);

		// Token: 0x17000FCA RID: 4042
		// (get) Token: 0x06005C5A RID: 23642
		FireMode mode { get; }

		// Token: 0x17000FCB RID: 4043
		// (get) Token: 0x06005C5B RID: 23643
		string displayName { get; }

		// Token: 0x17000FCC RID: 4044
		// (get) Token: 0x06005C5C RID: 23644
		string description { get; }

		// Token: 0x17000FCD RID: 4045
		// (get) Token: 0x06005C5D RID: 23645
		string iconPath { get; }
	}
}

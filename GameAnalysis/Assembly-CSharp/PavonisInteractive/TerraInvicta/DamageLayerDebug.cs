using System;
using PavonisInteractive.TerraInvicta.Ship;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005AB RID: 1451
	public class DamageLayerDebug : MonoBehaviour
	{
		// Token: 0x06002773 RID: 10099 RVA: 0x000D7E7F File Offset: 0x000D607F
		private void Update()
		{
			if (!this._ship)
			{
				return;
			}
			this._ship.AddDamagePoint(base.transform.position, this.radius, this.damageType);
		}

		// Token: 0x04001D58 RID: 7512
		[SerializeField]
		private DamageLayer _ship;

		// Token: 0x04001D59 RID: 7513
		[SerializeField]
		[Range(0f, 50f)]
		private float radius = 1f;

		// Token: 0x04001D5A RID: 7514
		[SerializeField]
		private DamageType damageType = DamageType.Explosive;
	}
}

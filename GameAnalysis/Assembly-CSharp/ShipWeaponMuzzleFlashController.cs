using System;
using UnityEngine;

// Token: 0x02000034 RID: 52
public class ShipWeaponMuzzleFlashController : MonoBehaviour
{
	// Token: 0x060001F8 RID: 504 RVA: 0x0000FC9B File Offset: 0x0000DE9B
	public void Flash()
	{
		if (this._particleSystem == null)
		{
			this._particleSystem = base.gameObject.GetComponent<ParticleSystem>();
		}
		this._particleSystem.Emit(1);
	}

	// Token: 0x0400020A RID: 522
	private ParticleSystem _particleSystem;
}

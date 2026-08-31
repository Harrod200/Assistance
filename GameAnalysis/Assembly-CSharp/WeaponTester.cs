using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200040C RID: 1036
public class WeaponTester : MonoBehaviour
{
	// Token: 0x0600153D RID: 5437 RVA: 0x000683FF File Offset: 0x000665FF
	private void Start()
	{
	}

	// Token: 0x0600153E RID: 5438 RVA: 0x00068404 File Offset: 0x00066604
	private void Update()
	{
		if (this.testMuzzleFlash)
		{
			this.flashTimer -= Time.deltaTime;
			if (this.flashTimer <= 0f)
			{
				this.flashTimer = this.flashInterval;
				foreach (ShipWeaponMuzzleFlashController shipWeaponMuzzleFlashController in this.muzzleFlashControllers)
				{
					shipWeaponMuzzleFlashController.Flash();
				}
			}
		}
	}

	// Token: 0x040012AD RID: 4781
	[Header("Muzzle Flash Tester")]
	public bool testMuzzleFlash;

	// Token: 0x040012AE RID: 4782
	public float flashInterval = 1f;

	// Token: 0x040012AF RID: 4783
	public List<ShipWeaponMuzzleFlashController> muzzleFlashControllers = new List<ShipWeaponMuzzleFlashController>();

	// Token: 0x040012B0 RID: 4784
	private float flashTimer = 1f;
}

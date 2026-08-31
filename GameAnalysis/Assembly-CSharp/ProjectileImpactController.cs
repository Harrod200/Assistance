using System;
using UnityEngine;

// Token: 0x02000033 RID: 51
public class ProjectileImpactController : MonoBehaviour
{
	// Token: 0x060001F5 RID: 501 RVA: 0x0000FC57 File Offset: 0x0000DE57
	private void OnEnable()
	{
		if (!this._poolFlag)
		{
			this._poolFlag = true;
			return;
		}
		base.Invoke("Disable", this.life);
	}

	// Token: 0x060001F6 RID: 502 RVA: 0x0000FC7A File Offset: 0x0000DE7A
	private void Disable()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04000208 RID: 520
	public float life = 0.5f;

	// Token: 0x04000209 RID: 521
	private bool _poolFlag;
}

using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000904 RID: 2308
	public class TutorialPointerAnimator : MonoBehaviour
	{
		// Token: 0x06005859 RID: 22617 RVA: 0x0028840C File Offset: 0x0028660C
		private void Update()
		{
			float num = (Mathf.Sin(Time.time * this.oscillateSpeed) / 2f + 0.5f) * this.oscillateDistance;
			this.rt.offsetMin = new Vector2(-num, -num);
			this.rt.offsetMax = new Vector2(num, num);
		}

		// Token: 0x0400400F RID: 16399
		public RectTransform rt;

		// Token: 0x04004010 RID: 16400
		public float oscillateDistance = 4f;

		// Token: 0x04004011 RID: 16401
		public float oscillateSpeed = 5.5f;
	}
}

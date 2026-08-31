using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200054C RID: 1356
	public class TerrestrialUnitModel : MonoBehaviour
	{
		// Token: 0x170004F0 RID: 1264
		// (get) Token: 0x060022C0 RID: 8896 RVA: 0x000B43D0 File Offset: 0x000B25D0
		public Animator Animator
		{
			get
			{
				if (!this.animatorInitialized)
				{
					this.animator = base.GetComponent<Animator>();
					this.animatorInitialized = true;
				}
				return this.animator;
			}
		}

		// Token: 0x060022C1 RID: 8897 RVA: 0x000B43F3 File Offset: 0x000B25F3
		private void Start()
		{
			this.OnFire = (TerrestrialUnitModel.OnFireDelegate)Delegate.Combine(this.OnFire, new TerrestrialUnitModel.OnFireDelegate(delegate
			{
				if (this.FireEffectPrefab == null)
				{
					return;
				}
				global::UnityEngine.Object.Instantiate<GameObject>(this.FireEffectPrefab, this.FirePosition).transform.localPosition = Vector3.zero;
			}));
		}

		// Token: 0x060022C2 RID: 8898 RVA: 0x000B4417 File Offset: 0x000B2617
		public void AnimationEvent_Fire()
		{
			if (this != null && this.OnFire != null)
			{
				this.OnFire();
			}
		}

		// Token: 0x04001A5F RID: 6751
		private Animator animator;

		// Token: 0x04001A60 RID: 6752
		private bool animatorInitialized;

		// Token: 0x04001A61 RID: 6753
		public Transform FirePosition;

		// Token: 0x04001A62 RID: 6754
		public GameObject FireEffectPrefab;

		// Token: 0x04001A63 RID: 6755
		private TerrestrialUnitModel.OnFireDelegate OnFire;

		// Token: 0x02000CC6 RID: 3270
		// (Invoke) Token: 0x06006DE1 RID: 28129
		public delegate void OnFireDelegate();
	}
}

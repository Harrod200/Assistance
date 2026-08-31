using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200070D RID: 1805
	public class RadiatorVisController : MonoBehaviour
	{
		// Token: 0x06002AE6 RID: 10982 RVA: 0x000E8AAD File Offset: 0x000E6CAD
		public void OnRadiatorRepaired()
		{
			this.intactRadiatorModel.SetActive(true);
			this.destroyedRadiatorModel.SetActive(false);
		}

		// Token: 0x06002AE7 RID: 10983 RVA: 0x000E8AC8 File Offset: 0x000E6CC8
		public void OnRadiatorDestroyed(bool radiatorsRetracted)
		{
			this.intactRadiatorModel.SetActive(false);
			if ((this.showDestroyedRetractedRadiator && radiatorsRetracted) || !radiatorsRetracted)
			{
				this.destroyedRadiatorModel.SetActive(true);
			}
			this.explosionParticles = global::UnityEngine.Object.Instantiate<GameObject>(this.explosionPrefab, this.intactRadiatorModel.transform.position, new Quaternion(0f, 0f, 0f, 1f), base.transform).GetComponent<ParticleSystem>();
			float num = this.explosionScale / this.explosionParticles.transform.parent.transform.localScale.x;
			this.explosionParticles.transform.localScale = new Vector3(num, num, num);
		}

		// Token: 0x06002AE8 RID: 10984 RVA: 0x000E8B7E File Offset: 0x000E6D7E
		public void OnPlay()
		{
			if (this.explosionParticles != null)
			{
				this.explosionParticles.Play();
			}
		}

		// Token: 0x06002AE9 RID: 10985 RVA: 0x000E8B99 File Offset: 0x000E6D99
		public void OnPause()
		{
			if (this.explosionParticles != null)
			{
				this.explosionParticles.Pause();
			}
		}

		// Token: 0x040020D3 RID: 8403
		public GameObject intactRadiatorModel;

		// Token: 0x040020D4 RID: 8404
		public GameObject destroyedRadiatorModel;

		// Token: 0x040020D5 RID: 8405
		public GameObject explosionPrefab;

		// Token: 0x040020D6 RID: 8406
		public bool showDestroyedRetractedRadiator;

		// Token: 0x040020D7 RID: 8407
		private ParticleSystem explosionParticles;

		// Token: 0x040020D8 RID: 8408
		private float explosionScale = 2.5f;
	}
}

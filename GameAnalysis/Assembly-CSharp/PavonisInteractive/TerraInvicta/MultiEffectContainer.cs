using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200058A RID: 1418
	public struct MultiEffectContainer
	{
		// Token: 0x060025B4 RID: 9652 RVA: 0x000CC750 File Offset: 0x000CA950
		public MultiEffectContainer(List<ParticleSystem> particleSystems)
		{
			this.effects = new List<ParticleSystem>(particleSystems);
		}

		// Token: 0x060025B5 RID: 9653 RVA: 0x000CC760 File Offset: 0x000CA960
		public void Play()
		{
			for (int i = 0; i < this.effects.Count; i++)
			{
				if (this.effects[i].isStopped)
				{
					this.effects[i].Play();
				}
			}
		}

		// Token: 0x060025B6 RID: 9654 RVA: 0x000CC7A8 File Offset: 0x000CA9A8
		public void Stop()
		{
			for (int i = 0; i < this.effects.Count; i++)
			{
				if (this.effects[i] != null)
				{
					this.effects[i].Stop();
				}
			}
		}

		// Token: 0x04001C20 RID: 7200
		public List<ParticleSystem> effects;
	}
}

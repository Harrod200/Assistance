using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005B0 RID: 1456
	[Obsolete]
	public class LightScaler : MonoBehaviour
	{
		// Token: 0x06002780 RID: 10112 RVA: 0x000D8300 File Offset: 0x000D6500
		private void Awake()
		{
			this.lights = new List<LightScaler.LightScale>();
			this.originalScale = base.transform.lossyScale.magnitude;
			foreach (Light light in base.GetComponentsInChildren<Light>())
			{
				this.lights.Add(new LightScaler.LightScale
				{
					light = light,
					originalRange = light.range
				});
			}
		}

		// Token: 0x06002781 RID: 10113 RVA: 0x000D8374 File Offset: 0x000D6574
		private void Update()
		{
			float magnitude = base.transform.lossyScale.magnitude;
			if (magnitude != this.lastScale)
			{
				float num = magnitude / this.originalScale;
				foreach (LightScaler.LightScale lightScale in this.lights)
				{
					float num2 = lightScale.originalRange * num;
					lightScale.light.range = num2;
					lightScale.light.enabled = num2 > 0.01f;
				}
				this.lastScale = magnitude;
			}
		}

		// Token: 0x04001D65 RID: 7525
		private List<LightScaler.LightScale> lights;

		// Token: 0x04001D66 RID: 7526
		private float originalScale;

		// Token: 0x04001D67 RID: 7527
		private float lastScale;

		// Token: 0x02000D07 RID: 3335
		private struct LightScale
		{
			// Token: 0x04005041 RID: 20545
			public Light light;

			// Token: 0x04005042 RID: 20546
			public float originalRange;
		}
	}
}

using System;
using System.Collections;
using UnityEngine;

namespace TMPro.Examples
{
	// Token: 0x02000503 RID: 1283
	public class ShaderPropAnimator : MonoBehaviour
	{
		// Token: 0x06001FBE RID: 8126 RVA: 0x000A4EB4 File Offset: 0x000A30B4
		private void Awake()
		{
			this.m_Renderer = base.GetComponent<Renderer>();
			this.m_Material = this.m_Renderer.material;
		}

		// Token: 0x06001FBF RID: 8127 RVA: 0x000A4ED3 File Offset: 0x000A30D3
		private void Start()
		{
			base.StartCoroutine(this.AnimateProperties());
		}

		// Token: 0x06001FC0 RID: 8128 RVA: 0x000A4EE2 File Offset: 0x000A30E2
		private IEnumerator AnimateProperties()
		{
			this.m_frame = global::UnityEngine.Random.Range(0f, 1f);
			for (;;)
			{
				float num = this.GlowCurve.Evaluate(this.m_frame);
				this.m_Material.SetFloat(ShaderUtilities.ID_GlowPower, num);
				this.m_frame += Time.deltaTime * global::UnityEngine.Random.Range(0.2f, 0.3f);
				yield return new WaitForEndOfFrame();
			}
			yield break;
		}

		// Token: 0x04001872 RID: 6258
		private Renderer m_Renderer;

		// Token: 0x04001873 RID: 6259
		private Material m_Material;

		// Token: 0x04001874 RID: 6260
		public AnimationCurve GlowCurve;

		// Token: 0x04001875 RID: 6261
		public float m_frame;
	}
}

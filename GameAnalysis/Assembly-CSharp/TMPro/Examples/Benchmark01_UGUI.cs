using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TMPro.Examples
{
	// Token: 0x020004FD RID: 1277
	public class Benchmark01_UGUI : MonoBehaviour
	{
		// Token: 0x06001FAD RID: 8109 RVA: 0x000A3FE5 File Offset: 0x000A21E5
		private IEnumerator Start()
		{
			if (this.BenchmarkType == 0)
			{
				this.m_textMeshPro = base.gameObject.AddComponent<TextMeshProUGUI>();
				if (this.TMProFont != null)
				{
					this.m_textMeshPro.font = this.TMProFont;
				}
				this.m_textMeshPro.fontSize = 48f;
				this.m_textMeshPro.alignment = TextAlignmentOptions.Center;
				this.m_textMeshPro.extraPadding = true;
				this.m_material01 = this.m_textMeshPro.font.material;
				this.m_material02 = Resources.Load<Material>("Fonts & Materials/LiberationSans SDF - BEVEL");
			}
			else if (this.BenchmarkType == 1)
			{
				this.m_textMesh = base.gameObject.AddComponent<Text>();
				if (this.TextMeshFont != null)
				{
					this.m_textMesh.font = this.TextMeshFont;
				}
				this.m_textMesh.fontSize = 48;
				this.m_textMesh.alignment = TextAnchor.MiddleCenter;
			}
			int num;
			for (int i = 0; i <= 1000000; i = num + 1)
			{
				if (this.BenchmarkType == 0)
				{
					this.m_textMeshPro.text = "The <#0050FF>count is: </color>" + (i % 1000).ToString();
					if (i % 1000 == 999)
					{
						this.m_textMeshPro.fontSharedMaterial = ((this.m_textMeshPro.fontSharedMaterial == this.m_material01) ? (this.m_textMeshPro.fontSharedMaterial = this.m_material02) : (this.m_textMeshPro.fontSharedMaterial = this.m_material01));
					}
				}
				else if (this.BenchmarkType == 1)
				{
					this.m_textMesh.text = "The <color=#0050FF>count is: </color>" + (i % 1000).ToString();
				}
				yield return null;
				num = i;
			}
			yield return null;
			yield break;
		}

		// Token: 0x04001839 RID: 6201
		public int BenchmarkType;

		// Token: 0x0400183A RID: 6202
		public Canvas canvas;

		// Token: 0x0400183B RID: 6203
		public TMP_FontAsset TMProFont;

		// Token: 0x0400183C RID: 6204
		public Font TextMeshFont;

		// Token: 0x0400183D RID: 6205
		private TextMeshProUGUI m_textMeshPro;

		// Token: 0x0400183E RID: 6206
		private Text m_textMesh;

		// Token: 0x0400183F RID: 6207
		private const string label01 = "The <#0050FF>count is: </color>";

		// Token: 0x04001840 RID: 6208
		private const string label02 = "The <color=#0050FF>count is: </color>";

		// Token: 0x04001841 RID: 6209
		private Material m_material01;

		// Token: 0x04001842 RID: 6210
		private Material m_material02;
	}
}

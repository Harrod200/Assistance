using System;
using UnityEngine;

namespace TMPro.Examples
{
	// Token: 0x02000504 RID: 1284
	public class SimpleScript : MonoBehaviour
	{
		// Token: 0x06001FC2 RID: 8130 RVA: 0x000A4EFC File Offset: 0x000A30FC
		private void Start()
		{
			this.m_textMeshPro = base.gameObject.AddComponent<TextMeshPro>();
			this.m_textMeshPro.autoSizeTextContainer = true;
			this.m_textMeshPro.fontSize = 48f;
			this.m_textMeshPro.alignment = TextAlignmentOptions.Center;
			this.m_textMeshPro.enableWordWrapping = false;
		}

		// Token: 0x06001FC3 RID: 8131 RVA: 0x000A4F52 File Offset: 0x000A3152
		private void Update()
		{
			this.m_textMeshPro.SetText("The <#0050FF>count is: </color>{0:2}", this.m_frame % 1000f);
			this.m_frame += 1f * Time.deltaTime;
		}

		// Token: 0x04001876 RID: 6262
		private TextMeshPro m_textMeshPro;

		// Token: 0x04001877 RID: 6263
		private const string label = "The <#0050FF>count is: </color>{0:2}";

		// Token: 0x04001878 RID: 6264
		private float m_frame;
	}
}

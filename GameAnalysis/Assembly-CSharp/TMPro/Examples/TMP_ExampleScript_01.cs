using System;
using UnityEngine;

namespace TMPro.Examples
{
	// Token: 0x02000506 RID: 1286
	public class TMP_ExampleScript_01 : MonoBehaviour
	{
		// Token: 0x06001FCA RID: 8138 RVA: 0x000A507C File Offset: 0x000A327C
		private void Awake()
		{
			if (this.ObjectType == TMP_ExampleScript_01.objectType.TextMeshPro)
			{
				this.m_text = base.GetComponent<TextMeshPro>() ?? base.gameObject.AddComponent<TextMeshPro>();
			}
			else
			{
				this.m_text = base.GetComponent<TextMeshProUGUI>() ?? base.gameObject.AddComponent<TextMeshProUGUI>();
			}
			this.m_text.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/Anton SDF");
			this.m_text.fontSharedMaterial = Resources.Load<Material>("Fonts & Materials/Anton SDF - Drop Shadow");
			this.m_text.fontSize = 120f;
			this.m_text.text = "A <#0080ff>simple</color> line of text.";
			Vector2 preferredValues = this.m_text.GetPreferredValues(float.PositiveInfinity, float.PositiveInfinity);
			this.m_text.rectTransform.sizeDelta = new Vector2(preferredValues.x, preferredValues.y);
		}

		// Token: 0x06001FCB RID: 8139 RVA: 0x000A514A File Offset: 0x000A334A
		private void Update()
		{
			if (!this.isStatic)
			{
				this.m_text.SetText("The count is <#0080ff>{0}</color>", (float)(this.count % 1000));
				this.count++;
			}
		}

		// Token: 0x0400187D RID: 6269
		public TMP_ExampleScript_01.objectType ObjectType;

		// Token: 0x0400187E RID: 6270
		public bool isStatic;

		// Token: 0x0400187F RID: 6271
		private TMP_Text m_text;

		// Token: 0x04001880 RID: 6272
		private const string k_label = "The count is <#0080ff>{0}</color>";

		// Token: 0x04001881 RID: 6273
		private int count;

		// Token: 0x02000C7A RID: 3194
		public enum objectType
		{
			// Token: 0x04004E83 RID: 20099
			TextMeshPro,
			// Token: 0x04004E84 RID: 20100
			TextMeshProUGUI
		}
	}
}

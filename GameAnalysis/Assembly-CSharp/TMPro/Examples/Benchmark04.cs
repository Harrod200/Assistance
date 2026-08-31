using System;
using UnityEngine;

namespace TMPro.Examples
{
	// Token: 0x02000500 RID: 1280
	public class Benchmark04 : MonoBehaviour
	{
		// Token: 0x06001FB4 RID: 8116 RVA: 0x000A43E8 File Offset: 0x000A25E8
		private void Start()
		{
			this.m_Transform = base.transform;
			float num = 0f;
			float num2 = (Camera.main.orthographicSize = (float)(Screen.height / 2));
			float num3 = (float)Screen.width / (float)Screen.height;
			for (int i = this.MinPointSize; i <= this.MaxPointSize; i += this.Steps)
			{
				if (this.SpawnType == 0)
				{
					GameObject gameObject = new GameObject("Text - " + i.ToString() + " Pts");
					if (num > num2 * 2f)
					{
						return;
					}
					gameObject.transform.position = this.m_Transform.position + new Vector3(num3 * -num2 * 0.975f, num2 * 0.975f - num, 0f);
					TextMeshPro textMeshPro = gameObject.AddComponent<TextMeshPro>();
					textMeshPro.rectTransform.pivot = new Vector2(0f, 0.5f);
					textMeshPro.enableWordWrapping = false;
					textMeshPro.extraPadding = true;
					textMeshPro.isOrthographic = true;
					textMeshPro.fontSize = (float)i;
					textMeshPro.text = i.ToString() + " pts - Lorem ipsum dolor sit...";
					textMeshPro.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
					num += (float)i;
				}
			}
		}

		// Token: 0x04001849 RID: 6217
		public int SpawnType;

		// Token: 0x0400184A RID: 6218
		public int MinPointSize = 12;

		// Token: 0x0400184B RID: 6219
		public int MaxPointSize = 64;

		// Token: 0x0400184C RID: 6220
		public int Steps = 4;

		// Token: 0x0400184D RID: 6221
		private Transform m_Transform;
	}
}

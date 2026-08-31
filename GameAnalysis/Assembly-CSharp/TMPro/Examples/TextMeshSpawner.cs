using System;
using UnityEngine;

namespace TMPro.Examples
{
	// Token: 0x0200050F RID: 1295
	public class TextMeshSpawner : MonoBehaviour
	{
		// Token: 0x06001FF8 RID: 8184 RVA: 0x000A65A4 File Offset: 0x000A47A4
		private void Awake()
		{
		}

		// Token: 0x06001FF9 RID: 8185 RVA: 0x000A65A8 File Offset: 0x000A47A8
		private void Start()
		{
			for (int i = 0; i < this.NumberOfNPC; i++)
			{
				if (this.SpawnType == 0)
				{
					GameObject gameObject = new GameObject();
					gameObject.transform.position = new Vector3(global::UnityEngine.Random.Range(-95f, 95f), 0.5f, global::UnityEngine.Random.Range(-95f, 95f));
					TextMeshPro textMeshPro = gameObject.AddComponent<TextMeshPro>();
					textMeshPro.fontSize = 96f;
					textMeshPro.text = "!";
					textMeshPro.color = new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue);
					this.floatingText_Script = gameObject.AddComponent<TextMeshProFloatingText>();
					this.floatingText_Script.SpawnType = 0;
				}
				else
				{
					GameObject gameObject2 = new GameObject();
					gameObject2.transform.position = new Vector3(global::UnityEngine.Random.Range(-95f, 95f), 0.5f, global::UnityEngine.Random.Range(-95f, 95f));
					TextMesh textMesh = gameObject2.AddComponent<TextMesh>();
					textMesh.GetComponent<Renderer>().sharedMaterial = this.TheFont.material;
					textMesh.font = this.TheFont;
					textMesh.anchor = TextAnchor.LowerCenter;
					textMesh.fontSize = 96;
					textMesh.color = new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue);
					textMesh.text = "!";
					this.floatingText_Script = gameObject2.AddComponent<TextMeshProFloatingText>();
					this.floatingText_Script.SpawnType = 1;
				}
			}
		}

		// Token: 0x040018B1 RID: 6321
		public int SpawnType;

		// Token: 0x040018B2 RID: 6322
		public int NumberOfNPC = 12;

		// Token: 0x040018B3 RID: 6323
		public Font TheFont;

		// Token: 0x040018B4 RID: 6324
		private TextMeshProFloatingText floatingText_Script;
	}
}

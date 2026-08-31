using System;
using UnityEngine;

namespace TMPro.Examples
{
	// Token: 0x020004FF RID: 1279
	public class Benchmark03 : MonoBehaviour
	{
		// Token: 0x06001FB1 RID: 8113 RVA: 0x000A42B6 File Offset: 0x000A24B6
		private void Awake()
		{
		}

		// Token: 0x06001FB2 RID: 8114 RVA: 0x000A42B8 File Offset: 0x000A24B8
		private void Start()
		{
			for (int i = 0; i < this.NumberOfNPC; i++)
			{
				if (this.SpawnType == 0)
				{
					TextMeshPro textMeshPro = new GameObject
					{
						transform = 
						{
							position = new Vector3(0f, 0f, 0f)
						}
					}.AddComponent<TextMeshPro>();
					textMeshPro.alignment = TextAlignmentOptions.Center;
					textMeshPro.fontSize = 96f;
					textMeshPro.text = "@";
					textMeshPro.color = new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue);
				}
				else
				{
					TextMesh textMesh = new GameObject
					{
						transform = 
						{
							position = new Vector3(0f, 0f, 0f)
						}
					}.AddComponent<TextMesh>();
					textMesh.GetComponent<Renderer>().sharedMaterial = this.TheFont.material;
					textMesh.font = this.TheFont;
					textMesh.anchor = TextAnchor.MiddleCenter;
					textMesh.fontSize = 96;
					textMesh.color = new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue);
					textMesh.text = "@";
				}
			}
		}

		// Token: 0x04001846 RID: 6214
		public int SpawnType;

		// Token: 0x04001847 RID: 6215
		public int NumberOfNPC = 12;

		// Token: 0x04001848 RID: 6216
		public Font TheFont;
	}
}

using System;
using System.Collections;
using UnityEngine;

namespace TMPro.Examples
{
	// Token: 0x02000511 RID: 1297
	public class VertexJitter : MonoBehaviour
	{
		// Token: 0x06001FFF RID: 8191 RVA: 0x000A675E File Offset: 0x000A495E
		private void Awake()
		{
			this.m_TextComponent = base.GetComponent<TMP_Text>();
		}

		// Token: 0x06002000 RID: 8192 RVA: 0x000A676C File Offset: 0x000A496C
		private void OnEnable()
		{
			TMPro_EventManager.TEXT_CHANGED_EVENT.Add(new Action<global::UnityEngine.Object>(this.ON_TEXT_CHANGED));
		}

		// Token: 0x06002001 RID: 8193 RVA: 0x000A6784 File Offset: 0x000A4984
		private void OnDisable()
		{
			TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(new Action<global::UnityEngine.Object>(this.ON_TEXT_CHANGED));
		}

		// Token: 0x06002002 RID: 8194 RVA: 0x000A679C File Offset: 0x000A499C
		private void Start()
		{
			base.StartCoroutine(this.AnimateVertexColors());
		}

		// Token: 0x06002003 RID: 8195 RVA: 0x000A67AB File Offset: 0x000A49AB
		private void ON_TEXT_CHANGED(global::UnityEngine.Object obj)
		{
			if (obj == this.m_TextComponent)
			{
				this.hasTextChanged = true;
			}
		}

		// Token: 0x06002004 RID: 8196 RVA: 0x000A67C2 File Offset: 0x000A49C2
		private IEnumerator AnimateVertexColors()
		{
			this.m_TextComponent.ForceMeshUpdate(false, false);
			TMP_TextInfo textInfo = this.m_TextComponent.textInfo;
			int loopCount = 0;
			this.hasTextChanged = true;
			VertexJitter.VertexAnim[] vertexAnim = new VertexJitter.VertexAnim[1024];
			for (int i = 0; i < 1024; i++)
			{
				vertexAnim[i].angleRange = global::UnityEngine.Random.Range(10f, 25f);
				vertexAnim[i].speed = global::UnityEngine.Random.Range(1f, 3f);
			}
			TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
			for (;;)
			{
				if (this.hasTextChanged)
				{
					cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
					this.hasTextChanged = false;
				}
				int characterCount = textInfo.characterCount;
				if (characterCount == 0)
				{
					yield return new WaitForSeconds(0.25f);
				}
				else
				{
					for (int j = 0; j < characterCount; j++)
					{
						if (textInfo.characterInfo[j].isVisible)
						{
							VertexJitter.VertexAnim vertexAnim2 = vertexAnim[j];
							int materialReferenceIndex = textInfo.characterInfo[j].materialReferenceIndex;
							int vertexIndex = textInfo.characterInfo[j].vertexIndex;
							Vector3[] vertices = cachedMeshInfo[materialReferenceIndex].vertices;
							Vector3 vector = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2f;
							Vector3[] vertices2 = textInfo.meshInfo[materialReferenceIndex].vertices;
							vertices2[vertexIndex] = vertices[vertexIndex] - vector;
							vertices2[vertexIndex + 1] = vertices[vertexIndex + 1] - vector;
							vertices2[vertexIndex + 2] = vertices[vertexIndex + 2] - vector;
							vertices2[vertexIndex + 3] = vertices[vertexIndex + 3] - vector;
							vertexAnim2.angle = Mathf.SmoothStep(-vertexAnim2.angleRange, vertexAnim2.angleRange, Mathf.PingPong((float)loopCount / 25f * vertexAnim2.speed, 1f));
							Matrix4x4 matrix4x = Matrix4x4.TRS(new Vector3(global::UnityEngine.Random.Range(-0.25f, 0.25f), global::UnityEngine.Random.Range(-0.25f, 0.25f), 0f) * this.CurveScale, Quaternion.Euler(0f, 0f, global::UnityEngine.Random.Range(-5f, 5f) * this.AngleMultiplier), Vector3.one);
							vertices2[vertexIndex] = matrix4x.MultiplyPoint3x4(vertices2[vertexIndex]);
							vertices2[vertexIndex + 1] = matrix4x.MultiplyPoint3x4(vertices2[vertexIndex + 1]);
							vertices2[vertexIndex + 2] = matrix4x.MultiplyPoint3x4(vertices2[vertexIndex + 2]);
							vertices2[vertexIndex + 3] = matrix4x.MultiplyPoint3x4(vertices2[vertexIndex + 3]);
							vertices2[vertexIndex] += vector;
							vertices2[vertexIndex + 1] += vector;
							vertices2[vertexIndex + 2] += vector;
							vertices2[vertexIndex + 3] += vector;
							vertexAnim[j] = vertexAnim2;
						}
					}
					for (int k = 0; k < textInfo.meshInfo.Length; k++)
					{
						textInfo.meshInfo[k].mesh.vertices = textInfo.meshInfo[k].vertices;
						this.m_TextComponent.UpdateGeometry(textInfo.meshInfo[k].mesh, k);
					}
					loopCount++;
					yield return new WaitForSeconds(0.1f);
				}
			}
			yield break;
		}

		// Token: 0x040018B6 RID: 6326
		public float AngleMultiplier = 1f;

		// Token: 0x040018B7 RID: 6327
		public float SpeedMultiplier = 1f;

		// Token: 0x040018B8 RID: 6328
		public float CurveScale = 1f;

		// Token: 0x040018B9 RID: 6329
		private TMP_Text m_TextComponent;

		// Token: 0x040018BA RID: 6330
		private bool hasTextChanged;

		// Token: 0x02000C84 RID: 3204
		private struct VertexAnim
		{
			// Token: 0x04004EC0 RID: 20160
			public float angleRange;

			// Token: 0x04004EC1 RID: 20161
			public float angle;

			// Token: 0x04004EC2 RID: 20162
			public float speed;
		}
	}
}

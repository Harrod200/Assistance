using System;
using System.Collections;
using UnityEngine;

namespace TMPro.Examples
{
	// Token: 0x02000513 RID: 1299
	public class VertexShakeB : MonoBehaviour
	{
		// Token: 0x0600200D RID: 8205 RVA: 0x000A68A3 File Offset: 0x000A4AA3
		private void Awake()
		{
			this.m_TextComponent = base.GetComponent<TMP_Text>();
		}

		// Token: 0x0600200E RID: 8206 RVA: 0x000A68B1 File Offset: 0x000A4AB1
		private void OnEnable()
		{
			TMPro_EventManager.TEXT_CHANGED_EVENT.Add(new Action<global::UnityEngine.Object>(this.ON_TEXT_CHANGED));
		}

		// Token: 0x0600200F RID: 8207 RVA: 0x000A68C9 File Offset: 0x000A4AC9
		private void OnDisable()
		{
			TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(new Action<global::UnityEngine.Object>(this.ON_TEXT_CHANGED));
		}

		// Token: 0x06002010 RID: 8208 RVA: 0x000A68E1 File Offset: 0x000A4AE1
		private void Start()
		{
			base.StartCoroutine(this.AnimateVertexColors());
		}

		// Token: 0x06002011 RID: 8209 RVA: 0x000A68F0 File Offset: 0x000A4AF0
		private void ON_TEXT_CHANGED(global::UnityEngine.Object obj)
		{
			if (this.m_TextComponent)
			{
				this.hasTextChanged = true;
			}
		}

		// Token: 0x06002012 RID: 8210 RVA: 0x000A6909 File Offset: 0x000A4B09
		private IEnumerator AnimateVertexColors()
		{
			this.m_TextComponent.ForceMeshUpdate(false, false);
			TMP_TextInfo textInfo = this.m_TextComponent.textInfo;
			Vector3[][] copyOfVertices = new Vector3[0][];
			this.hasTextChanged = true;
			for (;;)
			{
				if (this.hasTextChanged)
				{
					if (copyOfVertices.Length < textInfo.meshInfo.Length)
					{
						copyOfVertices = new Vector3[textInfo.meshInfo.Length][];
					}
					for (int i = 0; i < textInfo.meshInfo.Length; i++)
					{
						int num = textInfo.meshInfo[i].vertices.Length;
						copyOfVertices[i] = new Vector3[num];
					}
					this.hasTextChanged = false;
				}
				if (textInfo.characterCount == 0)
				{
					yield return new WaitForSeconds(0.25f);
				}
				else
				{
					int lineCount = textInfo.lineCount;
					for (int j = 0; j < lineCount; j++)
					{
						int firstCharacterIndex = textInfo.lineInfo[j].firstCharacterIndex;
						int lastCharacterIndex = textInfo.lineInfo[j].lastCharacterIndex;
						Vector3 vector = (textInfo.characterInfo[firstCharacterIndex].bottomLeft + textInfo.characterInfo[lastCharacterIndex].topRight) / 2f;
						Quaternion quaternion = Quaternion.Euler(0f, 0f, global::UnityEngine.Random.Range(-0.25f, 0.25f));
						for (int k = firstCharacterIndex; k <= lastCharacterIndex; k++)
						{
							if (textInfo.characterInfo[k].isVisible)
							{
								int materialReferenceIndex = textInfo.characterInfo[k].materialReferenceIndex;
								int vertexIndex = textInfo.characterInfo[k].vertexIndex;
								Vector3[] vertices = textInfo.meshInfo[materialReferenceIndex].vertices;
								Vector3 vector2 = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2f;
								copyOfVertices[materialReferenceIndex][vertexIndex] = vertices[vertexIndex] - vector2;
								copyOfVertices[materialReferenceIndex][vertexIndex + 1] = vertices[vertexIndex + 1] - vector2;
								copyOfVertices[materialReferenceIndex][vertexIndex + 2] = vertices[vertexIndex + 2] - vector2;
								copyOfVertices[materialReferenceIndex][vertexIndex + 3] = vertices[vertexIndex + 3] - vector2;
								float num2 = global::UnityEngine.Random.Range(0.95f, 1.05f);
								Matrix4x4 matrix4x = Matrix4x4.TRS(Vector3.one, Quaternion.identity, Vector3.one * num2);
								copyOfVertices[materialReferenceIndex][vertexIndex] = matrix4x.MultiplyPoint3x4(copyOfVertices[materialReferenceIndex][vertexIndex]);
								copyOfVertices[materialReferenceIndex][vertexIndex + 1] = matrix4x.MultiplyPoint3x4(copyOfVertices[materialReferenceIndex][vertexIndex + 1]);
								copyOfVertices[materialReferenceIndex][vertexIndex + 2] = matrix4x.MultiplyPoint3x4(copyOfVertices[materialReferenceIndex][vertexIndex + 2]);
								copyOfVertices[materialReferenceIndex][vertexIndex + 3] = matrix4x.MultiplyPoint3x4(copyOfVertices[materialReferenceIndex][vertexIndex + 3]);
								copyOfVertices[materialReferenceIndex][vertexIndex] += vector2;
								copyOfVertices[materialReferenceIndex][vertexIndex + 1] += vector2;
								copyOfVertices[materialReferenceIndex][vertexIndex + 2] += vector2;
								copyOfVertices[materialReferenceIndex][vertexIndex + 3] += vector2;
								copyOfVertices[materialReferenceIndex][vertexIndex] -= vector;
								copyOfVertices[materialReferenceIndex][vertexIndex + 1] -= vector;
								copyOfVertices[materialReferenceIndex][vertexIndex + 2] -= vector;
								copyOfVertices[materialReferenceIndex][vertexIndex + 3] -= vector;
								matrix4x = Matrix4x4.TRS(Vector3.one, quaternion, Vector3.one);
								copyOfVertices[materialReferenceIndex][vertexIndex] = matrix4x.MultiplyPoint3x4(copyOfVertices[materialReferenceIndex][vertexIndex]);
								copyOfVertices[materialReferenceIndex][vertexIndex + 1] = matrix4x.MultiplyPoint3x4(copyOfVertices[materialReferenceIndex][vertexIndex + 1]);
								copyOfVertices[materialReferenceIndex][vertexIndex + 2] = matrix4x.MultiplyPoint3x4(copyOfVertices[materialReferenceIndex][vertexIndex + 2]);
								copyOfVertices[materialReferenceIndex][vertexIndex + 3] = matrix4x.MultiplyPoint3x4(copyOfVertices[materialReferenceIndex][vertexIndex + 3]);
								copyOfVertices[materialReferenceIndex][vertexIndex] += vector;
								copyOfVertices[materialReferenceIndex][vertexIndex + 1] += vector;
								copyOfVertices[materialReferenceIndex][vertexIndex + 2] += vector;
								copyOfVertices[materialReferenceIndex][vertexIndex + 3] += vector;
							}
						}
					}
					for (int l = 0; l < textInfo.meshInfo.Length; l++)
					{
						textInfo.meshInfo[l].mesh.vertices = copyOfVertices[l];
						this.m_TextComponent.UpdateGeometry(textInfo.meshInfo[l].mesh, l);
					}
					yield return new WaitForSeconds(0.1f);
				}
			}
			yield break;
		}

		// Token: 0x040018C1 RID: 6337
		public float AngleMultiplier = 1f;

		// Token: 0x040018C2 RID: 6338
		public float SpeedMultiplier = 1f;

		// Token: 0x040018C3 RID: 6339
		public float CurveScale = 1f;

		// Token: 0x040018C4 RID: 6340
		private TMP_Text m_TextComponent;

		// Token: 0x040018C5 RID: 6341
		private bool hasTextChanged;
	}
}

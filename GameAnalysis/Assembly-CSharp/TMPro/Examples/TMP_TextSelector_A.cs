using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TMPro.Examples
{
	// Token: 0x02000509 RID: 1289
	public class TMP_TextSelector_A : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x06001FDB RID: 8155 RVA: 0x000A584B File Offset: 0x000A3A4B
		private void Awake()
		{
			this.m_TextMeshPro = base.gameObject.GetComponent<TextMeshPro>();
			this.m_Camera = Camera.main;
			this.m_TextMeshPro.ForceMeshUpdate(false, false);
		}

		// Token: 0x06001FDC RID: 8156 RVA: 0x000A5878 File Offset: 0x000A3A78
		private void LateUpdate()
		{
			this.m_isHoveringObject = false;
			if (TMP_TextUtilities.IsIntersectingRectTransform(this.m_TextMeshPro.rectTransform, Input.mousePosition, Camera.main))
			{
				this.m_isHoveringObject = true;
			}
			if (this.m_isHoveringObject)
			{
				int num = TMP_TextUtilities.FindIntersectingCharacter(this.m_TextMeshPro, Input.mousePosition, Camera.main, true);
				if (num != -1 && num != this.m_lastCharIndex && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
				{
					this.m_lastCharIndex = num;
					int materialReferenceIndex = this.m_TextMeshPro.textInfo.characterInfo[num].materialReferenceIndex;
					int vertexIndex = this.m_TextMeshPro.textInfo.characterInfo[num].vertexIndex;
					Color32 color = new Color32((byte)global::UnityEngine.Random.Range(0, 255), (byte)global::UnityEngine.Random.Range(0, 255), (byte)global::UnityEngine.Random.Range(0, 255), byte.MaxValue);
					Color32[] colors = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].colors32;
					colors[vertexIndex] = color;
					colors[vertexIndex + 1] = color;
					colors[vertexIndex + 2] = color;
					colors[vertexIndex + 3] = color;
					this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].mesh.colors32 = colors;
				}
				int num2 = TMP_TextUtilities.FindIntersectingLink(this.m_TextMeshPro, Input.mousePosition, this.m_Camera);
				if ((num2 == -1 && this.m_selectedLink != -1) || num2 != this.m_selectedLink)
				{
					this.m_selectedLink = -1;
				}
				if (num2 != -1 && num2 != this.m_selectedLink)
				{
					this.m_selectedLink = num2;
					TMP_LinkInfo tmp_LinkInfo = this.m_TextMeshPro.textInfo.linkInfo[num2];
					Debug.Log(string.Concat(new string[]
					{
						"Link ID: \"",
						tmp_LinkInfo.GetLinkID(),
						"\"   Link Text: \"",
						tmp_LinkInfo.GetLinkText(),
						"\""
					}));
					Vector3 zero = Vector3.zero;
					RectTransformUtility.ScreenPointToWorldPointInRectangle(this.m_TextMeshPro.rectTransform, Input.mousePosition, this.m_Camera, out zero);
					string linkID = tmp_LinkInfo.GetLinkID();
					if (linkID != null && !(linkID == "id_01"))
					{
						linkID == "id_02";
					}
				}
				int num3 = TMP_TextUtilities.FindIntersectingWord(this.m_TextMeshPro, Input.mousePosition, Camera.main);
				if (num3 != -1 && num3 != this.m_lastWordIndex)
				{
					this.m_lastWordIndex = num3;
					TMP_WordInfo tmp_WordInfo = this.m_TextMeshPro.textInfo.wordInfo[num3];
					Vector3 vector = this.m_TextMeshPro.transform.TransformPoint(this.m_TextMeshPro.textInfo.characterInfo[tmp_WordInfo.firstCharacterIndex].bottomLeft);
					vector = Camera.main.WorldToScreenPoint(vector);
					Color32[] colors2 = this.m_TextMeshPro.textInfo.meshInfo[0].colors32;
					Color32 color2 = new Color32((byte)global::UnityEngine.Random.Range(0, 255), (byte)global::UnityEngine.Random.Range(0, 255), (byte)global::UnityEngine.Random.Range(0, 255), byte.MaxValue);
					for (int i = 0; i < tmp_WordInfo.characterCount; i++)
					{
						int vertexIndex2 = this.m_TextMeshPro.textInfo.characterInfo[tmp_WordInfo.firstCharacterIndex + i].vertexIndex;
						colors2[vertexIndex2] = color2;
						colors2[vertexIndex2 + 1] = color2;
						colors2[vertexIndex2 + 2] = color2;
						colors2[vertexIndex2 + 3] = color2;
					}
					this.m_TextMeshPro.mesh.colors32 = colors2;
				}
			}
		}

		// Token: 0x06001FDD RID: 8157 RVA: 0x000A5C2A File Offset: 0x000A3E2A
		public void OnPointerEnter(PointerEventData eventData)
		{
			Debug.Log("OnPointerEnter()");
			this.m_isHoveringObject = true;
		}

		// Token: 0x06001FDE RID: 8158 RVA: 0x000A5C3D File Offset: 0x000A3E3D
		public void OnPointerExit(PointerEventData eventData)
		{
			Debug.Log("OnPointerExit()");
			this.m_isHoveringObject = false;
		}

		// Token: 0x0400188D RID: 6285
		private TextMeshPro m_TextMeshPro;

		// Token: 0x0400188E RID: 6286
		private Camera m_Camera;

		// Token: 0x0400188F RID: 6287
		private bool m_isHoveringObject;

		// Token: 0x04001890 RID: 6288
		private int m_selectedLink = -1;

		// Token: 0x04001891 RID: 6289
		private int m_lastCharIndex = -1;

		// Token: 0x04001892 RID: 6290
		private int m_lastWordIndex = -1;
	}
}

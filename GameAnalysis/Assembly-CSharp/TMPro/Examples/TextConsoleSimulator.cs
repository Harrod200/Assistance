using System;
using System.Collections;
using UnityEngine;

namespace TMPro.Examples
{
	// Token: 0x0200050D RID: 1293
	public class TextConsoleSimulator : MonoBehaviour
	{
		// Token: 0x06001FEB RID: 8171 RVA: 0x000A628E File Offset: 0x000A448E
		private void Awake()
		{
			this.m_TextComponent = base.gameObject.GetComponent<TMP_Text>();
		}

		// Token: 0x06001FEC RID: 8172 RVA: 0x000A62A1 File Offset: 0x000A44A1
		private void Start()
		{
			base.StartCoroutine(this.RevealCharacters(this.m_TextComponent));
		}

		// Token: 0x06001FED RID: 8173 RVA: 0x000A62B6 File Offset: 0x000A44B6
		private void OnEnable()
		{
			TMPro_EventManager.TEXT_CHANGED_EVENT.Add(new Action<global::UnityEngine.Object>(this.ON_TEXT_CHANGED));
		}

		// Token: 0x06001FEE RID: 8174 RVA: 0x000A62CE File Offset: 0x000A44CE
		private void OnDisable()
		{
			TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(new Action<global::UnityEngine.Object>(this.ON_TEXT_CHANGED));
		}

		// Token: 0x06001FEF RID: 8175 RVA: 0x000A62E6 File Offset: 0x000A44E6
		private void ON_TEXT_CHANGED(global::UnityEngine.Object obj)
		{
			this.hasTextChanged = true;
		}

		// Token: 0x06001FF0 RID: 8176 RVA: 0x000A62EF File Offset: 0x000A44EF
		private IEnumerator RevealCharacters(TMP_Text textComponent)
		{
			textComponent.ForceMeshUpdate(false, false);
			TMP_TextInfo textInfo = textComponent.textInfo;
			int totalVisibleCharacters = textInfo.characterCount;
			int visibleCount = 0;
			for (;;)
			{
				if (this.hasTextChanged)
				{
					totalVisibleCharacters = textInfo.characterCount;
					this.hasTextChanged = false;
				}
				if (visibleCount > totalVisibleCharacters)
				{
					yield return new WaitForSeconds(1f);
					visibleCount = 0;
				}
				textComponent.maxVisibleCharacters = visibleCount;
				visibleCount++;
				yield return null;
			}
			yield break;
		}

		// Token: 0x06001FF1 RID: 8177 RVA: 0x000A6305 File Offset: 0x000A4505
		private IEnumerator RevealWords(TMP_Text textComponent)
		{
			textComponent.ForceMeshUpdate(false, false);
			int totalWordCount = textComponent.textInfo.wordCount;
			int totalVisibleCharacters = textComponent.textInfo.characterCount;
			int counter = 0;
			int visibleCount = 0;
			for (;;)
			{
				int num = counter % (totalWordCount + 1);
				if (num == 0)
				{
					visibleCount = 0;
				}
				else if (num < totalWordCount)
				{
					visibleCount = textComponent.textInfo.wordInfo[num - 1].lastCharacterIndex + 1;
				}
				else if (num == totalWordCount)
				{
					visibleCount = totalVisibleCharacters;
				}
				textComponent.maxVisibleCharacters = visibleCount;
				if (visibleCount >= totalVisibleCharacters)
				{
					yield return new WaitForSeconds(1f);
				}
				counter++;
				yield return new WaitForSeconds(0.1f);
			}
			yield break;
		}

		// Token: 0x040018A5 RID: 6309
		private TMP_Text m_TextComponent;

		// Token: 0x040018A6 RID: 6310
		private bool hasTextChanged;
	}
}

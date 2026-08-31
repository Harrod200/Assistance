using System;
using UnityEngine;
using UnityEngine.Events;

namespace TMPro.Examples
{
	// Token: 0x02000508 RID: 1288
	public class TMP_TextEventCheck : MonoBehaviour
	{
		// Token: 0x06001FD3 RID: 8147 RVA: 0x000A5570 File Offset: 0x000A3770
		private void OnEnable()
		{
			if (this.TextEventHandler != null)
			{
				this.TextEventHandler.onCharacterSelection.AddListener(new UnityAction<char, int>(this.OnCharacterSelection));
				this.TextEventHandler.onSpriteSelection.AddListener(new UnityAction<char, int>(this.OnSpriteSelection));
				this.TextEventHandler.onWordSelection.AddListener(new UnityAction<string, int, int>(this.OnWordSelection));
				this.TextEventHandler.onLineSelection.AddListener(new UnityAction<string, int, int>(this.OnLineSelection));
				this.TextEventHandler.onLinkSelection.AddListener(new UnityAction<string, string, int>(this.OnLinkSelection));
			}
		}

		// Token: 0x06001FD4 RID: 8148 RVA: 0x000A561C File Offset: 0x000A381C
		private void OnDisable()
		{
			if (this.TextEventHandler != null)
			{
				this.TextEventHandler.onCharacterSelection.RemoveListener(new UnityAction<char, int>(this.OnCharacterSelection));
				this.TextEventHandler.onSpriteSelection.RemoveListener(new UnityAction<char, int>(this.OnSpriteSelection));
				this.TextEventHandler.onWordSelection.RemoveListener(new UnityAction<string, int, int>(this.OnWordSelection));
				this.TextEventHandler.onLineSelection.RemoveListener(new UnityAction<string, int, int>(this.OnLineSelection));
				this.TextEventHandler.onLinkSelection.RemoveListener(new UnityAction<string, string, int>(this.OnLinkSelection));
			}
		}

		// Token: 0x06001FD5 RID: 8149 RVA: 0x000A56C6 File Offset: 0x000A38C6
		private void OnCharacterSelection(char c, int index)
		{
			Debug.Log(string.Concat(new string[]
			{
				"Character [",
				c.ToString(),
				"] at Index: ",
				index.ToString(),
				" has been selected."
			}));
		}

		// Token: 0x06001FD6 RID: 8150 RVA: 0x000A5704 File Offset: 0x000A3904
		private void OnSpriteSelection(char c, int index)
		{
			Debug.Log(string.Concat(new string[]
			{
				"Sprite [",
				c.ToString(),
				"] at Index: ",
				index.ToString(),
				" has been selected."
			}));
		}

		// Token: 0x06001FD7 RID: 8151 RVA: 0x000A5744 File Offset: 0x000A3944
		private void OnWordSelection(string word, int firstCharacterIndex, int length)
		{
			Debug.Log(string.Concat(new string[]
			{
				"Word [",
				word,
				"] with first character index of ",
				firstCharacterIndex.ToString(),
				" and length of ",
				length.ToString(),
				" has been selected."
			}));
		}

		// Token: 0x06001FD8 RID: 8152 RVA: 0x000A579C File Offset: 0x000A399C
		private void OnLineSelection(string lineText, int firstCharacterIndex, int length)
		{
			Debug.Log(string.Concat(new string[]
			{
				"Line [",
				lineText,
				"] with first character index of ",
				firstCharacterIndex.ToString(),
				" and length of ",
				length.ToString(),
				" has been selected."
			}));
		}

		// Token: 0x06001FD9 RID: 8153 RVA: 0x000A57F4 File Offset: 0x000A39F4
		private void OnLinkSelection(string linkID, string linkText, int linkIndex)
		{
			Debug.Log(string.Concat(new string[]
			{
				"Link Index: ",
				linkIndex.ToString(),
				" with ID [",
				linkID,
				"] and Text \"",
				linkText,
				"\" has been selected."
			}));
		}

		// Token: 0x0400188C RID: 6284
		public TMP_TextEventHandler TextEventHandler;
	}
}

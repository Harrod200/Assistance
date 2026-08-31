using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TMPro
{
	// Token: 0x020004FB RID: 1275
	public class TMP_TextEventHandler : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x17000473 RID: 1139
		// (get) Token: 0x06001F97 RID: 8087 RVA: 0x000A3BC4 File Offset: 0x000A1DC4
		// (set) Token: 0x06001F98 RID: 8088 RVA: 0x000A3BCC File Offset: 0x000A1DCC
		public TMP_TextEventHandler.CharacterSelectionEvent onCharacterSelection
		{
			get
			{
				return this.m_OnCharacterSelection;
			}
			set
			{
				this.m_OnCharacterSelection = value;
			}
		}

		// Token: 0x17000474 RID: 1140
		// (get) Token: 0x06001F99 RID: 8089 RVA: 0x000A3BD5 File Offset: 0x000A1DD5
		// (set) Token: 0x06001F9A RID: 8090 RVA: 0x000A3BDD File Offset: 0x000A1DDD
		public TMP_TextEventHandler.SpriteSelectionEvent onSpriteSelection
		{
			get
			{
				return this.m_OnSpriteSelection;
			}
			set
			{
				this.m_OnSpriteSelection = value;
			}
		}

		// Token: 0x17000475 RID: 1141
		// (get) Token: 0x06001F9B RID: 8091 RVA: 0x000A3BE6 File Offset: 0x000A1DE6
		// (set) Token: 0x06001F9C RID: 8092 RVA: 0x000A3BEE File Offset: 0x000A1DEE
		public TMP_TextEventHandler.WordSelectionEvent onWordSelection
		{
			get
			{
				return this.m_OnWordSelection;
			}
			set
			{
				this.m_OnWordSelection = value;
			}
		}

		// Token: 0x17000476 RID: 1142
		// (get) Token: 0x06001F9D RID: 8093 RVA: 0x000A3BF7 File Offset: 0x000A1DF7
		// (set) Token: 0x06001F9E RID: 8094 RVA: 0x000A3BFF File Offset: 0x000A1DFF
		public TMP_TextEventHandler.LineSelectionEvent onLineSelection
		{
			get
			{
				return this.m_OnLineSelection;
			}
			set
			{
				this.m_OnLineSelection = value;
			}
		}

		// Token: 0x17000477 RID: 1143
		// (get) Token: 0x06001F9F RID: 8095 RVA: 0x000A3C08 File Offset: 0x000A1E08
		// (set) Token: 0x06001FA0 RID: 8096 RVA: 0x000A3C10 File Offset: 0x000A1E10
		public TMP_TextEventHandler.LinkSelectionEvent onLinkSelection
		{
			get
			{
				return this.m_OnLinkSelection;
			}
			set
			{
				this.m_OnLinkSelection = value;
			}
		}

		// Token: 0x06001FA1 RID: 8097 RVA: 0x000A3C1C File Offset: 0x000A1E1C
		private void Awake()
		{
			this.m_TextComponent = base.gameObject.GetComponent<TMP_Text>();
			if (this.m_TextComponent.GetType() == typeof(TextMeshProUGUI))
			{
				this.m_Canvas = base.gameObject.GetComponentInParent<Canvas>();
				if (this.m_Canvas != null)
				{
					if (this.m_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
					{
						this.m_Camera = null;
						return;
					}
					this.m_Camera = this.m_Canvas.worldCamera;
					return;
				}
			}
			else
			{
				this.m_Camera = Camera.main;
			}
		}

		// Token: 0x06001FA2 RID: 8098 RVA: 0x000A3CA8 File Offset: 0x000A1EA8
		private void LateUpdate()
		{
			if (TMP_TextUtilities.IsIntersectingRectTransform(this.m_TextComponent.rectTransform, Input.mousePosition, this.m_Camera))
			{
				int num = TMP_TextUtilities.FindIntersectingCharacter(this.m_TextComponent, Input.mousePosition, this.m_Camera, true);
				if (num != -1 && num != this.m_lastCharIndex)
				{
					this.m_lastCharIndex = num;
					TMP_TextElementType elementType = this.m_TextComponent.textInfo.characterInfo[num].elementType;
					if (elementType == TMP_TextElementType.Character)
					{
						this.SendOnCharacterSelection(this.m_TextComponent.textInfo.characterInfo[num].character, num);
					}
					else if (elementType == TMP_TextElementType.Sprite)
					{
						this.SendOnSpriteSelection(this.m_TextComponent.textInfo.characterInfo[num].character, num);
					}
				}
				int num2 = TMP_TextUtilities.FindIntersectingWord(this.m_TextComponent, Input.mousePosition, this.m_Camera);
				if (num2 != -1 && num2 != this.m_lastWordIndex)
				{
					this.m_lastWordIndex = num2;
					TMP_WordInfo tmp_WordInfo = this.m_TextComponent.textInfo.wordInfo[num2];
					this.SendOnWordSelection(tmp_WordInfo.GetWord(), tmp_WordInfo.firstCharacterIndex, tmp_WordInfo.characterCount);
				}
				int num3 = TMP_TextUtilities.FindIntersectingLine(this.m_TextComponent, Input.mousePosition, this.m_Camera);
				if (num3 != -1 && num3 != this.m_lastLineIndex)
				{
					this.m_lastLineIndex = num3;
					TMP_LineInfo tmp_LineInfo = this.m_TextComponent.textInfo.lineInfo[num3];
					char[] array = new char[tmp_LineInfo.characterCount];
					int num4 = 0;
					while (num4 < tmp_LineInfo.characterCount && num4 < this.m_TextComponent.textInfo.characterInfo.Length)
					{
						array[num4] = this.m_TextComponent.textInfo.characterInfo[num4 + tmp_LineInfo.firstCharacterIndex].character;
						num4++;
					}
					string text = new string(array);
					this.SendOnLineSelection(text, tmp_LineInfo.firstCharacterIndex, tmp_LineInfo.characterCount);
				}
				int num5 = TMP_TextUtilities.FindIntersectingLink(this.m_TextComponent, Input.mousePosition, this.m_Camera);
				if (num5 != -1 && num5 != this.m_selectedLink)
				{
					this.m_selectedLink = num5;
					TMP_LinkInfo tmp_LinkInfo = this.m_TextComponent.textInfo.linkInfo[num5];
					this.SendOnLinkSelection(tmp_LinkInfo.GetLinkID(), tmp_LinkInfo.GetLinkText(), num5);
				}
			}
		}

		// Token: 0x06001FA3 RID: 8099 RVA: 0x000A3EEE File Offset: 0x000A20EE
		public void OnPointerEnter(PointerEventData eventData)
		{
		}

		// Token: 0x06001FA4 RID: 8100 RVA: 0x000A3EF0 File Offset: 0x000A20F0
		public void OnPointerExit(PointerEventData eventData)
		{
		}

		// Token: 0x06001FA5 RID: 8101 RVA: 0x000A3EF2 File Offset: 0x000A20F2
		private void SendOnCharacterSelection(char character, int characterIndex)
		{
			if (this.onCharacterSelection != null)
			{
				this.onCharacterSelection.Invoke(character, characterIndex);
			}
		}

		// Token: 0x06001FA6 RID: 8102 RVA: 0x000A3F09 File Offset: 0x000A2109
		private void SendOnSpriteSelection(char character, int characterIndex)
		{
			if (this.onSpriteSelection != null)
			{
				this.onSpriteSelection.Invoke(character, characterIndex);
			}
		}

		// Token: 0x06001FA7 RID: 8103 RVA: 0x000A3F20 File Offset: 0x000A2120
		private void SendOnWordSelection(string word, int charIndex, int length)
		{
			if (this.onWordSelection != null)
			{
				this.onWordSelection.Invoke(word, charIndex, length);
			}
		}

		// Token: 0x06001FA8 RID: 8104 RVA: 0x000A3F38 File Offset: 0x000A2138
		private void SendOnLineSelection(string line, int charIndex, int length)
		{
			if (this.onLineSelection != null)
			{
				this.onLineSelection.Invoke(line, charIndex, length);
			}
		}

		// Token: 0x06001FA9 RID: 8105 RVA: 0x000A3F50 File Offset: 0x000A2150
		private void SendOnLinkSelection(string linkID, string linkText, int linkIndex)
		{
			if (this.onLinkSelection != null)
			{
				this.onLinkSelection.Invoke(linkID, linkText, linkIndex);
			}
		}

		// Token: 0x04001823 RID: 6179
		[SerializeField]
		private TMP_TextEventHandler.CharacterSelectionEvent m_OnCharacterSelection = new TMP_TextEventHandler.CharacterSelectionEvent();

		// Token: 0x04001824 RID: 6180
		[SerializeField]
		private TMP_TextEventHandler.SpriteSelectionEvent m_OnSpriteSelection = new TMP_TextEventHandler.SpriteSelectionEvent();

		// Token: 0x04001825 RID: 6181
		[SerializeField]
		private TMP_TextEventHandler.WordSelectionEvent m_OnWordSelection = new TMP_TextEventHandler.WordSelectionEvent();

		// Token: 0x04001826 RID: 6182
		[SerializeField]
		private TMP_TextEventHandler.LineSelectionEvent m_OnLineSelection = new TMP_TextEventHandler.LineSelectionEvent();

		// Token: 0x04001827 RID: 6183
		[SerializeField]
		private TMP_TextEventHandler.LinkSelectionEvent m_OnLinkSelection = new TMP_TextEventHandler.LinkSelectionEvent();

		// Token: 0x04001828 RID: 6184
		private TMP_Text m_TextComponent;

		// Token: 0x04001829 RID: 6185
		private Camera m_Camera;

		// Token: 0x0400182A RID: 6186
		private Canvas m_Canvas;

		// Token: 0x0400182B RID: 6187
		private int m_selectedLink = -1;

		// Token: 0x0400182C RID: 6188
		private int m_lastCharIndex = -1;

		// Token: 0x0400182D RID: 6189
		private int m_lastWordIndex = -1;

		// Token: 0x0400182E RID: 6190
		private int m_lastLineIndex = -1;

		// Token: 0x02000C6F RID: 3183
		[Serializable]
		public class CharacterSelectionEvent : UnityEvent<char, int>
		{
		}

		// Token: 0x02000C70 RID: 3184
		[Serializable]
		public class SpriteSelectionEvent : UnityEvent<char, int>
		{
		}

		// Token: 0x02000C71 RID: 3185
		[Serializable]
		public class WordSelectionEvent : UnityEvent<string, int, int>
		{
		}

		// Token: 0x02000C72 RID: 3186
		[Serializable]
		public class LineSelectionEvent : UnityEvent<string, int, int>
		{
		}

		// Token: 0x02000C73 RID: 3187
		[Serializable]
		public class LinkSelectionEvent : UnityEvent<string, string, int>
		{
		}
	}
}

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LapinerTools.uMyGUI
{
	// Token: 0x0200052A RID: 1322
	public class uMyGUI_PopupText : uMyGUI_PopupButtons
	{
		// Token: 0x060020E2 RID: 8418 RVA: 0x000AA394 File Offset: 0x000A8594
		public virtual uMyGUI_PopupText SetText(string p_headerText, string p_bodyText)
		{
			if (this.m_header != null)
			{
				this.m_header.text = p_headerText;
			}
			if (this.m_body != null)
			{
				this.m_body.text = p_bodyText;
			}
			return this;
		}

		// Token: 0x060020E3 RID: 8419 RVA: 0x000AA3CB File Offset: 0x000A85CB
		public override void Show()
		{
			base.Show();
			this.m_isFirstFrameShown = true;
		}

		// Token: 0x060020E4 RID: 8420 RVA: 0x000AA3DC File Offset: 0x000A85DC
		public virtual void LateUpdate()
		{
			if (this.m_isFirstFrameShown)
			{
				this.m_isFirstFrameShown = false;
				if (this.m_useExplicitNavigation)
				{
					List<Button> list = new List<Button>();
					for (int i = 0; i < this.m_buttons.Length; i++)
					{
						if (this.m_buttons[i] != null && this.m_buttons[i].gameObject.activeSelf && this.m_buttons[i].GetComponentInChildren<Button>() != null)
						{
							list.Add(this.m_buttons[i].GetComponentInChildren<Button>());
						}
					}
					for (int j = 0; j < list.Count; j++)
					{
						Button button = list[j];
						Navigation navigation = button.navigation;
						navigation.mode = Navigation.Mode.Explicit;
						if (j > 0)
						{
							navigation.selectOnLeft = list[j - 1];
						}
						if (j < list.Count - 1)
						{
							navigation.selectOnRight = list[j + 1];
						}
						button.navigation = navigation;
					}
				}
			}
		}

		// Token: 0x04001961 RID: 6497
		[SerializeField]
		protected TMP_Text m_header;

		// Token: 0x04001962 RID: 6498
		[SerializeField]
		protected TMP_Text m_body;

		// Token: 0x04001963 RID: 6499
		[SerializeField]
		protected bool m_useExplicitNavigation;

		// Token: 0x04001964 RID: 6500
		protected bool m_isFirstFrameShown;
	}
}

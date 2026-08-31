using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LapinerTools.uMyGUI
{
	// Token: 0x02000527 RID: 1319
	public class uMyGUI_PopupButtons : uMyGUI_Popup
	{
		// Token: 0x060020BF RID: 8383 RVA: 0x000A9A02 File Offset: 0x000A7C02
		public override void Show()
		{
			if (this.m_isClosing)
			{
				this.Hide();
				this.m_isCloseCanceled = true;
			}
			this.LoadLocalizedText();
			base.Show();
		}

		// Token: 0x060020C0 RID: 8384 RVA: 0x000A9A28 File Offset: 0x000A7C28
		public override void Hide()
		{
			base.Hide();
			for (int i = 0; i < this.m_buttons.Length; i++)
			{
				if (this.m_buttons[i] != null)
				{
					this.m_buttons[i].gameObject.SetActive(false);
				}
			}
			this.m_onBtnClickCallbacks.Clear();
		}

		// Token: 0x060020C1 RID: 8385 RVA: 0x000A9A7C File Offset: 0x000A7C7C
		public virtual uMyGUI_PopupButtons ShowButton(string p_buttonName)
		{
			return this.ShowButton(p_buttonName, null);
		}

		// Token: 0x060020C2 RID: 8386 RVA: 0x000A9A88 File Offset: 0x000A7C88
		public virtual uMyGUI_PopupButtons ShowButton(string p_buttonName, Action p_callback)
		{
			for (int i = 0; i < this.m_buttons.Length; i++)
			{
				if (this.m_buttons[i] != null && this.m_buttonNames[i] == p_buttonName)
				{
					this.m_buttons[i].gameObject.SetActive(true);
					if (this.m_improveNavigationFocus)
					{
						Selectable componentInChildren = this.m_buttons[i].GetComponentInChildren<Selectable>();
						if (componentInChildren != null)
						{
							componentInChildren.Select();
						}
					}
					if (p_callback != null)
					{
						this.m_onBtnClickCallbacks.Add(p_buttonName, p_callback);
					}
					return this;
				}
			}
			Debug.LogError("uMyGUI_PopupButtons: ShowButton: could not find button with name '" + p_buttonName + "'!");
			return this;
		}

		// Token: 0x060020C3 RID: 8387 RVA: 0x000A9B28 File Offset: 0x000A7D28
		public virtual void OnButtonClick(RectTransform p_btn)
		{
			this.m_isClosing = true;
			this.m_isCloseCanceled = false;
			int i = 0;
			while (i < this.m_buttons.Length)
			{
				if (this.m_buttons[i] == p_btn)
				{
					Action action;
					if (this.m_onBtnClickCallbacks.TryGetValue(this.m_buttonNames[i], out action))
					{
						action();
						break;
					}
					break;
				}
				else
				{
					i++;
				}
			}
			this.m_isClosing = false;
			if (!this.m_isCloseCanceled)
			{
				this.Hide();
			}
		}

		// Token: 0x060020C4 RID: 8388 RVA: 0x000A9B9C File Offset: 0x000A7D9C
		protected override void Start()
		{
			base.Start();
			if (this.m_buttons.Length != this.m_buttonNames.Length)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"uMyGUI_PopupButtons: m_buttons and m_buttonNames must have the same length (",
					this.m_buttons.Length.ToString(),
					"!=",
					this.m_buttonNames.Length.ToString(),
					")!"
				}));
			}
			this.m_audioSources = base.GetComponentsInChildren<AudioSource>();
			for (int i = 0; i < this.m_audioSources.Length; i++)
			{
				this.m_audioSources[i].transform.parent = base.transform.parent;
				this.m_audioSources[i].name = base.name + "_" + this.m_audioSources[i].name;
			}
			this.LoadLocalizedText();
		}

		// Token: 0x060020C5 RID: 8389 RVA: 0x000A9C7C File Offset: 0x000A7E7C
		protected void OnDestroy()
		{
			for (int i = 0; i < this.m_audioSources.Length; i++)
			{
				if (this.m_audioSources[i] != null)
				{
					global::UnityEngine.Object.Destroy(this.m_audioSources[i].gameObject);
				}
			}
		}

		// Token: 0x060020C6 RID: 8390 RVA: 0x000A9CC0 File Offset: 0x000A7EC0
		private void LoadLocalizedText()
		{
			if (this.noText != null)
			{
				this.noText.SetText(Loc.T("UI.StartScreen.Mods.No"));
			}
			if (this.yesText != null)
			{
				this.yesText.SetText(Loc.T("UI.StartScreen.Mods.Yes"));
			}
			if (this.okText != null)
			{
				this.okText.SetText(Loc.T("UI.StartScreen.Mods.Ok"));
			}
		}

		// Token: 0x0400194A RID: 6474
		[SerializeField]
		protected RectTransform[] m_buttons = new RectTransform[0];

		// Token: 0x0400194B RID: 6475
		[SerializeField]
		protected string[] m_buttonNames = new string[0];

		// Token: 0x0400194C RID: 6476
		[SerializeField]
		protected bool m_improveNavigationFocus = true;

		// Token: 0x0400194D RID: 6477
		protected Dictionary<string, Action> m_onBtnClickCallbacks = new Dictionary<string, Action>();

		// Token: 0x0400194E RID: 6478
		protected AudioSource[] m_audioSources = new AudioSource[0];

		// Token: 0x0400194F RID: 6479
		protected bool m_isClosing;

		// Token: 0x04001950 RID: 6480
		protected bool m_isCloseCanceled;

		// Token: 0x04001951 RID: 6481
		public TMP_Text noText;

		// Token: 0x04001952 RID: 6482
		public TMP_Text yesText;

		// Token: 0x04001953 RID: 6483
		public TMP_Text okText;
	}
}

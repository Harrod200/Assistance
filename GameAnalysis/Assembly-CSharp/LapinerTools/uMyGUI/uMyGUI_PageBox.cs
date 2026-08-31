using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LapinerTools.uMyGUI
{
	// Token: 0x02000525 RID: 1317
	public class uMyGUI_PageBox : MonoBehaviour
	{
		// Token: 0x1700048D RID: 1165
		// (get) Token: 0x0600209B RID: 8347 RVA: 0x000A91DF File Offset: 0x000A73DF
		// (set) Token: 0x0600209C RID: 8348 RVA: 0x000A91E7 File Offset: 0x000A73E7
		public int PageCount
		{
			get
			{
				return this.m_pageCount;
			}
			set
			{
				this.SetPageCount(value);
			}
		}

		// Token: 0x1700048E RID: 1166
		// (get) Token: 0x0600209D RID: 8349 RVA: 0x000A91F0 File Offset: 0x000A73F0
		// (set) Token: 0x0600209E RID: 8350 RVA: 0x000A91F8 File Offset: 0x000A73F8
		public int MaxPageBtnCount
		{
			get
			{
				return this.m_maxPageBtnCount;
			}
			set
			{
				this.m_maxPageBtnCount = value;
				this.SetPageCount(this.PageCount);
			}
		}

		// Token: 0x1700048F RID: 1167
		// (get) Token: 0x0600209F RID: 8351 RVA: 0x000A920D File Offset: 0x000A740D
		// (set) Token: 0x060020A0 RID: 8352 RVA: 0x000A9215 File Offset: 0x000A7415
		public int SelectedPage
		{
			get
			{
				return this.m_selectedPage;
			}
			set
			{
				this.SelectPage(value);
			}
		}

		// Token: 0x17000490 RID: 1168
		// (get) Token: 0x060020A1 RID: 8353 RVA: 0x000A9220 File Offset: 0x000A7420
		public RectTransform RTransform
		{
			get
			{
				if (!(this.m_rectTransform != null))
				{
					return this.m_rectTransform = base.GetComponent<RectTransform>();
				}
				return this.m_rectTransform;
			}
		}

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x060020A2 RID: 8354 RVA: 0x000A9254 File Offset: 0x000A7454
		// (remove) Token: 0x060020A3 RID: 8355 RVA: 0x000A928C File Offset: 0x000A748C
		public event Action<int> OnPageSelected;

		// Token: 0x17000491 RID: 1169
		// (get) Token: 0x060020A4 RID: 8356 RVA: 0x000A92C4 File Offset: 0x000A74C4
		private RectTransform PageButtonTransform
		{
			get
			{
				if (!(this.m_pageButtonTransform != null) && !(this.m_pageButton == null))
				{
					return this.m_pageButtonTransform = this.m_pageButton.GetComponent<RectTransform>();
				}
				return this.m_pageButtonTransform;
			}
		}

		// Token: 0x060020A5 RID: 8357 RVA: 0x000A9308 File Offset: 0x000A7508
		public void SetPageCount(int p_newPageCount)
		{
			this.m_pageCount = Mathf.Max(1, p_newPageCount);
			if (p_newPageCount <= 1)
			{
				base.gameObject.SetActive(false);
				return;
			}
			if (this.m_pageButton != null)
			{
				base.gameObject.SetActive(true);
				this.UpdateUI();
				return;
			}
			Debug.LogError("uMyGUI_PageBox: SetPageCount: m_pageButton must be set in the inspector!");
		}

		// Token: 0x060020A6 RID: 8358 RVA: 0x000A935E File Offset: 0x000A755E
		public void SelectPageAndCenterOffset(int p_selectedPage)
		{
			this.m_offset = Mathf.Min(this.m_pageCount - this.m_maxPageBtnCount, Mathf.Max(0, p_selectedPage - 1 - this.m_maxPageBtnCount / 2));
			this.SelectPage(p_selectedPage);
		}

		// Token: 0x060020A7 RID: 8359 RVA: 0x000A9394 File Offset: 0x000A7594
		public void SelectPage(int p_selectedPage)
		{
			int num = Mathf.Clamp(p_selectedPage, 0, this.m_pageCount);
			bool flag = num != this.m_selectedPage;
			this.m_selectedPage = num;
			this.UpdateUI();
			if (flag && this.OnPageSelected != null)
			{
				this.OnPageSelected(p_selectedPage);
			}
		}

		// Token: 0x060020A8 RID: 8360 RVA: 0x000A93E0 File Offset: 0x000A75E0
		public void UpdateUI()
		{
			this.Clear();
			int num = Mathf.Min(this.m_pageCount, this.m_maxPageBtnCount);
			float num2 = this.GetWidth(this.m_previousButton) + this.GetWidth(this.m_nextButton) + this.GetWidth(this.m_pageButton) * (float)num;
			this.RTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num2);
			int num3 = Mathf.Max(0, this.m_selectedPage - this.m_maxPageBtnCount);
			int num4 = Mathf.Max(0, Mathf.Min(this.m_pageCount - this.m_maxPageBtnCount, this.m_selectedPage - 1));
			if (num3 - 1 >= this.m_offset)
			{
				this.m_offset = num3;
			}
			else if (num4 + 1 <= this.m_offset)
			{
				this.m_offset = num4;
			}
			this.SetText(this.m_pageButton, (1 + this.m_offset).ToString());
			this.SetOnClick(this.m_pageButton, 1 + this.m_offset);
			for (int i = 2; i <= num; i++)
			{
				Button button = global::UnityEngine.Object.Instantiate<Button>(this.m_pageButton);
				RectTransform component = button.GetComponent<RectTransform>();
				component.SetParent(this.PageButtonTransform.parent, true);
				component.localScale = this.PageButtonTransform.localScale;
				component.localPosition = this.PageButtonTransform.localPosition + Vector3.right * (float)(i - 1) * this.GetWidth(this.m_pageButton);
				this.SetText(button, (i + this.m_offset).ToString());
				this.SetOnClick(button, i + this.m_offset);
				this.m_pageButtons.Add(button);
			}
			for (int j = 0; j < this.m_pageButtons.Count; j++)
			{
				int num5 = j + 1 + this.m_offset;
				this.m_pageButtons[j].enabled = num5 != this.m_selectedPage;
			}
			if (this.m_nextButton != null)
			{
				this.m_nextButton.GetComponent<RectTransform>().localPosition = this.PageButtonTransform.localPosition + Vector3.right * (float)num * this.GetWidth(this.m_pageButton);
			}
		}

		// Token: 0x060020A9 RID: 8361 RVA: 0x000A9614 File Offset: 0x000A7814
		private void Start()
		{
			this.SetPageCount(this.m_pageCount);
			if (this.m_previousButton != null)
			{
				this.m_previousButton.onClick.AddListener(delegate
				{
					this.SelectPageAndCenterOffset(Mathf.Max(1, this.m_selectedPage - 1));
				});
			}
			if (this.m_nextButton != null)
			{
				this.m_nextButton.onClick.AddListener(delegate
				{
					this.SelectPageAndCenterOffset(this.m_selectedPage + 1);
				});
			}
		}

		// Token: 0x060020AA RID: 8362 RVA: 0x000A9684 File Offset: 0x000A7884
		private void SetText(Button p_button, string p_text)
		{
			TMP_Text componentInChildren = p_button.GetComponentInChildren<TMP_Text>();
			if (componentInChildren != null)
			{
				componentInChildren.text = p_text;
			}
		}

		// Token: 0x060020AB RID: 8363 RVA: 0x000A96A8 File Offset: 0x000A78A8
		private void SetOnClick(Button p_button, int p_pageNumber)
		{
			p_button.onClick.RemoveAllListeners();
			p_button.onClick.AddListener(delegate
			{
				this.SelectPage(p_pageNumber);
			});
		}

		// Token: 0x060020AC RID: 8364 RVA: 0x000A96EB File Offset: 0x000A78EB
		private float GetWidth(Button p_button)
		{
			if (!(p_button != null))
			{
				return 0f;
			}
			return this.GetWidth(p_button.GetComponent<RectTransform>());
		}

		// Token: 0x060020AD RID: 8365 RVA: 0x000A9708 File Offset: 0x000A7908
		private float GetWidth(RectTransform p_rTransform)
		{
			if (!(p_rTransform != null))
			{
				return 0f;
			}
			return p_rTransform.rect.xMax - p_rTransform.rect.xMin;
		}

		// Token: 0x060020AE RID: 8366 RVA: 0x000A9744 File Offset: 0x000A7944
		private void Clear()
		{
			for (int i = 0; i < this.m_pageButtons.Count; i++)
			{
				if (this.m_pageButtons[i] != null && this.m_pageButtons[i] != this.m_pageButton)
				{
					global::UnityEngine.Object.Destroy(this.m_pageButtons[i].gameObject);
				}
			}
			this.m_pageButtons.Clear();
			this.m_pageButtons.Add(this.m_pageButton);
		}

		// Token: 0x0400193A RID: 6458
		[SerializeField]
		private Button m_previousButton;

		// Token: 0x0400193B RID: 6459
		[SerializeField]
		private Button m_nextButton;

		// Token: 0x0400193C RID: 6460
		[SerializeField]
		private Button m_pageButton;

		// Token: 0x0400193D RID: 6461
		[SerializeField]
		private int m_pageCount = 1;

		// Token: 0x0400193E RID: 6462
		[SerializeField]
		private int m_maxPageBtnCount = 9;

		// Token: 0x0400193F RID: 6463
		[SerializeField]
		private int m_selectedPage;

		// Token: 0x04001940 RID: 6464
		private RectTransform m_rectTransform;

		// Token: 0x04001942 RID: 6466
		private RectTransform m_pageButtonTransform;

		// Token: 0x04001943 RID: 6467
		private int m_offset;

		// Token: 0x04001944 RID: 6468
		private List<Button> m_pageButtons = new List<Button>();
	}
}

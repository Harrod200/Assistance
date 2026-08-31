using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LapinerTools.uMyGUI
{
	// Token: 0x02000524 RID: 1316
	public class uMyGUI_Dropdown : MonoBehaviour
	{
		// Token: 0x1700048B RID: 1163
		// (get) Token: 0x06002087 RID: 8327 RVA: 0x000A8B76 File Offset: 0x000A6D76
		// (set) Token: 0x06002088 RID: 8328 RVA: 0x000A8B7E File Offset: 0x000A6D7E
		public string[] Entries
		{
			get
			{
				return this.m_entries;
			}
			set
			{
				this.m_entries = value;
				this.HideEntries();
				this.ShowEntries(true);
			}
		}

		// Token: 0x1700048C RID: 1164
		// (get) Token: 0x06002089 RID: 8329 RVA: 0x000A8B94 File Offset: 0x000A6D94
		// (set) Token: 0x0600208A RID: 8330 RVA: 0x000A8B9C File Offset: 0x000A6D9C
		public int SelectedIndex
		{
			get
			{
				return this.m_selectedIndex;
			}
			set
			{
				this.m_selectedIndex = Mathf.Clamp(value, -1, this.m_entries.Length - 1);
			}
		}

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x0600208B RID: 8331 RVA: 0x000A8BB8 File Offset: 0x000A6DB8
		// (remove) Token: 0x0600208C RID: 8332 RVA: 0x000A8BF0 File Offset: 0x000A6DF0
		public event Action<int> OnSelected;

		// Token: 0x0600208D RID: 8333 RVA: 0x000A8C28 File Offset: 0x000A6E28
		public void Select(int p_selectedIndex)
		{
			int num = Mathf.Clamp(p_selectedIndex, -1, this.m_entries.Length - 1);
			bool flag = num != this.m_selectedIndex;
			this.m_selectedIndex = num;
			this.UpdateText();
			if (flag && this.OnSelected != null)
			{
				this.OnSelected(this.m_selectedIndex);
			}
		}

		// Token: 0x0600208E RID: 8334 RVA: 0x000A8C7C File Offset: 0x000A6E7C
		private void Start()
		{
			if (this.m_text != null)
			{
				this.UpdateText();
			}
			if (this.m_button != null)
			{
				this.m_button.onClick.AddListener(new UnityAction(this.OnClick));
			}
			else
			{
				Debug.LogError("uMyGUI_Dropdown: m_button must be set in the inspector!");
			}
			if (this.m_entriesRoot != null && this.m_entriesBG != null)
			{
				this.HideEntries();
				return;
			}
			Debug.LogError("uMyGUI_Dropdown: m_entriesRoot and m_entriesBG must be set in the inspector!");
		}

		// Token: 0x0600208F RID: 8335 RVA: 0x000A8D00 File Offset: 0x000A6F00
		private void LateUpdate()
		{
			if (this.m_improveNavigationFocus)
			{
				EventSystem current = EventSystem.current;
				if (current != null && (current.currentSelectedGameObject == null || !current.currentSelectedGameObject.activeInHierarchy))
				{
					if (current.lastSelectedGameObject != null && current.lastSelectedGameObject.activeInHierarchy)
					{
						current.SetSelectedGameObject(current.lastSelectedGameObject);
						return;
					}
					if (this.m_entriesRoot != null)
					{
						if (this.m_entriesRoot.gameObject.activeSelf && this.m_entriesRoot.childCount > 0 && this.m_entriesRoot.GetChild(0).GetComponentInChildren<Button>() != null)
						{
							this.m_entriesRoot.GetChild(0).GetComponentInChildren<Button>().Select();
							return;
						}
						if (this.m_button != null)
						{
							this.m_button.Select();
						}
					}
				}
			}
		}

		// Token: 0x06002090 RID: 8336 RVA: 0x000A8DE7 File Offset: 0x000A6FE7
		private void OnClick()
		{
			if (this.m_entriesRoot != null)
			{
				if (this.m_entriesRoot.gameObject.activeSelf)
				{
					this.HideEntries();
					return;
				}
				this.ShowEntries(true);
			}
		}

		// Token: 0x06002091 RID: 8337 RVA: 0x000A8E18 File Offset: 0x000A7018
		private void ShowEntries(bool updatescroll = true)
		{
			if (this.m_entriesRoot != null && this.m_entriesBG != null && this.m_entryButton != null)
			{
				if (this.m_entries.Length == 0)
				{
					return;
				}
				this.m_entriesRoot.gameObject.SetActive(true);
				this.ClearEntries();
				float num = (this.GetHeight(this.m_entryButton) + (float)this.m_entrySpacing) * (float)this.m_entries.Length + (float)this.m_entrySpacing;
				this.m_entriesBG.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num);
				RectTransform component = this.m_entryButton.GetComponent<RectTransform>();
				this.SetText(this.m_entryButton, this.m_entries[0]);
				this.SetOnClick(this.m_entryButton, 0);
				this.m_entryButton.interactable = this.m_selectedIndex != 0;
				for (int i = 1; i < this.m_entries.Length; i++)
				{
					Button button = global::UnityEngine.Object.Instantiate<Button>(this.m_entryButton);
					button.interactable = i != this.m_selectedIndex;
					RectTransform component2 = button.GetComponent<RectTransform>();
					component2.SetParent(component.parent, true);
					component2.localScale = component.localScale;
					component2.offsetMin = component.offsetMin;
					component2.offsetMax = component.offsetMax;
					component2.localPosition = component.localPosition + Vector3.down * (float)i * (this.GetHeight(this.m_entryButton) + (float)this.m_entrySpacing);
					this.SetText(button, this.m_entries[i]);
					this.SetOnClick(button, i);
				}
				if (this.m_entriesScrollbar != null && updatescroll)
				{
					base.StartCoroutine(this.UpdateScrollBarVisibility());
				}
			}
		}

		// Token: 0x06002092 RID: 8338 RVA: 0x000A8FCA File Offset: 0x000A71CA
		private void HideEntries()
		{
			if (this.m_entriesRoot != null)
			{
				this.ClearEntries();
				this.m_entriesRoot.gameObject.SetActive(false);
			}
		}

		// Token: 0x06002093 RID: 8339 RVA: 0x000A8FF4 File Offset: 0x000A71F4
		private void ClearEntries()
		{
			if (this.m_entriesBG != null && this.m_entryButton != null)
			{
				for (int i = 0; i < this.m_entriesBG.childCount; i++)
				{
					if (this.m_entriesBG.GetChild(i) != this.m_entryButton.transform)
					{
						global::UnityEngine.Object.Destroy(this.m_entriesBG.GetChild(i).gameObject);
					}
				}
			}
		}

		// Token: 0x06002094 RID: 8340 RVA: 0x000A9068 File Offset: 0x000A7268
		private void UpdateText()
		{
			if (this.m_text != null)
			{
				bool flag = this.m_selectedIndex >= 0 && this.m_selectedIndex < this.m_entries.Length;
				this.m_text.text = this.m_staticText + (flag ? this.m_entries[this.m_selectedIndex] : this.m_nothingSelectedText);
			}
		}

		// Token: 0x06002095 RID: 8341 RVA: 0x000A90D0 File Offset: 0x000A72D0
		private void SetOnClick(Button p_button, int p_selectedIndex)
		{
			p_button.onClick.RemoveAllListeners();
			p_button.onClick.AddListener(delegate
			{
				this.Select(p_selectedIndex);
			});
		}

		// Token: 0x06002096 RID: 8342 RVA: 0x000A9114 File Offset: 0x000A7314
		private void SetText(Button p_button, string p_text)
		{
			TMP_Text componentInChildren = p_button.GetComponentInChildren<TMP_Text>();
			if (componentInChildren != null)
			{
				componentInChildren.text = p_text;
			}
		}

		// Token: 0x06002097 RID: 8343 RVA: 0x000A9138 File Offset: 0x000A7338
		private float GetHeight(Button p_button)
		{
			if (!(p_button != null))
			{
				return 0f;
			}
			return this.GetHeight(p_button.GetComponent<RectTransform>());
		}

		// Token: 0x06002098 RID: 8344 RVA: 0x000A9158 File Offset: 0x000A7358
		private float GetHeight(RectTransform p_rTransform)
		{
			if (!(p_rTransform != null))
			{
				return 0f;
			}
			return p_rTransform.rect.yMax - p_rTransform.rect.yMin;
		}

		// Token: 0x06002099 RID: 8345 RVA: 0x000A9191 File Offset: 0x000A7391
		private IEnumerator UpdateScrollBarVisibility()
		{
			yield return new WaitForEndOfFrame();
			if (this.m_entriesScrollbar != null)
			{
				this.m_entriesScrollbar.gameObject.SetActive(this.m_entriesScrollbar.size < 0.985f);
			}
			yield break;
		}

		// Token: 0x0400192D RID: 6445
		[SerializeField]
		private Button m_button;

		// Token: 0x0400192E RID: 6446
		[SerializeField]
		private TMP_Text m_text;

		// Token: 0x0400192F RID: 6447
		[SerializeField]
		private RectTransform m_entriesRoot;

		// Token: 0x04001930 RID: 6448
		[SerializeField]
		private RectTransform m_entriesBG;

		// Token: 0x04001931 RID: 6449
		[SerializeField]
		private Scrollbar m_entriesScrollbar;

		// Token: 0x04001932 RID: 6450
		[SerializeField]
		private Button m_entryButton;

		// Token: 0x04001933 RID: 6451
		[SerializeField]
		private int m_entrySpacing = 5;

		// Token: 0x04001934 RID: 6452
		[SerializeField]
		private string m_staticText = "";

		// Token: 0x04001935 RID: 6453
		[SerializeField]
		private string m_nothingSelectedText = "";

		// Token: 0x04001936 RID: 6454
		[SerializeField]
		protected bool m_improveNavigationFocus = true;

		// Token: 0x04001937 RID: 6455
		[SerializeField]
		private string[] m_entries = new string[0];

		// Token: 0x04001938 RID: 6456
		[SerializeField]
		private int m_selectedIndex = -1;
	}
}

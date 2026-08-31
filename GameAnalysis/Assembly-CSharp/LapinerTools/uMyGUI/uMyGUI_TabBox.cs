using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace LapinerTools.uMyGUI
{
	// Token: 0x0200052E RID: 1326
	public class uMyGUI_TabBox : MonoBehaviour
	{
		// Token: 0x060020F3 RID: 8435 RVA: 0x000AA770 File Offset: 0x000A8970
		public void SelectTab(int p_tabIndex)
		{
			if (p_tabIndex == this.m_selectedIndex)
			{
				return;
			}
			if (p_tabIndex < 0 || p_tabIndex >= this.m_tabs.Length)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"uMyGUI_TabBox: SelectTab tab index '",
					p_tabIndex.ToString(),
					"' is out of bounds [0,",
					this.m_tabs.Length.ToString(),
					"]!"
				}));
				return;
			}
			if (this.m_tabs[p_tabIndex] == null)
			{
				Debug.LogError("uMyGUI_TabBox: SelectTab tab index '" + p_tabIndex.ToString() + "' is null! Check the tabs array in the inspector!");
				return;
			}
			if (this.m_isMoveDownInHierarchyOnSelect)
			{
				this.m_tabs[p_tabIndex].SetAsLastSibling();
			}
			switch (this.m_animMode)
			{
			case uMyGUI_TabBox.EAnimMode.TAB_ONLY:
				base.StopAllCoroutines();
				this.AnimateRectRectTransformSelection(p_tabIndex, this.m_tabs, this.m_fadeInAnimTab, this.m_fadeOutAnimTab, true);
				goto IL_0110;
			case uMyGUI_TabBox.EAnimMode.BTN_ONLY:
				base.StopAllCoroutines();
				this.AnimateRectRectTransformSelection(p_tabIndex, this.m_btns, this.m_fadeInAnimBtn, this.m_fadeOutAnimBtn, false);
				this.UpdateTabActiveStates(p_tabIndex);
				goto IL_0110;
			case uMyGUI_TabBox.EAnimMode.TAB_AND_BTN:
				base.StopAllCoroutines();
				this.AnimateRectRectTransformSelection(p_tabIndex, this.m_tabs, this.m_fadeInAnimTab, this.m_fadeOutAnimTab, true);
				this.AnimateRectRectTransformSelection(p_tabIndex, this.m_btns, this.m_fadeInAnimBtn, this.m_fadeOutAnimBtn, false);
				goto IL_0110;
			}
			this.UpdateTabActiveStates(p_tabIndex);
			IL_0110:
			this.m_selectedIndex = p_tabIndex;
		}

		// Token: 0x060020F4 RID: 8436 RVA: 0x000AA8DC File Offset: 0x000A8ADC
		private void Start()
		{
			if (this.m_isSelectTabOnStart)
			{
				this.UpdateTabActiveStates(this.m_selectedIndex);
				if (this.m_isPlayTabAnimOnStart && (this.m_animMode == uMyGUI_TabBox.EAnimMode.TAB_ONLY || this.m_animMode == uMyGUI_TabBox.EAnimMode.TAB_AND_BTN))
				{
					this.AnimateRectRectTransformSelection(this.m_selectedIndex, this.m_tabs, this.m_fadeInAnimTab, this.m_fadeOutAnimTab, false);
				}
				if (this.m_isPlayBtnAnimOnStart)
				{
					this.AnimateRectRectTransformSelection(this.m_selectedIndex, this.m_btns, this.m_fadeInAnimBtn, this.m_fadeOutAnimBtn, false);
				}
			}
		}

		// Token: 0x060020F5 RID: 8437 RVA: 0x000AA960 File Offset: 0x000A8B60
		private void UpdateTabActiveStates(int p_tabIndex)
		{
			for (int i = 0; i < this.m_tabs.Length; i++)
			{
				bool flag = i == p_tabIndex;
				if (this.m_tabs[i] != null)
				{
					if (this.m_isSendMessage)
					{
						this.m_tabs[i].gameObject.SendMessage(flag ? "uMyGUI_OnActivateTab" : "uMyGUI_OnDeactivateTab", SendMessageOptions.DontRequireReceiver);
					}
					this.m_tabs[i].gameObject.SetActive(flag);
				}
				if (this.m_btns.Length > i && this.m_btns[i] != null)
				{
					Selectable component = this.m_btns[i].GetComponent<Selectable>();
					if (component != null)
					{
						component.interactable = !flag;
					}
				}
			}
		}

		// Token: 0x060020F6 RID: 8438 RVA: 0x000AAA14 File Offset: 0x000A8C14
		private void AnimateRectRectTransformSelection(int p_tabIndex, RectTransform[] p_transforms, string p_fadeInAnim, string p_fadeOutAnim, bool p_isActivateChanged)
		{
			for (int i = 0; i < p_transforms.Length; i++)
			{
				if (p_transforms[i] != null)
				{
					Animation component = p_transforms[i].GetComponent<Animation>();
					if (component != null)
					{
						if (i == this.m_selectedIndex && p_tabIndex != this.m_selectedIndex)
						{
							if (component[p_fadeOutAnim] != null)
							{
								if (p_isActivateChanged)
								{
									base.StartCoroutine(this.DeactivateAfterDelay(p_transforms[i].gameObject, component[p_fadeOutAnim].length));
								}
								if (this.m_isSendMessage)
								{
									p_transforms[i].gameObject.SendMessage("uMyGUI_OnDeactivateTab", SendMessageOptions.DontRequireReceiver);
								}
								component.Play(p_fadeOutAnim);
							}
						}
						else if (i == p_tabIndex)
						{
							if (p_isActivateChanged)
							{
								p_transforms[i].gameObject.SetActive(true);
							}
							if (this.m_isSendMessage)
							{
								p_transforms[i].gameObject.SendMessage("uMyGUI_OnActivateTab", SendMessageOptions.DontRequireReceiver);
							}
							component.Play(p_fadeInAnim);
						}
					}
					else
					{
						Debug.LogError("uMyGUI_TabBox: AnimateRectRectTransformSelection: object at index '" + i.ToString() + "' has no Animation component and cannot fade in or out!");
					}
					Selectable component2 = p_transforms[i].GetComponent<Selectable>();
					if (component2 != null)
					{
						component2.interactable = i != p_tabIndex;
					}
				}
			}
		}

		// Token: 0x060020F7 RID: 8439 RVA: 0x000AAB3E File Offset: 0x000A8D3E
		private IEnumerator DeactivateAfterDelay(GameObject p_object, float p_delay)
		{
			yield return new WaitForSeconds(p_delay);
			if (p_object != null)
			{
				p_object.SetActive(false);
			}
			yield break;
		}

		// Token: 0x0400196E RID: 6510
		public const string SEND_MESSAGE_ACTIVATE_NAME = "uMyGUI_OnActivateTab";

		// Token: 0x0400196F RID: 6511
		public const string SEND_MESSAGE_DEACTIVATE_NAME = "uMyGUI_OnDeactivateTab";

		// Token: 0x04001970 RID: 6512
		[SerializeField]
		private RectTransform[] m_btns = new RectTransform[0];

		// Token: 0x04001971 RID: 6513
		[SerializeField]
		private RectTransform[] m_tabs = new RectTransform[0];

		// Token: 0x04001972 RID: 6514
		[SerializeField]
		private int m_selectedIndex;

		// Token: 0x04001973 RID: 6515
		[SerializeField]
		private bool m_isSelectTabOnStart = true;

		// Token: 0x04001974 RID: 6516
		[SerializeField]
		private bool m_isPlayTabAnimOnStart = true;

		// Token: 0x04001975 RID: 6517
		[SerializeField]
		private bool m_isPlayBtnAnimOnStart = true;

		// Token: 0x04001976 RID: 6518
		[SerializeField]
		private uMyGUI_TabBox.EAnimMode m_animMode;

		// Token: 0x04001977 RID: 6519
		[SerializeField]
		private string m_fadeInAnimTab = "tab_fade_in";

		// Token: 0x04001978 RID: 6520
		[SerializeField]
		private string m_fadeOutAnimTab = "tab_fade_out";

		// Token: 0x04001979 RID: 6521
		[SerializeField]
		private string m_fadeInAnimBtn = "btn_fade_in";

		// Token: 0x0400197A RID: 6522
		[SerializeField]
		private string m_fadeOutAnimBtn = "btn_fade_out";

		// Token: 0x0400197B RID: 6523
		[SerializeField]
		private bool m_isSendMessage;

		// Token: 0x0400197C RID: 6524
		[SerializeField]
		private bool m_isMoveDownInHierarchyOnSelect;

		// Token: 0x02000C94 RID: 3220
		public enum EAnimMode
		{
			// Token: 0x04004EFA RID: 20218
			NONE,
			// Token: 0x04004EFB RID: 20219
			TAB_ONLY,
			// Token: 0x04004EFC RID: 20220
			BTN_ONLY,
			// Token: 0x04004EFD RID: 20221
			TAB_AND_BTN
		}
	}
}

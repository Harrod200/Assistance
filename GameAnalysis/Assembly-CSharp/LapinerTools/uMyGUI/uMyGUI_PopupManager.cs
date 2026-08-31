using System;
using System.Collections.Generic;
using UnityEngine;

namespace LapinerTools.uMyGUI
{
	// Token: 0x02000529 RID: 1321
	public class uMyGUI_PopupManager : MonoBehaviour
	{
		// Token: 0x17000494 RID: 1172
		// (get) Token: 0x060020CE RID: 8398 RVA: 0x000A9E2C File Offset: 0x000A802C
		public static uMyGUI_PopupManager Instance
		{
			get
			{
				if (uMyGUI_PopupManager.s_instance == null)
				{
					uMyGUI_PopupManager.s_instance = global::UnityEngine.Object.FindObjectOfType<uMyGUI_PopupManager>();
				}
				if (uMyGUI_PopupManager.s_instance == null)
				{
					uMyGUI_PopupManager.s_instance = new GameObject(typeof(uMyGUI_PopupManager).Name).AddComponent<uMyGUI_PopupManager>();
				}
				return uMyGUI_PopupManager.s_instance;
			}
		}

		// Token: 0x17000495 RID: 1173
		// (get) Token: 0x060020CF RID: 8399 RVA: 0x000A9E80 File Offset: 0x000A8080
		public static bool IsInstanceSet
		{
			get
			{
				return uMyGUI_PopupManager.s_instance != null;
			}
		}

		// Token: 0x17000496 RID: 1174
		// (get) Token: 0x060020D0 RID: 8400 RVA: 0x000A9E8D File Offset: 0x000A808D
		// (set) Token: 0x060020D1 RID: 8401 RVA: 0x000A9E95 File Offset: 0x000A8095
		public uMyGUI_Popup[] Popups
		{
			get
			{
				return this.m_popups;
			}
			set
			{
				this.m_popups = value;
			}
		}

		// Token: 0x17000497 RID: 1175
		// (get) Token: 0x060020D2 RID: 8402 RVA: 0x000A9E9E File Offset: 0x000A809E
		// (set) Token: 0x060020D3 RID: 8403 RVA: 0x000A9EA6 File Offset: 0x000A80A6
		public string[] PopupNames
		{
			get
			{
				return this.m_popupNames;
			}
			set
			{
				this.m_popupNames = value;
			}
		}

		// Token: 0x17000498 RID: 1176
		// (get) Token: 0x060020D4 RID: 8404 RVA: 0x000A9EAF File Offset: 0x000A80AF
		// (set) Token: 0x060020D5 RID: 8405 RVA: 0x000A9EB7 File Offset: 0x000A80B7
		public CanvasGroup[] DeactivatedElementsWhenPopupIsShown
		{
			get
			{
				return this.m_deactivatedElementsWhenPopupIsShown;
			}
			set
			{
				this.m_deactivatedElementsWhenPopupIsShown = value;
			}
		}

		// Token: 0x060020D6 RID: 8406 RVA: 0x000A9EC0 File Offset: 0x000A80C0
		public uMyGUI_Popup ShowPopup(string p_name)
		{
			int num = 0;
			while (num < this.m_popupNames.Length && num < this.m_popups.Length)
			{
				if (this.m_popupNames[num] == p_name)
				{
					return this.ShowPopup(num);
				}
				num++;
			}
			if (this.LoadPopupFromResources(p_name) != null)
			{
				return this.ShowPopup(p_name);
			}
			return null;
		}

		// Token: 0x060020D7 RID: 8407 RVA: 0x000A9F1C File Offset: 0x000A811C
		public uMyGUI_Popup HidePopup(string p_name)
		{
			int num = 0;
			while (num < this.m_popupNames.Length && num < this.m_popups.Length)
			{
				if (this.m_popupNames[num] == p_name)
				{
					return this.HidePopup(num);
				}
				num++;
			}
			return null;
		}

		// Token: 0x060020D8 RID: 8408 RVA: 0x000A9F60 File Offset: 0x000A8160
		public uMyGUI_Popup ShowPopup(int p_index)
		{
			if (p_index >= 0 && p_index < this.m_popups.Length)
			{
				this.m_popups[p_index].Show();
				return this.m_popups[p_index];
			}
			Debug.LogError(string.Concat(new string[]
			{
				"uMyGUI_PopupManager: ShowPopup: popup index '",
				p_index.ToString(),
				"' is out of bounds [0,",
				this.m_popups.Length.ToString(),
				"]!"
			}));
			return null;
		}

		// Token: 0x060020D9 RID: 8409 RVA: 0x000A9FD8 File Offset: 0x000A81D8
		public uMyGUI_Popup HidePopup(int p_index)
		{
			if (p_index >= 0 && p_index < this.m_popups.Length)
			{
				this.m_popups[p_index].Hide();
				return this.m_popups[p_index];
			}
			Debug.LogError(string.Concat(new string[]
			{
				"uMyGUI_PopupManager: HidePopup: popup index '",
				p_index.ToString(),
				"' is out of bounds [0,",
				this.m_popups.Length.ToString(),
				"]!"
			}));
			return null;
		}

		// Token: 0x17000499 RID: 1177
		// (get) Token: 0x060020DA RID: 8410 RVA: 0x000AA050 File Offset: 0x000A8250
		public bool IsPopupShown
		{
			get
			{
				for (int i = 0; i < this.m_popups.Length; i++)
				{
					if (this.m_popups[i] != null && this.m_popups[i].IsShown)
					{
						return true;
					}
				}
				return false;
			}
		}

		// Token: 0x060020DB RID: 8411 RVA: 0x000AA094 File Offset: 0x000A8294
		public bool HasPopup(string p_name)
		{
			for (int i = 0; i < this.m_popupNames.Length; i++)
			{
				if (this.m_popupNames[i] == p_name)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060020DC RID: 8412 RVA: 0x000AA0C8 File Offset: 0x000A82C8
		public bool AddPopup(uMyGUI_Popup p_popup, string p_name)
		{
			Canvas canvas = null;
			if (this.m_popups.Length != 0 && this.m_popups[0] != null && this.m_popups[0].transform.parent != null)
			{
				canvas = this.m_popups[0].transform.parent.GetComponentInParent<Canvas>();
			}
			if (canvas == null)
			{
				canvas = base.GetComponentInParent<Canvas>();
			}
			if (canvas == null)
			{
				canvas = global::UnityEngine.Object.FindObjectOfType<Canvas>();
			}
			if (canvas == null)
			{
				Debug.LogError("uMyGUI_PopupManager: AddPopup: there is no Canvas in this level!");
				return false;
			}
			uMyGUI_Popup[] popups = this.m_popups;
			string[] popupNames = this.m_popupNames;
			this.m_popups = new uMyGUI_Popup[this.m_popups.Length + 1];
			this.m_popupNames = new string[this.m_popupNames.Length + 1];
			Array.Copy(popups, this.m_popups, popups.Length);
			Array.Copy(popupNames, this.m_popupNames, popupNames.Length);
			this.m_popups[this.m_popups.Length - 1] = p_popup;
			this.m_popupNames[this.m_popups.Length - 1] = p_name;
			p_popup.transform.SetParent(canvas.transform, false);
			this.HidePopup(this.m_popups.Length - 1);
			return true;
		}

		// Token: 0x060020DD RID: 8413 RVA: 0x000AA1F4 File Offset: 0x000A83F4
		public bool RemovePopup(uMyGUI_Popup p_popup)
		{
			for (int i = 0; i < this.m_popups.Length; i++)
			{
				if (this.m_popups[i] == p_popup)
				{
					List<uMyGUI_Popup> list = new List<uMyGUI_Popup>(this.m_popups);
					list.RemoveAt(i);
					this.m_popups = list.ToArray();
					List<string> list2 = new List<string>(this.m_popupNames);
					list2.RemoveAt(i);
					this.m_popupNames = list2.ToArray();
					return true;
				}
			}
			return false;
		}

		// Token: 0x060020DE RID: 8414 RVA: 0x000AA268 File Offset: 0x000A8468
		private uMyGUI_Popup LoadPopupFromResources(string p_name)
		{
			uMyGUI_Popup uMyGUI_Popup = global::UnityEngine.Object.Instantiate<uMyGUI_Popup>(Resources.Load<uMyGUI_Popup>("popup_" + p_name + "_root"));
			if (uMyGUI_Popup != null && this.AddPopup(uMyGUI_Popup, p_name))
			{
				return uMyGUI_Popup;
			}
			return null;
		}

		// Token: 0x060020DF RID: 8415 RVA: 0x000AA2A8 File Offset: 0x000A84A8
		private void Awake()
		{
			if (this.m_popups.Length != this.m_popupNames.Length)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"uMyGUI_PopupManager: m_popups and m_popupNames must have the same length (",
					this.m_popups.Length.ToString(),
					"!=",
					this.m_popupNames.Length.ToString(),
					")!"
				}));
			}
			for (int i = 0; i < this.m_popups.Length; i++)
			{
				this.HidePopup(i);
			}
		}

		// Token: 0x060020E0 RID: 8416 RVA: 0x000AA330 File Offset: 0x000A8530
		private void Update()
		{
			bool flag = !this.IsPopupShown;
			for (int i = 0; i < this.m_deactivatedElementsWhenPopupIsShown.Length; i++)
			{
				this.m_deactivatedElementsWhenPopupIsShown[i].interactable = flag;
			}
		}

		// Token: 0x04001957 RID: 6487
		public const string POPUP_LOADING = "loading";

		// Token: 0x04001958 RID: 6488
		public const string POPUP_TEXT = "text";

		// Token: 0x04001959 RID: 6489
		public const string POPUP_DROPDOWN = "dropdown";

		// Token: 0x0400195A RID: 6490
		public const string BTN_OK = "ok";

		// Token: 0x0400195B RID: 6491
		public const string BTN_YES = "yes";

		// Token: 0x0400195C RID: 6492
		public const string BTN_NO = "no";

		// Token: 0x0400195D RID: 6493
		private static uMyGUI_PopupManager s_instance;

		// Token: 0x0400195E RID: 6494
		[SerializeField]
		private uMyGUI_Popup[] m_popups = new uMyGUI_Popup[0];

		// Token: 0x0400195F RID: 6495
		[SerializeField]
		private string[] m_popupNames = new string[0];

		// Token: 0x04001960 RID: 6496
		[SerializeField]
		private CanvasGroup[] m_deactivatedElementsWhenPopupIsShown = new CanvasGroup[0];
	}
}

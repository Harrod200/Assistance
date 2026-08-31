using System;
using System.Collections;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Modding;
using TMPro;
using UnityEngine;

namespace LapinerTools.uMyGUI
{
	// Token: 0x02000526 RID: 1318
	public class uMyGUI_Popup : MonoBehaviour
	{
		// Token: 0x14000006 RID: 6
		// (add) Token: 0x060020B2 RID: 8370 RVA: 0x000A9810 File Offset: 0x000A7A10
		// (remove) Token: 0x060020B3 RID: 8371 RVA: 0x000A9848 File Offset: 0x000A7A48
		public event Action OnShow;

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x060020B4 RID: 8372 RVA: 0x000A9880 File Offset: 0x000A7A80
		// (remove) Token: 0x060020B5 RID: 8373 RVA: 0x000A98B8 File Offset: 0x000A7AB8
		public event Action OnHide;

		// Token: 0x17000492 RID: 1170
		// (get) Token: 0x060020B6 RID: 8374 RVA: 0x000A98ED File Offset: 0x000A7AED
		public virtual bool IsShown
		{
			get
			{
				return base.gameObject.activeSelf;
			}
		}

		// Token: 0x17000493 RID: 1171
		// (get) Token: 0x060020B7 RID: 8375 RVA: 0x000A98FA File Offset: 0x000A7AFA
		// (set) Token: 0x060020B8 RID: 8376 RVA: 0x000A9902 File Offset: 0x000A7B02
		public virtual bool DestroyOnHide { get; set; }

		// Token: 0x060020B9 RID: 8377 RVA: 0x000A990C File Offset: 0x000A7B0C
		public virtual void Show()
		{
			TMP_Text tmp_Text = this.loadingText;
			if (tmp_Text != null)
			{
				tmp_Text.SetText(ModManager.checkedForModUpdates ? Loc.T("UI.StartScreen.Loading") : Loc.T("UI.StartScreen.Mods.Checking"));
			}
			base.gameObject.transform.SetAsLastSibling();
			base.gameObject.SetActive(true);
			if (this.OnShow != null)
			{
				this.OnShow();
			}
		}

		// Token: 0x060020BA RID: 8378 RVA: 0x000A9978 File Offset: 0x000A7B78
		public virtual void Hide()
		{
			base.gameObject.SetActive(false);
			if (this.OnHide != null)
			{
				this.OnHide();
			}
			if (this.DestroyOnHide && this.m_createFrame != Time.frameCount && uMyGUI_PopupManager.IsInstanceSet)
			{
				uMyGUI_PopupManager.Instance.StartCoroutine(this.DestroyOnEndOfFrame());
			}
		}

		// Token: 0x060020BB RID: 8379 RVA: 0x000A99D1 File Offset: 0x000A7BD1
		protected virtual void Awake()
		{
			this.m_createFrame = Time.frameCount;
		}

		// Token: 0x060020BC RID: 8380 RVA: 0x000A99DE File Offset: 0x000A7BDE
		protected virtual void Start()
		{
			Loc.SwapFonts(base.gameObject);
		}

		// Token: 0x060020BD RID: 8381 RVA: 0x000A99EB File Offset: 0x000A7BEB
		protected IEnumerator DestroyOnEndOfFrame()
		{
			yield return new WaitForEndOfFrame();
			if (uMyGUI_PopupManager.IsInstanceSet)
			{
				uMyGUI_PopupManager.Instance.RemovePopup(this);
				global::UnityEngine.Object.Destroy(base.gameObject);
			}
			yield break;
		}

		// Token: 0x04001947 RID: 6471
		public TMP_Text loadingText;

		// Token: 0x04001949 RID: 6473
		protected int m_createFrame;
	}
}

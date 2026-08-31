using System;
using PavonisInteractive.TerraInvicta;
using TMPro;
using UnityEngine;

namespace LapinerTools.uMyGUI
{
	// Token: 0x02000528 RID: 1320
	public class uMyGUI_PopupDropdown : uMyGUI_PopupText
	{
		// Token: 0x060020C8 RID: 8392 RVA: 0x000A9D74 File Offset: 0x000A7F74
		private new void Start()
		{
			this.dropDownHeader.SetText(Loc.T("UI.StartScreen.Mods.UpdateMod"));
		}

		// Token: 0x060020C9 RID: 8393 RVA: 0x000A9D8B File Offset: 0x000A7F8B
		public override void Show()
		{
			base.Show();
			if (this.m_dropdown != null)
			{
				this.m_dropdown.Select(-1);
			}
		}

		// Token: 0x060020CA RID: 8394 RVA: 0x000A9DAD File Offset: 0x000A7FAD
		public override void Hide()
		{
			base.Hide();
			if (this.m_dropdown != null && this.m_onSelected != null)
			{
				this.m_dropdown.OnSelected -= this.m_onSelected;
				this.m_onSelected = null;
			}
		}

		// Token: 0x060020CB RID: 8395 RVA: 0x000A9DE3 File Offset: 0x000A7FE3
		public virtual uMyGUI_PopupDropdown SetEntries(string[] p_entries)
		{
			if (this.m_dropdown != null)
			{
				this.m_dropdown.Entries = p_entries;
			}
			return this;
		}

		// Token: 0x060020CC RID: 8396 RVA: 0x000A9E00 File Offset: 0x000A8000
		public virtual uMyGUI_PopupDropdown SetOnSelected(Action<int> p_onSelected)
		{
			if (this.m_dropdown != null)
			{
				this.m_onSelected = p_onSelected;
				this.m_dropdown.OnSelected += p_onSelected;
			}
			return this;
		}

		// Token: 0x04001954 RID: 6484
		[SerializeField]
		protected uMyGUI_Dropdown m_dropdown;

		// Token: 0x04001955 RID: 6485
		protected Action<int> m_onSelected;

		// Token: 0x04001956 RID: 6486
		public TMP_Text dropDownHeader;
	}
}

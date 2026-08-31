using System;
using LapinerTools.uMyGUI;
using UnityEngine;

namespace LapinerTools.Steam.UI
{
	// Token: 0x02000534 RID: 1332
	public class SteamWorkshopPopupBrowse : uMyGUI_Popup
	{
		// Token: 0x170004BB RID: 1211
		// (get) Token: 0x060021A4 RID: 8612 RVA: 0x000B03AB File Offset: 0x000AE5AB
		public SteamWorkshopUIBrowse BrowseUI
		{
			get
			{
				return this.m_browseUI;
			}
		}

		// Token: 0x060021A5 RID: 8613 RVA: 0x000B03B3 File Offset: 0x000AE5B3
		public SteamWorkshopPopupBrowse()
		{
			this.DestroyOnHide = true;
		}

		// Token: 0x040019CD RID: 6605
		[SerializeField]
		protected SteamWorkshopUIBrowse m_browseUI;
	}
}

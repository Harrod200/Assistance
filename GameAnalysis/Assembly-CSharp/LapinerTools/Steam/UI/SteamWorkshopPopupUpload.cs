using System;
using LapinerTools.Steam.Data;
using LapinerTools.uMyGUI;
using UnityEngine;

namespace LapinerTools.Steam.UI
{
	// Token: 0x02000535 RID: 1333
	public class SteamWorkshopPopupUpload : uMyGUI_Popup
	{
		// Token: 0x170004BC RID: 1212
		// (get) Token: 0x060021A6 RID: 8614 RVA: 0x000B03C2 File Offset: 0x000AE5C2
		public SteamWorkshopUIUpload UploadUI
		{
			get
			{
				return this.m_uploadUI;
			}
		}

		// Token: 0x060021A7 RID: 8615 RVA: 0x000B03CA File Offset: 0x000AE5CA
		public SteamWorkshopPopupUpload()
		{
			this.DestroyOnHide = true;
		}

		// Token: 0x060021A8 RID: 8616 RVA: 0x000B03D9 File Offset: 0x000AE5D9
		protected override void Start()
		{
			base.Start();
			if (this.m_uploadUI != null)
			{
				this.m_uploadUI.OnFinishedUpload += delegate(WorkshopItemUpdateEventArgs p_args)
				{
					this.Hide();
				};
			}
		}

		// Token: 0x040019CE RID: 6606
		[SerializeField]
		protected SteamWorkshopUIUpload m_uploadUI;
	}
}

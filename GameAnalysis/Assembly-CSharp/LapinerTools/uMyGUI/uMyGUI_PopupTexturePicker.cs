using System;
using UnityEngine;

namespace LapinerTools.uMyGUI
{
	// Token: 0x0200052B RID: 1323
	public class uMyGUI_PopupTexturePicker : uMyGUI_PopupText
	{
		// Token: 0x060020E6 RID: 8422 RVA: 0x000AA4CF File Offset: 0x000A86CF
		public override void Hide()
		{
			base.Hide();
			if (this.m_picker != null)
			{
				this.m_picker.ButtonCallback = null;
			}
		}

		// Token: 0x060020E7 RID: 8423 RVA: 0x000AA4F4 File Offset: 0x000A86F4
		public virtual uMyGUI_PopupText SetPicker(Texture2D[] p_textures, int p_selectedIndex, Action<int> p_buttonCallback)
		{
			if (this.m_picker != null)
			{
				this.m_picker.ButtonCallback = delegate(int p_clickedIndex)
				{
					p_buttonCallback(p_clickedIndex);
					this.Hide();
				};
				this.m_picker.SetTextures(p_textures, p_selectedIndex);
			}
			return this;
		}

		// Token: 0x04001965 RID: 6501
		[SerializeField]
		protected uMyGUI_TexturePicker m_picker;
	}
}

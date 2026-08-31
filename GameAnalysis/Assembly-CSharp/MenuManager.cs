using System;
using UnityEngine;

// Token: 0x0200043E RID: 1086
public class MenuManager : MonoBehaviour
{
	// Token: 0x0600167F RID: 5759 RVA: 0x00072E4C File Offset: 0x0007104C
	public void ShowMenu(Menu menu)
	{
		if (this.currentMenu != null)
		{
			this.HideMenu();
			if (this.currentMenu == menu)
			{
				this.currentMenu = null;
				return;
			}
		}
		this.currentMenu = menu;
		this.currentMenu.Open();
		this.currentMenu.IsOpen = true;
	}

	// Token: 0x06001680 RID: 5760 RVA: 0x00072EA1 File Offset: 0x000710A1
	public void HideMenu()
	{
		if (this.currentMenu != null)
		{
			Menu menu = this.currentMenu;
			this.currentMenu = null;
			menu.Close();
			menu.IsOpen = false;
		}
	}

	// Token: 0x06001681 RID: 5761 RVA: 0x00072ECA File Offset: 0x000710CA
	private void OnDisable()
	{
		this.hiddenMenu = this.currentMenu;
	}

	// Token: 0x06001682 RID: 5762 RVA: 0x00072ED8 File Offset: 0x000710D8
	private void OnEnable()
	{
		if (this.hiddenMenu != null)
		{
			this.ShowMenu(this.hiddenMenu);
		}
	}

	// Token: 0x040014D7 RID: 5335
	public Menu startMenu;

	// Token: 0x040014D8 RID: 5336
	public Menu currentMenu;

	// Token: 0x040014D9 RID: 5337
	private Menu hiddenMenu;
}

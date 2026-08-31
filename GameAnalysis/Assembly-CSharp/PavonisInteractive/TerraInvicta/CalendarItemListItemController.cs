using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000830 RID: 2096
	public class CalendarItemListItemController : MonoBehaviour
	{
		// Token: 0x06004B51 RID: 19281 RVA: 0x001F7314 File Offset: 0x001F5514
		public void UpdateListItem(CalendarDayGridItemController parent, string itemText, TIDateTime dateTime, bool alarm, string flag1Path = "", string flag2Path = "", string iconPath = "")
		{
			this.parentItem = parent;
			this.dateTime = dateTime;
			this.alarm = alarm;
			this.calendarItem.SetText(itemText);
			if (flag1Path != string.Empty)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(flag1Path, this.flag1);
				this.flag1.gameObject.SetActive(true);
			}
			else
			{
				this.flag1.gameObject.SetActive(false);
			}
			if (flag2Path != string.Empty)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(flag2Path, this.flag2);
				this.flag2.gameObject.SetActive(true);
			}
			else
			{
				this.flag2.gameObject.SetActive(false);
			}
			if (iconPath != string.Empty)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(iconPath, this.icon);
				this.icon.gameObject.SetActive(true);
			}
			else
			{
				this.icon.gameObject.SetActive(false);
			}
			this.button.interactable = !alarm;
		}

		// Token: 0x06004B52 RID: 19282 RVA: 0x001F741F File Offset: 0x001F561F
		public void OnButtonPressed()
		{
			this.parentItem.OnItemButtonClicked(this.dateTime, this.calendarItem.text);
		}

		// Token: 0x06004B53 RID: 19283 RVA: 0x001F743D File Offset: 0x001F563D
		private void OnEnable()
		{
			this.calHLG.enabled = true;
		}

		// Token: 0x06004B54 RID: 19284 RVA: 0x001F744B File Offset: 0x001F564B
		private void OnDisable()
		{
			this.calHLG.enabled = false;
		}

		// Token: 0x04002BF9 RID: 11257
		private CalendarDayGridItemController parentItem;

		// Token: 0x04002BFA RID: 11258
		private TIDateTime dateTime;

		// Token: 0x04002BFB RID: 11259
		public TMP_Text calendarItem;

		// Token: 0x04002BFC RID: 11260
		public Image flag1;

		// Token: 0x04002BFD RID: 11261
		public Image flag2;

		// Token: 0x04002BFE RID: 11262
		public Image icon;

		// Token: 0x04002BFF RID: 11263
		public Button button;

		// Token: 0x04002C00 RID: 11264
		private bool alarm;

		// Token: 0x04002C01 RID: 11265
		public HorizontalLayoutGroup calHLG;
	}
}

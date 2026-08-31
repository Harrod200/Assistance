using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000870 RID: 2160
	public class ResourceGridItemController : MonoBehaviour
	{
		// Token: 0x060050F7 RID: 20727 RVA: 0x00236A5C File Offset: 0x00234C5C
		public void UpdateListItem(string imagePath, string text)
		{
			if (imagePath != string.Empty)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(imagePath, this.resourceIcon);
				this.resourceIcon.enabled = true;
			}
			else
			{
				this.resourceIcon.enabled = false;
			}
			if (text != string.Empty)
			{
				this.resourceValue.gameObject.SetActive(true);
				this.resourceValue.SetText(text);
				return;
			}
			this.resourceValue.gameObject.SetActive(false);
		}

		// Token: 0x040034CF RID: 13519
		public Image resourceIcon;

		// Token: 0x040034D0 RID: 13520
		public TMP_Text resourceValue;
	}
}

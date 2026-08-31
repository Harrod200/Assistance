using System;
using ModelShark;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000858 RID: 2136
	internal class ShipOfficerGridItemController : MonoBehaviour
	{
		// Token: 0x06004E60 RID: 20064 RVA: 0x0021B954 File Offset: 0x00219B54
		public void UpdateGridItem(TIOfficerState officer)
		{
			GameControl.assetLoader.LoadAssetForImageAssignment(officer.template.GetIconPath(officer.rank), this.icon);
			this.officerTooltip.SetDelegate("BodyText", () => this.OfficerTip(officer));
			if (this.showName)
			{
				this.officerName.SetText(officer.DisplayNameAndJob);
				this.officerName.enabled = true;
				return;
			}
			if (this.officerName != null)
			{
				this.officerName.enabled = false;
			}
		}

		// Token: 0x06004E61 RID: 20065 RVA: 0x0021BA01 File Offset: 0x00219C01
		private string OfficerTip(TIOfficerState officer)
		{
			return officer.FullDescription;
		}

		// Token: 0x040031F9 RID: 12793
		public Image icon;

		// Token: 0x040031FA RID: 12794
		public TooltipTrigger officerTooltip;

		// Token: 0x040031FB RID: 12795
		public TMP_Text officerName;

		// Token: 0x040031FC RID: 12796
		public bool showName;
	}
}

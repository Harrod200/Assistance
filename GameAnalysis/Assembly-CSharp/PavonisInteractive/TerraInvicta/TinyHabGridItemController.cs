using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000887 RID: 2183
	public class TinyHabGridItemController : MonoBehaviour
	{
		// Token: 0x060051A3 RID: 20899 RVA: 0x0023E996 File Offset: 0x0023CB96
		public void SetGridItem(TIFactionState faction, int value, bool station)
		{
			GameControl.assetLoader.LoadAssetForImageAssignment(station ? faction.template.stationIcon : faction.template.baseIcon, this.habIcon);
			this.valueText.SetText(value.ToString());
		}

		// Token: 0x04003643 RID: 13891
		public Image habIcon;

		// Token: 0x04003644 RID: 13892
		public TMP_Text valueText;
	}
}

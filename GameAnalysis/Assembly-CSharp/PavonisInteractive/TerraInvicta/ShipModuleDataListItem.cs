using System;
using TMPro;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000857 RID: 2135
	public class ShipModuleDataListItem : MonoBehaviour
	{
		// Token: 0x06004E5E RID: 20062 RVA: 0x0021B91D File Offset: 0x00219B1D
		public void Init(FleetsScreenController controller, string name, string value)
		{
			this.controller = controller;
			this.moduleDataName.SetText(name);
			if (!string.IsNullOrEmpty(value))
			{
				this.moduleDataValue.SetText(value);
			}
		}

		// Token: 0x040031F6 RID: 12790
		private FleetsScreenController controller;

		// Token: 0x040031F7 RID: 12791
		public TMP_Text moduleDataName;

		// Token: 0x040031F8 RID: 12792
		public TMP_Text moduleDataValue;
	}
}

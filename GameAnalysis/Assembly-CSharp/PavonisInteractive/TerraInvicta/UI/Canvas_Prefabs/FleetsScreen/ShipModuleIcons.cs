using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.UI.Canvas_Prefabs.FleetsScreen
{
	// Token: 0x02000927 RID: 2343
	public class ShipModuleIcons : MonoBehaviour
	{
		// Token: 0x17000F3A RID: 3898
		// (get) Token: 0x0600597E RID: 22910 RVA: 0x002912E0 File Offset: 0x0028F4E0
		public IEnumerable<ShipModuleListItem> icons
		{
			get
			{
				return this.iconsContainer.GetComponentsInChildren<ShipModuleListItem>();
			}
		}

		// Token: 0x040040A6 RID: 16550
		public Canvas canvas;

		// Token: 0x040040A7 RID: 16551
		public Transform iconsContainer;
	}
}

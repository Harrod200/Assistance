using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000873 RID: 2163
	public class DebugPanel : MonoBehaviour
	{
		// Token: 0x06005102 RID: 20738 RVA: 0x00236CA9 File Offset: 0x00234EA9
		private void Start()
		{
			this.DebugTab.gameObject.SetActive(false);
			base.gameObject.SetActive(false);
		}

		// Token: 0x06005103 RID: 20739 RVA: 0x00236CC8 File Offset: 0x00234EC8
		private void Update()
		{
			this.Content.SetActive(this.TabbedPaneController.IsSelected);
		}

		// Token: 0x040034D2 RID: 13522
		public TabbedPaneController TabbedPaneController;

		// Token: 0x040034D3 RID: 13523
		public GameObject DebugTab;

		// Token: 0x040034D4 RID: 13524
		public GameObject Content;
	}
}

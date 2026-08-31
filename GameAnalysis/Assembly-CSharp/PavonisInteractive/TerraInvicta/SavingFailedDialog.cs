using System;
using TMPro;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008BC RID: 2236
	public class SavingFailedDialog : MonoBehaviour
	{
		// Token: 0x06005570 RID: 21872 RVA: 0x0026D841 File Offset: 0x0026BA41
		private void Awake()
		{
			this.HeaderText.text = Loc.T("UI.Save.Failed.Header");
			this.OkButtonText.text = Loc.T("UI.Save.Failed.Confirm");
		}

		// Token: 0x06005571 RID: 21873 RVA: 0x0026D86D File Offset: 0x0026BA6D
		public void Show(string errorMessage)
		{
			base.gameObject.SetActive(true);
			this.MessageText.text = errorMessage;
		}

		// Token: 0x06005572 RID: 21874 RVA: 0x0026D887 File Offset: 0x0026BA87
		public void Close()
		{
			base.gameObject.SetActive(false);
			this.overlay.SetActive(false);
		}

		// Token: 0x04003BC3 RID: 15299
		public TMP_Text HeaderText;

		// Token: 0x04003BC4 RID: 15300
		public TMP_Text MessageText;

		// Token: 0x04003BC5 RID: 15301
		public TMP_Text OkButtonText;

		// Token: 0x04003BC6 RID: 15302
		public GameObject overlay;
	}
}

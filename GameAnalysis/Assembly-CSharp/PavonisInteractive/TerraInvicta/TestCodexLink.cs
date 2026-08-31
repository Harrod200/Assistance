using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007FF RID: 2047
	public class TestCodexLink : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x06004A45 RID: 19013 RVA: 0x001F269F File Offset: 0x001F089F
		private void Start()
		{
			this.pTextMeshPro = base.GetComponent<TextMeshProUGUI>();
		}

		// Token: 0x06004A46 RID: 19014 RVA: 0x001F26B0 File Offset: 0x001F08B0
		public void OnPointerClick(PointerEventData eventData)
		{
			int num = TMP_TextUtilities.FindIntersectingLink(this.pTextMeshPro, Input.mousePosition, null);
			if (num != -1)
			{
				TMP_LinkInfo tmp_LinkInfo = this.pTextMeshPro.textInfo.linkInfo[num];
				Debug.Log("LinkFound: " + tmp_LinkInfo.GetLinkID());
			}
		}

		// Token: 0x04002B1A RID: 11034
		public TextMeshProUGUI pTextMeshPro;
	}
}

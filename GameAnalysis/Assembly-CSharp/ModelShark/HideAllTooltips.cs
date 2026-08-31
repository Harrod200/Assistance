using System;
using UnityEngine;
using UnityEngine.UI;

namespace ModelShark
{
	// Token: 0x020004AE RID: 1198
	[RequireComponent(typeof(Button))]
	public class HideAllTooltips : MonoBehaviour
	{
		// Token: 0x06001AE7 RID: 6887 RVA: 0x00091776 File Offset: 0x0008F976
		private void Start()
		{
			base.gameObject.GetComponent<Button>().onClick.AddListener(delegate
			{
				TooltipManager.Instance.HideAll();
			});
		}
	}
}

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000809 RID: 2057
	public class SkirmishAddShipListButton : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x06004A75 RID: 19061 RVA: 0x001F3A92 File Offset: 0x001F1C92
		public void OnPointerClick(PointerEventData pointerEventData)
		{
			if (pointerEventData.button == PointerEventData.InputButton.Left)
			{
				this.listItemController.SetAddShipButtonDropdownTooltipDelegates();
			}
		}

		// Token: 0x04002B71 RID: 11121
		public SkirmishShipListItemController listItemController;
	}
}

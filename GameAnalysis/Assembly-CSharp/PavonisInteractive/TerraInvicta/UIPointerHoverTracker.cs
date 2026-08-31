using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000872 RID: 2162
	public class UIPointerHoverTracker : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x060050FF RID: 20735 RVA: 0x00236C8F File Offset: 0x00234E8F
		public void OnPointerEnter(PointerEventData eventData)
		{
			this.IsPointerHovering = true;
		}

		// Token: 0x06005100 RID: 20736 RVA: 0x00236C98 File Offset: 0x00234E98
		public void OnPointerExit(PointerEventData eventData)
		{
			this.IsPointerHovering = false;
		}

		// Token: 0x040034D1 RID: 13521
		[HideInInspector]
		public bool IsPointerHovering;
	}
}

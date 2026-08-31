using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008CD RID: 2253
	public class MouseHoverZoomLockComponent : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x06005663 RID: 22115 RVA: 0x0027830E File Offset: 0x0027650E
		public void OnPointerEnter(PointerEventData eventData)
		{
			TIInputManager.blockCombatZoom = true;
		}

		// Token: 0x06005664 RID: 22116 RVA: 0x00278316 File Offset: 0x00276516
		public void OnPointerExit(PointerEventData eventData)
		{
			TIInputManager.blockCombatZoom = false;
		}
	}
}

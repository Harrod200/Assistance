using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008EB RID: 2283
	public class RightClickHandler : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
	{
		// Token: 0x060057BB RID: 22459 RVA: 0x00284C54 File Offset: 0x00282E54
		public void Awake()
		{
			this.associatedButton = base.gameObject.GetComponent<Button>();
		}

		// Token: 0x060057BC RID: 22460 RVA: 0x00284C68 File Offset: 0x00282E68
		public void OnPointerClick(PointerEventData eventData)
		{
			if (this.associatedButton == null || this.associatedButton.IsInteractable())
			{
				if (eventData.button == PointerEventData.InputButton.Right)
				{
					this.rightClick.Invoke();
					return;
				}
				if (eventData.button == PointerEventData.InputButton.Left)
				{
					this.leftClick.Invoke();
					return;
				}
				if (eventData.button == PointerEventData.InputButton.Middle)
				{
					this.middleClick.Invoke();
				}
			}
		}

		// Token: 0x060057BD RID: 22461 RVA: 0x00284CCD File Offset: 0x00282ECD
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.associatedButton == null || this.associatedButton.IsInteractable())
			{
				this.pointerEnter.Invoke();
			}
		}

		// Token: 0x060057BE RID: 22462 RVA: 0x00284CF5 File Offset: 0x00282EF5
		public void OnPointerExit(PointerEventData eventData)
		{
			if (this.associatedButton == null || this.associatedButton.IsInteractable())
			{
				this.pointerExit.Invoke();
			}
		}

		// Token: 0x04003F55 RID: 16213
		public UnityEvent leftClick;

		// Token: 0x04003F56 RID: 16214
		public UnityEvent middleClick;

		// Token: 0x04003F57 RID: 16215
		public UnityEvent rightClick;

		// Token: 0x04003F58 RID: 16216
		public UnityEvent pointerEnter;

		// Token: 0x04003F59 RID: 16217
		public UnityEvent pointerExit;

		// Token: 0x04003F5A RID: 16218
		private Button associatedButton;
	}
}

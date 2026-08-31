using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PavonisInteractive.TerraInvicta.UI
{
	// Token: 0x02000921 RID: 2337
	public class DragDestination : MonoBehaviour, IDropHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
	{
		// Token: 0x06005962 RID: 22882 RVA: 0x00290C4E File Offset: 0x0028EE4E
		public virtual void SetControllerBase(CanvasControllerBase canvasControllerBase)
		{
		}

		// Token: 0x06005963 RID: 22883 RVA: 0x00290C50 File Offset: 0x0028EE50
		public virtual void OnDrop(PointerEventData eventData)
		{
			if (base.gameObject.activeInHierarchy && DragManager.canDropCurrentItem)
			{
				DragManager.currentItem.Drop(this.dragTarget ? this.dragTarget : base.transform);
			}
		}

		// Token: 0x06005964 RID: 22884 RVA: 0x00290C8B File Offset: 0x0028EE8B
		public virtual void OnPointerEnter(PointerEventData eventData)
		{
			if (base.gameObject.activeInHierarchy && DragManager.currentItem != null && this.CanDropItemHere())
			{
				DragManager.canDropCurrentItem = true;
			}
		}

		// Token: 0x06005965 RID: 22885 RVA: 0x00290CB5 File Offset: 0x0028EEB5
		public virtual void OnPointerExit(PointerEventData eventData)
		{
			if (base.gameObject.activeInHierarchy && DragManager.currentItem != null && this.CanDropItemHere())
			{
				DragManager.canDropCurrentItem = false;
			}
		}

		// Token: 0x06005966 RID: 22886 RVA: 0x00290CDF File Offset: 0x0028EEDF
		protected virtual bool CanDropItemHere()
		{
			return base.gameObject.activeSelf && DragManager.currentDragItemType == this.dragItemType;
		}

		// Token: 0x0400408E RID: 16526
		[SerializeField]
		protected DragItemType dragItemType;

		// Token: 0x0400408F RID: 16527
		[SerializeField]
		protected Transform dragTarget;
	}
}

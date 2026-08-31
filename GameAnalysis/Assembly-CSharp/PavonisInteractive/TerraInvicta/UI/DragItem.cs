using System;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PavonisInteractive.TerraInvicta.UI
{
	// Token: 0x02000923 RID: 2339
	public class DragItem : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler
	{
		// Token: 0x17000F37 RID: 3895
		// (get) Token: 0x06005968 RID: 22888 RVA: 0x00290D05 File Offset: 0x0028EF05
		protected bool singular
		{
			get
			{
				return this.dragItemType == DragItemType.ORG;
			}
		}

		// Token: 0x06005969 RID: 22889 RVA: 0x00290D10 File Offset: 0x0028EF10
		protected virtual void Awake()
		{
			this.canvasGroup = base.GetComponent<CanvasGroup>();
			base.gameObject.name = base.gameObject.name.Replace("(Clone)", "");
			if (this.canvasGroup != null)
			{
				this.canvasGroup.interactable = true;
				this.canvasGroup.blocksRaycasts = true;
			}
		}

		// Token: 0x0600596A RID: 22890 RVA: 0x00290D74 File Offset: 0x0028EF74
		public virtual void Drop(Transform parent)
		{
			base.transform.parent = (parent ? parent : this.startParent);
		}

		// Token: 0x0600596B RID: 22891 RVA: 0x00290D94 File Offset: 0x0028EF94
		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (!this.draggable)
			{
				return;
			}
			if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
			{
				return;
			}
			World.Active.GetExistingManager<GameTimeManager>().Pause();
			if (this.parentWhileDragging == null)
			{
				this.parentWhileDragging = base.GetComponentInParent<Canvas>().transform;
			}
			this.dragging = true;
			DragManager.currentItem = this;
			DragManager.currentDragItemType = this.dragItemType;
			this.startParent = base.transform.parent;
			this.listManager = this.startParent.GetComponent<ListManagerBase>();
			bool flag = false;
			if (this.startParent.childCount == 1 && this.singular && this.listManager != null)
			{
				flag = true;
				global::UnityEngine.Object.Instantiate<GameObject>(base.gameObject, this.startParent);
			}
			this.startSiblingIndex = base.transform.GetSiblingIndex();
			base.transform.SetParent(this.parentWhileDragging, false);
			base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, 0f);
			base.transform.localScale = Vector3.one;
			this.canvasGroup.blocksRaycasts = false;
			if (flag)
			{
				this.listManager.SetListSize<GameObject>(1, false, false);
				this.startParent.GetChild(0).gameObject.SetActive(false);
			}
		}

		// Token: 0x0600596C RID: 22892 RVA: 0x00290EF9 File Offset: 0x0028F0F9
		public virtual void OnDrag(PointerEventData eventData)
		{
			if (this.dragging)
			{
				base.transform.position = eventData.position;
				if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
				{
					this.EndDragCleanup();
				}
			}
		}

		// Token: 0x0600596D RID: 22893 RVA: 0x00290F2F File Offset: 0x0028F12F
		public virtual void OnEndDrag(PointerEventData eventData)
		{
			this.EndDragCleanup();
		}

		// Token: 0x0600596E RID: 22894 RVA: 0x00290F37 File Offset: 0x0028F137
		public void EndDragCleanup()
		{
			DragManager.canDropCurrentItem = false;
			if (base.gameObject.activeInHierarchy)
			{
				this.Reset();
			}
			this.dragging = false;
		}

		// Token: 0x0600596F RID: 22895 RVA: 0x00290F5C File Offset: 0x0028F15C
		public void Reset()
		{
			if (this.startParent)
			{
				base.transform.SetParent(this.startParent, false);
				base.transform.localScale = Vector3.one;
				base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, 0f);
				base.transform.SetSiblingIndex(this.startSiblingIndex);
			}
			base.gameObject.SetActive(true);
			DragManager.currentItem = null;
			this.canvasGroup.blocksRaycasts = true;
			this.startParent = null;
		}

		// Token: 0x04004095 RID: 16533
		[SerializeField]
		protected DragItemType dragItemType;

		// Token: 0x04004096 RID: 16534
		protected Transform parentWhileDragging;

		// Token: 0x04004097 RID: 16535
		protected CanvasGroup canvasGroup;

		// Token: 0x04004098 RID: 16536
		protected Transform startParent;

		// Token: 0x04004099 RID: 16537
		protected int startSiblingIndex;

		// Token: 0x0400409A RID: 16538
		protected ListManagerBase listManager;

		// Token: 0x0400409B RID: 16539
		protected bool pausingClock;

		// Token: 0x0400409C RID: 16540
		protected bool dragging;

		// Token: 0x0400409D RID: 16541
		[HideInInspector]
		public bool draggable = true;
	}
}

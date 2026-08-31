using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LapinerTools.uMyGUI
{
	// Token: 0x02000523 RID: 1315
	public class uMyGUI_Draggable : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler
	{
		// Token: 0x17000486 RID: 1158
		// (get) Token: 0x06002079 RID: 8313 RVA: 0x000A88B1 File Offset: 0x000A6AB1
		// (set) Token: 0x0600207A RID: 8314 RVA: 0x000A88B9 File Offset: 0x000A6AB9
		public bool IsResetRotationWhenDragged
		{
			get
			{
				return this.m_isResetRotationWhenDragged;
			}
			set
			{
				this.m_isResetRotationWhenDragged = value;
			}
		}

		// Token: 0x17000487 RID: 1159
		// (get) Token: 0x0600207B RID: 8315 RVA: 0x000A88C2 File Offset: 0x000A6AC2
		// (set) Token: 0x0600207C RID: 8316 RVA: 0x000A88CA File Offset: 0x000A6ACA
		public bool IsSnapBackOnEndDrag
		{
			get
			{
				return this.m_isSnapBackOnEndDrag;
			}
			set
			{
				this.m_isSnapBackOnEndDrag = value;
			}
		}

		// Token: 0x17000488 RID: 1160
		// (get) Token: 0x0600207D RID: 8317 RVA: 0x000A88D3 File Offset: 0x000A6AD3
		// (set) Token: 0x0600207E RID: 8318 RVA: 0x000A88DB File Offset: 0x000A6ADB
		public bool IsTopInHierarchyWhenDragged
		{
			get
			{
				return this.m_isTopInHierarchyWhenDragged;
			}
			set
			{
				this.m_isTopInHierarchyWhenDragged = value;
			}
		}

		// Token: 0x17000489 RID: 1161
		// (get) Token: 0x0600207F RID: 8319 RVA: 0x000A88E4 File Offset: 0x000A6AE4
		// (set) Token: 0x06002080 RID: 8320 RVA: 0x000A88EC File Offset: 0x000A6AEC
		public CanvasGroup DisableBlocksRaycastsOnDrag
		{
			get
			{
				return this.m_disableBlocksRaycastsOnDrag;
			}
			set
			{
				this.m_disableBlocksRaycastsOnDrag = value;
			}
		}

		// Token: 0x1700048A RID: 1162
		// (get) Token: 0x06002081 RID: 8321 RVA: 0x000A88F5 File Offset: 0x000A6AF5
		public bool IsDragged
		{
			get
			{
				return this.m_isDragged;
			}
		}

		// Token: 0x06002082 RID: 8322 RVA: 0x000A8900 File Offset: 0x000A6B00
		public void OnBeginDrag(PointerEventData p_event)
		{
			this.m_isDragged = true;
			this.m_initialParentTransform = this.m_transform.parent as RectTransform;
			this.m_initialPosition = this.m_transform.position;
			this.m_initialRotation = this.m_transform.rotation;
			Vector3 vector;
			if (RectTransformUtility.ScreenPointToWorldPointInRectangle(this.m_canvasTransform, p_event.position, p_event.pressEventCamera, out vector))
			{
				this.m_dragOffset = this.m_transform.position - vector;
			}
			else
			{
				this.m_dragOffset = Vector3.zero;
			}
			if (this.m_isResetRotationWhenDragged)
			{
				this.m_transform.rotation = Quaternion.identity;
			}
			if (this.m_isTopInHierarchyWhenDragged)
			{
				this.m_initialSiblingIndex = this.m_transform.GetSiblingIndex();
				this.m_transform.SetParent(this.m_canvasTransform, true);
				this.m_transform.SetAsLastSibling();
			}
			if (this.m_disableBlocksRaycastsOnDrag != null)
			{
				this.m_disableBlocksRaycastsOnDrag.blocksRaycasts = false;
			}
			if (this.m_onBeginDrag != null)
			{
				this.m_onBeginDrag(this, new uMyGUI_Draggable.DragEvent(p_event));
			}
		}

		// Token: 0x06002083 RID: 8323 RVA: 0x000A8A10 File Offset: 0x000A6C10
		public void OnDrag(PointerEventData p_event)
		{
			Vector3 vector;
			if (RectTransformUtility.ScreenPointToWorldPointInRectangle(this.m_canvasTransform, p_event.position, p_event.pressEventCamera, out vector))
			{
				this.m_transform.position = vector + this.m_dragOffset;
			}
			if (this.m_onDrag != null)
			{
				this.m_onDrag(this, new uMyGUI_Draggable.DragEvent(p_event));
			}
		}

		// Token: 0x06002084 RID: 8324 RVA: 0x000A8A6C File Offset: 0x000A6C6C
		public void OnEndDrag(PointerEventData p_event)
		{
			if (this.m_isDragged)
			{
				this.m_isDragged = false;
				if (this.m_isSnapBackOnEndDrag)
				{
					this.m_transform.position = this.m_initialPosition;
					this.m_transform.rotation = this.m_initialRotation;
				}
				if (this.m_isTopInHierarchyWhenDragged)
				{
					this.m_transform.SetParent(this.m_initialParentTransform, true);
					this.m_transform.SetSiblingIndex(this.m_initialSiblingIndex);
				}
				if (this.m_disableBlocksRaycastsOnDrag != null)
				{
					this.m_disableBlocksRaycastsOnDrag.blocksRaycasts = true;
				}
				if (this.m_onEndDrag != null)
				{
					this.m_onEndDrag(this, new uMyGUI_Draggable.DragEvent(p_event));
				}
			}
		}

		// Token: 0x06002085 RID: 8325 RVA: 0x000A8B14 File Offset: 0x000A6D14
		private void Start()
		{
			Canvas componentInParent = base.GetComponentInParent<Canvas>();
			if (componentInParent != null)
			{
				this.m_canvasTransform = componentInParent.GetComponent<RectTransform>();
			}
			else
			{
				Debug.LogError("uMyGUI_Draggable: no Canvas component was found in parent!");
				base.enabled = false;
			}
			this.m_transform = base.GetComponent<RectTransform>();
		}

		// Token: 0x0400191E RID: 6430
		[SerializeField]
		private bool m_isResetRotationWhenDragged;

		// Token: 0x0400191F RID: 6431
		[SerializeField]
		private bool m_isSnapBackOnEndDrag;

		// Token: 0x04001920 RID: 6432
		[SerializeField]
		private bool m_isTopInHierarchyWhenDragged = true;

		// Token: 0x04001921 RID: 6433
		[SerializeField]
		private CanvasGroup m_disableBlocksRaycastsOnDrag;

		// Token: 0x04001922 RID: 6434
		private bool m_isDragged;

		// Token: 0x04001923 RID: 6435
		public EventHandler<uMyGUI_Draggable.DragEvent> m_onBeginDrag;

		// Token: 0x04001924 RID: 6436
		public EventHandler<uMyGUI_Draggable.DragEvent> m_onDrag;

		// Token: 0x04001925 RID: 6437
		public EventHandler<uMyGUI_Draggable.DragEvent> m_onEndDrag;

		// Token: 0x04001926 RID: 6438
		private RectTransform m_initialParentTransform;

		// Token: 0x04001927 RID: 6439
		private RectTransform m_canvasTransform;

		// Token: 0x04001928 RID: 6440
		private RectTransform m_transform;

		// Token: 0x04001929 RID: 6441
		private int m_initialSiblingIndex;

		// Token: 0x0400192A RID: 6442
		private Vector3 m_initialPosition;

		// Token: 0x0400192B RID: 6443
		private Quaternion m_initialRotation;

		// Token: 0x0400192C RID: 6444
		private Vector3 m_dragOffset = Vector3.zero;

		// Token: 0x02000C8E RID: 3214
		public class DragEvent : EventArgs
		{
			// Token: 0x06006D22 RID: 27938 RVA: 0x0030A63A File Offset: 0x0030883A
			public DragEvent(PointerEventData p_event)
			{
				this.m_event = p_event;
			}

			// Token: 0x04004EEC RID: 20204
			public readonly PointerEventData m_event;
		}
	}
}

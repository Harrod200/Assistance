using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LapinerTools.uMyGUI
{
	// Token: 0x02000530 RID: 1328
	public class uMyGUI_TreeBrowser : MonoBehaviour
	{
		// Token: 0x170004A6 RID: 1190
		// (get) Token: 0x0600210F RID: 8463 RVA: 0x000AAFDD File Offset: 0x000A91DD
		// (set) Token: 0x06002110 RID: 8464 RVA: 0x000AAFE5 File Offset: 0x000A91E5
		public GameObject InnerNodePrefab
		{
			get
			{
				return this.m_innerNodePrefab;
			}
			set
			{
				this.m_innerNodePrefab = value;
			}
		}

		// Token: 0x170004A7 RID: 1191
		// (get) Token: 0x06002111 RID: 8465 RVA: 0x000AAFEE File Offset: 0x000A91EE
		// (set) Token: 0x06002112 RID: 8466 RVA: 0x000AAFF6 File Offset: 0x000A91F6
		public GameObject LeafNodePrefab
		{
			get
			{
				return this.m_leafNodePrefab;
			}
			set
			{
				this.m_leafNodePrefab = value;
			}
		}

		// Token: 0x170004A8 RID: 1192
		// (get) Token: 0x06002113 RID: 8467 RVA: 0x000AAFFF File Offset: 0x000A91FF
		// (set) Token: 0x06002114 RID: 8468 RVA: 0x000AB007 File Offset: 0x000A9207
		public float OffsetStart
		{
			get
			{
				return this.m_offsetStart;
			}
			set
			{
				this.m_offsetStart = value;
			}
		}

		// Token: 0x170004A9 RID: 1193
		// (get) Token: 0x06002115 RID: 8469 RVA: 0x000AB010 File Offset: 0x000A9210
		// (set) Token: 0x06002116 RID: 8470 RVA: 0x000AB018 File Offset: 0x000A9218
		public float OffsetEnd
		{
			get
			{
				return this.m_offsetEnd;
			}
			set
			{
				this.m_offsetEnd = value;
			}
		}

		// Token: 0x170004AA RID: 1194
		// (get) Token: 0x06002117 RID: 8471 RVA: 0x000AB021 File Offset: 0x000A9221
		// (set) Token: 0x06002118 RID: 8472 RVA: 0x000AB029 File Offset: 0x000A9229
		public float Padding
		{
			get
			{
				return this.m_padding;
			}
			set
			{
				this.m_padding = value;
			}
		}

		// Token: 0x170004AB RID: 1195
		// (get) Token: 0x06002119 RID: 8473 RVA: 0x000AB032 File Offset: 0x000A9232
		// (set) Token: 0x0600211A RID: 8474 RVA: 0x000AB03A File Offset: 0x000A923A
		public float IndentSize
		{
			get
			{
				return this.m_indentSize;
			}
			set
			{
				this.m_indentSize = value;
			}
		}

		// Token: 0x170004AC RID: 1196
		// (get) Token: 0x0600211B RID: 8475 RVA: 0x000AB043 File Offset: 0x000A9243
		// (set) Token: 0x0600211C RID: 8476 RVA: 0x000AB04B File Offset: 0x000A924B
		public float ForcedEntryHeight
		{
			get
			{
				return this.m_forcedEntryHeight;
			}
			set
			{
				this.m_forcedEntryHeight = value;
			}
		}

		// Token: 0x170004AD RID: 1197
		// (get) Token: 0x0600211D RID: 8477 RVA: 0x000AB054 File Offset: 0x000A9254
		// (set) Token: 0x0600211E RID: 8478 RVA: 0x000AB05C File Offset: 0x000A925C
		public bool UseExplicitNavigation
		{
			get
			{
				return this.m_useExplicitNavigation;
			}
			set
			{
				this.m_useExplicitNavigation = value;
			}
		}

		// Token: 0x170004AE RID: 1198
		// (get) Token: 0x0600211F RID: 8479 RVA: 0x000AB065 File Offset: 0x000A9265
		// (set) Token: 0x06002120 RID: 8480 RVA: 0x000AB06D File Offset: 0x000A926D
		public float NavScrollSpeed
		{
			get
			{
				return this.m_navScrollSpeed;
			}
			set
			{
				this.m_navScrollSpeed = value;
			}
		}

		// Token: 0x170004AF RID: 1199
		// (get) Token: 0x06002121 RID: 8481 RVA: 0x000AB076 File Offset: 0x000A9276
		// (set) Token: 0x06002122 RID: 8482 RVA: 0x000AB07E File Offset: 0x000A927E
		public float NavScrollSmooth
		{
			get
			{
				return this.m_navScrollSmooth;
			}
			set
			{
				this.m_navScrollSmooth = value;
			}
		}

		// Token: 0x170004B0 RID: 1200
		// (get) Token: 0x06002123 RID: 8483 RVA: 0x000AB087 File Offset: 0x000A9287
		public ScrollRect ParentScroller
		{
			get
			{
				return this.m_parentScroller;
			}
		}

		// Token: 0x170004B1 RID: 1201
		// (get) Token: 0x06002124 RID: 8484 RVA: 0x000AB090 File Offset: 0x000A9290
		private RectTransform RTransform
		{
			get
			{
				if (!(this.m_rectTransform != null))
				{
					return this.m_rectTransform = base.GetComponent<RectTransform>();
				}
				return this.m_rectTransform;
			}
		}

		// Token: 0x06002125 RID: 8485 RVA: 0x000AB0C1 File Offset: 0x000A92C1
		public void BuildTree(uMyGUI_TreeBrowser.Node[] p_rootNodes)
		{
			this.BuildTree(p_rootNodes, 0, 0);
		}

		// Token: 0x06002126 RID: 8486 RVA: 0x000AB0CC File Offset: 0x000A92CC
		public void BuildTree(uMyGUI_TreeBrowser.Node[] p_rootNodes, int p_insertAt, int p_indentLevel)
		{
			if (this.m_innerNodePrefab != null && this.m_leafNodePrefab != null)
			{
				List<uMyGUI_TreeBrowser.InternalNode> list = new List<uMyGUI_TreeBrowser.InternalNode>();
				float num = 0f;
				float num2 = ((this.m_nodes.Count >= p_insertAt && p_insertAt > 0) ? this.m_nodes[p_insertAt - 1].m_minY : (-this.m_offsetStart));
				for (int i = 0; i < p_rootNodes.Length; i++)
				{
					if (p_rootNodes[i] != null)
					{
						bool flag = p_rootNodes[i].Children != null && p_rootNodes[i].Children.Length != 0;
						GameObject gameObject;
						if (flag)
						{
							gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.m_innerNodePrefab);
						}
						else
						{
							gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.m_leafNodePrefab);
						}
						RectTransform component = gameObject.GetComponent<RectTransform>();
						if (this.m_forcedEntryHeight != 0f)
						{
							component.sizeDelta = new Vector2(component.sizeDelta.x, this.m_forcedEntryHeight);
						}
						float height = component.rect.height;
						if (p_rootNodes[i].SendMessageData != null)
						{
							if (!gameObject.activeInHierarchy)
							{
								Debug.LogError("uMyGUI_TreeBrowser: BuildTree: node has SendMessageData set, but instance is inactive! SendMessage call will fail! Make your prefab active!");
							}
							gameObject.SendMessage("uMyGUI_TreeBrowser_InitNode", p_rootNodes[i].SendMessageData);
						}
						uMyGUI_TreeBrowser.InternalNode internalNode = new uMyGUI_TreeBrowser.InternalNode(p_rootNodes[i], gameObject, p_indentLevel);
						list.Add(internalNode);
						if (flag)
						{
							this.SetupInnerNode(internalNode);
						}
						else
						{
							this.SetupLeafNode(internalNode);
						}
						num2 = this.SetRectTransformPosition(component, num2, height, p_indentLevel);
						internalNode.m_minY = component.anchoredPosition.y - height;
						num = internalNode.m_minY;
						if (this.OnNodeInstantiate != null)
						{
							this.OnNodeInstantiate(this, new uMyGUI_TreeBrowser.NodeInstantiateEventArgs(p_rootNodes[i], gameObject));
						}
					}
				}
				if (p_insertAt < this.m_nodes.Count)
				{
					float num3;
					if (p_insertAt == 0)
					{
						num3 = num;
					}
					else
					{
						num3 = num - this.m_nodes[p_insertAt - 1].m_minY;
					}
					this.UpdateNodePosition(p_insertAt, num3);
				}
				if (p_insertAt < this.m_nodes.Count)
				{
					this.m_nodes.InsertRange(p_insertAt, list);
				}
				else
				{
					this.m_nodes.AddRange(list);
				}
				if (this.m_nodes.Count > 0)
				{
					this.RTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Abs(this.m_nodes[this.m_nodes.Count - 1].m_minY - this.RTransform.rect.yMax - this.m_offsetEnd));
				}
				if (this.m_useExplicitNavigation)
				{
					this.SetExplicitNavigationTargets();
					return;
				}
			}
			else
			{
				Debug.LogError("uMyGUI_TreeBrowser: BuildTree: you must provide the InnerNodePrefab and LeafNodePrefab in the inspector or via script!");
			}
		}

		// Token: 0x06002127 RID: 8487 RVA: 0x000AB344 File Offset: 0x000A9544
		public void Clear()
		{
			for (int i = 0; i < this.m_nodes.Count; i++)
			{
				global::UnityEngine.Object.Destroy(this.m_nodes[i].m_instance);
			}
			this.m_nodes.Clear();
		}

		// Token: 0x06002128 RID: 8488 RVA: 0x000AB388 File Offset: 0x000A9588
		private void Start()
		{
			this.m_parentScroller = base.GetComponentInParent<ScrollRect>();
		}

		// Token: 0x06002129 RID: 8489 RVA: 0x000AB398 File Offset: 0x000A9598
		private void LateUpdate()
		{
			if (this.m_parentScroller == null)
			{
				return;
			}
			EventSystem current = EventSystem.current;
			GameObject currentSelectedGameObject;
			if (current != null && (currentSelectedGameObject = current.currentSelectedGameObject) != null && currentSelectedGameObject.transform.IsChildOf(base.transform))
			{
				if (currentSelectedGameObject != this.m_lastSelectedGO)
				{
					this.m_lastSelectedGO = currentSelectedGameObject;
					Transform transform = currentSelectedGameObject.transform;
					while (transform.parent != base.transform && transform.parent != null)
					{
						transform = transform.parent;
					}
					RectTransform component = transform.GetComponent<RectTransform>();
					if (component == null)
					{
						return;
					}
					Vector3[] array = new Vector3[4];
					component.GetWorldCorners(array);
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = this.m_parentScroller.transform.InverseTransformPoint(array[i]);
					}
					Vector3 vector = Vector3.Min(Vector3.Min(array[0], array[1]), Vector3.Min(array[2], array[3]));
					Vector3 vector2 = Vector3.Max(Vector3.Max(array[0], array[1]), Vector3.Max(array[2], array[3]));
					this.m_parentScroller.GetComponent<RectTransform>().GetLocalCorners(array);
					Vector3 vector3 = Vector3.Min(Vector3.Min(array[0], array[1]), Vector3.Min(array[2], array[3]));
					Vector3 vector4 = Vector3.Max(Vector3.Max(array[0], array[1]), Vector3.Max(array[2], array[3]));
					if (vector.y < vector3.y)
					{
						if (this.m_parentScroller.verticalNormalizedPosition >= 1f)
						{
							this.m_parentScroller.verticalNormalizedPosition = 0.999f;
						}
						this.m_parentScroller.velocity = Vector3.up * Mathf.Max(5f, this.m_navScrollSpeed * ((this.m_navScrollSmooth != 0f) ? ((vector3.y - vector.y) / this.m_navScrollSmooth) : 1f));
						this.m_lastSelectedGO = null;
						return;
					}
					if (vector2.y > vector4.y)
					{
						if (this.m_parentScroller.verticalNormalizedPosition <= 0f)
						{
							this.m_parentScroller.verticalNormalizedPosition = 0.001f;
						}
						this.m_parentScroller.velocity = Vector3.down * Mathf.Max(5f, this.m_navScrollSpeed * ((this.m_navScrollSmooth != 0f) ? ((vector2.y - vector4.y) / this.m_navScrollSmooth) : 1f));
						this.m_lastSelectedGO = null;
						return;
					}
				}
			}
			else
			{
				this.m_lastSelectedGO = null;
			}
		}

		// Token: 0x0600212A RID: 8490 RVA: 0x000AB68C File Offset: 0x000A988C
		private void OnDestroy()
		{
			this.OnInnerNodeClick = null;
			this.OnLeafNodeClick = null;
			this.OnNodeInstantiate = null;
		}

		// Token: 0x0600212B RID: 8491 RVA: 0x000AB6A4 File Offset: 0x000A98A4
		private void SetExplicitNavigationTargets()
		{
			if (this.m_nodes.Count > 2)
			{
				RectTransform rectTransform = this.m_nodes[0].m_transform;
				RectTransform rectTransform2 = this.m_nodes[1].m_transform;
				Selectable[] array = rectTransform.GetComponentsInChildren<Selectable>();
				Selectable[] array2 = rectTransform2.GetComponentsInChildren<Selectable>();
				this.SetAutomaticNavigation(this.m_nodes[0].m_transform);
				this.SetAutomaticNavigation(this.m_nodes[this.m_nodes.Count - 1].m_transform);
				for (int i = 1; i < this.m_nodes.Count - 1; i++)
				{
					global::UnityEngine.Object @object = rectTransform;
					rectTransform = rectTransform2;
					rectTransform2 = this.m_nodes[i + 1].m_transform;
					Selectable[] array3 = array;
					array = array2;
					array2 = rectTransform2.GetComponentsInChildren<Selectable>();
					if (@object != null && rectTransform != null && rectTransform2 != null && array3.Length == array.Length && array2.Length == array.Length)
					{
						for (int j = 0; j < array.Length; j++)
						{
							Navigation navigation = array[j].navigation;
							navigation.mode = Navigation.Mode.Explicit;
							navigation.selectOnUp = array3[j];
							navigation.selectOnDown = array2[j];
							array[j].navigation = navigation;
						}
					}
				}
			}
		}

		// Token: 0x0600212C RID: 8492 RVA: 0x000AB7EC File Offset: 0x000A99EC
		private void SetAutomaticNavigation(RectTransform p_nodeTransform)
		{
			if (p_nodeTransform != null)
			{
				Selectable[] componentsInChildren = p_nodeTransform.GetComponentsInChildren<Selectable>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					Navigation navigation = componentsInChildren[i].navigation;
					navigation.mode = Navigation.Mode.Automatic;
					componentsInChildren[i].navigation = navigation;
				}
			}
		}

		// Token: 0x0600212D RID: 8493 RVA: 0x000AB834 File Offset: 0x000A9A34
		private float SetRectTransformPosition(RectTransform p_transform, float p_currY, float p_size, int p_indentLevel)
		{
			p_transform.SetParent(this.RTransform, false);
			Vector2 anchoredPosition = p_transform.anchoredPosition;
			anchoredPosition.x += (float)p_indentLevel * this.m_indentSize;
			anchoredPosition.y += p_currY;
			p_currY -= this.m_padding + p_size;
			p_transform.anchoredPosition = anchoredPosition;
			return p_currY;
		}

		// Token: 0x0600212E RID: 8494 RVA: 0x000AB88C File Offset: 0x000A9A8C
		private void UpdateNodePosition(int p_startIndex, float p_moveDist)
		{
			for (int i = p_startIndex; i < this.m_nodes.Count; i++)
			{
				Vector2 anchoredPosition = this.m_nodes[i].m_transform.anchoredPosition;
				anchoredPosition.y += p_moveDist;
				this.m_nodes[i].m_transform.anchoredPosition = anchoredPosition;
				this.m_nodes[i].m_minY = anchoredPosition.y - this.m_nodes[i].m_transform.rect.height;
			}
		}

		// Token: 0x0600212F RID: 8495 RVA: 0x000AB924 File Offset: 0x000A9B24
		private void SetupInnerNode(uMyGUI_TreeBrowser.InternalNode p_node)
		{
			if (p_node.m_instance.GetComponent<Button>() != null)
			{
				p_node.m_instance.GetComponent<Button>().onClick.AddListener(delegate
				{
					this.ToggleInnerNodeFoldout(p_node);
				});
				return;
			}
			if (p_node.m_instance.GetComponent<Toggle>() != null)
			{
				p_node.m_instance.GetComponent<Toggle>().onValueChanged.AddListener(delegate(bool p_isOn)
				{
					this.ToggleInnerNodeFoldout(p_node);
				});
				return;
			}
			Debug.LogError("uMyGUI_TreeBrowser: BuildTree: the inner node prefabs must have either a Button or a Toggle script attached to the root. Otherwise they cannot fold out!");
		}

		// Token: 0x06002130 RID: 8496 RVA: 0x000AB9D0 File Offset: 0x000A9BD0
		private void SetupLeafNode(uMyGUI_TreeBrowser.InternalNode p_node)
		{
			if (p_node.m_instance.GetComponent<Button>() != null)
			{
				p_node.m_instance.GetComponent<Button>().onClick.AddListener(delegate
				{
					this.SafeCallOnLeafNodeClick(p_node);
				});
			}
			else if (p_node.m_instance.GetComponent<Toggle>() != null)
			{
				p_node.m_instance.GetComponent<Toggle>().onValueChanged.AddListener(delegate(bool p_isOn)
				{
					this.SafeCallOnLeafNodeClick(p_node);
				});
			}
			EventTrigger eventTrigger = p_node.m_instance.GetComponent<EventTrigger>();
			if (eventTrigger == null)
			{
				eventTrigger = p_node.m_instance.AddComponent<EventTrigger>();
			}
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerDown;
			EventTrigger.TriggerEvent triggerEvent = new EventTrigger.TriggerEvent();
			triggerEvent.AddListener(delegate(BaseEventData p_downEvent)
			{
				this.SafeCallOnLeafNodePointerDown(p_node);
			});
			entry.callback = triggerEvent;
			if (eventTrigger.triggers == null)
			{
				eventTrigger.triggers = new List<EventTrigger.Entry>();
			}
			eventTrigger.triggers.Add(entry);
		}

		// Token: 0x06002131 RID: 8497 RVA: 0x000ABAE8 File Offset: 0x000A9CE8
		private void ToggleInnerNodeFoldout(uMyGUI_TreeBrowser.InternalNode p_node)
		{
			int num = this.m_nodes.IndexOf(p_node);
			p_node.m_isFoldout = !p_node.m_isFoldout;
			if (p_node.m_isFoldout)
			{
				this.BuildTree(p_node.m_node.Children, num + 1, p_node.m_indentLevel + 1);
			}
			else
			{
				float num2 = 0f;
				for (int i = 0; i < p_node.m_node.Children.Length; i++)
				{
					int num3 = num + p_node.m_node.Children.Length - i;
					uMyGUI_TreeBrowser.InternalNode internalNode = this.m_nodes[num3];
					num2 += internalNode.m_transform.rect.height;
					if (i + 1 < p_node.m_node.Children.Length)
					{
						num2 += this.m_padding;
					}
					if (internalNode.m_isFoldout)
					{
						this.ToggleInnerNodeFoldout(internalNode);
					}
					this.m_nodes.RemoveAt(num3);
					global::UnityEngine.Object.Destroy(internalNode.m_instance);
				}
				this.UpdateNodePosition(num + 1, num2);
				this.RTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.RTransform.sizeDelta.y - num2);
			}
			if (this.OnInnerNodeClick != null)
			{
				this.OnInnerNodeClick(this, new uMyGUI_TreeBrowser.NodeClickEventArgs(p_node.m_node));
			}
		}

		// Token: 0x06002132 RID: 8498 RVA: 0x000ABC21 File Offset: 0x000A9E21
		private void SafeCallOnLeafNodePointerDown(uMyGUI_TreeBrowser.InternalNode p_node)
		{
			if (this.OnLeafNodePointerDown != null)
			{
				this.OnLeafNodePointerDown(this, new uMyGUI_TreeBrowser.NodeClickEventArgs(p_node.m_node));
			}
		}

		// Token: 0x06002133 RID: 8499 RVA: 0x000ABC42 File Offset: 0x000A9E42
		private void SafeCallOnLeafNodeClick(uMyGUI_TreeBrowser.InternalNode p_node)
		{
			if (this.OnLeafNodeClick != null)
			{
				this.OnLeafNodeClick(this, new uMyGUI_TreeBrowser.NodeClickEventArgs(p_node.m_node));
			}
		}

		// Token: 0x04001988 RID: 6536
		[SerializeField]
		private GameObject m_innerNodePrefab;

		// Token: 0x04001989 RID: 6537
		[SerializeField]
		private GameObject m_leafNodePrefab;

		// Token: 0x0400198A RID: 6538
		[SerializeField]
		private float m_offsetStart;

		// Token: 0x0400198B RID: 6539
		[SerializeField]
		private float m_offsetEnd;

		// Token: 0x0400198C RID: 6540
		[SerializeField]
		private float m_padding = 4f;

		// Token: 0x0400198D RID: 6541
		[SerializeField]
		private float m_indentSize = 20f;

		// Token: 0x0400198E RID: 6542
		[SerializeField]
		private float m_forcedEntryHeight;

		// Token: 0x0400198F RID: 6543
		[SerializeField]
		private bool m_useExplicitNavigation;

		// Token: 0x04001990 RID: 6544
		[SerializeField]
		private float m_navScrollSpeed = 200f;

		// Token: 0x04001991 RID: 6545
		[SerializeField]
		private float m_navScrollSmooth = 20f;

		// Token: 0x04001992 RID: 6546
		private ScrollRect m_parentScroller;

		// Token: 0x04001993 RID: 6547
		public EventHandler<uMyGUI_TreeBrowser.NodeClickEventArgs> OnInnerNodeClick;

		// Token: 0x04001994 RID: 6548
		public EventHandler<uMyGUI_TreeBrowser.NodeClickEventArgs> OnLeafNodeClick;

		// Token: 0x04001995 RID: 6549
		public EventHandler<uMyGUI_TreeBrowser.NodeClickEventArgs> OnLeafNodePointerDown;

		// Token: 0x04001996 RID: 6550
		public EventHandler<uMyGUI_TreeBrowser.NodeInstantiateEventArgs> OnNodeInstantiate;

		// Token: 0x04001997 RID: 6551
		private RectTransform m_rectTransform;

		// Token: 0x04001998 RID: 6552
		private List<uMyGUI_TreeBrowser.InternalNode> m_nodes = new List<uMyGUI_TreeBrowser.InternalNode>();

		// Token: 0x04001999 RID: 6553
		private GameObject m_lastSelectedGO;

		// Token: 0x02000C97 RID: 3223
		public class Node
		{
			// Token: 0x06006D3D RID: 27965 RVA: 0x0030A87B File Offset: 0x00308A7B
			public Node(object p_sendMessageData, uMyGUI_TreeBrowser.Node[] p_children)
			{
				this.SendMessageData = p_sendMessageData;
				this.Children = p_children;
			}

			// Token: 0x04004F04 RID: 20228
			public readonly object SendMessageData;

			// Token: 0x04004F05 RID: 20229
			public readonly uMyGUI_TreeBrowser.Node[] Children;
		}

		// Token: 0x02000C98 RID: 3224
		public class NodeClickEventArgs : EventArgs
		{
			// Token: 0x06006D3E RID: 27966 RVA: 0x0030A891 File Offset: 0x00308A91
			public NodeClickEventArgs(uMyGUI_TreeBrowser.Node p_clickedNode)
			{
				this.ClickedNode = p_clickedNode;
			}

			// Token: 0x04004F06 RID: 20230
			public readonly uMyGUI_TreeBrowser.Node ClickedNode;
		}

		// Token: 0x02000C99 RID: 3225
		public class NodeInstantiateEventArgs : EventArgs
		{
			// Token: 0x06006D3F RID: 27967 RVA: 0x0030A8A0 File Offset: 0x00308AA0
			public NodeInstantiateEventArgs(uMyGUI_TreeBrowser.Node p_node, GameObject p_instance)
			{
				this.Node = p_node;
				this.Instance = p_instance;
			}

			// Token: 0x04004F07 RID: 20231
			public readonly uMyGUI_TreeBrowser.Node Node;

			// Token: 0x04004F08 RID: 20232
			public readonly GameObject Instance;
		}

		// Token: 0x02000C9A RID: 3226
		private class InternalNode
		{
			// Token: 0x06006D40 RID: 27968 RVA: 0x0030A8B6 File Offset: 0x00308AB6
			public InternalNode(uMyGUI_TreeBrowser.Node p_node, GameObject p_instance, int p_indentLevel)
			{
				this.m_node = p_node;
				this.m_instance = p_instance;
				this.m_indentLevel = p_indentLevel;
				this.m_transform = this.m_instance.GetComponent<RectTransform>();
			}

			// Token: 0x04004F09 RID: 20233
			public readonly uMyGUI_TreeBrowser.Node m_node;

			// Token: 0x04004F0A RID: 20234
			public GameObject m_instance;

			// Token: 0x04004F0B RID: 20235
			public int m_indentLevel;

			// Token: 0x04004F0C RID: 20236
			public RectTransform m_transform;

			// Token: 0x04004F0D RID: 20237
			public bool m_isFoldout;

			// Token: 0x04004F0E RID: 20238
			public float m_minY;
		}
	}
}

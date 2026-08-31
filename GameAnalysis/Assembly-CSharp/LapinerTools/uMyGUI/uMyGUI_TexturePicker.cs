using System;
using UnityEngine;
using UnityEngine.UI;

namespace LapinerTools.uMyGUI
{
	// Token: 0x0200052F RID: 1327
	public class uMyGUI_TexturePicker : MonoBehaviour
	{
		// Token: 0x1700049D RID: 1181
		// (get) Token: 0x060020F9 RID: 8441 RVA: 0x000AABC0 File Offset: 0x000A8DC0
		// (set) Token: 0x060020FA RID: 8442 RVA: 0x000AABC8 File Offset: 0x000A8DC8
		public GameObject TexturePrefab
		{
			get
			{
				return this.m_texturePrefab;
			}
			set
			{
				this.m_texturePrefab = value;
			}
		}

		// Token: 0x1700049E RID: 1182
		// (get) Token: 0x060020FB RID: 8443 RVA: 0x000AABD1 File Offset: 0x000A8DD1
		// (set) Token: 0x060020FC RID: 8444 RVA: 0x000AABD9 File Offset: 0x000A8DD9
		public GameObject SelectionPrefab
		{
			get
			{
				return this.m_selectionPrefab;
			}
			set
			{
				this.m_selectionPrefab = value;
			}
		}

		// Token: 0x1700049F RID: 1183
		// (get) Token: 0x060020FD RID: 8445 RVA: 0x000AABE2 File Offset: 0x000A8DE2
		// (set) Token: 0x060020FE RID: 8446 RVA: 0x000AABEA File Offset: 0x000A8DEA
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

		// Token: 0x170004A0 RID: 1184
		// (get) Token: 0x060020FF RID: 8447 RVA: 0x000AABF3 File Offset: 0x000A8DF3
		// (set) Token: 0x06002100 RID: 8448 RVA: 0x000AABFB File Offset: 0x000A8DFB
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

		// Token: 0x170004A1 RID: 1185
		// (get) Token: 0x06002101 RID: 8449 RVA: 0x000AAC04 File Offset: 0x000A8E04
		// (set) Token: 0x06002102 RID: 8450 RVA: 0x000AAC0C File Offset: 0x000A8E0C
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

		// Token: 0x170004A2 RID: 1186
		// (get) Token: 0x06002103 RID: 8451 RVA: 0x000AAC15 File Offset: 0x000A8E15
		// (set) Token: 0x06002104 RID: 8452 RVA: 0x000AAC1D File Offset: 0x000A8E1D
		public Action<int> ButtonCallback
		{
			get
			{
				return this.m_buttonCallback;
			}
			set
			{
				this.m_buttonCallback = value;
			}
		}

		// Token: 0x170004A3 RID: 1187
		// (get) Token: 0x06002105 RID: 8453 RVA: 0x000AAC26 File Offset: 0x000A8E26
		// (set) Token: 0x06002106 RID: 8454 RVA: 0x000AAC2E File Offset: 0x000A8E2E
		public Texture2D[] Textures
		{
			get
			{
				return this.m_textures;
			}
			set
			{
				this.m_textures = value;
			}
		}

		// Token: 0x170004A4 RID: 1188
		// (get) Token: 0x06002107 RID: 8455 RVA: 0x000AAC38 File Offset: 0x000A8E38
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

		// Token: 0x170004A5 RID: 1189
		// (get) Token: 0x06002108 RID: 8456 RVA: 0x000AAC69 File Offset: 0x000A8E69
		public GameObject[] Instances
		{
			get
			{
				return this.m_instances;
			}
		}

		// Token: 0x06002109 RID: 8457 RVA: 0x000AAC74 File Offset: 0x000A8E74
		public void SetSelection(int p_selectionIndex)
		{
			if (!(this.m_selectionPrefab != null))
			{
				Debug.LogError("uMyGUI_TexturePicker: SetSelection: you have passed a non negative selection index '" + p_selectionIndex.ToString() + "', but the SelectionPrefab was not provided in the inspector or via script!");
				return;
			}
			if (p_selectionIndex < 0 || p_selectionIndex >= this.m_instances.Length)
			{
				global::UnityEngine.Object.Destroy(this.m_selectionInstance);
				this.m_selectionInstance = null;
				return;
			}
			if (this.m_selectionInstance == null)
			{
				this.m_selectionInstance = global::UnityEngine.Object.Instantiate<GameObject>(this.m_selectionPrefab);
			}
			else
			{
				RectTransform component = this.m_selectionInstance.GetComponent<RectTransform>();
				Vector2 anchoredPosition = component.anchoredPosition;
				anchoredPosition.x = this.m_selectionPrefab.GetComponent<RectTransform>().anchoredPosition.x;
				component.anchoredPosition = anchoredPosition;
			}
			this.SetRectTransformPosition(this.m_selectionInstance.GetComponent<RectTransform>(), p_selectionIndex, this.m_elementSize);
		}

		// Token: 0x0600210A RID: 8458 RVA: 0x000AAD40 File Offset: 0x000A8F40
		public void SetTextures(Texture2D[] p_textures, int p_selectedIndex)
		{
			if (this.m_texturePrefab != null)
			{
				this.m_textures = p_textures;
				global::UnityEngine.Object.Destroy(this.m_selectionInstance);
				for (int i = 0; i < this.m_instances.Length; i++)
				{
					global::UnityEngine.Object.Destroy(this.m_instances[i]);
				}
				this.m_instances = new GameObject[p_textures.Length];
				float num = 0f;
				for (int j = 0; j < p_textures.Length; j++)
				{
					this.m_instances[j] = global::UnityEngine.Object.Instantiate<GameObject>(this.m_texturePrefab);
					RectTransform component = this.m_instances[j].GetComponent<RectTransform>();
					this.m_elementSize = component.rect.width;
					this.SetRectTransformPosition(component, j, this.m_elementSize);
					RawImage rawImage = this.TryFindComponent<RawImage>(this.m_instances[j]);
					if (rawImage != null)
					{
						rawImage.texture = p_textures[j];
					}
					else
					{
						Debug.LogError("uMyGUI_TexturePicker: SetTextures: TexturePrefab must have a RawImage component attached (can be in children).");
					}
					if (this.m_buttonCallback != null)
					{
						Button button = this.TryFindComponent<Button>(this.m_instances[j]);
						if (button != null)
						{
							int indexCopy = j;
							button.onClick.AddListener(delegate
							{
								this.m_buttonCallback(indexCopy);
							});
						}
					}
					num = component.anchoredPosition.x + this.m_elementSize;
					if (j == p_selectedIndex)
					{
						if (this.m_selectionPrefab != null)
						{
							this.m_selectionInstance = global::UnityEngine.Object.Instantiate<GameObject>(this.m_selectionPrefab);
							this.SetRectTransformPosition(this.m_selectionInstance.GetComponent<RectTransform>(), j, this.m_elementSize);
						}
						else
						{
							Debug.LogError("uMyGUI_TexturePicker: SetTextures: you have passed a non negative selection index '" + p_selectedIndex.ToString() + "', but the SelectionPrefab was not provided in the inspector or via script!");
						}
					}
				}
				this.RTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num - this.RTransform.rect.xMin + this.m_offsetEnd);
				return;
			}
			Debug.LogError("uMyGUI_TexturePicker: SetTextures: you must provide the TexturePrefab in the inspector or via script!");
		}

		// Token: 0x0600210B RID: 8459 RVA: 0x000AAF1D File Offset: 0x000A911D
		private void OnDestroy()
		{
			this.m_buttonCallback = null;
		}

		// Token: 0x0600210C RID: 8460 RVA: 0x000AAF28 File Offset: 0x000A9128
		private void SetRectTransformPosition(RectTransform p_transform, int p_positionIndex, float p_size)
		{
			p_transform.SetParent(this.RTransform, false);
			Vector2 anchoredPosition = p_transform.anchoredPosition;
			anchoredPosition.x += this.m_offsetStart + (float)p_positionIndex * (p_size + this.m_padding);
			p_transform.anchoredPosition = anchoredPosition;
		}

		// Token: 0x0600210D RID: 8461 RVA: 0x000AAF70 File Offset: 0x000A9170
		private T TryFindComponent<T>(GameObject p_object) where T : Component
		{
			T t = p_object.GetComponent<T>();
			if (t == null)
			{
				T[] componentsInChildren = p_object.GetComponentsInChildren<T>(true);
				if (componentsInChildren.Length != 0)
				{
					t = componentsInChildren[0];
				}
			}
			return t;
		}

		// Token: 0x0400197D RID: 6525
		[SerializeField]
		private GameObject m_texturePrefab;

		// Token: 0x0400197E RID: 6526
		[SerializeField]
		private GameObject m_selectionPrefab;

		// Token: 0x0400197F RID: 6527
		[SerializeField]
		private float m_offsetStart;

		// Token: 0x04001980 RID: 6528
		[SerializeField]
		private float m_offsetEnd;

		// Token: 0x04001981 RID: 6529
		[SerializeField]
		private float m_padding = 4f;

		// Token: 0x04001982 RID: 6530
		[SerializeField]
		private Action<int> m_buttonCallback;

		// Token: 0x04001983 RID: 6531
		private Texture2D[] m_textures = new Texture2D[0];

		// Token: 0x04001984 RID: 6532
		private RectTransform m_rectTransform;

		// Token: 0x04001985 RID: 6533
		private float m_elementSize = 1f;

		// Token: 0x04001986 RID: 6534
		private GameObject m_selectionInstance;

		// Token: 0x04001987 RID: 6535
		private GameObject[] m_instances = new GameObject[0];
	}
}

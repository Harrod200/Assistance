using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008F0 RID: 2288
	public class TabbedPaneController : MonoBehaviour
	{
		// Token: 0x17000F21 RID: 3873
		// (get) Token: 0x060057D6 RID: 22486 RVA: 0x002851AC File Offset: 0x002833AC
		// (set) Token: 0x060057D7 RID: 22487 RVA: 0x002851B4 File Offset: 0x002833B4
		public TabbedPaneManager paneManager { get; private set; }

		// Token: 0x17000F22 RID: 3874
		// (get) Token: 0x060057D8 RID: 22488 RVA: 0x002851BD File Offset: 0x002833BD
		public bool IsSelected
		{
			get
			{
				return this.paneManager.activeTab == this;
			}
		}

		// Token: 0x060057D9 RID: 22489 RVA: 0x002851D0 File Offset: 0x002833D0
		public void Awake()
		{
			RectTransform component = base.GetComponent<RectTransform>();
			Vector2 vector = new Vector2(0f, 0f);
			component.offsetMin = vector;
			component.offsetMax = vector;
			this.paneCanvasGroup = base.GetComponent<CanvasGroup>();
			this.paneCanvas = base.GetComponent<Canvas>();
			this.paneRaycaster = base.GetComponent<GraphicRaycaster>();
			this.paneManager = base.GetComponentInParent<TabbedPaneManager>();
			if (this != this.paneManager.activeTab && this.paneRaycaster != null)
			{
				this.paneRaycaster.enabled = false;
			}
			if (this.tab == null)
			{
				int siblingIndex = base.transform.GetSiblingIndex();
				this.tab = base.transform.parent.parent.GetChild(0).GetChild(siblingIndex).gameObject;
				if (this.tab == null)
				{
					throw new TIException("Cannot find tab for " + base.name);
				}
			}
			this.tabButton = this.tab.GetComponent<Button>();
			this.tabButtonRT = this.tab.GetComponent<RectTransform>();
			this.tabImage = this.tab.GetComponent<Image>();
			this.originalSprite = this.tabImage.sprite;
			this.originalButtonHeight = this.tabButtonRT.sizeDelta.y;
		}

		// Token: 0x060057DA RID: 22490 RVA: 0x0028531D File Offset: 0x0028351D
		public void Start()
		{
			if (this.paneManager.activeTab == this)
			{
				this.Show(true);
			}
			else
			{
				this.Hide();
			}
			this.tabButton.onClick.AddListener(delegate
			{
				this.paneManager.Toggle(this);
			});
		}

		// Token: 0x060057DB RID: 22491 RVA: 0x00285360 File Offset: 0x00283560
		public void Show(bool update = true)
		{
			if (this.paneCanvas != null)
			{
				this.paneCanvas.enabled = true;
				if (this.paneRaycaster != null)
				{
					this.paneRaycaster.enabled = true;
				}
			}
			else
			{
				base.gameObject.SetActive(true);
				this.paneCanvasGroup.blocksRaycasts = (this.paneCanvasGroup.interactable = true);
			}
			this.tabImage.sprite = TIUtilities.assetLoader.LoadAsset<Sprite>(this.activeTabSpriteAssetPath);
			this.tabButtonRT.sizeDelta = new Vector2(this.tabButtonRT.sizeDelta.x, this.originalButtonHeight + this.activeTabHeightOffset);
			if (!this.skipResize)
			{
				this.UpdateSize();
			}
			if (this.updateWhenShowingPane != null && update)
			{
				this.updateWhenShowingPane.Invoke();
			}
		}

		// Token: 0x060057DC RID: 22492 RVA: 0x00285437 File Offset: 0x00283637
		public void SetSize(float menuDepth, float headerDepth, float itemDepth, int numItems)
		{
			this.menuDepth = menuDepth;
			this.headerDepth = headerDepth;
			this.itemDepth = itemDepth;
			this.numItems = numItems;
		}

		// Token: 0x060057DD RID: 22493 RVA: 0x00285456 File Offset: 0x00283656
		public void UpdateSize()
		{
			if (this.numItems > 0)
			{
				this.paneManager.Resize(this.menuDepth, this.headerDepth, this.itemDepth, this.numItems);
				return;
			}
			this.paneManager.Resize();
		}

		// Token: 0x060057DE RID: 22494 RVA: 0x00285490 File Offset: 0x00283690
		public void Hide()
		{
			if (this.paneCanvas != null)
			{
				this.paneCanvas.enabled = false;
				if (this.paneRaycaster != null)
				{
					this.paneRaycaster.enabled = false;
				}
			}
			else
			{
				base.gameObject.SetActive(false);
				this.paneCanvasGroup.blocksRaycasts = (this.paneCanvasGroup.interactable = false);
			}
			this.tabImage.sprite = this.originalSprite;
			this.tabButtonRT.sizeDelta = new Vector2(this.tabButtonRT.sizeDelta.x, this.originalButtonHeight);
			if (this.updateWhenHidingPane != null)
			{
				this.updateWhenHidingPane.Invoke();
			}
		}

		// Token: 0x17000F23 RID: 3875
		// (get) Token: 0x060057DF RID: 22495 RVA: 0x00285543 File Offset: 0x00283743
		public Button TabButton
		{
			get
			{
				return this.tabButton;
			}
		}

		// Token: 0x04003F5D RID: 16221
		[SerializeField]
		private GameObject tab;

		// Token: 0x04003F5E RID: 16222
		public UnityEvent updateWhenShowingPane;

		// Token: 0x04003F5F RID: 16223
		public UnityEvent updateWhenHidingPane;

		// Token: 0x04003F60 RID: 16224
		public bool skipResize;

		// Token: 0x04003F62 RID: 16226
		private CanvasGroup paneCanvasGroup;

		// Token: 0x04003F63 RID: 16227
		public Canvas paneCanvas;

		// Token: 0x04003F64 RID: 16228
		public GraphicRaycaster paneRaycaster;

		// Token: 0x04003F65 RID: 16229
		private Button tabButton;

		// Token: 0x04003F66 RID: 16230
		private RectTransform tabButtonRT;

		// Token: 0x04003F67 RID: 16231
		private Image tabImage;

		// Token: 0x04003F68 RID: 16232
		private Sprite originalSprite;

		// Token: 0x04003F69 RID: 16233
		public float activeTabHeightOffset;

		// Token: 0x04003F6A RID: 16234
		private float menuDepth;

		// Token: 0x04003F6B RID: 16235
		private float headerDepth;

		// Token: 0x04003F6C RID: 16236
		private float itemDepth;

		// Token: 0x04003F6D RID: 16237
		private float originalButtonHeight;

		// Token: 0x04003F6E RID: 16238
		private int numItems;

		// Token: 0x04003F6F RID: 16239
		public string activeTabSpriteAssetPath = "ui/UI_ActiveTab";
	}
}

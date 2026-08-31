using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta.Audio;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008F1 RID: 2289
	public class TabbedPaneManager : MonoBehaviour
	{
		// Token: 0x17000F24 RID: 3876
		// (get) Token: 0x060057E2 RID: 22498 RVA: 0x0028556C File Offset: 0x0028376C
		// (set) Token: 0x060057E3 RID: 22499 RVA: 0x00285574 File Offset: 0x00283774
		public TabbedPaneController activeTab { get; private set; }

		// Token: 0x17000F25 RID: 3877
		// (get) Token: 0x060057E4 RID: 22500 RVA: 0x0028557D File Offset: 0x0028377D
		public IEnumerable<TabbedPaneController> tabs
		{
			get
			{
				return base.GetComponentsInChildren<TabbedPaneController>(true);
			}
		}

		// Token: 0x060057E5 RID: 22501 RVA: 0x00285588 File Offset: 0x00283788
		private void Awake()
		{
			if (this.initialActiveTab != null)
			{
				this.activeTab = this.initialActiveTab.GetComponent<TabbedPaneController>();
			}
			this.rt = base.transform.GetComponent<RectTransform>();
			this.originalDepth = this.rt.sizeDelta.y;
			foreach (TabbedPaneController tabbedPaneController in this.tabs)
			{
				tabbedPaneController.activeTabHeightOffset = this.tabVerticalSpacing;
			}
		}

		// Token: 0x060057E6 RID: 22502 RVA: 0x00285620 File Offset: 0x00283820
		private void Start()
		{
			if (this.activeTab != null)
			{
				this.activeTab.Show(true);
			}
		}

		// Token: 0x060057E7 RID: 22503 RVA: 0x0028563C File Offset: 0x0028383C
		public void Toggle(TabbedPaneController tabbedPane)
		{
			if (tabbedPane == this.activeTab)
			{
				if (this.reclickToHide)
				{
					this.activeTab.Hide();
					this.activeTab = null;
					if (GameControl.loadcycle100)
					{
						AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_SwitchTabInTabbedPane", false, true);
						return;
					}
				}
			}
			else
			{
				if (this.activeTab != null)
				{
					this.activeTab.Hide();
				}
				if (GameControl.loadcycle100)
				{
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_SwitchTabInTabbedPane", false, true);
				}
				tabbedPane.Show(true);
				this.activeTab = tabbedPane;
			}
		}

		// Token: 0x060057E8 RID: 22504 RVA: 0x002856BF File Offset: 0x002838BF
		public void ClearActiveTab()
		{
			this.activeTab = null;
		}

		// Token: 0x060057E9 RID: 22505 RVA: 0x002856C8 File Offset: 0x002838C8
		public void Resize(float menuDepth, float headerDepth, float itemDepth, int items)
		{
			float num = Mathf.Clamp(menuDepth + headerDepth + itemDepth * (float)items, 0f, this.originalDepth);
			this.rt.sizeDelta = new Vector2(this.rt.sizeDelta.x, num);
		}

		// Token: 0x060057EA RID: 22506 RVA: 0x00285710 File Offset: 0x00283910
		public void Resize()
		{
			this.rt.sizeDelta = new Vector2(this.rt.sizeDelta.x, this.originalDepth);
		}

		// Token: 0x04003F70 RID: 16240
		public GameObject initialActiveTab;

		// Token: 0x04003F71 RID: 16241
		[Tooltip("Vertical offset from tab to data window, active tabs increase in size by this value to connect to the window")]
		public float tabVerticalSpacing = 5f;

		// Token: 0x04003F73 RID: 16243
		public bool reclickToHide = true;

		// Token: 0x04003F74 RID: 16244
		private float originalDepth;

		// Token: 0x04003F75 RID: 16245
		private RectTransform rt;
	}
}

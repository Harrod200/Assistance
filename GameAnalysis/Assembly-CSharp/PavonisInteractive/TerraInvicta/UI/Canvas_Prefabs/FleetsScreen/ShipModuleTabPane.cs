using System;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.UI.Canvas_Prefabs.FleetsScreen
{
	// Token: 0x0200092A RID: 2346
	[RequireComponent(typeof(TabbedPaneController))]
	public class ShipModuleTabPane : MonoBehaviour
	{
		// Token: 0x17000F3F RID: 3903
		// (get) Token: 0x0600599E RID: 22942 RVA: 0x002921C8 File Offset: 0x002903C8
		public TabbedPaneController tabPane
		{
			get
			{
				return base.GetComponent<TabbedPaneController>();
			}
		}

		// Token: 0x17000F40 RID: 3904
		// (get) Token: 0x0600599F RID: 22943 RVA: 0x002921D0 File Offset: 0x002903D0
		public FleetsScreenController fleetsScreenController
		{
			get
			{
				return base.GetComponentInParent<FleetsScreenController>();
			}
		}

		// Token: 0x17000F41 RID: 3905
		// (get) Token: 0x060059A0 RID: 22944 RVA: 0x002921D8 File Offset: 0x002903D8
		public bool tabActive
		{
			get
			{
				return this.tabObject.activeSelf;
			}
		}

		// Token: 0x060059A1 RID: 22945 RVA: 0x002921E5 File Offset: 0x002903E5
		public void ShowIcons()
		{
			this.icons.canvas.enabled = true;
			this.table.canvas.enabled = false;
		}

		// Token: 0x060059A2 RID: 22946 RVA: 0x00292209 File Offset: 0x00290409
		public void ShowTable()
		{
			this.icons.canvas.enabled = false;
			this.table.canvas.enabled = true;
			this.ForceUpdateColumnWidths();
		}

		// Token: 0x060059A3 RID: 22947 RVA: 0x00292233 File Offset: 0x00290433
		public void OnTabLeftClick()
		{
			this.tabPane.paneManager.Toggle(this.tabPane);
			if (this.fleetsScreenController.showShipPartsAsIcons)
			{
				this.ShowIcons();
			}
			else
			{
				this.ShowTable();
			}
			this.fleetsScreenController.UpdateWeaponTabAllSubtabInteractive();
		}

		// Token: 0x060059A4 RID: 22948 RVA: 0x00292271 File Offset: 0x00290471
		public void OnTabRightClick()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
			this.fleetsScreenController.showShipPartsAsIcons = !this.fleetsScreenController.showShipPartsAsIcons;
			this.OnTabLeftClick();
		}

		// Token: 0x060059A5 RID: 22949 RVA: 0x0029229E File Offset: 0x0029049E
		public void ForceUpdateColumnWidths()
		{
			this.table.ForceUpdateRefreshColumnWidths();
		}

		// Token: 0x040040BE RID: 16574
		public ShipModuleIcons icons;

		// Token: 0x040040BF RID: 16575
		public ShipModuleTable table;

		// Token: 0x040040C0 RID: 16576
		public TMP_Text tabText;

		// Token: 0x040040C1 RID: 16577
		public GameObject tabObject;
	}
}

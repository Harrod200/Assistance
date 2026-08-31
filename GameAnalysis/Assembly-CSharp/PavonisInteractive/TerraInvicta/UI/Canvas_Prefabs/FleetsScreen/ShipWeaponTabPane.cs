using System;
using TMPro;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.UI.Canvas_Prefabs.FleetsScreen
{
	// Token: 0x0200092C RID: 2348
	[RequireComponent(typeof(TabbedPaneController))]
	public class ShipWeaponTabPane : MonoBehaviour
	{
		// Token: 0x17000F44 RID: 3908
		// (get) Token: 0x060059AD RID: 22957 RVA: 0x0029245B File Offset: 0x0029065B
		public TabbedPaneController tabPane
		{
			get
			{
				return base.GetComponent<TabbedPaneController>();
			}
		}

		// Token: 0x17000F45 RID: 3909
		// (get) Token: 0x060059AE RID: 22958 RVA: 0x00292463 File Offset: 0x00290663
		public FleetsScreenController fleetsScreenController
		{
			get
			{
				return base.GetComponentInParent<FleetsScreenController>();
			}
		}

		// Token: 0x17000F46 RID: 3910
		// (get) Token: 0x060059AF RID: 22959 RVA: 0x0029246B File Offset: 0x0029066B
		public bool tabActive
		{
			get
			{
				return this.tabObject.activeSelf;
			}
		}

		// Token: 0x060059B0 RID: 22960 RVA: 0x00292478 File Offset: 0x00290678
		public void ShowIcons()
		{
			if (this.fleetsScreenController.noseModulesTabPane.IsSelected)
			{
				this.noseIcons.canvas.enabled = true;
				this.hullIcons.canvas.enabled = false;
			}
			else
			{
				this.hullIcons.canvas.enabled = true;
				this.noseIcons.canvas.enabled = false;
			}
			this.noseTable.canvas.enabled = false;
			this.hullTable.canvas.enabled = false;
		}

		// Token: 0x060059B1 RID: 22961 RVA: 0x00292500 File Offset: 0x00290700
		public void ShowTable()
		{
			if (this.fleetsScreenController.noseModulesTabPane.IsSelected)
			{
				this.noseTable.canvas.enabled = true;
				this.hullTable.canvas.enabled = false;
			}
			else
			{
				this.hullTable.canvas.enabled = true;
				this.noseTable.canvas.enabled = false;
			}
			this.noseIcons.canvas.enabled = false;
			this.hullIcons.canvas.enabled = false;
			this.ForceUpdateColumnWidths();
		}

		// Token: 0x060059B2 RID: 22962 RVA: 0x0029258D File Offset: 0x0029078D
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

		// Token: 0x060059B3 RID: 22963 RVA: 0x002925CB File Offset: 0x002907CB
		public void OnTabRightClick()
		{
			this.fleetsScreenController.showShipPartsAsIcons = !this.fleetsScreenController.showShipPartsAsIcons;
			this.OnTabLeftClick();
		}

		// Token: 0x060059B4 RID: 22964 RVA: 0x002925EC File Offset: 0x002907EC
		public void ForceUpdateColumnWidths()
		{
			this.noseTable.ForceUpdateRefreshColumnWidths();
			this.hullTable.ForceUpdateRefreshColumnWidths();
		}

		// Token: 0x040040C6 RID: 16582
		public ShipModuleIcons noseIcons;

		// Token: 0x040040C7 RID: 16583
		public ShipModuleIcons hullIcons;

		// Token: 0x040040C8 RID: 16584
		public ShipModuleTable noseTable;

		// Token: 0x040040C9 RID: 16585
		public ShipModuleTable hullTable;

		// Token: 0x040040CA RID: 16586
		public TMP_Text tabText;

		// Token: 0x040040CB RID: 16587
		public GameObject tabObject;
	}
}

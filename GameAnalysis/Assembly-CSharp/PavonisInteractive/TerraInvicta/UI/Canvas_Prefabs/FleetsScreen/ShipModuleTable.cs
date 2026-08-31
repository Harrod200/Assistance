using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta.UI.Canvas_Prefabs.FleetsScreen
{
	// Token: 0x0200092B RID: 2347
	public class ShipModuleTable : MonoBehaviour
	{
		// Token: 0x17000F42 RID: 3906
		// (get) Token: 0x060059A7 RID: 22951 RVA: 0x002922B3 File Offset: 0x002904B3
		public IEnumerable<ShipModuleListItem> rows
		{
			get
			{
				return this.rowsContainer.GetComponentsInChildren<ShipModuleListItem>();
			}
		}

		// Token: 0x17000F43 RID: 3907
		// (get) Token: 0x060059A8 RID: 22952 RVA: 0x002922C0 File Offset: 0x002904C0
		public IEnumerable<ShipModuleListItem> rowsAndLabels
		{
			get
			{
				return new ShipModuleListItem[] { this.labels }.Union<ShipModuleListItem>(this.rows);
			}
		}

		// Token: 0x060059A9 RID: 22953 RVA: 0x002922DC File Offset: 0x002904DC
		private void Update()
		{
			if (!this.canvas.enabled)
			{
				return;
			}
			float width = (this.rowsContainer as RectTransform).rect.width;
			if (this.cachedContainerWidth != width)
			{
				this.ResizeColumns();
				this.cachedContainerWidth = width;
			}
		}

		// Token: 0x060059AA RID: 22954 RVA: 0x00292328 File Offset: 0x00290528
		public void ForceUpdateRefreshColumnWidths()
		{
			this.ResizeColumns();
			this.cachedContainerWidth = (this.rowsContainer as RectTransform).rect.width;
		}

		// Token: 0x060059AB RID: 22955 RVA: 0x0029235C File Offset: 0x0029055C
		public void ResizeColumns()
		{
			if (this.rows.Count<ShipModuleListItem>() == 0)
			{
				return;
			}
			int i;
			int j;
			for (i = 0; i < this.rows.First<ShipModuleListItem>().entries.Count<ShipModuleListItemEntry>(); i = j + 1)
			{
				float num = this.rowsAndLabels.Select<ShipModuleListItem, float>((ShipModuleListItem row) => row.entries.ToList<ShipModuleListItemEntry>()[i].preferredWidth).Max();
				bool flag = true;
				foreach (ShipModuleListItem shipModuleListItem in this.rowsAndLabels)
				{
					ShipModuleListItemEntry shipModuleListItemEntry = shipModuleListItem.entries.ToList<ShipModuleListItemEntry>()[i];
					shipModuleListItemEntry.SetWidth(num);
					shipModuleListItemEntry.SetIsLabel(flag);
					flag = false;
				}
				j = i;
			}
			LayoutRebuilder.MarkLayoutForRebuild(this.labels.transform as RectTransform);
		}

		// Token: 0x040040C2 RID: 16578
		public ShipModuleListItem labels;

		// Token: 0x040040C3 RID: 16579
		public Transform rowsContainer;

		// Token: 0x040040C4 RID: 16580
		public Canvas canvas;

		// Token: 0x040040C5 RID: 16581
		private float cachedContainerWidth = -1f;
	}
}

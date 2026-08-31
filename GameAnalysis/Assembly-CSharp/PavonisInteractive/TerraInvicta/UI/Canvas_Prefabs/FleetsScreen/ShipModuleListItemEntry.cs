using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta.UI.Canvas_Prefabs.FleetsScreen
{
	// Token: 0x02000929 RID: 2345
	public class ShipModuleListItemEntry : MonoBehaviour
	{
		// Token: 0x17000F3D RID: 3901
		// (get) Token: 0x06005998 RID: 22936 RVA: 0x002920A3 File Offset: 0x002902A3
		public float preferredWidth
		{
			get
			{
				return this.textElement.preferredWidth;
			}
		}

		// Token: 0x17000F3E RID: 3902
		// (get) Token: 0x06005999 RID: 22937 RVA: 0x002920B0 File Offset: 0x002902B0
		public ShipModuleListItem row
		{
			get
			{
				return base.GetComponentInParent<ShipModuleListItem>();
			}
		}

		// Token: 0x0600599A RID: 22938 RVA: 0x002920B8 File Offset: 0x002902B8
		public void OnClick()
		{
			if (this.isLabel)
			{
				int index = this.row.entries.IndexOf(this);
				Func<ShipModuleListItem, IComparable> func = (ShipModuleListItem row) => row.entries.ElementAt<ShipModuleListItemEntry>(index).value;
				bool flag = this.row.table.rows.MinBy_IComparable<ShipModuleListItem, IComparable>((ShipModuleListItem row) => row.entries.ElementAt<ShipModuleListItemEntry>(index).value).transform.GetSiblingIndex() > 0;
				this.row.table.rowsContainer.SortChildren<ShipModuleListItem>(func, flag);
				return;
			}
			base.GetComponentInParent<ShipModuleListItem>().OnClickItem();
		}

		// Token: 0x0600599B RID: 22939 RVA: 0x0029214C File Offset: 0x0029034C
		public void SetWidth(float newWidth)
		{
			RectTransform rectTransform = base.transform as RectTransform;
			rectTransform.sizeDelta = new Vector2(newWidth, rectTransform.sizeDelta.y);
			this.layout.preferredWidth = newWidth;
		}

		// Token: 0x0600599C RID: 22940 RVA: 0x00292188 File Offset: 0x00290388
		public void SetIsLabel(bool isLabel)
		{
			if (this.isLabel != isLabel)
			{
				this.isLabel = isLabel;
				if (isLabel)
				{
					this.backgroundImage.sprite = this.tableHeader;
					return;
				}
				this.backgroundImage.sprite = this.tableItem;
			}
		}

		// Token: 0x040040B7 RID: 16567
		public TextMeshProUGUI textElement;

		// Token: 0x040040B8 RID: 16568
		public Image backgroundImage;

		// Token: 0x040040B9 RID: 16569
		public Sprite tableHeader;

		// Token: 0x040040BA RID: 16570
		public Sprite tableItem;

		// Token: 0x040040BB RID: 16571
		public LayoutElement layout;

		// Token: 0x040040BC RID: 16572
		public IComparable value;

		// Token: 0x040040BD RID: 16573
		private bool isLabel;
	}
}

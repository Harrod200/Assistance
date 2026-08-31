using System;
using System.Collections.Generic;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using Com.ForbiddenByte.OSA.DataHelpers;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008E1 RID: 2273
	public class LedgerListAdapter : OSA<BaseParamsWithPrefab, LedgerListItemViewsHolder>
	{
		// Token: 0x17000F1C RID: 3868
		// (get) Token: 0x06005794 RID: 22420 RVA: 0x002844F1 File Offset: 0x002826F1
		// (set) Token: 0x06005795 RID: 22421 RVA: 0x002844F9 File Offset: 0x002826F9
		public SimpleDataHelper<LedgerListItemModel> Data { get; private set; }

		// Token: 0x06005796 RID: 22422 RVA: 0x00284502 File Offset: 0x00282702
		protected override void Start()
		{
			this.Data = new SimpleDataHelper<LedgerListItemModel>(this, true);
			base.Start();
		}

		// Token: 0x06005797 RID: 22423 RVA: 0x00284517 File Offset: 0x00282717
		protected override LedgerListItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			LedgerListItemViewsHolder ledgerListItemViewsHolder = new LedgerListItemViewsHolder();
			ledgerListItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return ledgerListItemViewsHolder;
		}

		// Token: 0x06005798 RID: 22424 RVA: 0x00284540 File Offset: 0x00282740
		protected override void UpdateViewsHolder(LedgerListItemViewsHolder newOrRecycled)
		{
			LedgerListItemModel ledgerListItemModel = this.Data[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateFromModel(ledgerListItemModel, this._Params);
		}

		// Token: 0x06005799 RID: 22425 RVA: 0x0028456C File Offset: 0x0028276C
		public void SetItems(IList<LedgerListItemModel> items)
		{
			IList<LedgerListItemModel> list = new List<LedgerListItemModel>();
			foreach (LedgerListItemModel ledgerListItemModel in items)
			{
				if (!ledgerListItemModel.ledgerListItemData.collapsible || !ledgerListItemModel.ledgerListItemData.collapsed)
				{
					list.Add(ledgerListItemModel);
				}
			}
			this.Data.ResetItems(list, false);
		}
	}
}

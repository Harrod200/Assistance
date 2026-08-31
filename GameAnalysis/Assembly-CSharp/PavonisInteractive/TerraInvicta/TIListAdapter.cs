using System;
using System.Collections.Generic;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using Com.ForbiddenByte.OSA.DataHelpers;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008ED RID: 2285
	public class TIListAdapter : OSA<BaseParamsWithPrefab, TIListItemViewsHolder>
	{
		// Token: 0x17000F20 RID: 3872
		// (get) Token: 0x060057C8 RID: 22472 RVA: 0x002850D3 File Offset: 0x002832D3
		// (set) Token: 0x060057C9 RID: 22473 RVA: 0x002850DB File Offset: 0x002832DB
		public SimpleDataHelper<TIListItemModel> Data { get; private set; }

		// Token: 0x060057CA RID: 22474 RVA: 0x002850E4 File Offset: 0x002832E4
		protected override void Start()
		{
			this.Data = new SimpleDataHelper<TIListItemModel>(this, true);
			base.Start();
		}

		// Token: 0x060057CB RID: 22475 RVA: 0x002850F9 File Offset: 0x002832F9
		protected override TIListItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			TIListItemViewsHolder tilistItemViewsHolder = new TIListItemViewsHolder();
			tilistItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return tilistItemViewsHolder;
		}

		// Token: 0x060057CC RID: 22476 RVA: 0x00285120 File Offset: 0x00283320
		protected override void UpdateViewsHolder(TIListItemViewsHolder newOrRecycled)
		{
			TIListItemModel tilistItemModel = this.Data[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateFromModel(tilistItemModel, this._Params);
		}

		// Token: 0x060057CD RID: 22477 RVA: 0x0028514C File Offset: 0x0028334C
		public void AddItemsAt(int index, IList<TIListItemModel> items)
		{
			this.Data.InsertItems(index, items, false);
		}

		// Token: 0x060057CE RID: 22478 RVA: 0x0028515C File Offset: 0x0028335C
		public void RemoveItemsFrom(int index, int count)
		{
			this.Data.RemoveItems(index, count, false);
		}

		// Token: 0x060057CF RID: 22479 RVA: 0x0028516C File Offset: 0x0028336C
		public void SetItems(IList<TIListItemModel> items)
		{
			this.Data.ResetItems(items, false);
		}

		// Token: 0x060057D0 RID: 22480 RVA: 0x0028517B File Offset: 0x0028337B
		public override void ChangeItemsCount(ItemCountChangeMode changeMode, int itemsCount, int indexIfInsertingOrRemoving = -1, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
		{
			base.ChangeItemsCount(changeMode, itemsCount, indexIfInsertingOrRemoving, contentPanelEndEdgeStationary, keepVelocity);
		}
	}
}

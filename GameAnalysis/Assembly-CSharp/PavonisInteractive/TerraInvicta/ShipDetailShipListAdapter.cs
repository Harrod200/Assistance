using System;
using System.Collections.Generic;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using Com.ForbiddenByte.OSA.DataHelpers;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200082A RID: 2090
	public class ShipDetailShipListAdapter : OSA<BaseParamsWithPrefab, ShipDetailShipListItemViewsHolder>
	{
		// Token: 0x17000EA0 RID: 3744
		// (get) Token: 0x06004AFF RID: 19199 RVA: 0x001F50C4 File Offset: 0x001F32C4
		// (set) Token: 0x06004B00 RID: 19200 RVA: 0x001F50CC File Offset: 0x001F32CC
		public SimpleDataHelper<ShipDetailShipListItemModel> Data { get; private set; }

		// Token: 0x06004B01 RID: 19201 RVA: 0x001F50D5 File Offset: 0x001F32D5
		protected override void Start()
		{
			this.Data = new SimpleDataHelper<ShipDetailShipListItemModel>(this, true);
			base.Start();
		}

		// Token: 0x06004B02 RID: 19202 RVA: 0x001F50EA File Offset: 0x001F32EA
		protected override ShipDetailShipListItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			ShipDetailShipListItemViewsHolder shipDetailShipListItemViewsHolder = new ShipDetailShipListItemViewsHolder();
			shipDetailShipListItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return shipDetailShipListItemViewsHolder;
		}

		// Token: 0x06004B03 RID: 19203 RVA: 0x001F5110 File Offset: 0x001F3310
		protected override void UpdateViewsHolder(ShipDetailShipListItemViewsHolder newOrRecycled)
		{
			ShipDetailShipListItemModel shipDetailShipListItemModel = this.Data[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateFromModel(shipDetailShipListItemModel, this._Params);
		}

		// Token: 0x06004B04 RID: 19204 RVA: 0x001F513C File Offset: 0x001F333C
		public void SetItems(IList<ShipDetailShipListItemModel> items)
		{
			this.Data.ResetItems(items, false);
		}
	}
}

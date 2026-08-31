using System;
using System.Collections.Generic;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using Com.ForbiddenByte.OSA.DataHelpers;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200080F RID: 2063
	public class BattleLogListAdapter : OSA<BaseParamsWithPrefab, BattleLogListItemViewsHolder>
	{
		// Token: 0x17000E96 RID: 3734
		// (get) Token: 0x06004A93 RID: 19091 RVA: 0x001F4496 File Offset: 0x001F2696
		// (set) Token: 0x06004A94 RID: 19092 RVA: 0x001F449E File Offset: 0x001F269E
		public SimpleDataHelper<BattleLogListItemModel> Data { get; private set; }

		// Token: 0x06004A95 RID: 19093 RVA: 0x001F44A7 File Offset: 0x001F26A7
		protected override void Start()
		{
			this.Data = new SimpleDataHelper<BattleLogListItemModel>(this, true);
			base.Start();
		}

		// Token: 0x06004A96 RID: 19094 RVA: 0x001F44BC File Offset: 0x001F26BC
		protected override BattleLogListItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			BattleLogListItemViewsHolder battleLogListItemViewsHolder = new BattleLogListItemViewsHolder();
			battleLogListItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return battleLogListItemViewsHolder;
		}

		// Token: 0x06004A97 RID: 19095 RVA: 0x001F44E4 File Offset: 0x001F26E4
		protected override void UpdateViewsHolder(BattleLogListItemViewsHolder newOrRecycled)
		{
			BattleLogListItemModel battleLogListItemModel = this.Data[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateFromModel(battleLogListItemModel, this._Params);
		}

		// Token: 0x06004A98 RID: 19096 RVA: 0x001F4510 File Offset: 0x001F2710
		public void SetItems(IList<BattleLogListItemModel> items)
		{
			IList<BattleLogListItemModel> list = new List<BattleLogListItemModel>();
			foreach (BattleLogListItemModel battleLogListItemModel in items)
			{
				if (battleLogListItemModel.battleLogEntryData.showInList)
				{
					list.Add(battleLogListItemModel);
				}
			}
			this.Data.ResetItems(list, false);
		}
	}
}

using System;
using System.Collections.Generic;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using Com.ForbiddenByte.OSA.DataHelpers;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200081B RID: 2075
	public class HabScreenHabListAdapter : OSA<BaseParamsWithPrefab, HabScreenHabListItemViewsHolder>
	{
		// Token: 0x17000E9B RID: 3739
		// (get) Token: 0x06004AC8 RID: 19144 RVA: 0x001F4ACB File Offset: 0x001F2CCB
		// (set) Token: 0x06004AC9 RID: 19145 RVA: 0x001F4AD3 File Offset: 0x001F2CD3
		public SimpleDataHelper<HabScreenHabListItemModel> Data { get; private set; }

		// Token: 0x06004ACA RID: 19146 RVA: 0x001F4ADC File Offset: 0x001F2CDC
		protected override void Start()
		{
			this.Data = new SimpleDataHelper<HabScreenHabListItemModel>(this, true);
			base.Start();
		}

		// Token: 0x06004ACB RID: 19147 RVA: 0x001F4AF1 File Offset: 0x001F2CF1
		protected override HabScreenHabListItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			HabScreenHabListItemViewsHolder habScreenHabListItemViewsHolder = new HabScreenHabListItemViewsHolder();
			habScreenHabListItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return habScreenHabListItemViewsHolder;
		}

		// Token: 0x06004ACC RID: 19148 RVA: 0x001F4B18 File Offset: 0x001F2D18
		protected override void UpdateViewsHolder(HabScreenHabListItemViewsHolder newOrRecycled)
		{
			HabScreenHabListItemModel habScreenHabListItemModel = this.Data[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateFromModel(habScreenHabListItemModel, this._Params);
		}

		// Token: 0x06004ACD RID: 19149 RVA: 0x001F4B44 File Offset: 0x001F2D44
		public void SetItems(IList<HabScreenHabListItemModel> items)
		{
			IList<HabScreenHabListItemModel> list = new List<HabScreenHabListItemModel>();
			foreach (HabScreenHabListItemModel habScreenHabListItemModel in items)
			{
				if (habScreenHabListItemModel.HabScreenHabListItemData.showInList)
				{
					list.Add(habScreenHabListItemModel);
				}
			}
			this.Data.ResetItems(list, false);
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using Com.ForbiddenByte.OSA.DataHelpers;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008DC RID: 2268
	public class BasicListAdapter : OSA<BaseParamsWithPrefab, MyListItemViewsHolder>
	{
		// Token: 0x17000F1B RID: 3867
		// (get) Token: 0x0600577F RID: 22399 RVA: 0x00284359 File Offset: 0x00282559
		// (set) Token: 0x06005780 RID: 22400 RVA: 0x00284361 File Offset: 0x00282561
		public SimpleDataHelper<MyListItemModel> Data { get; private set; }

		// Token: 0x06005781 RID: 22401 RVA: 0x0028436A File Offset: 0x0028256A
		protected override void Start()
		{
			this.Data = new SimpleDataHelper<MyListItemModel>(this, true);
			base.Start();
		}

		// Token: 0x06005782 RID: 22402 RVA: 0x0028437F File Offset: 0x0028257F
		protected override MyListItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			MyListItemViewsHolder myListItemViewsHolder = new MyListItemViewsHolder();
			myListItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return myListItemViewsHolder;
		}

		// Token: 0x06005783 RID: 22403 RVA: 0x002843A8 File Offset: 0x002825A8
		protected override void UpdateViewsHolder(MyListItemViewsHolder newOrRecycled)
		{
			MyListItemModel myListItemModel = this.Data[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateFromModel(myListItemModel, this._Params);
		}

		// Token: 0x06005784 RID: 22404 RVA: 0x002843D4 File Offset: 0x002825D4
		public void AddItemsAt(int index, IList<MyListItemModel> items)
		{
			this.Data.InsertItems(index, items, false);
		}

		// Token: 0x06005785 RID: 22405 RVA: 0x002843E4 File Offset: 0x002825E4
		public void RemoveItemsFrom(int index, int count)
		{
			this.Data.RemoveItems(index, count, false);
		}

		// Token: 0x06005786 RID: 22406 RVA: 0x002843F4 File Offset: 0x002825F4
		public void SetItems(IList<MyListItemModel> items)
		{
			this.Data.ResetItems(items, false);
		}

		// Token: 0x06005787 RID: 22407 RVA: 0x00284403 File Offset: 0x00282603
		private void RetrieveDataAndUpdate(int count)
		{
			this.FetchMoreItemsFromDataSourceAndUpdate2(count);
		}

		// Token: 0x06005788 RID: 22408 RVA: 0x0028440C File Offset: 0x0028260C
		private IEnumerator FetchMoreItemsFromDataSourceAndUpdate(int count)
		{
			yield return new WaitForSeconds(0.01f);
			MyListItemModel[] array = new MyListItemModel[count];
			this.OnDataRetrieved(array);
			yield break;
		}

		// Token: 0x06005789 RID: 22409 RVA: 0x00284424 File Offset: 0x00282624
		private void FetchMoreItemsFromDataSourceAndUpdate2(int count)
		{
			MyListItemModel[] array = new MyListItemModel[count];
			this.OnDataRetrieved(array);
		}

		// Token: 0x0600578A RID: 22410 RVA: 0x0028443F File Offset: 0x0028263F
		private void OnDataRetrieved(MyListItemModel[] newItems)
		{
			this.Data.InsertItemsAtEnd(newItems, false);
		}

		// Token: 0x0600578B RID: 22411 RVA: 0x0028444E File Offset: 0x0028264E
		public override void ChangeItemsCount(ItemCountChangeMode changeMode, int itemsCount, int indexIfInsertingOrRemoving = -1, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
		{
			base.ChangeItemsCount(changeMode, itemsCount, indexIfInsertingOrRemoving, contentPanelEndEdgeStationary, keepVelocity);
		}
	}
}

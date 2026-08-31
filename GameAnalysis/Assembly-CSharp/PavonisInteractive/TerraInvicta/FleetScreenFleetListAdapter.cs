using System;
using System.Collections.Generic;
using System.Linq;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using Com.ForbiddenByte.OSA.DataHelpers;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000815 RID: 2069
	public class FleetScreenFleetListAdapter : OSA<BaseParamsWithPrefab, FleetScreenFleetListItemViewsHolder>
	{
		// Token: 0x17000E98 RID: 3736
		// (get) Token: 0x06004AA9 RID: 19113 RVA: 0x001F4711 File Offset: 0x001F2911
		// (set) Token: 0x06004AAA RID: 19114 RVA: 0x001F4719 File Offset: 0x001F2919
		public SimpleDataHelper<FleetScreenFleetListItemModel> Data { get; private set; }

		// Token: 0x06004AAB RID: 19115 RVA: 0x001F4722 File Offset: 0x001F2922
		protected override void Start()
		{
			this.Data = new SimpleDataHelper<FleetScreenFleetListItemModel>(this, true);
			base.Start();
		}

		// Token: 0x06004AAC RID: 19116 RVA: 0x001F4737 File Offset: 0x001F2937
		protected override FleetScreenFleetListItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			FleetScreenFleetListItemViewsHolder fleetScreenFleetListItemViewsHolder = new FleetScreenFleetListItemViewsHolder();
			fleetScreenFleetListItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return fleetScreenFleetListItemViewsHolder;
		}

		// Token: 0x06004AAD RID: 19117 RVA: 0x001F4760 File Offset: 0x001F2960
		protected override void UpdateViewsHolder(FleetScreenFleetListItemViewsHolder newOrRecycled)
		{
			FleetScreenFleetListItemModel fleetScreenFleetListItemModel = this.Data[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateFromModel(fleetScreenFleetListItemModel, this._Params);
			if (fleetScreenFleetListItemModel.HasPendingSizeChange)
			{
				base.ScheduleComputeVisibilityTwinPass(false);
			}
		}

		// Token: 0x06004AAE RID: 19118 RVA: 0x001F479B File Offset: 0x001F299B
		protected override void OnItemHeightChangedPreTwinPass(FleetScreenFleetListItemViewsHolder vh)
		{
			base.OnItemHeightChangedPreTwinPass(vh);
			this.Data[vh.ItemIndex].HasPendingSizeChange = false;
		}

		// Token: 0x06004AAF RID: 19119 RVA: 0x001F47BB File Offset: 0x001F29BB
		protected override void RebuildLayoutDueToScrollViewSizeChange()
		{
			this.SetAllModelsHavePendingSizeChange();
			base.RebuildLayoutDueToScrollViewSizeChange();
		}

		// Token: 0x06004AB0 RID: 19120 RVA: 0x001F47C9 File Offset: 0x001F29C9
		public override void ChangeItemsCount(ItemCountChangeMode changeMode, int itemsCount, int indexIfInsertingOrRemoving = -1, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
		{
			if (changeMode == ItemCountChangeMode.RESET)
			{
				this.SetAllModelsHavePendingSizeChange();
			}
			base.ChangeItemsCount(changeMode, itemsCount, indexIfInsertingOrRemoving, contentPanelEndEdgeStationary, keepVelocity);
		}

		// Token: 0x06004AB1 RID: 19121 RVA: 0x001F47E4 File Offset: 0x001F29E4
		private void SetAllModelsHavePendingSizeChange()
		{
			for (int i = 0; i < this.Data.Count; i++)
			{
				this.Data[i].HasPendingSizeChange = false;
			}
		}

		// Token: 0x06004AB2 RID: 19122 RVA: 0x001F481C File Offset: 0x001F2A1C
		public void SetItems(IList<FleetScreenFleetListItemModel> items)
		{
			if (this.thisRT == null)
			{
				this.thisRT = base.GetComponent<RectTransform>();
			}
			this.Data.ResetItems(items, false);
			FleetScreenFleetListItemModel fleetScreenFleetListItemModel = this.Data.List.FirstOrDefault<FleetScreenFleetListItemModel>((FleetScreenFleetListItemModel x) => x.FleetScreenFleetListItemData.gameStateFleetOrShip.ID == this.idToBringToView && x.FleetScreenFleetListItemData.isGroupItem == this.idToBringIsGroup);
			if (this.idToBringToView != -1 && fleetScreenFleetListItemModel != null)
			{
				this.ScrollTo(this.Data.List.IndexOf(fleetScreenFleetListItemModel), 1f - TIUtilities.GetMouseHeightRelativeToRectTransformBounds(base.GetComponent<RectTransform>()), 0.5f);
			}
			this.idToBringToView = -1;
		}

		// Token: 0x04002B93 RID: 11155
		public GameStateID idToBringToView;

		// Token: 0x04002B94 RID: 11156
		public bool idToBringIsGroup;

		// Token: 0x04002B95 RID: 11157
		private RectTransform thisRT;
	}
}

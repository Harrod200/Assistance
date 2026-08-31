using System;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000826 RID: 2086
	public class NationInfoRegionListItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x06004AF1 RID: 19185 RVA: 0x001F4F6C File Offset: 0x001F316C
		public override void CollectViews()
		{
			base.CollectViews();
			this.regionListItem = this.root.GetComponent<RegionListItemController>();
		}

		// Token: 0x06004AF2 RID: 19186 RVA: 0x001F4F85 File Offset: 0x001F3185
		public void UpdateFromModel(NationInfoRegionListItemModel model, BaseParamsWithPrefab parameters)
		{
			this.regionListItem.UpdateListItem(model.regionListItemData);
		}

		// Token: 0x04002BA8 RID: 11176
		public RegionListItemController regionListItem;
	}
}

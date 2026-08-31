using System;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200081D RID: 2077
	public class HabScreenHabListItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x06004AD0 RID: 19152 RVA: 0x001F4BBC File Offset: 0x001F2DBC
		public override void CollectViews()
		{
			base.CollectViews();
			this.HabScreenHabListItem = this.root.GetComponent<HabListItem>();
		}

		// Token: 0x06004AD1 RID: 19153 RVA: 0x001F4BD5 File Offset: 0x001F2DD5
		public void UpdateFromModel(HabScreenHabListItemModel model, BaseParamsWithPrefab parameters)
		{
			this.HabScreenHabListItem.SetHabState(model.HabScreenHabListItemData.habState, model.HabScreenHabListItemData.controller, model.HabScreenHabListItemData.previewer);
		}

		// Token: 0x04002B9F RID: 11167
		public HabListItem HabScreenHabListItem;
	}
}

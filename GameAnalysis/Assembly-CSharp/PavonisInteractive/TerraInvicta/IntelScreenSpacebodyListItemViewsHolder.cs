using System;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000823 RID: 2083
	public class IntelScreenSpacebodyListItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x06004AE6 RID: 19174 RVA: 0x001F4E30 File Offset: 0x001F3030
		public override void CollectViews()
		{
			base.CollectViews();
			this.IntelScreenSpacebodyListItem = this.root.GetComponent<IntelSpaceBodyListItemController>();
			Loc.SwapFonts(this.root.gameObject);
		}

		// Token: 0x06004AE7 RID: 19175 RVA: 0x001F4E59 File Offset: 0x001F3059
		public void UpdateFromModel(IntelScreenSpacebodyListItemModel model, BaseParamsWithPrefab parameters)
		{
			this.IntelScreenSpacebodyListItem.Initialize(model.IntelScreenSpacebodyListItemData.spacebodyState);
		}

		// Token: 0x04002BA5 RID: 11173
		public IntelSpaceBodyListItemController IntelScreenSpacebodyListItem;
	}
}

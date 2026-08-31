using System;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000820 RID: 2080
	public class IntelScreenHabSiteListItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x06004ADB RID: 19163 RVA: 0x001F4CFC File Offset: 0x001F2EFC
		public override void CollectViews()
		{
			base.CollectViews();
			this.IntelScreenHabSiteListItem = this.root.GetComponent<IntelHabSiteListItemController>();
			Loc.SwapFonts(this.root.gameObject);
		}

		// Token: 0x06004ADC RID: 19164 RVA: 0x001F4D25 File Offset: 0x001F2F25
		public void UpdateFromModel(IntelScreenHabSiteListItemModel model, BaseParamsWithPrefab parameters)
		{
			this.IntelScreenHabSiteListItem.Initialize(model.IntelScreenHabSiteListItemData);
		}

		// Token: 0x04002BA2 RID: 11170
		public IntelHabSiteListItemController IntelScreenHabSiteListItem;
	}
}

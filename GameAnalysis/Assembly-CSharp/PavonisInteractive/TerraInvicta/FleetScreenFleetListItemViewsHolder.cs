using System;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000817 RID: 2071
	public class FleetScreenFleetListItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x17000E99 RID: 3737
		// (get) Token: 0x06004AB6 RID: 19126 RVA: 0x001F4902 File Offset: 0x001F2B02
		// (set) Token: 0x06004AB7 RID: 19127 RVA: 0x001F490A File Offset: 0x001F2B0A
		private ContentSizeFitter CSF { get; set; }

		// Token: 0x06004AB8 RID: 19128 RVA: 0x001F4913 File Offset: 0x001F2B13
		public override void CollectViews()
		{
			base.CollectViews();
			this.CSF = this.root.GetComponent<ContentSizeFitter>();
			this.CSF.enabled = false;
			this.FleetScreenFleetListItem = this.root.GetComponent<FleetsSceenFleetListItemController>();
		}

		// Token: 0x06004AB9 RID: 19129 RVA: 0x001F4949 File Offset: 0x001F2B49
		public override void MarkForRebuild()
		{
			base.MarkForRebuild();
			if (this.CSF)
			{
				this.CSF.enabled = true;
			}
		}

		// Token: 0x06004ABA RID: 19130 RVA: 0x001F496A File Offset: 0x001F2B6A
		public override void UnmarkForRebuild()
		{
			if (this.CSF)
			{
				this.CSF.enabled = false;
			}
			base.UnmarkForRebuild();
		}

		// Token: 0x06004ABB RID: 19131 RVA: 0x001F498C File Offset: 0x001F2B8C
		public void UpdateFromModel(FleetScreenFleetListItemModel model, BaseParamsWithPrefab parameters)
		{
			this.FleetScreenFleetListItem.Init(model.FleetScreenFleetListItemData.controller);
			if (model.FleetScreenFleetListItemData.isGroupItem)
			{
				this.FleetScreenFleetListItem.CreateGroupItem(model.FleetScreenFleetListItemData.gameStateFleetOrShip.ref_faction);
			}
			else
			{
				this.FleetScreenFleetListItem.UpdateListItem(model.FleetScreenFleetListItemData.gameStateFleetOrShip);
			}
			model.HasPendingSizeChange = true;
		}

		// Token: 0x04002B98 RID: 11160
		public FleetsSceenFleetListItemController FleetScreenFleetListItem;
	}
}

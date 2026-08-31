using System;
using System.Collections.Generic;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using Com.ForbiddenByte.OSA.DataHelpers;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000827 RID: 2087
	public class NotificationOptionListAdapter : OSA<BaseParamsWithPrefab, NotificationOptionListItemViewsHolder>
	{
		// Token: 0x17000E9F RID: 3743
		// (get) Token: 0x06004AF4 RID: 19188 RVA: 0x001F4FA0 File Offset: 0x001F31A0
		// (set) Token: 0x06004AF5 RID: 19189 RVA: 0x001F4FA8 File Offset: 0x001F31A8
		public SimpleDataHelper<NotificationOptionListItemModel> Data { get; private set; }

		// Token: 0x06004AF6 RID: 19190 RVA: 0x001F4FB1 File Offset: 0x001F31B1
		protected override void Start()
		{
			this.Data = new SimpleDataHelper<NotificationOptionListItemModel>(this, true);
			base.Start();
		}

		// Token: 0x06004AF7 RID: 19191 RVA: 0x001F4FC6 File Offset: 0x001F31C6
		protected override NotificationOptionListItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			NotificationOptionListItemViewsHolder notificationOptionListItemViewsHolder = new NotificationOptionListItemViewsHolder();
			notificationOptionListItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return notificationOptionListItemViewsHolder;
		}

		// Token: 0x06004AF8 RID: 19192 RVA: 0x001F4FEC File Offset: 0x001F31EC
		protected override void UpdateViewsHolder(NotificationOptionListItemViewsHolder newOrRecycled)
		{
			NotificationOptionListItemModel notificationOptionListItemModel = this.Data[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateFromModel(notificationOptionListItemModel, this._Params);
		}

		// Token: 0x06004AF9 RID: 19193 RVA: 0x001F5018 File Offset: 0x001F3218
		public void SetItems(IList<NotificationOptionListItemModel> items)
		{
			IList<NotificationOptionListItemModel> list = new List<NotificationOptionListItemModel>();
			foreach (NotificationOptionListItemModel notificationOptionListItemModel in items)
			{
				if (notificationOptionListItemModel.notificationOptionListItemData.showInList)
				{
					list.Add(notificationOptionListItemModel);
				}
			}
			this.Data.ResetItems(list, false);
		}
	}
}

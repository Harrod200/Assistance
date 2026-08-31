using System;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using PavonisInteractive.TerraInvicta.SpaceCombat.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000811 RID: 2065
	public class BattleLogListItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x06004A9B RID: 19099 RVA: 0x001F4588 File Offset: 0x001F2788
		public override void CollectViews()
		{
			base.CollectViews();
			this.BattleLogListItemController = this.root.GetComponent<BattleLogEntry>();
		}

		// Token: 0x06004A9C RID: 19100 RVA: 0x001F45A1 File Offset: 0x001F27A1
		public void UpdateFromModel(BattleLogListItemModel model, BaseParamsWithPrefab parameters)
		{
			if (GameControl.gameStartedUnloading)
			{
				return;
			}
			this.BattleLogListItemController.Init(model.battleLogEntryData);
		}

		// Token: 0x04002B8E RID: 11150
		public BattleLogEntry BattleLogListItemController;
	}
}

using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A65 RID: 2661
	public class SetDisengageStatus : PlayerAction
	{
		// Token: 0x06006517 RID: 25879 RVA: 0x002FB283 File Offset: 0x002F9483
		public SetDisengageStatus(TISpaceShipState ship, bool setting)
		{
			this.shipID = ship.ID;
			this.setting = setting;
		}

		// Token: 0x06006518 RID: 25880 RVA: 0x002FB29E File Offset: 0x002F949E
		public override void Execute()
		{
			this.shipID.GetState<TISpaceShipState>(false).SetDisengageOrder(this.setting);
		}

		// Token: 0x0400473A RID: 18234
		private GameStateID shipID;

		// Token: 0x0400473B RID: 18235
		private bool setting;
	}
}

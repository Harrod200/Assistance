using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A59 RID: 2649
	internal class ClearArmyDestinationQueueAction : PlayerAction
	{
		// Token: 0x060064FF RID: 25855 RVA: 0x002FAD8F File Offset: 0x002F8F8F
		public ClearArmyDestinationQueueAction(TIArmyState army)
		{
			this.armyID = army.ID;
		}

		// Token: 0x06006500 RID: 25856 RVA: 0x002FADA3 File Offset: 0x002F8FA3
		public override void Execute()
		{
			this.armyID.GetState<TIArmyState>(true).destinationQueue.Clear();
		}

		// Token: 0x04004727 RID: 18215
		private GameStateID armyID;
	}
}

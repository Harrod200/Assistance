using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A7A RID: 2682
	internal class QueueArmyDestinationAction : PlayerAction
	{
		// Token: 0x06006548 RID: 25928 RVA: 0x002FC215 File Offset: 0x002FA415
		public QueueArmyDestinationAction(TIArmyState army, TIRegionState destination)
		{
			this.armyID = army.ID;
			this.destinationID = destination.ID;
		}

		// Token: 0x06006549 RID: 25929 RVA: 0x002FC238 File Offset: 0x002FA438
		public override void Execute()
		{
			TIArmyState state = this.armyID.GetState<TIArmyState>(true);
			TIRegionState state2 = this.destinationID.GetState<TIRegionState>(true);
			state.destinationQueue.Add(state2);
		}

		// Token: 0x04004774 RID: 18292
		private GameStateID armyID;

		// Token: 0x04004775 RID: 18293
		private GameStateID destinationID;
	}
}

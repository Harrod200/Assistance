using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A9D RID: 2717
	public class SplitFleetOperationAction : PlayerAction
	{
		// Token: 0x1700111B RID: 4379
		// (get) Token: 0x0600658F RID: 25999 RVA: 0x002FD45F File Offset: 0x002FB65F
		// (set) Token: 0x06006590 RID: 26000 RVA: 0x002FD467 File Offset: 0x002FB667
		public TISpaceFleetState newFleet { get; private set; }

		// Token: 0x06006591 RID: 26001 RVA: 0x002FD470 File Offset: 0x002FB670
		public SplitFleetOperationAction(TISpaceFleetState oldFleet, List<TISpaceShipState> newFleetShips, FactionGoal_Fleet goal = null)
		{
			this.oldFleetID = oldFleet.ID;
			foreach (TISpaceShipState tispaceShipState in newFleetShips)
			{
				this.newFleetShipIDs.Add(tispaceShipState.ID);
			}
			this.goal = goal;
		}

		// Token: 0x06006592 RID: 26002 RVA: 0x002FD4EC File Offset: 0x002FB6EC
		public override void Execute()
		{
			TISpaceFleetState state = this.oldFleetID.GetState<TISpaceFleetState>(false);
			List<TISpaceShipState> newShipList = new List<TISpaceShipState>();
			this.newFleetShipIDs.ForEach(delegate(GameStateID x)
			{
				newShipList.Add(x.GetState<TISpaceShipState>(false));
			});
			this.newFleet = SplitFleetOperation.BuildFleetFromSelectedTargets(state, newShipList, this.goal);
			state.unreachableLocations.Clear();
		}

		// Token: 0x040047E0 RID: 18400
		private GameStateID oldFleetID;

		// Token: 0x040047E1 RID: 18401
		private List<GameStateID> newFleetShipIDs = new List<GameStateID>();

		// Token: 0x040047E2 RID: 18402
		private FactionGoal_Fleet goal;
	}
}

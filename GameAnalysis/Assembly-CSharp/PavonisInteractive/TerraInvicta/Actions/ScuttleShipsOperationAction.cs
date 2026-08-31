using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A85 RID: 2693
	public class ScuttleShipsOperationAction : PlayerAction
	{
		// Token: 0x0600655E RID: 25950 RVA: 0x002FC6B4 File Offset: 0x002FA8B4
		public ScuttleShipsOperationAction(TISpaceFleetState fleet, List<TISpaceShipState> shipsToDestroy)
		{
			this.fleet = fleet.ID;
			foreach (TISpaceShipState tispaceShipState in shipsToDestroy)
			{
				this.shipsToDestroyIDs.Add(tispaceShipState.ID);
			}
		}

		// Token: 0x0600655F RID: 25951 RVA: 0x002FC72C File Offset: 0x002FA92C
		public override void Execute()
		{
			TISpaceFleetState state = this.fleet.GetState<TISpaceFleetState>(false);
			List<TISpaceShipState> newShipList = new List<TISpaceShipState>();
			this.shipsToDestroyIDs.ForEach(delegate(GameStateID x)
			{
				newShipList.Add(x.GetState<TISpaceShipState>(false));
			});
			ScuttleShipsOperation.ScuttleShipsFromFleet(state, newShipList);
		}

		// Token: 0x04004790 RID: 18320
		private GameStateID fleet;

		// Token: 0x04004791 RID: 18321
		private List<GameStateID> shipsToDestroyIDs = new List<GameStateID>();
	}
}

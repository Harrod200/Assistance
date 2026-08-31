using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta.SpaceCombat;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A88 RID: 2696
	public class SelectFleetFormationAction : PlayerAction
	{
		// Token: 0x06006564 RID: 25956 RVA: 0x002FC9C8 File Offset: 0x002FABC8
		public SelectFleetFormationAction(TISpaceFleetState fleet, Formation formation, TISpaceCombatState combat, int numberOfPositions, IList<CombatShipController> activeShips, bool saveFormation = false)
		{
			this.fleetID = fleet.ID;
			if (combat != null)
			{
				this.combatID = combat.ID;
			}
			this.formation = formation;
			this.saveFormation = saveFormation;
			this.numberOfPositions = numberOfPositions;
			this.activeShips = activeShips;
		}

		// Token: 0x06006565 RID: 25957 RVA: 0x002FCA1C File Offset: 0x002FAC1C
		public override void Execute()
		{
			TISpaceFleetState state = this.fleetID.GetState<TISpaceFleetState>(false);
			TISpaceCombatState tispaceCombatState;
			this.combatID.TryGetState<TISpaceCombatState>(out tispaceCombatState, false);
			TIPromptQueueState.RemovePromptStatic(state.faction, state, tispaceCombatState, "PromptSelectFleetFormation", 0);
			state.AssignFormation(this.formation, tispaceCombatState.fleets.IndexOf(state) == 1, false, this.saveFormation, true, false);
			List<TISpaceShipState> list = new List<TISpaceShipState>();
			foreach (CombatShipController combatShipController in this.activeShips)
			{
				list.Add(combatShipController.ShipState);
			}
			foreach (TISpaceShipState tispaceShipState in list)
			{
				tispaceShipState.SetCombatFormationOffset(list, state.formation, this.numberOfPositions, tispaceCombatState.fleets.IndexOf(state) == 1, true);
			}
			GameControl.eventManager.TriggerEvent(new FleetFormationSelected(state), null, new object[] { state });
		}

		// Token: 0x0400479A RID: 18330
		public GameStateID fleetID;

		// Token: 0x0400479B RID: 18331
		public string formationDataName;

		// Token: 0x0400479C RID: 18332
		public GameStateID combatID;

		// Token: 0x0400479D RID: 18333
		public Formation formation;

		// Token: 0x0400479E RID: 18334
		public bool saveFormation;

		// Token: 0x0400479F RID: 18335
		public int numberOfPositions;

		// Token: 0x040047A0 RID: 18336
		public IList<CombatShipController> activeShips;
	}
}

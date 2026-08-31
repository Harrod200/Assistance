using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A86 RID: 2694
	public class SelectCombatBid : PlayerAction
	{
		// Token: 0x06006560 RID: 25952 RVA: 0x002FC778 File Offset: 0x002FA978
		public SelectCombatBid(TISpaceCombatState combat, TIFactionState faction, float bid_kps, CombatStance extendedOverride, List<TISpaceShipState> extendedPursuers)
		{
			this.combatID = combat.ID;
			this.factionID = faction.ID;
			this.extendedPursuerIDs = extendedPursuers.Select<TISpaceShipState, GameStateID>((TISpaceShipState x) => x.ID).ToList<GameStateID>();
			this.bid_kps = bid_kps;
			this.extendedOverride = extendedOverride;
		}

		// Token: 0x06006561 RID: 25953 RVA: 0x002FC7E4 File Offset: 0x002FA9E4
		public override void Execute()
		{
			TIFactionState faction = this.factionID.GetState<TIFactionState>(false);
			TISpaceCombatState state = this.combatID.GetState<TISpaceCombatState>(false);
			TIPromptQueueState.RemovePromptStatic(faction, null, state, "PromptSelectSpaceCombatBid", 0);
			if (this.extendedOverride != CombatStance.NotYetSet)
			{
				List<TISpaceShipState> list = new List<TISpaceShipState>();
				foreach (GameStateID gameStateID in this.extendedPursuerIDs)
				{
					list.Add(gameStateID.GetState<TISpaceShipState>(false));
				}
				state.stances[faction] = this.extendedOverride;
				state.RemoveShipsFromBattleInExtendedPursuit(list, state.fleets.First<TISpaceFleetState>((TISpaceFleetState x) => x.faction == faction));
			}
			state.bids_kps[faction] = Mathf.Max(0f, Mathf.Min(this.bid_kps, state.GetFleet(faction).availableDeltaVforPrecombat_kps));
			state.SetRequiresCombat();
			GameControl.eventManager.TriggerEvent(new CombatBidSelected(), null, Array.Empty<object>());
		}

		// Token: 0x04004792 RID: 18322
		public GameStateID factionID;

		// Token: 0x04004793 RID: 18323
		public GameStateID combatID;

		// Token: 0x04004794 RID: 18324
		public float bid_kps;

		// Token: 0x04004795 RID: 18325
		public CombatStance extendedOverride;

		// Token: 0x04004796 RID: 18326
		public List<GameStateID> extendedPursuerIDs;
	}
}

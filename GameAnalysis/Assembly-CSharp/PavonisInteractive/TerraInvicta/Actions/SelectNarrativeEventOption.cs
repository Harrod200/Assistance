using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A89 RID: 2697
	public class SelectNarrativeEventOption : PlayerAction
	{
		// Token: 0x06006566 RID: 25958 RVA: 0x002FCB3C File Offset: 0x002FAD3C
		public SelectNarrativeEventOption(TIFactionState faction, TIGameState eventTarget, TIGameState secondaryTarget, TINarrativeEventTemplate eventTemplate, int optionSelected, Dictionary<TIGameState, TIGameState> allTargets, Prompt prompt)
		{
			this.factionID = ((faction != null) ? faction.ID : default(GameStateID));
			this.eventTargetID = eventTarget.ID;
			this.secondaryTargetID = ((secondaryTarget != null) ? secondaryTarget.ID : default(GameStateID));
			this.optionSelected = optionSelected;
			this.eventTemplate = eventTemplate;
			this.prompt = prompt;
			this.allTargetIDs = new Dictionary<GameStateID, GameStateID>();
			if (allTargets != null)
			{
				foreach (KeyValuePair<TIGameState, TIGameState> keyValuePair in allTargets)
				{
					this.allTargetIDs.Add(keyValuePair.Key.ID, (keyValuePair.Value != null) ? keyValuePair.Value.ID : default(GameStateID));
				}
			}
		}

		// Token: 0x06006567 RID: 25959 RVA: 0x002FCC3C File Offset: 0x002FAE3C
		public override void Execute()
		{
			TIFactionState tifactionState;
			this.factionID.TryGetState<TIFactionState>(out tifactionState, false);
			TIGameState state = this.eventTargetID.GetState<TIGameState>(true);
			TIGameState tigameState;
			this.secondaryTargetID.TryGetState<TIGameState>(out tigameState, true);
			Dictionary<TIGameState, TIGameState> dictionary = new Dictionary<TIGameState, TIGameState>();
			foreach (GameStateID gameStateID in this.allTargetIDs.Keys)
			{
				dictionary.Add(gameStateID.GetState<TIGameState>(true), (this.allTargetIDs[gameStateID] != default(GameStateID)) ? this.allTargetIDs[gameStateID].GetState<TIGameState>(true) : null);
			}
			GameStateManager.FindGameState<TIGlobalValuesState>().ExecuteNarrativeEventOption(this.eventTemplate, tifactionState, state, tigameState, this.optionSelected, dictionary, this.prompt);
			TIPromptQueueState.RemovePromptStatic(tifactionState, state, tigameState, "PromptAddressNarrativeEvent", 0);
		}

		// Token: 0x040047A1 RID: 18337
		private GameStateID factionID;

		// Token: 0x040047A2 RID: 18338
		private GameStateID eventTargetID;

		// Token: 0x040047A3 RID: 18339
		private GameStateID secondaryTargetID;

		// Token: 0x040047A4 RID: 18340
		private int optionSelected;

		// Token: 0x040047A5 RID: 18341
		private TINarrativeEventTemplate eventTemplate;

		// Token: 0x040047A6 RID: 18342
		private Dictionary<GameStateID, GameStateID> allTargetIDs;

		// Token: 0x040047A7 RID: 18343
		private Prompt prompt;
	}
}

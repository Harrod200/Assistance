using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A8E RID: 2702
	public class SellSpaceResourcesToEarthAction : PlayerAction
	{
		// Token: 0x06006571 RID: 25969 RVA: 0x002FCEE5 File Offset: 0x002FB0E5
		public SellSpaceResourcesToEarthAction(TIFactionState faction, Dictionary<FactionResource, int> plannedSales)
		{
			this.plannedSales = plannedSales;
			this.factionID = faction.ID;
		}

		// Token: 0x06006572 RID: 25970 RVA: 0x002FCF00 File Offset: 0x002FB100
		public override void Execute()
		{
			TIFactionState state = this.factionID.GetState<TIFactionState>(false);
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			foreach (KeyValuePair<FactionResource, int> keyValuePair in this.plannedSales)
			{
				tiresourcesCost.AddCost(keyValuePair.Key, (float)keyValuePair.Value, true);
				tiresourcesCost.AddCost(FactionResource.Money, (float)keyValuePair.Value * -TIGlobalValuesState.GlobalValues.GetModifiedResourceMarketValueForSelling(state, keyValuePair.Key), true);
			}
			tiresourcesCost.PayCost(state, "Sell Space Resources To Earth");
			TIGlobalValuesState.GlobalValues.ModifyMarketValuesForResourceSale(this.plannedSales);
		}

		// Token: 0x040047B4 RID: 18356
		private Dictionary<FactionResource, int> plannedSales;

		// Token: 0x040047B5 RID: 18357
		private GameStateID factionID;
	}
}

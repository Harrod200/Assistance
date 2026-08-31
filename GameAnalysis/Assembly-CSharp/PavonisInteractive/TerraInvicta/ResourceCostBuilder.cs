using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000713 RID: 1811
	public struct ResourceCostBuilder
	{
		// Token: 0x06002B60 RID: 11104 RVA: 0x000EC5D8 File Offset: 0x000EA7D8
		public float GetWeightedCost(FactionResource resource)
		{
			switch (resource)
			{
			case FactionResource.Money:
				return this.money;
			case FactionResource.Influence:
				return this.influence;
			case FactionResource.Operations:
				return this.operations;
			case FactionResource.Research:
				return this.research;
			case FactionResource.Boost:
				return this.boost;
			case FactionResource.Water:
				return this.water;
			case FactionResource.Volatiles:
				return this.volatiles;
			case FactionResource.Metals:
				return this.metals;
			case FactionResource.NobleMetals:
				return this.nobleMetals;
			case FactionResource.Fissiles:
				return this.fissiles;
			case FactionResource.Antimatter:
				return this.antimatter;
			case FactionResource.Exotics:
				return this.exotics;
			}
			return 0f;
		}

		// Token: 0x06002B61 RID: 11105 RVA: 0x000EC680 File Offset: 0x000EA880
		public TIResourcesCost ToResourcesCost(float multiplier = 1f)
		{
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			if (this.money != 0f)
			{
				tiresourcesCost.AddCost(FactionResource.Money, this.money * multiplier, true);
			}
			if (this.influence != 0f)
			{
				tiresourcesCost.AddCost(FactionResource.Influence, this.influence * multiplier, true);
			}
			if (this.operations != 0f)
			{
				tiresourcesCost.AddCost(FactionResource.Operations, this.operations * multiplier, true);
			}
			if (this.research < 0f)
			{
				tiresourcesCost.AddCost(FactionResource.Research, this.research * multiplier, true);
			}
			if (this.boost != 0f)
			{
				tiresourcesCost.AddCost(FactionResource.Boost, this.boost * multiplier, true);
			}
			if (this.water != 0f)
			{
				tiresourcesCost.AddCost(FactionResource.Water, this.water * multiplier, true);
			}
			if (this.volatiles != 0f)
			{
				tiresourcesCost.AddCost(FactionResource.Volatiles, this.volatiles * multiplier, true);
			}
			if (this.metals != 0f)
			{
				tiresourcesCost.AddCost(FactionResource.Metals, this.metals * multiplier, true);
			}
			if (this.nobleMetals != 0f)
			{
				tiresourcesCost.AddCost(FactionResource.NobleMetals, this.nobleMetals * multiplier, true);
			}
			if (this.fissiles != 0f)
			{
				tiresourcesCost.AddCost(FactionResource.Fissiles, this.fissiles * multiplier, true);
			}
			if (this.antimatter != 0f)
			{
				tiresourcesCost.AddCost(FactionResource.Antimatter, this.antimatter * multiplier, true);
			}
			if (this.exotics != 0f)
			{
				tiresourcesCost.AddCost(FactionResource.Exotics, this.exotics * multiplier, true);
			}
			return tiresourcesCost;
		}

		// Token: 0x06002B62 RID: 11106 RVA: 0x000EC7F8 File Offset: 0x000EA9F8
		public Dictionary<FactionResource, float> ToRVCollection(float multiplier = 1f)
		{
			Dictionary<FactionResource, float> dictionary = new Dictionary<FactionResource, float>();
			if (this.money != 0f)
			{
				dictionary.Add(FactionResource.Money, this.money * multiplier);
			}
			if (this.influence != 0f)
			{
				dictionary.Add(FactionResource.Influence, this.influence * multiplier);
			}
			if (this.operations != 0f)
			{
				dictionary.Add(FactionResource.Operations, this.operations * multiplier);
			}
			if (this.research < 0f)
			{
				dictionary.Add(FactionResource.Research, this.research * multiplier);
			}
			if (this.boost != 0f)
			{
				dictionary.Add(FactionResource.Boost, this.boost * multiplier);
			}
			if (this.water != 0f)
			{
				dictionary.Add(FactionResource.Water, this.water * multiplier);
			}
			if (this.volatiles != 0f)
			{
				dictionary.Add(FactionResource.Volatiles, this.volatiles * multiplier);
			}
			if (this.metals != 0f)
			{
				dictionary.Add(FactionResource.Metals, this.metals * multiplier);
			}
			if (this.nobleMetals != 0f)
			{
				dictionary.Add(FactionResource.NobleMetals, this.nobleMetals * multiplier);
			}
			if (this.fissiles != 0f)
			{
				dictionary.Add(FactionResource.Fissiles, this.fissiles * multiplier);
			}
			if (this.antimatter != 0f)
			{
				dictionary.Add(FactionResource.Antimatter, this.antimatter * multiplier);
			}
			if (this.exotics != 0f)
			{
				dictionary.Add(FactionResource.Exotics, this.exotics * multiplier);
			}
			return dictionary;
		}

		// Token: 0x0400212D RID: 8493
		public float money;

		// Token: 0x0400212E RID: 8494
		public float influence;

		// Token: 0x0400212F RID: 8495
		public float operations;

		// Token: 0x04002130 RID: 8496
		public float research;

		// Token: 0x04002131 RID: 8497
		public float boost;

		// Token: 0x04002132 RID: 8498
		public float water;

		// Token: 0x04002133 RID: 8499
		public float volatiles;

		// Token: 0x04002134 RID: 8500
		public float metals;

		// Token: 0x04002135 RID: 8501
		public float nobleMetals;

		// Token: 0x04002136 RID: 8502
		public float fissiles;

		// Token: 0x04002137 RID: 8503
		public float antimatter;

		// Token: 0x04002138 RID: 8504
		public float exotics;
	}
}

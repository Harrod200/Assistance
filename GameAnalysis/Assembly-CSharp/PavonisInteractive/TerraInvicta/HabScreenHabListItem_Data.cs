using System;
using System.Linq;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200086C RID: 2156
	public class HabScreenHabListItem_Data
	{
		// Token: 0x0600500D RID: 20493 RVA: 0x00229034 File Offset: 0x00227234
		public void SetData(TIHabState habState)
		{
			this.habState = habState;
			this.MCSortValue = habState.GetNetCurrentMonthlyIncome(habState.coreFaction, FactionResource.MissionControl, false, false);
			this.WaterSortValue = habState.GetNetCurrentMonthlyIncome(habState.coreFaction, FactionResource.Water, false, false);
			this.VolatilesSortValue = habState.GetNetCurrentMonthlyIncome(habState.coreFaction, FactionResource.Volatiles, false, false);
			this.MetalsSortValue = habState.GetNetCurrentMonthlyIncome(habState.coreFaction, FactionResource.Metals, false, false);
			this.NobleMetalsSortValue = habState.GetNetCurrentMonthlyIncome(habState.coreFaction, FactionResource.NobleMetals, false, false);
			this.FissilesSortValue = habState.GetNetCurrentMonthlyIncome(habState.coreFaction, FactionResource.Fissiles, false, false);
			this.AntimatterSortValue = habState.GetNetCurrentMonthlyIncome(habState.coreFaction, FactionResource.Antimatter, false, false);
			this.ExoticsSortValue = habState.GetNetCurrentMonthlyIncome(habState.coreFaction, FactionResource.Exotics, false, false);
			this.ResupplySortValue = habState.AllowsResupply(habState.coreFaction, false, false);
			this.ShipyardSortValue = habState.AllowsShipConstruction(habState.coreFaction, false, false);
			this.ConstructionSortValue = habState.AllModules().Any<TIHabModuleState>((TIHabModuleState x) => x.underConstruction);
			this.TierSortValue = habState.tier;
			this.PopulationSortValue = habState.crew;
			this.PowerSortValue = habState.FunctionalModules().Any<TIHabModuleState>((TIHabModuleState x) => !x.powered);
			this.ModuleConstructionSortValue = habState.GetModuleConstructionTimeModifier(false, null) < 1f;
			this.MoneySortValue = habState.GetNetCurrentMonthlyIncome(habState.coreFaction, FactionResource.Money, false, false);
			this.InfluenceSortValue = habState.GetNetCurrentMonthlyIncome(habState.coreFaction, FactionResource.Influence, false, false);
			this.OpsSortValue = habState.GetNetCurrentMonthlyIncome(habState.coreFaction, FactionResource.Operations, false, false);
			this.ResearchSortValue = habState.GetNetCurrentMonthlyIncome(habState.coreFaction, FactionResource.Research, false, false);
			this.ProjectsSortValue = habState.GetNetCurrentMonthlyIncome(habState.coreFaction, FactionResource.Projects, false, false);
			this.BoostSortValue = habState.GetNetCurrentMonthlyIncome(habState.coreFaction, FactionResource.Boost, false, false);
		}

		// Token: 0x04003384 RID: 13188
		public bool showInList;

		// Token: 0x04003385 RID: 13189
		public TIHabState habState;

		// Token: 0x04003386 RID: 13190
		public HabitatsScreenController controller;

		// Token: 0x04003387 RID: 13191
		public IHabitatsPreviewer previewer;

		// Token: 0x04003388 RID: 13192
		public float MCSortValue;

		// Token: 0x04003389 RID: 13193
		public float WaterSortValue;

		// Token: 0x0400338A RID: 13194
		public float VolatilesSortValue;

		// Token: 0x0400338B RID: 13195
		public float MetalsSortValue;

		// Token: 0x0400338C RID: 13196
		public float NobleMetalsSortValue;

		// Token: 0x0400338D RID: 13197
		public float FissilesSortValue;

		// Token: 0x0400338E RID: 13198
		public float AntimatterSortValue;

		// Token: 0x0400338F RID: 13199
		public float ExoticsSortValue;

		// Token: 0x04003390 RID: 13200
		public bool ResupplySortValue;

		// Token: 0x04003391 RID: 13201
		public bool ShipyardSortValue;

		// Token: 0x04003392 RID: 13202
		public bool ConstructionSortValue;

		// Token: 0x04003393 RID: 13203
		public int TierSortValue;

		// Token: 0x04003394 RID: 13204
		public int PopulationSortValue;

		// Token: 0x04003395 RID: 13205
		public bool PowerSortValue;

		// Token: 0x04003396 RID: 13206
		public bool ModuleConstructionSortValue;

		// Token: 0x04003397 RID: 13207
		public float MoneySortValue;

		// Token: 0x04003398 RID: 13208
		public float InfluenceSortValue;

		// Token: 0x04003399 RID: 13209
		public float OpsSortValue;

		// Token: 0x0400339A RID: 13210
		public float ResearchSortValue;

		// Token: 0x0400339B RID: 13211
		public float ProjectsSortValue;

		// Token: 0x0400339C RID: 13212
		public float BoostSortValue;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007E2 RID: 2018
	public static class Enums
	{
		// Token: 0x04002A67 RID: 10855
		public static readonly FactionResource[] FactionResources = ((FactionResource[])Enum.GetValues(typeof(FactionResource))).Except<FactionResource>(new FactionResource[1]).ToArray<FactionResource>();

		// Token: 0x04002A68 RID: 10856
		public static readonly PriorityType[] PriorityTypes = ((PriorityType[])Enum.GetValues(typeof(PriorityType))).Except<PriorityType>(Enumerable.Repeat<PriorityType>(PriorityType.None, 1)).ToArray<PriorityType>();

		// Token: 0x04002A69 RID: 10857
		public static readonly TechCategory[] TechCategories = (TechCategory[])Enum.GetValues(typeof(TechCategory));

		// Token: 0x04002A6A RID: 10858
		public static readonly ShipRole[] ShipRoles = (ShipRole[])Enum.GetValues(typeof(ShipRole));

		// Token: 0x04002A6B RID: 10859
		public static readonly ShipRole[] ActiveShipRoles = Enums.ShipRoles.Except<ShipRole>(new ShipRole[1]).ToArray<ShipRole>();

		// Token: 0x04002A6C RID: 10860
		public static readonly ShipRole[] HumanShipRoles = Enums.ActiveShipRoles.Except<ShipRole>(new ShipRole[] { ShipRole.ArmyCarrier }).ToArray<ShipRole>();

		// Token: 0x04002A6D RID: 10861
		public static readonly CouncilorAttribute[] CouncilorAttributes = ((CouncilorAttribute[])Enum.GetValues(typeof(CouncilorAttribute))).Except<CouncilorAttribute>(new CouncilorAttribute[1]).ToArray<CouncilorAttribute>();

		// Token: 0x04002A6E RID: 10862
		public static readonly ShipSystem[] ShipSystems = ((ShipSystem[])Enum.GetValues(typeof(ShipSystem))).Except<ShipSystem>(new ShipSystem[1]).ToArray<ShipSystem>();

		// Token: 0x04002A6F RID: 10863
		public static readonly ShipSystem[] DamageableShipSystems = Enums.ShipSystems.Except<ShipSystem>(new ShipSystem[]
		{
			ShipSystem.HullWeapons,
			ShipSystem.NoseWeapons,
			ShipSystem.UtilityModules,
			ShipSystem.Propellant,
			ShipSystem.Drive,
			ShipSystem.Radiators,
			ShipSystem.PowerPlant,
			ShipSystem.None
		}).ToArray<ShipSystem>();

		// Token: 0x04002A70 RID: 10864
		public static readonly HashSet<ShipSystem> DamageableShipSystemsSet = new HashSet<ShipSystem>(Enums.DamageableShipSystems);

		// Token: 0x04002A71 RID: 10865
		public static readonly GoalType[] GoalTypes = ((GoalType[])Enum.GetValues(typeof(GoalType))).Except<GoalType>(new GoalType[1]).ToArray<GoalType>();
	}
}

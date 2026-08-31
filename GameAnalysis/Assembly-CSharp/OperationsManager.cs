using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x020002F7 RID: 759
public static class OperationsManager
{
	// Token: 0x06000B8A RID: 2954 RVA: 0x0003E768 File Offset: 0x0003C968
	public static void Initalize()
	{
		OperationsManager.armyOperations.Clear();
		OperationsManager.fleetOperations.Clear();
		OperationsManager.spaceOperations.Clear();
		OperationsManager.nationOperations.Clear();
		OperationsManager.operationsLookup.Clear();
		OperationsManager.AIArmyOperations.Clear();
		OperationsManager.CancelArmyOperation.Clear();
		OperationsManager.LegalArmyOperationsWhileMoving.Clear();
		OperationsManager.armyOperations.Add(new DeployArmyOperation_OpenTarget(false));
		OperationsManager.armyOperations.Add(new DeployArmiesOperation(false));
		OperationsManager.armyOperations.Add(new ArmyGoHomeOperation());
		OperationsManager.armyOperations.Add(new AllArmiesGoHomeOperation());
		OperationsManager.armyOperations.Add(new DeployArmyOperation_TargetHome());
		OperationsManager.armyOperations.Add(new AllArmiesPathHomeOperation());
		OperationsManager.armyOperations.Add(new AssaultAlienAssetOperation());
		OperationsManager.armyOperations.Add(new SetHuntXenoformingOperation());
		OperationsManager.armyOperations.Add(new CancelHuntXenoformingOperation());
		OperationsManager.armyOperations.Add(new AssaultSpaceFacilityOperation());
		OperationsManager.armyOperations.Add(new AnnexRegionOperation());
		OperationsManager.armyOperations.Add(new RazeRegionOperation());
		OperationsManager.armyOperations.Add(new CancelArmyOperation());
		OperationsManager.AIArmyOperations.AddRange(OperationsManager.armyOperations.Where<IOperation>((IOperation x) => !(x as TIArmyOperationTemplate).isConvenienceOperation));
		OperationsManager.LegalArmyOperationsWhileMoving.AddRange(OperationsManager.armyOperations.Where<IOperation>((IOperation x) => x is DeployArmyOperation || x is CancelArmyOperation));
		OperationsManager.CancelArmyOperation.Add(OperationsManager.armyOperations.FirstOrDefault<IOperation>((IOperation x) => x is CancelArmyOperation));
		OperationsManager.fleetOperations.Add(new TransferOperation());
		OperationsManager.fleetOperations.Add(new BombardOperation_Low());
		OperationsManager.fleetOperations.Add(new BombardOperation_Med());
		OperationsManager.fleetOperations.Add(new BombardOperation_High());
		OperationsManager.fleetOperations.Add(new AssaultHabOperation());
		OperationsManager.fleetOperations.Add(new DestroyHabOperation());
		OperationsManager.fleetOperations.Add(new MergeFleetOperation());
		OperationsManager.fleetOperations.Add(new MergeAllFleetOperation());
		OperationsManager.fleetOperations.Add(new SplitFleetOperation());
		OperationsManager.fleetOperations.Add(new ResupplyAndRepairOperation());
		OperationsManager.fleetOperations.Add(new ResupplyOperation());
		OperationsManager.fleetOperations.Add(new RepairFleetOperation());
		OperationsManager.fleetOperations.Add(new InterfleetRefuelOperation());
		OperationsManager.fleetOperations.Add(new LandOnSurfaceOperation());
		OperationsManager.fleetOperations.Add(new LaunchFromSurfaceOperation());
		OperationsManager.fleetOperations.Add(new UndockFromStationOperation());
		OperationsManager.fleetOperations.Add(new SurveyPlanetFromFleetOperation());
		OperationsManager.fleetOperations.Add(new FoundSolarPlatformOperation());
		OperationsManager.fleetOperations.Add(new FoundFissionPlatformOperation());
		OperationsManager.fleetOperations.Add(new FoundFusionPlatformOperation());
		OperationsManager.fleetOperations.Add(new FoundSolarOutpostOperation());
		OperationsManager.fleetOperations.Add(new FoundFissionOutpostOperation());
		OperationsManager.fleetOperations.Add(new FoundFusionOutpostOperation());
		OperationsManager.fleetOperations.Add(new FoundAutomatedSolarPlatformOperation());
		OperationsManager.fleetOperations.Add(new FoundAutomatedFissionPlatformOperation());
		OperationsManager.fleetOperations.Add(new FoundAutomatedSolarOutpostOperation());
		OperationsManager.fleetOperations.Add(new FoundAutomatedFissionOutpostOperation());
		OperationsManager.fleetOperations.Add(new ScuttleShipsOperation());
		OperationsManager.fleetOperations.Add(new SetHomeportOperation());
		OperationsManager.fleetOperations.Add(new ClearHomeportOperation());
		OperationsManager.fleetOperations.Add(new TransferOfficersOperation());
		OperationsManager.fleetOperations.Add(new AlienEarthSurveillanceOperation());
		OperationsManager.fleetOperations.Add(new AlienCrashdownOperation());
		OperationsManager.fleetOperations.Add(new AlienLandArmyOperation());
		OperationsManager.fleetOperations.Add(new CancelFleetOperation());
		OperationsManager.fleetOperations.Add(new FoundAlienSurveillancePlatform());
		OperationsManager.fleetOperations.Add(new FoundAlienSurveillanceOrbital());
		OperationsManager.fleetOperations.Add(new FoundAlienSurveillanceRing());
		OperationsManager.fleetOperations.Add(new SetContinuousBombardXenoformingOperation());
		OperationsManager.fleetOperations.Add(new CancelContinuousBombardXenoformingOperation());
		OperationsManager.spaceOperations.Add(new LaunchSTOInterceptorsOperation());
		OperationsManager.spaceOperations.Add(new LaunchProbeOperation());
		OperationsManager.spaceOperations.Add(new LaunchOverrideProbeOperation());
		OperationsManager.spaceOperations.Add(new FoundPlatformOperation());
		OperationsManager.spaceOperations.Add(new FoundOrbitalOperation());
		OperationsManager.spaceOperations.Add(new FoundRingOperation());
		OperationsManager.spaceOperations.Add(new FoundOutpostOperation());
		OperationsManager.spaceOperations.Add(new FoundSettlementOperation());
		OperationsManager.spaceOperations.Add(new FoundColonyOperation());
		OperationsManager.spaceOperations.Add(new FoundAutomatedPlatformOperation());
		OperationsManager.spaceOperations.Add(new FoundAutomatedOutpostOperation());
		OperationsManager.nationOperations.Add(new NuclearWeaponsStrike());
		foreach (IOperation operation in OperationsManager.armyOperations)
		{
			OperationsManager.operationsLookup.Add(operation.GetType(), operation);
		}
		foreach (IOperation operation2 in OperationsManager.fleetOperations)
		{
			OperationsManager.operationsLookup.Add(operation2.GetType(), operation2);
		}
		foreach (IOperation operation3 in OperationsManager.spaceOperations)
		{
			OperationsManager.operationsLookup.Add(operation3.GetType(), operation3);
		}
		foreach (IOperation operation4 in OperationsManager.nationOperations)
		{
			OperationsManager.operationsLookup.Add(operation4.GetType(), operation4);
		}
	}

	// Token: 0x04000EA4 RID: 3748
	public static List<IOperation> armyOperations = new List<IOperation>();

	// Token: 0x04000EA5 RID: 3749
	public static List<IOperation> fleetOperations = new List<IOperation>();

	// Token: 0x04000EA6 RID: 3750
	public static List<IOperation> spaceOperations = new List<IOperation>();

	// Token: 0x04000EA7 RID: 3751
	public static List<IOperation> nationOperations = new List<IOperation>();

	// Token: 0x04000EA8 RID: 3752
	public static Dictionary<Type, IOperation> operationsLookup = new Dictionary<Type, IOperation>();

	// Token: 0x04000EA9 RID: 3753
	public static List<IOperation> AIArmyOperations = new List<IOperation>();

	// Token: 0x04000EAA RID: 3754
	public static List<IOperation> LegalArmyOperationsWhileMoving = new List<IOperation>();

	// Token: 0x04000EAB RID: 3755
	public static List<IOperation> CancelArmyOperation = new List<IOperation>();
}

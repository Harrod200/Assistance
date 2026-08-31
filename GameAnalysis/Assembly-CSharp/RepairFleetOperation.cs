using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000336 RID: 822
public class RepairFleetOperation : FixUpFleetOperation
{
	// Token: 0x06000DD4 RID: 3540 RVA: 0x0004520B File Offset: 0x0004340B
	public override int SortOrder()
	{
		return 9;
	}

	// Token: 0x06000DD5 RID: 3541 RVA: 0x0004520F File Offset: 0x0004340F
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		return new List<TIGameState> { actorState };
	}

	// Token: 0x06000DD6 RID: 3542 RVA: 0x0004521D File Offset: 0x0004341D
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return actorState.ref_fleet.NeedsRepair();
	}

	// Token: 0x06000DD7 RID: 3543 RVA: 0x0004522C File Offset: 0x0004342C
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		TISpaceFleetState ref_fleet = actorState.ref_fleet;
		return ref_fleet.dockedAtHab && !ref_fleet.inCombatOrWaitingForCombat && ref_fleet.NeedsRepair() && ref_fleet.ref_hab.CanPartiallyRepairFleet(ref_fleet) && ref_fleet.CanAffordAnyRepairs(ref_fleet.ref_hab) && base.ActorCanPerformOperation_PassInterruptCheck(actorState);
	}

	// Token: 0x06000DD8 RID: 3544 RVA: 0x00045280 File Offset: 0x00043480
	public override string GetDescription(TIGameState actorState = null, TIGameState target = null)
	{
		if (actorState != null)
		{
			TISpaceFleetState ref_fleet = actorState.ref_fleet;
			StringBuilder stringBuilder = new StringBuilder(Loc.T(new StringBuilder(base.GetType().Name).Append(".description").ToString()));
			TIHabState ref_hab = ref_fleet.ref_hab;
			if (ref_hab != null && ref_hab.CanPartiallyRepairFleet(ref_fleet))
			{
				TIHabState ref_hab2 = ref_fleet.ref_hab;
				if (ref_hab2 != null && !ref_hab2.CanFullyRepairFleet(ref_fleet))
				{
					stringBuilder.Append(Loc.T("RepairFleetOperation.warning"));
				}
			}
			List<TIResourcesCost> list = this.ResourceCostOptions(actorState.ref_faction, target, actorState, false);
			if (list != null && list.Count > 0 && list[0].anyDebit)
			{
				stringBuilder.Append(Loc.T("TIOperationTemplate.FullCost", new object[] { list[0].ToString("Relevant", false, false, actorState.ref_faction, false, FactionResource.None) }));
			}
			return stringBuilder.ToString();
		}
		return base.GetDescription(actorState, target);
	}

	// Token: 0x06000DD9 RID: 3545 RVA: 0x00045378 File Offset: 0x00043578
	public override List<TIResourcesCost> ResourceCostOptions(TIFactionState faction, TIGameState target, TIGameState actor, bool checkCanAfford = true)
	{
		if (actor.isSpaceFleetState)
		{
			List<TIResourcesCost> list = new List<TIResourcesCost>();
			Dictionary<TISpaceShipState, float> dictionary;
			TIResourcesCost tiresourcesCost = RepairFleetOperation.ExpectedCost(actor.ref_fleet, actor.ref_fleet.ref_hab, checkCanAfford, out dictionary);
			list.Add(tiresourcesCost);
			return list;
		}
		return null;
	}

	// Token: 0x06000DDA RID: 3546 RVA: 0x000453B8 File Offset: 0x000435B8
	public static TIResourcesCost ExpectedCost(TISpaceFleetState fleet, TIHabState hab, bool checkAffordability, out Dictionary<TISpaceShipState, float> plannedMagazineRepairsMultiplier)
	{
		TIResourcesCost tiresourcesCost = new TIResourcesCost();
		plannedMagazineRepairsMultiplier = new Dictionary<TISpaceShipState, float>();
		Dictionary<TISpaceShipState, List<ShipSystem>> dictionary = fleet.ships.ToDictionary<TISpaceShipState, TISpaceShipState, List<ShipSystem>>((TISpaceShipState ship) => ship, delegate(TISpaceShipState ship)
		{
			if (!checkAffordability)
			{
				return ship.DamagedSystems();
			}
			return ship.GetAffordableSystemRepairs(fleet.faction, hab);
		});
		for (;;)
		{
			if (!dictionary.Values.Any<List<ShipSystem>>((List<ShipSystem> x) => x.Count > 0))
			{
				break;
			}
			int num = TISpaceShipState.SystemRepairPriority[ShipSystem.None];
			KeyValuePair<TISpaceShipState, ShipSystem> keyValuePair;
			foreach (TISpaceShipState tispaceShipState in dictionary.Keys)
			{
				foreach (ShipSystem shipSystem in dictionary[tispaceShipState])
				{
					if (TISpaceShipState.SystemRepairPriority[shipSystem] < num)
					{
						keyValuePair = new KeyValuePair<TISpaceShipState, ShipSystem>(tispaceShipState, shipSystem);
						num = TISpaceShipState.SystemRepairPriority[shipSystem];
					}
				}
			}
			if (keyValuePair.Value == ShipSystem.None)
			{
				break;
			}
			TISpaceShipState key = keyValuePair.Key;
			TIResourcesCost tiresourcesCost2 = key.SystemRepairCost(keyValuePair.Value, key.faction, hab, false);
			TIResourcesCost tiresourcesCost3 = new TIResourcesCost(tiresourcesCost);
			tiresourcesCost3.SumCostsWithDuration(tiresourcesCost2);
			if (tiresourcesCost3.CanAfford(key.faction, 1f, null, float.PositiveInfinity) || !checkAffordability)
			{
				tiresourcesCost.SumCostsWithDuration(tiresourcesCost2);
			}
			dictionary[key].Remove(keyValuePair.Value);
		}
		Dictionary<TISpaceShipState, List<DamagedShipPartData>> dictionary2 = fleet.ships.ToDictionary<TISpaceShipState, TISpaceShipState, List<DamagedShipPartData>>((TISpaceShipState ship) => ship, delegate(TISpaceShipState ship)
		{
			if (!checkAffordability)
			{
				return ship.damagedParts.ToList<DamagedShipPartData>();
			}
			return ship.GetAffordablePartRepairs(ship.faction, hab);
		});
		for (;;)
		{
			if (!dictionary2.Values.Any<List<DamagedShipPartData>>((List<DamagedShipPartData> x) => x.Count > 0))
			{
				break;
			}
			int num2 = TISpaceShipState.ModuleRepairPriority[ShipModuleSlotType.None];
			KeyValuePair<TISpaceShipState, DamagedShipPartData> keyValuePair2;
			foreach (TISpaceShipState tispaceShipState2 in dictionary2.Keys)
			{
				foreach (DamagedShipPartData damagedShipPartData in dictionary2[tispaceShipState2])
				{
					if (TISpaceShipState.ModuleRepairPriority[damagedShipPartData.module.moduleTemplate.allowedSlots[0]] < num2)
					{
						keyValuePair2 = new KeyValuePair<TISpaceShipState, DamagedShipPartData>(tispaceShipState2, damagedShipPartData);
						num2 = TISpaceShipState.ModuleRepairPriority[damagedShipPartData.module.moduleTemplate.allowedSlots[0]];
					}
				}
			}
			TISpaceShipState key2 = keyValuePair2.Key;
			TIHabState hab2 = hab;
			if (hab2 == null || hab2.CanBuildAndRepairShipPart(keyValuePair2.Value.module.moduleTemplate) || !checkAffordability)
			{
				TIResourcesCost tiresourcesCost4 = key2.PartRepairCost(keyValuePair2.Value.module, key2.faction, hab, false);
				TIResourcesCost tiresourcesCost5 = new TIResourcesCost(tiresourcesCost);
				tiresourcesCost5.SumCostsWithDuration(tiresourcesCost4);
				if (tiresourcesCost5.CanAfford(key2.faction, 1f, null, float.PositiveInfinity) || !checkAffordability)
				{
					if (keyValuePair2.Value.module.moduleTemplate.isUtilityModule && keyValuePair2.Value.module.moduleTemplate.ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.Magazine))
					{
						if (!plannedMagazineRepairsMultiplier.ContainsKey(key2))
						{
							plannedMagazineRepairsMultiplier.Add(key2, 0f);
						}
						Dictionary<TISpaceShipState, float> dictionary3 = plannedMagazineRepairsMultiplier;
						TISpaceShipState tispaceShipState3 = key2;
						dictionary3[tispaceShipState3] += keyValuePair2.Value.module.moduleTemplate.ref_utilityModule.specialModuleValue;
					}
					tiresourcesCost.SumCostsWithDuration(tiresourcesCost4);
				}
			}
			dictionary2[key2].Remove(keyValuePair2.Value);
		}
		foreach (TISpaceShipState tispaceShipState4 in fleet.ships)
		{
			foreach (ArmorFacing armorFacing in tispaceShipState4.armor.Keys)
			{
				if (tispaceShipState4.armor[armorFacing].damaged)
				{
					TIResourcesCost tiresourcesCost6 = tispaceShipState4.ArmorFacingRepairCost(armorFacing, tispaceShipState4.faction, hab, false);
					TIResourcesCost tiresourcesCost7 = new TIResourcesCost(tiresourcesCost);
					tiresourcesCost7.SumCostsWithDuration(tiresourcesCost6);
					if ((tiresourcesCost7 != null && tiresourcesCost7.CanAfford(tispaceShipState4.faction, 1f, null, float.PositiveInfinity)) || !checkAffordability)
					{
						tiresourcesCost.SumCostsWithDuration(tiresourcesCost6);
					}
				}
			}
		}
		TIHabState hab3 = hab;
		float num3 = ((hab3 != null) ? hab3.DaysUntilCanStartRepair() : 0f);
		tiresourcesCost.SetCompletionTime_Days(num3 + tiresourcesCost.completionTime_days);
		return tiresourcesCost;
	}

	// Token: 0x06000DDB RID: 3547 RVA: 0x0004593C File Offset: 0x00043B3C
	public static TIResourcesCost ExpectedRefitShipRepairCost(TISpaceShipState refitShip, TIHabState hab, TIFactionState designingFaction, out Dictionary<TISpaceShipState, int> plannedMagazineRepairs)
	{
		TIResourcesCost tiresourcesCost = new TIResourcesCost();
		plannedMagazineRepairs = new Dictionary<TISpaceShipState, int>();
		Dictionary<TISpaceShipState, List<ShipSystem>> dictionary = new Dictionary<TISpaceShipState, List<ShipSystem>> { 
		{
			refitShip,
			refitShip.GetAffordableSystemRepairs(designingFaction, hab)
		} };
		for (;;)
		{
			if (!dictionary.Values.Any<List<ShipSystem>>((List<ShipSystem> x) => x.Count > 0))
			{
				break;
			}
			int num = TISpaceShipState.SystemRepairPriority[ShipSystem.None];
			KeyValuePair<TISpaceShipState, ShipSystem> keyValuePair;
			foreach (TISpaceShipState tispaceShipState in dictionary.Keys)
			{
				foreach (ShipSystem shipSystem in dictionary[tispaceShipState])
				{
					if (TISpaceShipState.SystemRepairPriority[shipSystem] < num)
					{
						keyValuePair = new KeyValuePair<TISpaceShipState, ShipSystem>(tispaceShipState, shipSystem);
						num = TISpaceShipState.SystemRepairPriority[shipSystem];
					}
				}
			}
			if (keyValuePair.Value == ShipSystem.None)
			{
				break;
			}
			TISpaceShipState key = keyValuePair.Key;
			TIResourcesCost tiresourcesCost2 = key.SystemRepairCost(keyValuePair.Value, designingFaction, hab, false);
			if (tiresourcesCost2.CanAfford(designingFaction, 1f, null, float.PositiveInfinity))
			{
				tiresourcesCost.SumCostsWithDuration(tiresourcesCost2);
			}
			dictionary[key].Remove(keyValuePair.Value);
		}
		Dictionary<TISpaceShipState, List<DamagedShipPartData>> dictionary2 = new Dictionary<TISpaceShipState, List<DamagedShipPartData>> { 
		{
			refitShip,
			refitShip.GetAffordablePartRepairs(designingFaction, hab)
		} };
		for (;;)
		{
			if (!dictionary2.Values.Any<List<DamagedShipPartData>>((List<DamagedShipPartData> x) => x.Count > 0))
			{
				break;
			}
			int num2 = TISpaceShipState.ModuleRepairPriority[ShipModuleSlotType.None];
			KeyValuePair<TISpaceShipState, DamagedShipPartData> keyValuePair2;
			foreach (TISpaceShipState tispaceShipState2 in dictionary2.Keys)
			{
				foreach (DamagedShipPartData damagedShipPartData in dictionary2[tispaceShipState2])
				{
					if (TISpaceShipState.ModuleRepairPriority[damagedShipPartData.module.moduleTemplate.allowedSlots[0]] < num2)
					{
						keyValuePair2 = new KeyValuePair<TISpaceShipState, DamagedShipPartData>(tispaceShipState2, damagedShipPartData);
						num2 = TISpaceShipState.ModuleRepairPriority[damagedShipPartData.module.moduleTemplate.allowedSlots[0]];
					}
				}
			}
			TISpaceShipState key2 = keyValuePair2.Key;
			TIResourcesCost tiresourcesCost3 = key2.PartRepairCost(keyValuePair2.Value.module, designingFaction, hab, false);
			if (hab == null || hab.CanBuildAndRepairShipPart(keyValuePair2.Value.module.moduleTemplate))
			{
				if (keyValuePair2.Value.module.moduleTemplate.isUtilityModule && keyValuePair2.Value.module.moduleTemplate.ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.Magazine))
				{
					if (!plannedMagazineRepairs.ContainsKey(key2))
					{
						plannedMagazineRepairs.Add(key2, 0);
					}
					Dictionary<TISpaceShipState, int> dictionary3 = plannedMagazineRepairs;
					TISpaceShipState tispaceShipState3 = key2;
					dictionary3[tispaceShipState3]++;
				}
				if (tiresourcesCost3.CanAfford(designingFaction, 1f, null, float.PositiveInfinity))
				{
					tiresourcesCost.SumCostsWithDuration(tiresourcesCost3);
				}
			}
			dictionary2[key2].Remove(keyValuePair2.Value);
		}
		foreach (ArmorFacing armorFacing in refitShip.armor.Keys)
		{
			if (refitShip.armor[armorFacing].damaged)
			{
				TIResourcesCost tiresourcesCost4 = refitShip.ArmorFacingRepairCost(armorFacing, designingFaction, hab, false);
				if (tiresourcesCost4 != null && tiresourcesCost4.CanAfford(designingFaction, 1f, null, float.PositiveInfinity))
				{
					tiresourcesCost.SumCostsWithDuration(tiresourcesCost4);
				}
			}
		}
		float num3 = ((hab != null) ? hab.DaysUntilCanStartRepair() : 0f);
		tiresourcesCost.SetCompletionTime_Days(num3 + tiresourcesCost.completionTime_days);
		return tiresourcesCost;
	}

	// Token: 0x06000DDC RID: 3548 RVA: 0x00045D60 File Offset: 0x00043F60
	public TIResourcesCost SetRepair(TISpaceFleetState fleet)
	{
		TIResourcesCost tiresourcesCost = new TIResourcesCost();
		Dictionary<TISpaceShipState, List<ShipSystem>> dictionary = fleet.ships.ToDictionary<TISpaceShipState, TISpaceShipState, List<ShipSystem>>((TISpaceShipState ship) => ship, (TISpaceShipState ship) => ship.GetAffordableSystemRepairs(ship.faction, fleet.ref_hab));
		for (;;)
		{
			if (!dictionary.Values.Any<List<ShipSystem>>((List<ShipSystem> x) => x.Count > 0))
			{
				break;
			}
			int num = TISpaceShipState.SystemRepairPriority[ShipSystem.None];
			KeyValuePair<TISpaceShipState, ShipSystem> keyValuePair;
			foreach (TISpaceShipState tispaceShipState in dictionary.Keys)
			{
				foreach (ShipSystem shipSystem in dictionary[tispaceShipState])
				{
					if (TISpaceShipState.SystemRepairPriority[shipSystem] < num)
					{
						keyValuePair = new KeyValuePair<TISpaceShipState, ShipSystem>(tispaceShipState, shipSystem);
						num = TISpaceShipState.SystemRepairPriority[shipSystem];
					}
				}
			}
			if (keyValuePair.Value == ShipSystem.None)
			{
				break;
			}
			TISpaceShipState key = keyValuePair.Key;
			TIResourcesCost tiresourcesCost2 = key.SystemRepairCost(keyValuePair.Value, key.faction, fleet.ref_hab, false);
			TIResourcesCost tiresourcesCost3 = new TIResourcesCost(tiresourcesCost);
			tiresourcesCost3.SumCostsWithDuration(tiresourcesCost2);
			if (tiresourcesCost3.CanAfford(key.faction, 1f, null, float.PositiveInfinity))
			{
				tiresourcesCost.SumCostsWithDuration(tiresourcesCost2);
				key.plannedResupplyAndRepair.AddSystemToRepair(keyValuePair.Value);
				key.plannedResupplyAndRepair.AddtoRepairCost(tiresourcesCost2);
			}
			dictionary[key].Remove(keyValuePair.Value);
		}
		Dictionary<TISpaceShipState, List<DamagedShipPartData>> dictionary2 = fleet.ships.ToDictionary<TISpaceShipState, TISpaceShipState, List<DamagedShipPartData>>((TISpaceShipState ship) => ship, (TISpaceShipState ship) => ship.GetAffordablePartRepairs(ship.faction, fleet.ref_hab));
		for (;;)
		{
			if (!dictionary2.Values.Any<List<DamagedShipPartData>>((List<DamagedShipPartData> x) => x.Count > 0))
			{
				break;
			}
			int num2 = TISpaceShipState.ModuleRepairPriority[ShipModuleSlotType.None];
			KeyValuePair<TISpaceShipState, DamagedShipPartData> keyValuePair2;
			foreach (TISpaceShipState tispaceShipState2 in dictionary2.Keys)
			{
				foreach (DamagedShipPartData damagedShipPartData in dictionary2[tispaceShipState2])
				{
					if (TISpaceShipState.ModuleRepairPriority[damagedShipPartData.module.moduleTemplate.allowedSlots[0]] < num2)
					{
						keyValuePair2 = new KeyValuePair<TISpaceShipState, DamagedShipPartData>(tispaceShipState2, damagedShipPartData);
						num2 = TISpaceShipState.ModuleRepairPriority[damagedShipPartData.module.moduleTemplate.allowedSlots[0]];
					}
				}
			}
			TISpaceShipState key2 = keyValuePair2.Key;
			if (fleet.ref_hab.CanBuildAndRepairShipPart(keyValuePair2.Value.module.moduleTemplate))
			{
				TIResourcesCost tiresourcesCost4 = key2.PartRepairCost(keyValuePair2.Value.module, key2.faction, fleet.ref_hab, false);
				TIResourcesCost tiresourcesCost5 = new TIResourcesCost(tiresourcesCost);
				tiresourcesCost5.SumCostsWithDuration(tiresourcesCost4);
				if (tiresourcesCost5.CanAfford(key2.faction, 1f, null, float.PositiveInfinity))
				{
					tiresourcesCost.SumCostsWithDuration(tiresourcesCost4);
					key2.plannedResupplyAndRepair.AddModuleToRepair(keyValuePair2.Value);
					key2.plannedResupplyAndRepair.AddtoRepairCost(tiresourcesCost4);
				}
			}
			dictionary2[key2].Remove(keyValuePair2.Value);
		}
		foreach (TISpaceShipState tispaceShipState3 in fleet.ships)
		{
			foreach (ArmorFacing armorFacing in tispaceShipState3.armor.Keys)
			{
				if (tispaceShipState3.armor[armorFacing].damaged)
				{
					TIResourcesCost tiresourcesCost6 = tispaceShipState3.ArmorFacingRepairCost(armorFacing, tispaceShipState3.faction, fleet.ref_hab, false);
					TIResourcesCost tiresourcesCost7 = new TIResourcesCost(tiresourcesCost);
					tiresourcesCost7.SumCostsWithDuration(tiresourcesCost6);
					if (tiresourcesCost7 != null && tiresourcesCost7.CanAfford(tispaceShipState3.faction, 1f, null, float.PositiveInfinity))
					{
						tiresourcesCost.SumCostsWithDuration(tiresourcesCost6);
						tispaceShipState3.plannedResupplyAndRepair.AddArmorFacingToRepair(armorFacing);
						tispaceShipState3.plannedResupplyAndRepair.AddtoRepairCost(tiresourcesCost6);
					}
				}
			}
		}
		float num3 = fleet.ref_hab.DaysUntilCanStartRepair();
		TIDateTime tidateTime = TITimeState.Now();
		tidateTime.AddDays(num3);
		foreach (TISpaceShipState tispaceShipState4 in fleet.ships)
		{
			tispaceShipState4.plannedResupplyAndRepair.SetStartDate(tidateTime);
		}
		tiresourcesCost.SetCompletionTime_Days(num3 + tiresourcesCost.completionTime_days);
		return tiresourcesCost;
	}

	// Token: 0x06000DDD RID: 3549 RVA: 0x000462E4 File Offset: 0x000444E4
	public override bool OnOperationConfirm(TIGameState actorState, TIGameState target, TIResourcesCost resourcesCost = null, Trajectory trajectory = null)
	{
		TIResourcesCost tiresourcesCost = this.SetRepair(actorState.ref_fleet);
		if (!base.OnOperationConfirm(actorState, target, tiresourcesCost, trajectory))
		{
			base.CleanUpFleetRepairData(actorState.ref_fleet);
			return false;
		}
		actorState.ref_faction.RecordExpenditure(TIFactionState.Expenditure.ShipMaintainence, tiresourcesCost);
		return true;
	}

	// Token: 0x06000DDE RID: 3550 RVA: 0x00046328 File Offset: 0x00044528
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		TISpaceFleetState ref_fleet = actorState.ref_fleet;
		if (ref_fleet.dockedAtHab && !ref_fleet.inCombatOrWaitingForCombat && !ref_fleet.transferAssigned && ref_fleet.ref_hab.CanPartiallyRepairFleet(ref_fleet))
		{
			foreach (TISpaceShipState tispaceShipState in ref_fleet.ships)
			{
				tispaceShipState.plannedResupplyAndRepair.ProcessResupplyAndRepair(tispaceShipState);
			}
			TINotificationQueueState.LogOurFleetRepaired(ref_fleet);
			return;
		}
		foreach (TISpaceShipState tispaceShipState2 in ref_fleet.ships)
		{
			tispaceShipState2.plannedResupplyAndRepair.CancelRepair(ref_fleet.faction);
		}
	}

	// Token: 0x06000DDF RID: 3551 RVA: 0x00046400 File Offset: 0x00044600
	public override void OnOperationCancel(TIGameState actorState, TIGameState target, TIDateTime opCompleteDate)
	{
		base.OnOperationCancel(actorState, target, opCompleteDate);
		base.HandlePartialCompletion(actorState.ref_fleet, opCompleteDate);
		actorState.ref_fleet.ships.ForEach(delegate(TISpaceShipState x)
		{
			x.plannedResupplyAndRepair.CancelResupply(actorState.ref_faction);
		});
		actorState.ref_fleet.ships.ForEach(delegate(TISpaceShipState x)
		{
			x.plannedResupplyAndRepair.CancelRepair(actorState.ref_faction);
		});
		float num = (float)opCompleteDate.DifferenceInDays(TITimeState.Now());
		foreach (TISpaceFleetState tispaceFleetState in actorState.ref_fleet.ref_hab.dockedFleets)
		{
			if (tispaceFleetState != actorState.ref_fleet)
			{
				foreach (OperationData operationData in tispaceFleetState.CurrentOperations().ToList<OperationData>())
				{
					if (operationData.operation is RepairFleetOperation && operationData.completionDate > opCompleteDate)
					{
						base.RescheduleFleetOperation(tispaceFleetState, operationData, -num);
					}
				}
			}
		}
	}
}

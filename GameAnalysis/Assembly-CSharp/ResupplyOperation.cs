using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000335 RID: 821
public class ResupplyOperation : FixUpFleetOperation
{
	// Token: 0x06000DC7 RID: 3527 RVA: 0x0004413B File Offset: 0x0004233B
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return actorState.ref_fleet.NeedsRefuel() || actorState.ref_fleet.NeedsRearm();
	}

	// Token: 0x06000DC8 RID: 3528 RVA: 0x00044157 File Offset: 0x00042357
	public override int SortOrder()
	{
		return 8;
	}

	// Token: 0x06000DC9 RID: 3529 RVA: 0x0004415C File Offset: 0x0004235C
	public override string GetDescription(TIGameState actorState = null, TIGameState target = null)
	{
		if (actorState != null)
		{
			List<TIResourcesCost> list = this.ResourceCostOptions(actorState.ref_faction, target, actorState, false);
			if (list != null && list.Count > 0 && list[0].anyDebit)
			{
				StringBuilder stringBuilder = new StringBuilder(Loc.T(new StringBuilder(base.GetType().Name).Append(".description").ToString()));
				stringBuilder.Append(Loc.T("TIOperationTemplate.FullCost", new object[] { list[0].ToString("Relevant", false, false, actorState.ref_faction, false, FactionResource.None) }));
				return stringBuilder.ToString();
			}
		}
		return base.GetDescription(actorState, target);
	}

	// Token: 0x06000DCA RID: 3530 RVA: 0x00044210 File Offset: 0x00042410
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		TISpaceFleetState fleet = actorState.ref_fleet;
		if (!fleet.transferAssigned)
		{
			if (fleet.dockedAtHab && fleet.ref_hab.AllowsResupply(fleet.faction, false, false) && !fleet.inCombatOrWaitingForCombat && (fleet.CanAffordAnyPropellant(fleet.ref_hab.faction) || fleet.CanAffordAnyReloading(fleet.ref_hab)))
			{
				return base.ActorCanPerformOperation_PassInterruptCheck(actorState);
			}
			if (fleet.NeedsRefuel())
			{
				if (fleet.ships.Where<TISpaceShipState>((TISpaceShipState x) => x.CanRefuelFromHabSite(fleet.ref_habSite)).Intersect<TISpaceShipState>(fleet.ships.Where<TISpaceShipState>((TISpaceShipState x) => x.NeedsRefuel())).Any<TISpaceShipState>())
				{
					return base.ActorCanPerformOperation_PassInterruptCheck(actorState);
				}
				if (fleet.ships.Where<TISpaceShipState>((TISpaceShipState x) => x.CanRefuelFromJovianAtmosphere()).Intersect<TISpaceShipState>(fleet.ships.Where<TISpaceShipState>((TISpaceShipState x) => x.NeedsRefuel())).Any<TISpaceShipState>())
				{
					return base.ActorCanPerformOperation_PassInterruptCheck(actorState);
				}
			}
		}
		return false;
	}

	// Token: 0x06000DCB RID: 3531 RVA: 0x00044398 File Offset: 0x00042598
	public override List<TIResourcesCost> ResourceCostOptions(TIFactionState faction, TIGameState target, TIGameState actor, bool checkCanAfford = true)
	{
		if (actor.isSpaceFleetState)
		{
			bool flag;
			return new List<TIResourcesCost>(1) { this.PlanResupply(actor.ref_fleet, true, out flag, null, null, checkCanAfford) };
		}
		return null;
	}

	// Token: 0x06000DCC RID: 3532 RVA: 0x000443D0 File Offset: 0x000425D0
	public static TIResourcesCost ExpectedShipRefuelCost(TISpaceShipState ship, TIFactionState faction, TISpaceShipTemplate refitTemplate)
	{
		TIResourcesCost tiresourcesCost = new TIResourcesCost();
		float num;
		if (refitTemplate.propellantTanks < ship.template.propellantTanks)
		{
			num = refitTemplate.propellantMass_tons - ship.propellant_tons;
			if (num < 0f)
			{
				num = 0f;
			}
		}
		else
		{
			num = ship.PropellantShortage_tons;
		}
		int num2 = Mathf.CeilToInt(num / 100f);
		for (int i = 0; i < num2; i++)
		{
			TIResourcesCost preferredPropellantTankCost = ship.GetPreferredPropellantTankCost(faction, num, false);
			tiresourcesCost.SumCosts_NoDuration(preferredPropellantTankCost);
			num -= 100f;
		}
		return tiresourcesCost;
	}

	// Token: 0x06000DCD RID: 3533 RVA: 0x00044450 File Offset: 0x00042650
	private float RefuelTankAtHabDuration(TISpaceShipState ship, TIHabState hab)
	{
		return (TemplateManager.global.daysToRefuelAPropellantTank + ship.SumOfficerEffectsModifiers(OfficerEffectType.DockResupplySpeed, TemplateManager.global.daysToRefuelAPropellantTank)) / (float)hab.ResupplySpeedDivisor();
	}

	// Token: 0x06000DCE RID: 3534 RVA: 0x00044477 File Offset: 0x00042677
	private float RearmWeaponAtHabDuration(TISpaceShipState ship, TIHabState hab)
	{
		return (TemplateManager.global.daysToReloadAShipWeaponStep + ship.SumOfficerEffectsModifiers(OfficerEffectType.DockResupplySpeed, TemplateManager.global.daysToReloadAShipWeaponStep)) / (float)hab.ResupplySpeedDivisor();
	}

	// Token: 0x06000DCF RID: 3535 RVA: 0x000444A0 File Offset: 0x000426A0
	public TIResourcesCost PlanResupply(TISpaceFleetState fleet, bool prospectiveOnly, out bool freeRefueling, Dictionary<TISpaceShipState, float> pendingRepairedMagazines = null, TIResourcesCost committedRepairCost = null, bool checkCanAfford = true)
	{
		TIResourcesCost tiresourcesCost;
		if (committedRepairCost != null)
		{
			tiresourcesCost = new TIResourcesCost(committedRepairCost);
		}
		else
		{
			tiresourcesCost = new TIResourcesCost();
		}
		TIHabState ref_hab = fleet.ref_hab;
		TIFactionState tifactionState = ((ref_hab != null) ? ref_hab.faction : null) ?? null;
		float num = ((tifactionState == fleet.faction || tifactionState == null) ? 1f : (1f / (float)fleet.ref_hab.faction.habs.Count));
		List<TISpaceShipState> list = (from x in fleet.ships
			where x.NeedsRefuel()
			select x into y
			orderby y.currentDeltaV_kps
			select y).ToList<TISpaceShipState>();
		TIHabState ref_hab2 = fleet.ref_hab;
		if (ref_hab2 != null && ref_hab2.AllowsResupply(fleet.faction, false, false))
		{
			Dictionary<TISpaceShipState, float> dictionary = fleet.ships.ToDictionary<TISpaceShipState, TISpaceShipState, float>((TISpaceShipState x) => x, (TISpaceShipState y) => y.PropellantShortage_tons);
			while (list.Count > 0)
			{
				List<TISpaceShipState> list2 = new List<TISpaceShipState>();
				foreach (TISpaceShipState tispaceShipState in list)
				{
					float num2 = this.RefuelTankAtHabDuration(tispaceShipState, tispaceShipState.ref_hab);
					TIResourcesCost tiresourcesCost2 = new TIResourcesCost();
					float num3 = Mathf.Min(dictionary[tispaceShipState], 100f);
					if (num3 > 0f)
					{
						TIResourcesCost tiresourcesCost3 = tispaceShipState.GetPreferredPropellantTankCost(fleet.ref_hab.faction, num3, false);
						tiresourcesCost3.SetCompletionTime_Days(Mathf.Max(0.01f, num2 * num3 / 100f));
						TIResourcesCost tiresourcesCost4 = new TIResourcesCost(tiresourcesCost);
						tiresourcesCost4.SumCosts_NoDuration(tiresourcesCost3);
						bool flag = false;
						if (!checkCanAfford || tiresourcesCost4.CanAfford(tifactionState, num, null, float.PositiveInfinity))
						{
							flag = true;
						}
						else if (fleet.AllowUseBoostForRepairsResupply)
						{
							tiresourcesCost3 = TISpaceShipTemplate.MixedResourceConstructionCost(fleet.faction, fleet.ref_hab, tiresourcesCost3, fleet.faction.AvailableSpaceResourcesExcept(1f, tiresourcesCost), false);
							tiresourcesCost3.SetCompletionTime_Days(Mathf.Max(0.01f, num2 * num3 / 100f));
							tiresourcesCost4 = new TIResourcesCost(tiresourcesCost);
							tiresourcesCost4.SumCosts_NoDuration(tiresourcesCost3);
							if (tiresourcesCost4.CanAfford(tifactionState, num, null, float.PositiveInfinity))
							{
								flag = true;
							}
						}
						if (flag)
						{
							if (!prospectiveOnly)
							{
								tispaceShipState.plannedResupplyAndRepair.AddPropellantToReload(num3);
							}
							Dictionary<TISpaceShipState, float> dictionary2 = dictionary;
							TISpaceShipState tispaceShipState2 = tispaceShipState;
							dictionary2[tispaceShipState2] -= num3;
							tiresourcesCost2.SumCostsWithDuration(tiresourcesCost3);
							tiresourcesCost.SumCostsWithDuration(tiresourcesCost3);
						}
						else
						{
							list2.Add(tispaceShipState);
						}
					}
					else
					{
						list2.Add(tispaceShipState);
					}
					if (!prospectiveOnly)
					{
						tispaceShipState.plannedResupplyAndRepair.AddtoResupplyCost(tiresourcesCost2);
					}
				}
				foreach (TISpaceShipState tispaceShipState3 in list2)
				{
					list.Remove(tispaceShipState3);
				}
			}
			if (fleet.ref_hab.faction == fleet.faction)
			{
				Dictionary<TISpaceShipState, Dictionary<ModuleDataEntry, int>> dictionary3 = fleet.ships.ToDictionary<TISpaceShipState, TISpaceShipState, Dictionary<ModuleDataEntry, int>>((TISpaceShipState sh) => sh, (TISpaceShipState weaps) => weaps.AllWeaponModuleData().ToDictionary<ModuleDataEntry, ModuleDataEntry, int>((ModuleDataEntry mod) => mod, (ModuleDataEntry ammoNeeded) => 0));
				using (List<TISpaceShipState>.Enumerator enumerator = fleet.ships.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TISpaceShipState tispaceShipState4 = enumerator.Current;
						foreach (ModuleDataEntry moduleDataEntry in tispaceShipState4.AllWeaponModuleData())
						{
							if (pendingRepairedMagazines != null && pendingRepairedMagazines.ContainsKey(tispaceShipState4))
							{
								dictionary3[tispaceShipState4][moduleDataEntry] = ((moduleDataEntry.moduleTemplate.ref_projectileWeapon == null || !moduleDataEntry.moduleTemplate.ref_projectileWeapon.hasMagazine()) ? 0 : (moduleDataEntry.moduleTemplate.ref_projectileWeapon.FullAmmoCount_PendingRepairs(tispaceShipState4, pendingRepairedMagazines[tispaceShipState4]) - tispaceShipState4.ammo[moduleDataEntry]));
							}
							else
							{
								dictionary3[tispaceShipState4][moduleDataEntry] = ((moduleDataEntry.moduleTemplate.ref_projectileWeapon == null || !moduleDataEntry.moduleTemplate.ref_projectileWeapon.hasMagazine()) ? 0 : (moduleDataEntry.moduleTemplate.ref_projectileWeapon.FullAmmoCount_Current(tispaceShipState4) - tispaceShipState4.ammo[moduleDataEntry]));
							}
						}
					}
					goto IL_070D;
				}
				IL_051B:
				foreach (TISpaceShipState tispaceShipState5 in fleet.ships)
				{
					TIResourcesCost tiresourcesCost5 = new TIResourcesCost();
					float num4 = this.RearmWeaponAtHabDuration(tispaceShipState5, tispaceShipState5.ref_hab);
					foreach (ModuleDataEntry moduleDataEntry2 in tispaceShipState5.AllWeaponModuleData())
					{
						if (dictionary3[tispaceShipState5][moduleDataEntry2] > 0)
						{
							int num5 = Mathf.Min(50, dictionary3[tispaceShipState5][moduleDataEntry2]);
							TIResourcesCost tiresourcesCost6 = tispaceShipState5.CostToReloadPartialAmmo(moduleDataEntry2, num5, fleet.ref_hab, false);
							tiresourcesCost6.SetCompletionTime_Days(num4);
							if (tiresourcesCost6 != null)
							{
								TIResourcesCost tiresourcesCost7 = new TIResourcesCost(tiresourcesCost);
								tiresourcesCost7.SumCosts_NoDuration(tiresourcesCost6);
								bool flag2 = false;
								if (!checkCanAfford || tiresourcesCost7.CanAfford(tifactionState, 1f, null, float.PositiveInfinity))
								{
									flag2 = true;
								}
								else if (fleet.AllowUseBoostForRepairsResupply)
								{
									tiresourcesCost6 = TISpaceShipTemplate.MixedResourceConstructionCost(tifactionState, tispaceShipState5.ref_hab, tiresourcesCost6, tifactionState.AvailableSpaceResourcesExcept(1f, tiresourcesCost), false);
									tiresourcesCost6.SetCompletionTime_Days(num4);
									tiresourcesCost7 = new TIResourcesCost(tiresourcesCost);
									tiresourcesCost7.SumCosts_NoDuration(tiresourcesCost6);
									if (tiresourcesCost7.CanAfford(tifactionState, num, null, float.PositiveInfinity))
									{
										flag2 = true;
									}
								}
								if (flag2)
								{
									if (!prospectiveOnly)
									{
										tispaceShipState5.plannedResupplyAndRepair.AddAmmoOrder(moduleDataEntry2, num5);
									}
									tiresourcesCost.SumCostsWithDuration(tiresourcesCost6);
									tiresourcesCost5.SumCostsWithDuration(tiresourcesCost6);
									Dictionary<ModuleDataEntry, int> dictionary4 = dictionary3[tispaceShipState5];
									ModuleDataEntry moduleDataEntry3 = moduleDataEntry2;
									dictionary4[moduleDataEntry3] -= 50;
								}
								else
								{
									dictionary3[tispaceShipState5][moduleDataEntry2] = 0;
								}
							}
							else
							{
								dictionary3[tispaceShipState5][moduleDataEntry2] = 0;
							}
						}
					}
					if (!prospectiveOnly)
					{
						tispaceShipState5.plannedResupplyAndRepair.AddtoResupplyCost(tiresourcesCost5);
					}
				}
				IL_070D:
				if (dictionary3.Values.Any<Dictionary<ModuleDataEntry, int>>((Dictionary<ModuleDataEntry, int> x) => x.Values.Any<int>((int unHandledAmmo) => unHandledAmmo > 0)))
				{
					goto IL_051B;
				}
			}
			float num6 = fleet.ref_hab.DaysUntilCanStartResupply();
			if (!prospectiveOnly)
			{
				TIDateTime tidateTime = TITimeState.Now();
				tidateTime.AddDays(num6);
				foreach (TISpaceShipState tispaceShipState6 in fleet.ships)
				{
					tispaceShipState6.plannedResupplyAndRepair.SetStartDate(tidateTime);
				}
			}
			tiresourcesCost.AddToCompletionTime_Days(num6);
		}
		List<TISpaceShipState> list3 = list.Where<TISpaceShipState>((TISpaceShipState x) => x.CanRefuelFromJovianAtmosphere() || x.CanRefuelFromHabSite(fleet.ref_habSite)).ToList<TISpaceShipState>();
		freeRefueling = list3.Count > 0;
		TIResourcesCost tiresourcesCost8 = new TIResourcesCost();
		if (freeRefueling)
		{
			Func<FactionResource, float> <>9__11;
			Func<KeyValuePair<FactionResource, float>, float> <>9__12;
			foreach (TISpaceShipState tispaceShipState7 in list3)
			{
				if (fleet.landed)
				{
					float num7;
					float num8;
					if (tispaceShipState7.drive.propellant == Propellant.Anything)
					{
						IEnumerable<FactionResource> basicSpaceResources = TIResourcesCost.basicSpaceResources;
						Func<FactionResource, float> func;
						if ((func = <>9__11) == null)
						{
							func = (<>9__11 = (FactionResource x) => fleet.ref_habSite.GetDailyProduction(x) * 3f);
						}
						num7 = basicSpaceResources.Sum<FactionResource>(func) / TemplateManager.global.spaceResourceToTons;
						num8 = tispaceShipState7.PropellantShortage_tons;
					}
					else
					{
						IEnumerable<KeyValuePair<FactionResource, float>> enumerable = tispaceShipState7.drive.GetPerTankPropellantMaterials(tispaceShipState7.faction).ToRVCollection(1f);
						Func<KeyValuePair<FactionResource, float>, float> func2;
						if ((func2 = <>9__12) == null)
						{
							func2 = (<>9__12 = (KeyValuePair<FactionResource, float> x) => fleet.ref_habSite.GetDailyProduction(x.Key) / x.Value);
						}
						FactionResource key = enumerable.MinBy<KeyValuePair<FactionResource, float>, float>(func2).Key;
						num7 = fleet.ref_habSite.GetDailyProduction(key) / TemplateManager.global.spaceResourceToTons;
						num8 = tispaceShipState7.drive.GetPerTankPropellantMaterials(tispaceShipState7.faction).ToResourcesCost(tispaceShipState7.PropellantShortage_tons).GetSingleCostValue(key);
					}
					float num9 = num8 / num7;
					if (num9 < 90f)
					{
						if (!prospectiveOnly)
						{
							tispaceShipState7.plannedResupplyAndRepair.AddPropellantToReload(tispaceShipState7.PropellantShortage_tons);
							tispaceShipState7.plannedResupplyAndRepair.SetStartDate(TITimeState.Now());
							tispaceShipState7.plannedResupplyAndRepair.resupplyCost.SetCompletionTime_Days(Mathf.Max(tispaceShipState7.plannedResupplyAndRepair.duration_days, num9));
						}
					}
					else
					{
						num9 = 90f;
						if (!prospectiveOnly)
						{
							tispaceShipState7.plannedResupplyAndRepair.AddPropellantToReload(num7 * num9);
							tispaceShipState7.plannedResupplyAndRepair.SetStartDate(TITimeState.Now());
							tispaceShipState7.plannedResupplyAndRepair.resupplyCost.SetCompletionTime_Days(Mathf.Max(tispaceShipState7.plannedResupplyAndRepair.duration_days, num9));
						}
					}
					tiresourcesCost8.SetCompletionTime_Days(Mathf.Max(tiresourcesCost8.completionTime_days, num9));
				}
				else
				{
					float num10 = Mathf.Max(tispaceShipState7.PropellantShortage_tons / 1000f, 10f);
					tiresourcesCost8.SetCompletionTime_Days(Mathf.Max(tiresourcesCost8.completionTime_days, num10));
					if (!prospectiveOnly)
					{
						tispaceShipState7.plannedResupplyAndRepair.AddPropellantToReload(tispaceShipState7.PropellantShortage_tons);
						tispaceShipState7.plannedResupplyAndRepair.SetStartDate(TITimeState.Now());
						tispaceShipState7.plannedResupplyAndRepair.resupplyCost.SetCompletionTime_Days(Mathf.Max(tispaceShipState7.plannedResupplyAndRepair.duration_days, num10));
					}
				}
				list.Remove(tispaceShipState7);
			}
			tiresourcesCost.SetCompletionTime_Days(Mathf.Max(tiresourcesCost.completionTime_days, tiresourcesCost8.completionTime_days));
		}
		return tiresourcesCost;
	}

	// Token: 0x06000DD0 RID: 3536 RVA: 0x00044FE8 File Offset: 0x000431E8
	public override bool OnOperationConfirm(TIGameState actorState, TIGameState target, TIResourcesCost resourcesCost = null, Trajectory trajectory = null)
	{
		bool flag;
		TIResourcesCost tiresourcesCost = this.PlanResupply(actorState.ref_fleet, false, out flag, null, null, true);
		if (!base.OnOperationConfirm(actorState, target, tiresourcesCost, trajectory))
		{
			base.CleanUpFleetRepairData(actorState.ref_fleet);
			return false;
		}
		actorState.ref_faction.RecordExpenditure(TIFactionState.Expenditure.ShipMaintainence, tiresourcesCost);
		return true;
	}

	// Token: 0x06000DD1 RID: 3537 RVA: 0x00045034 File Offset: 0x00043234
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		TISpaceFleetState ref_fleet = actorState.ref_fleet;
		bool flag = ref_fleet.ships.Any<TISpaceShipState>((TISpaceShipState x) => x.CanRefuelFromJovianAtmosphere() && x.plannedResupplyAndRepair.propellantToReload > 0f);
		if (!ref_fleet.transferAssigned && (ref_fleet.dockedOrLanded || flag) && !ref_fleet.inCombatOrWaitingForCombat)
		{
			foreach (TISpaceShipState tispaceShipState in ref_fleet.ships)
			{
				tispaceShipState.plannedResupplyAndRepair.ProcessResupplyAndRepair(tispaceShipState);
			}
			TINotificationQueueState.LogOurFleetRefueled(ref_fleet, false);
			if (ref_fleet.ref_hab != null && ref_fleet.ref_hab.faction != ref_fleet.faction && ref_fleet.ref_hab.AllowsResupply(ref_fleet.ref_hab.faction, true, false))
			{
				ref_fleet.ref_hab.faction.GainFactionHate(ref_fleet.faction, 1f, false, "Fuel robbed", true);
				TINotificationQueueState.LogFleetStoleOurFuel(ref_fleet, ref_fleet.ref_hab);
				return;
			}
		}
		else
		{
			foreach (TISpaceShipState tispaceShipState2 in ref_fleet.ships)
			{
				tispaceShipState2.plannedResupplyAndRepair.CancelResupply(ref_fleet.faction);
			}
		}
	}

	// Token: 0x06000DD2 RID: 3538 RVA: 0x000451A8 File Offset: 0x000433A8
	public override void OnOperationCancel(TIGameState actorState, TIGameState target, TIDateTime opCompleteDate)
	{
		base.OnOperationCancel(actorState, target, opCompleteDate);
		base.HandlePartialCompletion(actorState.ref_fleet, opCompleteDate);
		actorState.ref_fleet.ships.ForEach(delegate(TISpaceShipState x)
		{
			x.plannedResupplyAndRepair.CancelResupply(actorState.ref_faction);
		});
	}

	// Token: 0x04000EB7 RID: 3767
	private const float MinRefuelTime_days = 0.01f;
}

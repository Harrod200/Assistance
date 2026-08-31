using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000337 RID: 823
public class ResupplyAndRepairOperation : RepairFleetOperation
{
	// Token: 0x06000DE1 RID: 3553 RVA: 0x00046560 File Offset: 0x00044760
	public override int SortOrder()
	{
		return 7;
	}

	// Token: 0x06000DE2 RID: 3554 RVA: 0x00046563 File Offset: 0x00044763
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return this.resupply.OpVisibleToActor(actorState, targetState) && this.repair.OpVisibleToActor(actorState, targetState);
	}

	// Token: 0x06000DE3 RID: 3555 RVA: 0x00046583 File Offset: 0x00044783
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return this.resupply.ActorCanPerformOperation(actorState, target) && this.repair.ActorCanPerformOperation(actorState, target) && this.ResourceCostOptions(actorState.ref_faction, target, actorState, true).Any<TIResourcesCost>();
	}

	// Token: 0x06000DE4 RID: 3556 RVA: 0x000465BC File Offset: 0x000447BC
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
				return stringBuilder.ToString();
			}
		}
		return base.GetDescription(actorState, target);
	}

	// Token: 0x06000DE5 RID: 3557 RVA: 0x000466B4 File Offset: 0x000448B4
	public override List<TIResourcesCost> ResourceCostOptions(TIFactionState faction, TIGameState target, TIGameState actor, bool checkCanAfford = true)
	{
		TISpaceFleetState ref_fleet = actor.ref_fleet;
		TIResourcesCost tiresourcesCost = new TIResourcesCost();
		Dictionary<TISpaceShipState, float> dictionary = new Dictionary<TISpaceShipState, float>();
		if (this.repair.ActorCanPerformOperation(ref_fleet, ref_fleet))
		{
			TIResourcesCost tiresourcesCost2 = RepairFleetOperation.ExpectedCost(ref_fleet, ref_fleet.ref_hab, checkCanAfford, out dictionary);
			tiresourcesCost.SumCostsWithDuration(tiresourcesCost2);
		}
		if (this.resupply.ActorCanPerformOperation(ref_fleet, ref_fleet))
		{
			bool flag;
			TIResourcesCost tiresourcesCost3 = this.resupply.PlanResupply(ref_fleet, true, out flag, dictionary, checkCanAfford ? tiresourcesCost : null, checkCanAfford);
			if (checkCanAfford)
			{
				tiresourcesCost = new TIResourcesCost(tiresourcesCost3);
			}
			else
			{
				tiresourcesCost.SumCostsWithDuration(tiresourcesCost3);
			}
		}
		tiresourcesCost.SetCompletionTime_Days(tiresourcesCost.completionTime_days);
		if (checkCanAfford && !tiresourcesCost.CanAfford(faction, 1f, null, float.PositiveInfinity))
		{
			return new List<TIResourcesCost>();
		}
		return new List<TIResourcesCost> { tiresourcesCost };
	}

	// Token: 0x06000DE6 RID: 3558 RVA: 0x00046774 File Offset: 0x00044974
	public override bool OnOperationConfirm(TIGameState actorState, TIGameState target, TIResourcesCost resourcesCost = null, Trajectory trajectory = null)
	{
		ResupplyAndRepairOperation.<>c__DisplayClass7_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.fleet = actorState.ref_fleet;
		CS$<>8__locals1.cost = new TIResourcesCost();
		CS$<>8__locals1.freeRefueling = false;
		CS$<>8__locals1.checkBase = false;
		CS$<>8__locals1.repairedMagazinesMultiplier = new Dictionary<TISpaceShipState, float>();
		this.<OnOperationConfirm>g__PrepRepair|7_0(ref CS$<>8__locals1);
		this.<OnOperationConfirm>g__PrepResupply|7_1(ref CS$<>8__locals1);
		if (!(CS$<>8__locals1.checkBase | CS$<>8__locals1.freeRefueling))
		{
			return false;
		}
		if (!base.OnOperationConfirm_Base(actorState, target, CS$<>8__locals1.cost, trajectory))
		{
			base.CleanUpFleetRepairData(actorState.ref_fleet);
			return false;
		}
		return true;
	}

	// Token: 0x06000DE8 RID: 3560 RVA: 0x00046820 File Offset: 0x00044A20
	[CompilerGenerated]
	private void <OnOperationConfirm>g__PrepRepair|7_0(ref ResupplyAndRepairOperation.<>c__DisplayClass7_0 A_1)
	{
		if (this.repair.ActorCanPerformOperation(A_1.fleet, A_1.fleet))
		{
			A_1.cost.SumCostsWithDuration(this.repair.SetRepair(A_1.fleet));
			A_1.checkBase = true;
			foreach (TISpaceShipState tispaceShipState in A_1.fleet.ships)
			{
				if (tispaceShipState.plannedResupplyAndRepair.active)
				{
					float num = tispaceShipState.plannedResupplyAndRepair.modulesToRepair.Where<DamagedShipPartData>(delegate(DamagedShipPartData x)
					{
						if (x.module.moduleTemplate.isUtilityModule)
						{
							TIUtilityModuleTemplate ref_utilityModule = x.module.moduleTemplate.ref_utilityModule;
							return ref_utilityModule != null && ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.Magazine);
						}
						return false;
					}).Sum<DamagedShipPartData>((DamagedShipPartData x) => x.module.moduleTemplate.ref_utilityModule.specialModuleValue);
					if (num > 0f)
					{
						A_1.repairedMagazinesMultiplier.Add(tispaceShipState, num);
					}
				}
			}
		}
	}

	// Token: 0x06000DE9 RID: 3561 RVA: 0x0004692C File Offset: 0x00044B2C
	[CompilerGenerated]
	private void <OnOperationConfirm>g__PrepResupply|7_1(ref ResupplyAndRepairOperation.<>c__DisplayClass7_0 A_1)
	{
		if (this.resupply.ActorCanPerformOperation(A_1.fleet, A_1.fleet))
		{
			A_1.cost = this.resupply.PlanResupply(A_1.fleet, false, out A_1.freeRefueling, A_1.repairedMagazinesMultiplier, A_1.cost, true);
			A_1.checkBase = true;
		}
	}

	// Token: 0x04000EB8 RID: 3768
	private ResupplyOperation resupply = new ResupplyOperation();

	// Token: 0x04000EB9 RID: 3769
	private RepairFleetOperation repair = new RepairFleetOperation();
}

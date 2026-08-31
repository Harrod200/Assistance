using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200032E RID: 814
public abstract class BombardOperation : TISpaceFleetOperationTemplate
{
	// Token: 0x06000D88 RID: 3464
	public abstract float bombardmentAltitude_km(TISpaceBodyState targetBody);

	// Token: 0x06000D89 RID: 3465 RVA: 0x0004371E File Offset: 0x0004191E
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.EffectWithDuration;
	}

	// Token: 0x06000D8A RID: 3466 RVA: 0x00043721 File Offset: 0x00041921
	public override bool IsBlockingOperation()
	{
		return true;
	}

	// Token: 0x06000D8B RID: 3467 RVA: 0x00043724 File Offset: 0x00041924
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return !actorState.ref_fleet.transferAssigned && actorState.ref_fleet.orbitState.barycenter.isSpaceBodyState && actorState.ref_fleet.BombardmentValue(actorState.ref_fleet.ref_spaceBody) > 0f;
	}

	// Token: 0x06000D8C RID: 3468 RVA: 0x00043774 File Offset: 0x00041974
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return 14f;
	}

	// Token: 0x06000D8D RID: 3469 RVA: 0x0004377B File Offset: 0x0004197B
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Bombardment);
	}

	// Token: 0x06000D8E RID: 3470 RVA: 0x00043787 File Offset: 0x00041987
	public override bool ExecuteUponCancel()
	{
		return true;
	}

	// Token: 0x06000D8F RID: 3471 RVA: 0x0004378A File Offset: 0x0004198A
	public override bool Repeatable()
	{
		return true;
	}

	// Token: 0x06000D90 RID: 3472 RVA: 0x0004378D File Offset: 0x0004198D
	public override bool CanCancel()
	{
		return true;
	}

	// Token: 0x06000D91 RID: 3473 RVA: 0x00043790 File Offset: 0x00041990
	public override bool CancelUponCombat()
	{
		return true;
	}

	// Token: 0x06000D92 RID: 3474 RVA: 0x00043793 File Offset: 0x00041993
	public override bool MustAcceptCombat()
	{
		return true;
	}

	// Token: 0x06000D93 RID: 3475 RVA: 0x00043796 File Offset: 0x00041996
	public override bool WarnTarget(TIGameState target)
	{
		return target.isHabState && target.ref_hab.AtrocitiesFromDestruction() > 0;
	}

	// Token: 0x06000D94 RID: 3476 RVA: 0x000437B0 File Offset: 0x000419B0
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		TISpaceFleetState ref_fleet = actorState.ref_fleet;
		return !ref_fleet.transferAssigned && !ref_fleet.dockedOrLanded && ref_fleet.orbitState.barycenter.isSpaceBodyState && ref_fleet.orbitState.interfaceOrbit && !ref_fleet.orbitState.template.synch && !ref_fleet.inCombatOrWaitingForCombat && actorState.ref_fleet.BombardmentValue(actorState.ref_fleet.ref_spaceBody) > 0f && base.ActorCanPerformOperation_PassInterruptCheck(actorState) && this.GetPossibleTargets(actorState, null).Count > 0 && (target == null || !target.archived);
	}

	// Token: 0x06000D95 RID: 3477 RVA: 0x00043860 File Offset: 0x00041A60
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		TISpaceFleetState fleet = actorState.ref_fleet;
		TIOrbitState orbitState = fleet.orbitState;
		TISpaceBodyState ref_spaceBody = orbitState.ref_spaceBody;
		TIFactionState bombingFaction = actorState.ref_faction;
		List<TIGameState> list = new List<TIGameState>();
		if (orbitState.ref_spaceBody.isEarth)
		{
			bool flag = !bombingFaction.permanentAlly(GameStateManager.AlienFaction()) && (float)GameStateManager.AlienNation().regions.Count >= (float)GameStateManager.AllRegions().Length * 0.9f;
			List<TIRegionState> list2 = new List<TIRegionState>();
			List<TIRegionState> list3 = new List<TIRegionState>();
			if (flag || bombingFaction.IsAlienFaction)
			{
				list3 = GameStateManager.AllRegions().ToList<TIRegionState>();
			}
			else
			{
				Func<TINationState, bool> <>9__0;
				Func<TINationState, bool> <>9__1;
				foreach (TINationState tinationState in GameStateManager.AllExtantNations())
				{
					if (tinationState.FactionHasControlPoint(fleet.faction))
					{
						goto IL_0107;
					}
					IEnumerable<TINationState> allies = tinationState.allies;
					Func<TINationState, bool> func;
					if ((func = <>9__0) == null)
					{
						func = (<>9__0 = (TINationState x) => x.FactionHasControlPoint(fleet.faction));
					}
					if (allies.Any<TINationState>(func))
					{
						goto IL_0107;
					}
					IL_0114:
					IEnumerable<TINationState> wars = tinationState.wars;
					Func<TINationState, bool> func2;
					if ((func2 = <>9__1) == null)
					{
						func2 = (<>9__1 = (TINationState x) => x.FactionHasControlPoint(fleet.faction));
					}
					if (wars.Any<TINationState>(func2))
					{
						list3.AddRangeUnique<TIRegionState>(tinationState.regions);
						continue;
					}
					continue;
					IL_0107:
					list2.AddRangeUnique<TIRegionState>(tinationState.regions);
					goto IL_0114;
				}
			}
			Func<TIArmyState, bool> <>9__2;
			foreach (TIRegionState tiregionState in list3)
			{
				List<TIGameState> list4 = list;
				IEnumerable<TIArmyState> armies = tiregionState.armies;
				Func<TIArmyState, bool> func3;
				if ((func3 = <>9__2) == null)
				{
					func3 = (<>9__2 = (TIArmyState x) => !bombingFaction.permanentAlly(x.faction));
				}
				list4.AddRange(armies.Where<TIArmyState>(func3));
				list.AddRange(tiregionState.spaceFacilities.Where<TIRegionSpaceFacilityState>((TIRegionSpaceFacilityState x) => x.Extant()));
				if (tiregionState.xenoforming.VisibleToFaction(bombingFaction))
				{
					list.Add(tiregionState.xenoforming);
				}
				list.Add(tiregionState);
			}
			using (List<TIRegionState>.Enumerator enumerator2 = list2.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					TIRegionState region = enumerator2.Current;
					if (!bombingFaction.IsAlienFaction)
					{
						list.AddRange(region.armies.Where<TIArmyState>((TIArmyState x) => x.AlienMegafaunaArmy));
					}
					list.AddRange(region.armies.Where<TIArmyState>((TIArmyState x) => !x.AlienMegafaunaArmy && x.homeNation.wars.Contains(region.nation)));
					if (!bombingFaction.IsAlienFaction && region.xenoforming.VisibleToFaction(bombingFaction))
					{
						list.Add(region.xenoforming);
					}
				}
			}
			if (!bombingFaction.permanentAlly(GameStateManager.AlienFaction()))
			{
				list.AddRange(bombingFaction.KnownAlienFacilities);
				list.AddRange(bombingFaction.KnownUFOLandings);
			}
		}
		else
		{
			Func<TIFactionState, bool> <>9__7;
			Func<TISpaceFleetState, bool> <>9__6;
			foreach (TIHabSiteState tihabSiteState in ref_spaceBody.habSites)
			{
				if (tihabSiteState.hasPlannedOrOperatingBase)
				{
					IEnumerable<TIFactionState> ref_factions = tihabSiteState.hab.ref_factions;
					Func<TIFactionState, bool> func4;
					if ((func4 = <>9__7) == null)
					{
						func4 = (<>9__7 = (TIFactionState x) => x.permanentAlly(bombingFaction));
					}
					if (ref_factions.None<TIFactionState>(func4) && tihabSiteState.hab.PresentModules().Count > 0)
					{
						list.Add(tihabSiteState.hab);
					}
				}
				List<TIGameState> list5 = list;
				IEnumerable<TISpaceFleetState> landedFleets = tihabSiteState.landedFleets;
				Func<TISpaceFleetState, bool> func5;
				if ((func5 = <>9__6) == null)
				{
					func5 = (<>9__6 = (TISpaceFleetState x) => !x.faction.permanentAlly(bombingFaction));
				}
				list5.AddRange(landedFleets.Where<TISpaceFleetState>(func5));
			}
		}
		return list.Distinct<TIGameState>().ToList<TIGameState>();
	}

	// Token: 0x06000D96 RID: 3478 RVA: 0x00043CA0 File Offset: 0x00041EA0
	public override bool OnOperationConfirm(TIGameState actorState, TIGameState target, TIResourcesCost resourcesCost = null, Trajectory trajectory = null)
	{
		if (base.OnOperationConfirm(actorState, target, null, null))
		{
			if (target.isRegionState && !target.ref_nation.alienNation)
			{
				actorState.ref_faction.CommitAtrocity(1, TIFactionState.AtrocityCause.SpaceBombardHumanNationRegions, false, 0.333f);
			}
			actorState.ref_fleet.InitiateBombardment(target, false, this.bombardmentAltitude_km(target.ref_spaceBody));
			actorState.ref_fleet.AddFleetLog("Bombarding " + target.GetType().ToString());
			return true;
		}
		return false;
	}

	// Token: 0x06000D97 RID: 3479 RVA: 0x00043D20 File Offset: 0x00041F20
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		TISpaceFleetState ref_fleet = actorState.ref_fleet;
		if (ref_fleet.endBombardmentReason == TISpaceFleetState.EndBombardmentReason.None)
		{
			ref_fleet.endBombardmentReason = TISpaceFleetState.EndBombardmentReason.DurationExpired;
		}
		ref_fleet.EndBombardment(ref_fleet.endBombardmentReason);
		if (ref_fleet.huntingXenofauna)
		{
			if (!ref_fleet.CanHuntXenofauna())
			{
				ref_fleet.SetHuntingXenofauna(false, true);
				return;
			}
			ref_fleet.AttemptBombardXenofauna();
		}
	}

	// Token: 0x06000D98 RID: 3480 RVA: 0x00043D70 File Offset: 0x00041F70
	public override string GetDescription(TIGameState actorState = null, TIGameState target = null)
	{
		return Loc.T(new StringBuilder(base.GetType().Name).Append(".description").ToString(), new object[] { this.bombardmentAltitude_km((target != null) ? target.ref_spaceBody : null).ToString("N0") });
	}

	// Token: 0x06000D99 RID: 3481 RVA: 0x00043DC9 File Offset: 0x00041FC9
	public override List<Type> BreakthroughOps()
	{
		return new List<Type>
		{
			typeof(CancelFleetOperation),
			typeof(MergeFleetOperation)
		};
	}

	// Token: 0x04000EB3 RID: 3763
	public const float BOMBARDMENT_DURATION_DAYS = 14f;
}

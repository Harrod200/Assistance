using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x0200033D RID: 829
public class AssaultHabOperation : TISpaceFleetOperationTemplate_Special, IContestedOperation
{
	// Token: 0x06000E25 RID: 3621 RVA: 0x000471F9 File Offset: 0x000453F9
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.DelayedExecutionOfInstantEffect;
	}

	// Token: 0x06000E26 RID: 3622 RVA: 0x000471FC File Offset: 0x000453FC
	public override int SortOrder()
	{
		return 4;
	}

	// Token: 0x06000E27 RID: 3623 RVA: 0x000471FF File Offset: 0x000453FF
	public override bool IsBlockingOperation()
	{
		return true;
	}

	// Token: 0x06000E28 RID: 3624 RVA: 0x00047202 File Offset: 0x00045402
	public override List<SpecialModuleRule> RequiredCapability()
	{
		return new List<SpecialModuleRule> { SpecialModuleRule.Assault };
	}

	// Token: 0x06000E29 RID: 3625 RVA: 0x00047210 File Offset: 0x00045410
	public override bool UpdatePropulsionOnComplete()
	{
		return false;
	}

	// Token: 0x06000E2A RID: 3626 RVA: 0x00047213 File Offset: 0x00045413
	public override bool CancelUponCombat()
	{
		return true;
	}

	// Token: 0x06000E2B RID: 3627 RVA: 0x00047218 File Offset: 0x00045418
	public override bool WarnTarget(TIGameState target)
	{
		if (target == null)
		{
			return false;
		}
		TIHabState ref_hab = target.ref_hab;
		int? num = ((ref_hab != null) ? new int?(ref_hab.AtrocitiesFromDestruction()) : null);
		int num2 = 0;
		return (num.GetValueOrDefault() > num2) & (num != null);
	}

	// Token: 0x06000E2C RID: 3628 RVA: 0x00047260 File Offset: 0x00045460
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		TISpaceFleetState ref_fleet = actorState.ref_fleet;
		return ref_fleet.AssaultCombatValue(false) > 0f || (ref_fleet.dockedAtHab && ((ref_fleet.ref_hab.ref_faction == ref_fleet.faction && ref_fleet.dockedLocation.ref_hab.AssaultCombatValue(false) > 0f) || ref_fleet.ref_hab.AssaultCombatValueFromDockedFleets(ref_fleet.faction, false) > 0f));
	}

	// Token: 0x06000E2D RID: 3629 RVA: 0x000472DC File Offset: 0x000454DC
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return (float)target.ref_hab.tier + (float)target.ref_hab.OkayModules().Sum<TIHabModuleState>((TIHabModuleState x) => x.tier) * 0.041666668f;
	}

	// Token: 0x06000E2E RID: 3630 RVA: 0x0004732C File Offset: 0x0004552C
	public override bool CancelUponDepartHab()
	{
		return true;
	}

	// Token: 0x06000E2F RID: 3631 RVA: 0x0004732F File Offset: 0x0004552F
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Hab);
	}

	// Token: 0x06000E30 RID: 3632 RVA: 0x0004733C File Offset: 0x0004553C
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		TISpaceFleetState ref_fleet = actorState.ref_fleet;
		if (!ref_fleet.transferAssigned && !ref_fleet.inCombatOrWaitingForCombat && base.ActorCanPerformOperation_PassInterruptCheck(actorState))
		{
			TIOrbitState orbitState = ref_fleet.orbitState;
			if (((orbitState != null && orbitState.interfaceOrbit) || (ref_fleet.dockedOrLanded && ref_fleet.ref_hab.ref_faction != ref_fleet.faction)) && this.GetPossibleTargets(ref_fleet, null).Count > 0)
			{
				return TIMissionCondition_SeizeSpaceAssetTroopsPresent.AvailableAssaultAssetsAtLocation(ref_fleet, null);
			}
		}
		return false;
	}

	// Token: 0x06000E31 RID: 3633 RVA: 0x000473B6 File Offset: 0x000455B6
	public override bool OperationConfirmed(TIGameState actor, TIGameState target, TIDateTime opCompleteDate)
	{
		target.ref_hab.SetUnderAssault(actor, true, true);
		return base.OperationConfirmed(actor, target, opCompleteDate);
	}

	// Token: 0x06000E32 RID: 3634 RVA: 0x000473D0 File Offset: 0x000455D0
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		List<TIGameState> list = new List<TIGameState>();
		TISpaceFleetState fleet = actorState.ref_fleet;
		if (fleet.dockedAtHab && !fleet.ref_hab.ref_faction.permanentAlly(fleet.faction) && !fleet.ref_hab.underAssault)
		{
			list.Add(fleet.ref_hab);
		}
		else if (!fleet.landed && fleet.orbitState.interfaceOrbit)
		{
			list.AddRange(from x in fleet.orbitState.ref_spaceBody.occupiedHabSites
				where !x.hab.ref_faction.permanentAlly(fleet.faction) && !x.ref_hab.underAssault
				select x.hab);
		}
		else if (fleet.landed)
		{
			list.AddRange(fleet.ref_spaceBody.surfaceBases.Where<TIHabState>((TIHabState x) => !x.underAssault && !x.ref_faction.permanentAlly(fleet.faction) && x.ref_habSite.AdjacentSites().Contains(fleet.ref_habSite)));
		}
		TIHabState primaryHab = GameStateManager.AlienFaction().primaryHab;
		if (list.Contains(primaryHab) && primaryHab != null)
		{
			List<TIHabModuleState> list2 = primaryHab.OkayModules();
			if (list2.Count == 2)
			{
				if (list2.Any<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.coreModule))
				{
					if (list2.Any<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.AlienWormhole)))
					{
						list.Remove(primaryHab);
					}
				}
			}
		}
		return list;
	}

	// Token: 0x06000E33 RID: 3635 RVA: 0x00047578 File Offset: 0x00045778
	public float GetSuccessChance(TIGameState actor, TIGameState defender)
	{
		TISpaceFleetState ref_fleet = actor.ref_fleet;
		TIFactionState ref_faction = actor.ref_faction;
		int missionControlShortage = ref_faction.MissionControlShortage;
		float num = ref_fleet.AssaultCombatValue(false);
		if (ref_fleet.dockedAtHab && ref_fleet.ref_hab.faction == ref_fleet.faction)
		{
			if (missionControlShortage <= 0 && ref_faction.DailyHabBoostShortage() <= 0f && !ref_faction.Insolvent)
			{
				num += actor.ref_fleet.ref_hab.AssaultCombatValue(false);
			}
			foreach (TISpaceFleetState tispaceFleetState in ref_fleet.ref_hab.dockedFleets)
			{
				if (tispaceFleetState != ref_fleet && tispaceFleetState.faction == ref_fleet.faction)
				{
					num += tispaceFleetState.AssaultCombatValue(false);
				}
			}
		}
		num -= (float)missionControlShortage;
		float num2 = defender.ref_hab.ModifiedDefenseCombatValue(defender.ref_hab.IsBase && !ref_fleet.landed);
		float num3 = num - num2;
		float num4 = 0.5f * Mathf.Pow(0.775f, Mathf.Abs(num3));
		if (num3 >= 0f)
		{
			num4 = 1f - num4;
		}
		return num4;
	}

	// Token: 0x06000E34 RID: 3636 RVA: 0x000476C4 File Offset: 0x000458C4
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		if (!TIGameState.Valid(target))
		{
			return;
		}
		TISpaceFleetState ref_fleet = actorState.ref_fleet;
		TIHabState ref_hab = target.ref_hab;
		ref_hab.SetUnderAssault(actorState, false, false);
		if (!ref_fleet.transferAssigned && !ref_fleet.inCombatOrWaitingForCombat)
		{
			TIOrbitState orbitState = ref_fleet.orbitState;
			if (((orbitState != null && orbitState.interfaceOrbit) || ref_fleet.dockedOrLanded) && ref_hab.ref_faction != ref_fleet.faction)
			{
				TIFactionState ref_faction = actorState.ref_faction;
				TIFactionState ref_faction2 = target.ref_faction;
				if (ref_faction2 != null)
				{
					ref_faction2.GainFactionHate(ref_faction, TemplateManager.global.factionHateForHabAssaultOperationPerTier * (float)ref_hab.tier, false, "Hab Assault Operation", true);
				}
				float successChance = this.GetSuccessChance(ref_fleet, ref_hab);
				float num = TIUtilities.RandomFloatValue();
				TIMissionOutcome timissionOutcome;
				if (num <= successChance / 10f)
				{
					timissionOutcome = TIMissionOutcome.CriticalSuccess;
				}
				else if (num <= successChance)
				{
					timissionOutcome = TIMissionOutcome.Success;
				}
				else
				{
					float num2 = 1f - (1f - successChance) / 10f;
					if (num >= num2)
					{
						timissionOutcome = TIMissionOutcome.CriticalFailure;
					}
					else
					{
						timissionOutcome = TIMissionOutcome.Failure;
					}
				}
				if (ref_faction != null && ref_faction.isActivePlayer && ref_fleet.AssaultCombatValue(false) > 0f)
				{
					ref_fleet.faction.UnlockAchievement("assaultHabMarines");
				}
				List<TIOfficerState> list = new List<TIOfficerState>();
				List<TIOfficerState> list2 = new List<TIOfficerState>();
				List<TIOfficerState> list3 = new List<TIOfficerState>();
				List<TIOfficerState> list4 = new List<TIOfficerState>();
				ref_fleet.PostAssaultDamage(timissionOutcome, true);
				List<TIOfficerState> list5;
				list.AddRange(ref_fleet.PostAssaultPromotionsAndDeaths(timissionOutcome, true, out list5));
				list3.AddRange(list5);
				if (ref_fleet.dockedAtHab)
				{
					foreach (TISpaceFleetState tispaceFleetState in ref_fleet.ref_hab.dockedFleets)
					{
						if (ref_fleet != tispaceFleetState && ref_fleet.faction == tispaceFleetState.faction)
						{
							tispaceFleetState.PostAssaultDamage(timissionOutcome, true);
							List<TIOfficerState> list6;
							list.AddRange(tispaceFleetState.PostAssaultPromotionsAndDeaths(timissionOutcome, true, out list6));
							list3.AddRange(list6);
						}
					}
					if (ref_fleet.ref_hab.faction == ref_fleet.faction && ref_fleet.ref_hab.faction.MissionControlShortage <= 0 && ref_fleet.ref_hab.faction.DailyHabBoostShortage() <= 0f && !ref_fleet.ref_hab.faction.Insolvent)
					{
						ref_fleet.ref_hab.TakeDamageFromParticipatingInAssault_Offense(timissionOutcome, ref_hab.faction);
					}
				}
				ref_hab.TakeDamageFromParticipatingInAssault_Defense(timissionOutcome, ref_fleet.faction);
				foreach (TISpaceFleetState tispaceFleetState2 in ref_hab.dockedFleets)
				{
					if (tispaceFleetState2.faction == ref_faction2)
					{
						tispaceFleetState2.PostAssaultDamage(timissionOutcome, false);
						List<TIOfficerState> list7;
						list2.AddRange(tispaceFleetState2.PostAssaultPromotionsAndDeaths(timissionOutcome, false, out list7));
						list4.AddRange(list7);
					}
				}
				Dictionary<TIFactionState, string> dictionary = new Dictionary<TIFactionState, string>();
				if (list.Count > 0)
				{
					dictionary.Add(ref_faction, TIOfficerTemplate.BuildOfficerPromotionReport(list, ref_faction));
				}
				if (list3.Count > 0)
				{
					dictionary.Add(ref_faction, TIOfficerTemplate.BuildOfficerDeathsReport(list3, ref_faction));
					foreach (TIOfficerState tiofficerState in list3.ToList<TIOfficerState>())
					{
						tiofficerState.DeleteOfficer(true);
					}
				}
				if (list2.Count > 0)
				{
					dictionary.Add(ref_faction2, TIOfficerTemplate.BuildOfficerPromotionReport(list2, ref_faction2));
				}
				if (list4.Count > 0)
				{
					dictionary.Add(ref_faction2, TIOfficerTemplate.BuildOfficerDeathsReport(list4, ref_faction2));
					foreach (TIOfficerState tiofficerState2 in list4.ToList<TIOfficerState>())
					{
						tiofficerState2.DeleteOfficer(true);
					}
				}
				switch (timissionOutcome)
				{
				case TIMissionOutcome.CriticalFailure:
				case TIMissionOutcome.Failure:
					TINotificationQueueState.LogHabAssaultFailed(ref_fleet, ref_hab, dictionary, timissionOutcome);
					break;
				case TIMissionOutcome.Aborted:
					break;
				case TIMissionOutcome.Success:
					ref_hab.CaptureHab(ref_fleet.ref_faction, -1, false, false, dictionary, ref_fleet);
					return;
				case TIMissionOutcome.CriticalSuccess:
					ref_hab.CaptureHab(ref_fleet.ref_faction, 0, false, false, dictionary, ref_fleet);
					return;
				default:
					return;
				}
			}
		}
	}

	// Token: 0x06000E35 RID: 3637 RVA: 0x00047AF0 File Offset: 0x00045CF0
	public override void OnOperationCancel(TIGameState actorState, TIGameState target, TIDateTime opCompleteDate)
	{
		TIHabState ref_hab = target.ref_hab;
		if (ref_hab != null)
		{
			ref_hab.SetUnderAssault(actorState, false, false);
		}
		base.OnOperationCancel(actorState, target, opCompleteDate);
	}

	// Token: 0x06000E36 RID: 3638 RVA: 0x00047B0F File Offset: 0x00045D0F
	public override bool Repeatable()
	{
		return true;
	}

	// Token: 0x04000EBB RID: 3771
	private const float assaultDurationPerTier_days = 0.041666668f;
}

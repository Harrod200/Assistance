using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x0200033B RID: 827
public class DestroyHabOperation : TISpaceFleetOperationTemplate
{
	// Token: 0x06000E12 RID: 3602 RVA: 0x00046D4F File Offset: 0x00044F4F
	public override bool IsBlockingOperation()
	{
		return true;
	}

	// Token: 0x06000E13 RID: 3603 RVA: 0x00046D52 File Offset: 0x00044F52
	public override bool UpdatePropulsionOnComplete()
	{
		return false;
	}

	// Token: 0x06000E14 RID: 3604 RVA: 0x00046D55 File Offset: 0x00044F55
	public override bool CancelUponCombat()
	{
		return true;
	}

	// Token: 0x06000E15 RID: 3605 RVA: 0x00046D58 File Offset: 0x00044F58
	public override int SortOrder()
	{
		return 5;
	}

	// Token: 0x06000E16 RID: 3606 RVA: 0x00046D5B File Offset: 0x00044F5B
	public override bool CanCancel()
	{
		return true;
	}

	// Token: 0x06000E17 RID: 3607 RVA: 0x00046D5E File Offset: 0x00044F5E
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.DelayedExecutionOfInstantEffect;
	}

	// Token: 0x06000E18 RID: 3608 RVA: 0x00046D61 File Offset: 0x00044F61
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Hab);
	}

	// Token: 0x06000E19 RID: 3609 RVA: 0x00046D70 File Offset: 0x00044F70
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		if (!actorState.ref_fleet.transferAssigned)
		{
			return actorState.ref_fleet.ships.Any<TISpaceShipState>((TISpaceShipState x) => x.AnyWeaponCanFire()) || actorState.ref_fleet.AssaultCombatValue(false) > 0f;
		}
		return false;
	}

	// Token: 0x06000E1A RID: 3610 RVA: 0x00046DD4 File Offset: 0x00044FD4
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return !actorState.ref_fleet.inCombatOrWaitingForCombat && actorState.ref_fleet.dockedAtHab && !actorState.ref_hab.faction.permanentAlly(actorState.ref_faction) && !actorState.ref_fleet.ref_hab.underAssault && base.ActorCanPerformOperation_PassInterruptCheck(actorState) && (actorState.ref_hab.IsStation || actorState.ref_fleet.AssaultCombatValue(false) > 0f);
	}

	// Token: 0x06000E1B RID: 3611 RVA: 0x00046E52 File Offset: 0x00045052
	public override bool CancelUponDepartHab()
	{
		return true;
	}

	// Token: 0x06000E1C RID: 3612 RVA: 0x00046E55 File Offset: 0x00045055
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return (float)target.ref_hab.OkayModules().Sum<TIHabModuleState>((TIHabModuleState x) => x.tier) * 0.010416667f;
	}

	// Token: 0x06000E1D RID: 3613 RVA: 0x00046E8D File Offset: 0x0004508D
	public override bool OperationConfirmed(TIGameState actor, TIGameState target, TIDateTime opCompleteDate)
	{
		target.ref_hab.SetUnderAssault(actor, true, false);
		return base.OperationConfirmed(actor, target, opCompleteDate);
	}

	// Token: 0x06000E1E RID: 3614 RVA: 0x00046EA8 File Offset: 0x000450A8
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		List<TIGameState> list = new List<TIGameState>();
		if (actorState != null && actorState.ref_fleet.dockedAtHab && !actorState.ref_fleet.ref_hab.ref_faction.permanentAlly(actorState.ref_faction) && !actorState.ref_fleet.ref_hab.underAssault)
		{
			list.Add(actorState.ref_fleet.ref_hab);
		}
		return list;
	}

	// Token: 0x06000E1F RID: 3615 RVA: 0x00046F14 File Offset: 0x00045114
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

	// Token: 0x06000E20 RID: 3616 RVA: 0x00046F5C File Offset: 0x0004515C
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		if (TIGameState.Valid(target))
		{
			TIFactionState ref_faction = actorState.ref_faction;
			TIHabState ref_hab = target.ref_hab;
			if (ref_hab != null)
			{
				if (ref_hab.crew > 0)
				{
					if (ref_hab.IsAlien())
					{
						ref_faction.CompleteMilestone(CampaignMilestone.AccessAlienTech);
					}
					else if (ref_faction.IsAlienFaction)
					{
						ref_faction.GainIntel(ref_hab.faction, (float)(20 * ref_hab.tier), null, false);
					}
				}
				ref_hab.SetUnderAssault(actorState, false, false);
				ref_hab.DestroyHab(ref_faction, 0.1f, false, actorState.ref_fleet, 0f);
			}
		}
	}

	// Token: 0x06000E21 RID: 3617 RVA: 0x00046FE4 File Offset: 0x000451E4
	public override void OnOperationCancel(TIGameState actorState, TIGameState target, TIDateTime opCompleteDate)
	{
		TIHabState ref_hab = target.ref_hab;
		ref_hab.SetUnderAssault(actorState, false, false);
		float duration_days = this.GetDuration_days(actorState, target, null);
		TIDateTime tidateTime = new TIDateTime(opCompleteDate);
		tidateTime.AddDays(-duration_days);
		float num = Mathf.Clamp01((float)TITimeState.Now().DifferenceInDays(tidateTime) / duration_days);
		float num2 = (float)(ref_hab.OkayModules().Sum<TIHabModuleState>((TIHabModuleState x) => x.tier) - ref_hab.tier) * num;
		foreach (TIHabModuleState tihabModuleState in ref_hab.OkayModules().Shuffle<TIHabModuleState>())
		{
			if (tihabModuleState != ref_hab.coreModule && num2 >= (float)tihabModuleState.tier)
			{
				ref_hab.DestroyModule(actorState.ref_faction, tihabModuleState, false, true, false, 1f, true, false);
				num2 -= (float)tihabModuleState.tier;
			}
			if (num2 <= 0f)
			{
				break;
			}
		}
		ref_hab.UpdatePowerManagement(false, null, ref_hab.faction.player.isAI);
		base.OnOperationCancel(actorState, target, opCompleteDate);
	}

	// Token: 0x06000E22 RID: 3618 RVA: 0x00047118 File Offset: 0x00045318
	public override string GetDescription(TIGameState actorState = null, TIGameState target = null)
	{
		List<TIGameState> possibleTargets = this.GetPossibleTargets(actorState, null);
		StringBuilder stringBuilder = new StringBuilder(Loc.T(new StringBuilder(base.GetType().Name).Append(".description").ToString()));
		if (possibleTargets.Count > 0)
		{
			int num = possibleTargets[0].ref_hab.AtrocitiesFromDestruction();
			if (num == 1)
			{
				stringBuilder.Append(Loc.T(new StringBuilder(base.GetType().Name).Append(".description2").ToString()));
			}
			else if (num > 1)
			{
				stringBuilder.Append(Loc.T(new StringBuilder(base.GetType().Name).Append(".description3").ToString(), new object[] { num.ToString("N0") }));
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x04000EBA RID: 3770
	private const float destroyDurationPerTier_days = 0.010416667f;
}

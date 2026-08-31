using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000289 RID: 649
public class TIMissionTargeting_CouncilorHab : TIMissionTargeting
{
	// Token: 0x060008BB RID: 2235 RVA: 0x00028BB2 File Offset: 0x00026DB2
	public override List<Type> TargetedGameStates()
	{
		return new List<Type>
		{
			typeof(TICouncilorState),
			typeof(TIHabState)
		};
	}

	// Token: 0x060008BC RID: 2236 RVA: 0x00028BDC File Offset: 0x00026DDC
	public override void Activate()
	{
		if (!base.activated)
		{
			this.actingCouncilor = this.councilor;
			base.SetActivation(this);
			this.possibleTargets = base.missionTemplate.target.GetValidTargets(base.missionTemplate, this.councilor);
			base.SetDefaultTarget();
			GameControl.eventManager.AddListener<CouncilorMapItemSelected>(new EventManager.EventDelegate<CouncilorMapItemSelected>(this.CouncilorSelectedForTargeting), null, null, true, false);
			GameControl.eventManager.AddListener<HabSelectedEvent>(new EventManager.EventDelegate<HabSelectedEvent>(this.HabSelectedForTargeting), null, null, true, false);
			GameControl.eventManager.TriggerEvent(new TargetCouncilors(this.councilor, base.missionTemplate), null, Array.Empty<object>());
		}
	}

	// Token: 0x060008BD RID: 2237 RVA: 0x00028C84 File Offset: 0x00026E84
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
			GameControl.eventManager.RemoveListener<CouncilorMapItemSelected>(new EventManager.EventDelegate<CouncilorMapItemSelected>(this.CouncilorSelectedForTargeting), null);
			GameControl.eventManager.RemoveListener<HabSelectedEvent>(new EventManager.EventDelegate<HabSelectedEvent>(this.HabSelectedForTargeting), null);
			GameControl.eventManager.TriggerEvent(new DeTargetCouncilors(), null, Array.Empty<object>());
		}
	}

	// Token: 0x060008BE RID: 2238 RVA: 0x00028CE4 File Offset: 0x00026EE4
	public override TIGameState GetDefaultTarget()
	{
		ICollection<TIGameState> possibleTargets = this.possibleTargets;
		TIMissionState activeMission = this.councilor.activeMission;
		if (possibleTargets.Contains((activeMission != null) ? activeMission.target : null))
		{
			TIMissionState activeMission2 = this.councilor.activeMission;
			if (activeMission2 == null)
			{
				return null;
			}
			return activeMission2.target;
		}
		else
		{
			if (this.councilor.ref_hab != null && this.possibleTargets.Contains(this.councilor.ref_hab))
			{
				return this.councilor.ref_hab;
			}
			if (this.possibleTargets.Contains(GeneralControlsController.UIOtherSelectedState))
			{
				return GeneralControlsController.UIOtherSelectedState;
			}
			if (this.councilor.ref_region != null)
			{
				List<TIGameState> list = this.councilor.ref_region.GetVisibleCouncilorsInRegion(this.councilor.faction).Intersect<TIGameState>(this.possibleTargets).ToList<TIGameState>();
				if (list.Any<TIGameState>())
				{
					return list[0];
				}
				list = this.councilor.ref_nation.GetVisibleCouncilorsInNation(this.councilor.faction).Intersect<TIGameState>(this.possibleTargets).ToList<TIGameState>();
				if (list.Any<TIGameState>())
				{
					return list[0];
				}
			}
			if (this.councilor.ref_hab != null)
			{
				List<TIGameState> list2 = this.councilor.ref_hab.CouncilorsPresentAndKnownToFaction(this.councilor.faction, false, null).Intersect<TIGameState>(this.possibleTargets).ToList<TIGameState>();
				if (list2.Any<TIGameState>())
				{
					return list2[0];
				}
			}
			return base.GetDefaultTarget();
		}
	}

	// Token: 0x060008BF RID: 2239 RVA: 0x00028E5D File Offset: 0x0002705D
	private void HabSelectedForTargeting(HabSelectedEvent e)
	{
		GameControl.eventManager.ClearPendingEvents(e, null, Array.Empty<object>());
		base.SetTarget(e.hab);
	}

	// Token: 0x060008C0 RID: 2240 RVA: 0x00028E7C File Offset: 0x0002707C
	private void CouncilorSelectedForTargeting(CouncilorMapItemSelected e)
	{
		if (this.actingCouncilor == e.councilor)
		{
			GameControl.eventManager.ClearPendingEvents(e, null, Array.Empty<object>());
		}
		base.SetTarget(e.councilor);
	}

	// Token: 0x04000640 RID: 1600
	private TICouncilorState actingCouncilor;
}

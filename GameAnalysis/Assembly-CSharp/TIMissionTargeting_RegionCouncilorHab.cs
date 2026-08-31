using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200028A RID: 650
public class TIMissionTargeting_RegionCouncilorHab : TIMissionTargeting
{
	// Token: 0x060008C2 RID: 2242 RVA: 0x00028EB6 File Offset: 0x000270B6
	public override List<Type> TargetedGameStates()
	{
		return new List<Type>
		{
			typeof(TICouncilorState),
			typeof(TIHabState),
			typeof(TIRegionState)
		};
	}

	// Token: 0x060008C3 RID: 2243 RVA: 0x00028EF0 File Offset: 0x000270F0
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
			GameControl.eventManager.AddListener<RegionStateSelected>(new EventManager.EventDelegate<RegionStateSelected>(this.RegionSelectedForTargeting), null, null, true, false);
			GameControl.eventManager.TriggerEvent(new TargetCouncilors(this.councilor, base.missionTemplate), null, Array.Empty<object>());
			GameControl.eventManager.TriggerEvent(new TargetRegions(this.councilor, base.missionTemplate), null, Array.Empty<object>());
		}
	}

	// Token: 0x060008C4 RID: 2244 RVA: 0x00028FD4 File Offset: 0x000271D4
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
			GameControl.eventManager.TriggerEvent(new DeTargetRegions(), null, Array.Empty<object>());
			GameControl.eventManager.TriggerEvent(new DeTargetCouncilors(), null, Array.Empty<object>());
			GameControl.eventManager.RemoveListener<CouncilorMapItemSelected>(new EventManager.EventDelegate<CouncilorMapItemSelected>(this.CouncilorSelectedForTargeting), null);
			GameControl.eventManager.RemoveListener<HabSelectedEvent>(new EventManager.EventDelegate<HabSelectedEvent>(this.HabSelectedForTargeting), null);
			GameControl.eventManager.RemoveListener<RegionStateSelected>(new EventManager.EventDelegate<RegionStateSelected>(this.RegionSelectedForTargeting), null);
		}
	}

	// Token: 0x060008C5 RID: 2245 RVA: 0x00029060 File Offset: 0x00027260
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
				if (list.Count > 0)
				{
					return list[0];
				}
				list = this.councilor.ref_nation.GetVisibleCouncilorsInNation(this.councilor.faction).Intersect<TIGameState>(this.possibleTargets).ToList<TIGameState>();
				if (list.Count > 0)
				{
					return list[0];
				}
				list = this.councilor.faction.councilors.Intersect<TIGameState>(this.possibleTargets).ToList<TIGameState>();
				if (list.Count > 0)
				{
					return list[0];
				}
				if (this.possibleTargets.Contains(this.councilor.ref_region))
				{
					return this.councilor.ref_region;
				}
			}
			if (this.councilor.ref_hab != null)
			{
				List<TIGameState> list2 = this.councilor.ref_hab.CouncilorsPresentAndKnownToFaction(this.councilor.faction, false, null).Intersect<TIGameState>(this.possibleTargets).ToList<TIGameState>();
				if (list2.Count > 0)
				{
					return list2[0];
				}
			}
			return base.GetDefaultTarget();
		}
	}

	// Token: 0x060008C6 RID: 2246 RVA: 0x00029235 File Offset: 0x00027435
	private void HabSelectedForTargeting(HabSelectedEvent e)
	{
		GameControl.eventManager.ClearPendingEvents(e, null, Array.Empty<object>());
		base.SetTarget(e.hab);
	}

	// Token: 0x060008C7 RID: 2247 RVA: 0x00029254 File Offset: 0x00027454
	private void CouncilorSelectedForTargeting(CouncilorMapItemSelected e)
	{
		if (this.actingCouncilor == e.councilor)
		{
			GameControl.eventManager.ClearPendingEvents(e, null, Array.Empty<object>());
		}
		base.SetTarget(e.councilor);
	}

	// Token: 0x060008C8 RID: 2248 RVA: 0x00029286 File Offset: 0x00027486
	private void RegionSelectedForTargeting(RegionStateSelected e)
	{
		base.SetTarget(e.region);
	}

	// Token: 0x04000641 RID: 1601
	private TICouncilorState actingCouncilor;
}

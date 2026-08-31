using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200028D RID: 653
public class TIMissionTargeting_RegionBase : TIMissionTargeting
{
	// Token: 0x060008D8 RID: 2264 RVA: 0x000296CC File Offset: 0x000278CC
	public override List<Type> TargetedGameStates()
	{
		return new List<Type>
		{
			typeof(TIRegionState),
			typeof(TIHabState)
		};
	}

	// Token: 0x17000109 RID: 265
	// (get) Token: 0x060008D9 RID: 2265 RVA: 0x000296F3 File Offset: 0x000278F3
	public override bool forceMap
	{
		get
		{
			return this.councilor.OnOrAroundEarth;
		}
	}

	// Token: 0x060008DA RID: 2266 RVA: 0x00029700 File Offset: 0x00027900
	public override void Activate()
	{
		if (!base.activated)
		{
			base.SetActivation(this);
			this.possibleTargets = base.missionTemplate.target.GetValidTargets(base.missionTemplate, this.councilor);
			base.SetDefaultTarget();
			GameControl.eventManager.TriggerEvent(new TargetRegions(this.councilor, base.missionTemplate), null, Array.Empty<object>());
			GameControl.eventManager.AddListener<RegionStateSelected>(new EventManager.EventDelegate<RegionStateSelected>(this.RegionSelectedForTargeting), null, null, true, false);
			GameControl.eventManager.AddListener<HabSelectedEvent>(new EventManager.EventDelegate<HabSelectedEvent>(this.HabSelectedForTargeting), null, null, true, false);
			if (this.councilor.OnOrAroundEarth && GameControl.control.viewMgr.currentView != ViewType.PoliticalMap)
			{
				GameControl.control.viewMgr.GotoView(ViewType.PoliticalMap);
				GameControl.control.viewMgr.earthObject.GetComponent<SpaceObjectController>().mapController.ResetMapColors();
			}
		}
	}

	// Token: 0x060008DB RID: 2267 RVA: 0x000297E9 File Offset: 0x000279E9
	private void RegionSelectedForTargeting(RegionStateSelected e)
	{
		base.SetTarget(e.region);
	}

	// Token: 0x060008DC RID: 2268 RVA: 0x000297F7 File Offset: 0x000279F7
	private void HabSelectedForTargeting(HabSelectedEvent e)
	{
		base.SetTarget(e.hab);
	}

	// Token: 0x060008DD RID: 2269 RVA: 0x00029808 File Offset: 0x00027A08
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
			if (this.possibleTargets.Contains(GeneralControlsController.UIOtherSelectedState))
			{
				return GeneralControlsController.UIOtherSelectedState;
			}
			TIRegionState tiregionState = this.councilor.location as TIRegionState;
			if (tiregionState != null && this.possibleTargets.Contains(tiregionState))
			{
				return tiregionState;
			}
			TIHabState tihabState = this.councilor.location as TIHabState;
			if (tihabState != null && this.possibleTargets.Contains(tihabState))
			{
				return tihabState;
			}
			return base.GetDefaultTarget();
		}
	}

	// Token: 0x060008DE RID: 2270 RVA: 0x000298C4 File Offset: 0x00027AC4
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
			GameControl.eventManager.TriggerEvent(new DeTargetRegions(), null, Array.Empty<object>());
			GameControl.eventManager.RemoveListener<RegionStateSelected>(new EventManager.EventDelegate<RegionStateSelected>(this.RegionSelectedForTargeting), null);
			GameControl.eventManager.RemoveListener<HabSelectedEvent>(new EventManager.EventDelegate<HabSelectedEvent>(this.HabSelectedForTargeting), null);
			if (this.councilor.ref_naturalSpaceObject.isEarth)
			{
				GameControl.control.viewMgr.GotoView(ViewType.PoliticalMap);
				GameControl.control.viewMgr.earthObject.GetComponent<SpaceObjectController>().mapController.ResetMapColors();
			}
		}
	}
}

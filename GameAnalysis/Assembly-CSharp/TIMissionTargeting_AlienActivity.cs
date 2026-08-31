using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000284 RID: 644
public class TIMissionTargeting_AlienActivity : TIMissionTargeting
{
	// Token: 0x06000898 RID: 2200 RVA: 0x000280B8 File Offset: 0x000262B8
	public override List<Type> TargetedGameStates()
	{
		return new List<Type> { typeof(TIRegionAlienActivityState) };
	}

	// Token: 0x06000899 RID: 2201 RVA: 0x000280D0 File Offset: 0x000262D0
	public override void Activate()
	{
		if (!base.activated)
		{
			base.SetActivation(this);
			this.possibleTargets = base.missionTemplate.target.GetValidTargets(base.missionTemplate, this.councilor);
			base.SetDefaultTarget();
			GameControl.eventManager.TriggerEvent(new CouncilorTargetAlienActivity(this.councilor, base.missionTemplate), null, Array.Empty<object>());
			GameControl.eventManager.AddListener<AlienRegionMapEntitySelected>(new EventManager.EventDelegate<AlienRegionMapEntitySelected>(this.AlienActivitySelectedForTargeting), null, null, true, false);
		}
	}

	// Token: 0x0600089A RID: 2202 RVA: 0x0002814F File Offset: 0x0002634F
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
			GameControl.eventManager.TriggerEvent(new DeTargetAlienActivity(), null, Array.Empty<object>());
			GameControl.eventManager.RemoveListener<AlienRegionMapEntitySelected>(new EventManager.EventDelegate<AlienRegionMapEntitySelected>(this.AlienActivitySelectedForTargeting), null);
		}
	}

	// Token: 0x0600089B RID: 2203 RVA: 0x0002818B File Offset: 0x0002638B
	private void AlienActivitySelectedForTargeting(AlienRegionMapEntitySelected e)
	{
		base.SetTarget(e.alienEntity);
	}

	// Token: 0x0600089C RID: 2204 RVA: 0x0002819C File Offset: 0x0002639C
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
			if (this.councilor.location.isRegionState)
			{
				TIRegionState ref_region = this.councilor.ref_region;
				if (ref_region.alienCrashdown.VisibleToFaction(this.councilor.faction) && this.possibleTargets.Contains(ref_region.alienCrashdown))
				{
					return ref_region.alienCrashdown;
				}
				if (ref_region.alienActivity.VisibleToFaction(this.councilor.faction) && this.possibleTargets.Contains(ref_region.alienActivity))
				{
					return ref_region.alienActivity;
				}
			}
			return base.GetDefaultTarget();
		}
	}
}

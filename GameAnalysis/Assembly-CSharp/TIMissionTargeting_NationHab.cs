using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000287 RID: 647
public class TIMissionTargeting_NationHab : TIMissionTargeting
{
	// Token: 0x060008AE RID: 2222 RVA: 0x0002886B File Offset: 0x00026A6B
	public override List<Type> TargetedGameStates()
	{
		return new List<Type>
		{
			typeof(TINationState),
			typeof(TIHabState)
		};
	}

	// Token: 0x060008AF RID: 2223 RVA: 0x00028894 File Offset: 0x00026A94
	public override void Activate()
	{
		if (!base.activated)
		{
			base.SetActivation(this);
			this.possibleTargets = base.missionTemplate.target.GetValidTargets(base.missionTemplate, this.councilor);
			base.SetDefaultTarget();
			GameControl.eventManager.TriggerEvent(new TargetGov(this.councilor, base.missionTemplate), null, Array.Empty<object>());
			GameControl.eventManager.AddListener<NationStateSelected>(new EventManager.EventDelegate<NationStateSelected>(this.NationSelectedForTargeting), null, null, true, false);
			GameControl.eventManager.AddListener<HabSelectedEvent>(new EventManager.EventDelegate<HabSelectedEvent>(this.HabSelectedForTargeting), null, null, true, false);
			if (this.councilor.OnEarth)
			{
				GameControl.control.viewMgr.earthObject.GetComponent<SpaceObjectController>().mapController.ResetMapColors();
			}
		}
	}

	// Token: 0x060008B0 RID: 2224 RVA: 0x0002895B File Offset: 0x00026B5B
	private void NationSelectedForTargeting(NationStateSelected e)
	{
		base.SetTarget(e.nation);
	}

	// Token: 0x060008B1 RID: 2225 RVA: 0x00028969 File Offset: 0x00026B69
	private void HabSelectedForTargeting(HabSelectedEvent e)
	{
		base.SetTarget(e.hab);
	}

	// Token: 0x060008B2 RID: 2226 RVA: 0x00028978 File Offset: 0x00026B78
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
			if (this.councilor.OnEarth)
			{
				ICollection<TIGameState> possibleTargets2 = this.possibleTargets;
				TIGameState uiotherSelectedState = GeneralControlsController.UIOtherSelectedState;
				if (possibleTargets2.Contains((uiotherSelectedState != null) ? uiotherSelectedState.ref_nation : null))
				{
					return GeneralControlsController.UIOtherSelectedState.ref_nation;
				}
			}
			TINationState ref_nation = this.councilor.ref_nation;
			if (ref_nation != null && this.possibleTargets.Contains(ref_nation))
			{
				return ref_nation;
			}
			TIHabState ref_hab = this.councilor.ref_hab;
			if (ref_hab != null && this.possibleTargets.Contains(ref_hab))
			{
				return ref_hab;
			}
			return base.GetDefaultTarget();
		}
	}

	// Token: 0x060008B3 RID: 2227 RVA: 0x00028A48 File Offset: 0x00026C48
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
			GameControl.eventManager.TriggerEvent(new DeTargetGov(), null, Array.Empty<object>());
			GameControl.eventManager.RemoveListener<NationStateSelected>(new EventManager.EventDelegate<NationStateSelected>(this.NationSelectedForTargeting), null);
			GameControl.eventManager.RemoveListener<HabSelectedEvent>(new EventManager.EventDelegate<HabSelectedEvent>(this.HabSelectedForTargeting), null);
			if (this.councilor.OnEarth)
			{
				GameControl.control.viewMgr.earthObject.GetComponent<SpaceObjectController>().mapController.ResetMapColors();
			}
		}
	}
}

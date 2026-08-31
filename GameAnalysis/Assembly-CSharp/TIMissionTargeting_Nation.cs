using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200027D RID: 637
public class TIMissionTargeting_Nation : TIMissionTargeting
{
	// Token: 0x06000872 RID: 2162 RVA: 0x0002741F File Offset: 0x0002561F
	public override List<Type> TargetedGameStates()
	{
		return new List<Type> { typeof(TINationState) };
	}

	// Token: 0x06000873 RID: 2163 RVA: 0x00027438 File Offset: 0x00025638
	public override void Activate()
	{
		if (!base.activated)
		{
			this.possibleTargets = base.missionTemplate.target.GetValidTargets(base.missionTemplate, this.councilor);
			base.SetActivation(this);
			base.SetDefaultTarget();
			GameControl.eventManager.AddListener<NationStateSelected>(new EventManager.EventDelegate<NationStateSelected>(this.NationSelectedForTargeting), null, null, true, false);
			GameControl.eventManager.TriggerEvent(new TargetGov(this.councilor, base.missionTemplate), null, Array.Empty<object>());
			if (base.missionTemplate.targetEffects.Any<TIMissionEffect>((TIMissionEffect x) => x.GetType() == typeof(TIMissionEffect_GainOpenControlPoint)))
			{
				GameControl.eventManager.TriggerEvent(new TargetOpenControlPoint(this.councilor, base.missionTemplate, this.possibleTargets), null, Array.Empty<object>());
			}
			GameControl.control.viewMgr.earthObject.GetComponent<SpaceObjectController>().mapController.ResetMapColors();
		}
	}

	// Token: 0x06000874 RID: 2164 RVA: 0x00027530 File Offset: 0x00025730
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
			GameControl.eventManager.RemoveListener<NationStateSelected>(new EventManager.EventDelegate<NationStateSelected>(this.NationSelectedForTargeting), null);
			GameControl.eventManager.TriggerEvent(new DeTargetGov(), null, Array.Empty<object>());
			if (base.missionTemplate.targetEffects.Any<TIMissionEffect>((TIMissionEffect x) => x.GetType() == typeof(TIMissionEffect_GainOpenControlPoint)))
			{
				GameControl.eventManager.TriggerEvent(new DeTargetOpenControlPoint(), null, Array.Empty<object>());
			}
		}
		GameControl.control.viewMgr.earthObject.GetComponent<SpaceObjectController>().mapController.ResetMapColors();
	}

	// Token: 0x06000875 RID: 2165 RVA: 0x000275DB File Offset: 0x000257DB
	private void NationSelectedForTargeting(NationStateSelected e)
	{
		base.SetTarget(e.nation);
	}

	// Token: 0x06000876 RID: 2166 RVA: 0x000275EC File Offset: 0x000257EC
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
			TIGameState uiotherSelectedState = GeneralControlsController.UIOtherSelectedState;
			if (uiotherSelectedState != null && uiotherSelectedState.isRegionState && this.possibleTargets.Contains(GeneralControlsController.UIOtherSelectedState.ref_nation))
			{
				return GeneralControlsController.UIOtherSelectedState.ref_nation;
			}
			if (this.possibleTargets.Contains(this.councilor.currentNation))
			{
				return this.councilor.currentNation;
			}
			ICollection<TIGameState> possibleTargets2 = this.possibleTargets;
			TIGameState uiotherSelectedState2 = GeneralControlsController.UIOtherSelectedState;
			if (possibleTargets2.Contains((uiotherSelectedState2 != null) ? uiotherSelectedState2.ref_nation : null))
			{
				return GeneralControlsController.UIOtherSelectedState.ref_nation;
			}
			return base.GetDefaultTarget();
		}
	}
}

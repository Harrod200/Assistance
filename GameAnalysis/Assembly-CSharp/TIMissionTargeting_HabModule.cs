using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200028E RID: 654
public class TIMissionTargeting_HabModule : TIMissionTargeting
{
	// Token: 0x060008E0 RID: 2272 RVA: 0x0002996D File Offset: 0x00027B6D
	public override List<Type> TargetedGameStates()
	{
		return new List<Type> { typeof(TIHabModuleState) };
	}

	// Token: 0x060008E1 RID: 2273 RVA: 0x00029984 File Offset: 0x00027B84
	public override void Activate()
	{
		if (!base.activated)
		{
			base.SetActivation(this);
			this.possibleTargets = base.missionTemplate.target.GetValidTargets(base.missionTemplate, this.councilor);
			base.SetDefaultTarget();
			GameControl.eventManager.AddListener<HabModuleSelected>(new EventManager.EventDelegate<HabModuleSelected>(this.HabModuleSelectedForTargeting), null, null, true, false);
		}
	}

	// Token: 0x060008E2 RID: 2274 RVA: 0x000299E2 File Offset: 0x00027BE2
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
			GameControl.eventManager.RemoveListener<HabModuleSelected>(new EventManager.EventDelegate<HabModuleSelected>(this.HabModuleSelectedForTargeting), null);
		}
	}

	// Token: 0x060008E3 RID: 2275 RVA: 0x00029A0C File Offset: 0x00027C0C
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
			if (this.councilor.ref_hab != null)
			{
				return this.possibleTargets[0];
			}
			return base.GetDefaultTarget();
		}
	}

	// Token: 0x060008E4 RID: 2276 RVA: 0x00029A7A File Offset: 0x00027C7A
	public void HabModuleSelectedForTargeting(HabModuleSelected e)
	{
		base.SetTarget(e.module);
	}
}

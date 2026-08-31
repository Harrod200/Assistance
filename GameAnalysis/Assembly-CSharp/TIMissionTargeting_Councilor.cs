using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000280 RID: 640
public class TIMissionTargeting_Councilor : TIMissionTargeting
{
	// Token: 0x06000880 RID: 2176 RVA: 0x00027960 File Offset: 0x00025B60
	public override List<Type> TargetedGameStates()
	{
		return new List<Type> { typeof(TICouncilorState) };
	}

	// Token: 0x06000881 RID: 2177 RVA: 0x00027978 File Offset: 0x00025B78
	public override void Activate()
	{
		if (!base.activated)
		{
			base.SetActivation(this);
			this.actingCouncilor = this.councilor;
			this.possibleTargets = base.missionTemplate.target.GetValidTargets(base.missionTemplate, this.councilor);
			base.SetDefaultTarget();
			GameControl.eventManager.TriggerEvent(new TargetCouncilors(this.councilor, base.missionTemplate), null, Array.Empty<object>());
			GameControl.eventManager.AddListener<CouncilorMapItemSelected>(new EventManager.EventDelegate<CouncilorMapItemSelected>(this.CouncilorSelectedForTargeting), null, null, true, false);
		}
	}

	// Token: 0x06000882 RID: 2178 RVA: 0x00027A03 File Offset: 0x00025C03
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
			GameControl.eventManager.TriggerEvent(new DeTargetCouncilors(), null, Array.Empty<object>());
			GameControl.eventManager.RemoveListener<CouncilorMapItemSelected>(new EventManager.EventDelegate<CouncilorMapItemSelected>(this.CouncilorSelectedForTargeting), null);
		}
	}

	// Token: 0x06000883 RID: 2179 RVA: 0x00027A40 File Offset: 0x00025C40
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
			foreach (TIGameState tigameState in this.possibleTargets)
			{
				TICouncilorState ticouncilorState = tigameState as TICouncilorState;
				if (ticouncilorState != null && this.councilor.location == ticouncilorState.location)
				{
					return tigameState;
				}
			}
			return base.GetDefaultTarget();
		}
	}

	// Token: 0x06000884 RID: 2180 RVA: 0x00027B10 File Offset: 0x00025D10
	private void CouncilorSelectedForTargeting(CouncilorMapItemSelected e)
	{
		if (this.actingCouncilor.faction == e.councilor.faction)
		{
			GameControl.eventManager.ClearPendingEvents(e, null, Array.Empty<object>());
		}
		base.SetTarget(e.councilor);
	}

	// Token: 0x0400063F RID: 1599
	private TICouncilorState actingCouncilor;
}

using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000283 RID: 643
public class TIMissionTargeting_AlienAsset : TIMissionTargeting
{
	// Token: 0x06000892 RID: 2194 RVA: 0x00027F06 File Offset: 0x00026106
	public override List<Type> TargetedGameStates()
	{
		return new List<Type> { typeof(TIRegionAlienAssetState) };
	}

	// Token: 0x06000893 RID: 2195 RVA: 0x00027F20 File Offset: 0x00026120
	public override void Activate()
	{
		if (!base.activated)
		{
			base.SetActivation(this);
			this.possibleTargets = base.missionTemplate.target.GetValidTargets(base.missionTemplate, this.councilor);
			base.SetDefaultTarget();
			GameControl.eventManager.TriggerEvent(new CouncilorTargetAlienAsset(this.councilor, base.missionTemplate), null, Array.Empty<object>());
			GameControl.eventManager.AddListener<AlienAssetTargetSelected>(new EventManager.EventDelegate<AlienAssetTargetSelected>(this.AlienAssetSelectedForTargeting), null, null, true, false);
		}
	}

	// Token: 0x06000894 RID: 2196 RVA: 0x00027F9F File Offset: 0x0002619F
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
			GameControl.eventManager.TriggerEvent(new DeTargetAlienAssets(), null, Array.Empty<object>());
			GameControl.eventManager.RemoveListener<AlienAssetTargetSelected>(new EventManager.EventDelegate<AlienAssetTargetSelected>(this.AlienAssetSelectedForTargeting), null);
		}
	}

	// Token: 0x06000895 RID: 2197 RVA: 0x00027FDB File Offset: 0x000261DB
	private void AlienAssetSelectedForTargeting(AlienAssetTargetSelected e)
	{
		base.SetTarget(e.alienAsset);
	}

	// Token: 0x06000896 RID: 2198 RVA: 0x00027FEC File Offset: 0x000261EC
	public override TIGameState GetDefaultTarget()
	{
		ICollection<TIGameState> possibleTargets = this.possibleTargets;
		TIMissionState activeMission = this.councilor.activeMission;
		if (!possibleTargets.Contains((activeMission != null) ? activeMission.target : null))
		{
			foreach (TIGameState tigameState in this.possibleTargets)
			{
				if (tigameState.ref_region == this.councilor.location)
				{
					return tigameState.ref_regionAlienAsset;
				}
			}
			if (this.possibleTargets.Contains(GeneralControlsController.UIOtherSelectedState))
			{
				return GeneralControlsController.UIOtherSelectedState;
			}
			return base.GetDefaultTarget();
		}
		TIMissionState activeMission2 = this.councilor.activeMission;
		if (activeMission2 == null)
		{
			return null;
		}
		return activeMission2.target;
	}
}

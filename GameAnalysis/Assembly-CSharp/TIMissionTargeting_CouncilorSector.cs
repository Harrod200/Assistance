using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000290 RID: 656
public class TIMissionTargeting_CouncilorSector : TIMissionTargeting
{
	// Token: 0x060008EC RID: 2284 RVA: 0x00029BD8 File Offset: 0x00027DD8
	public override List<Type> TargetedGameStates()
	{
		return new List<Type>
		{
			typeof(TICouncilorState),
			typeof(TISectorState)
		};
	}

	// Token: 0x060008ED RID: 2285 RVA: 0x00029C00 File Offset: 0x00027E00
	public override void Activate()
	{
		if (!base.activated)
		{
			this.actingCouncilor = this.councilor;
			base.SetActivation(this);
			this.possibleTargets = base.missionTemplate.target.GetValidTargets(base.missionTemplate, this.councilor);
			base.SetDefaultTarget();
			GameControl.eventManager.AddListener<CouncilorMapItemSelected>(new EventManager.EventDelegate<CouncilorMapItemSelected>(this.CouncilorSelectedForTargeting), null, null, true, false);
			GameControl.eventManager.AddListener<SectorSelectedEvent>(new EventManager.EventDelegate<SectorSelectedEvent>(this.SectorSelectedForTargeting), null, null, true, false);
		}
	}

	// Token: 0x060008EE RID: 2286 RVA: 0x00029C84 File Offset: 0x00027E84
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
			GameControl.eventManager.RemoveListener<CouncilorMapItemSelected>(new EventManager.EventDelegate<CouncilorMapItemSelected>(this.CouncilorSelectedForTargeting), null);
			GameControl.eventManager.RemoveListener<SectorSelectedEvent>(new EventManager.EventDelegate<SectorSelectedEvent>(this.SectorSelectedForTargeting), null);
		}
	}

	// Token: 0x060008EF RID: 2287 RVA: 0x00029CC4 File Offset: 0x00027EC4
	public override TIGameState GetDefaultTarget()
	{
		ICollection<TIGameState> possibleTargets = this.possibleTargets;
		TIMissionState activeMission = this.councilor.activeMission;
		if (!possibleTargets.Contains((activeMission != null) ? activeMission.target : null))
		{
			if (this.councilor.ref_hab != null)
			{
				List<TIGameState> list = this.councilor.ref_hab.sectors.Intersect<TIGameState>(this.possibleTargets).ToList<TIGameState>();
				if (list.Any<TIGameState>())
				{
					return list[0];
				}
			}
			else if (this.councilor.ref_region != null)
			{
				List<TIGameState> list2 = this.councilor.ref_region.GetVisibleCouncilorsInRegion(this.councilor.faction).Intersect<TIGameState>(this.possibleTargets).ToList<TIGameState>();
				if (list2.Any<TIGameState>())
				{
					return list2[0];
				}
				list2 = this.councilor.ref_nation.GetVisibleCouncilorsInNation(this.councilor.faction).Intersect<TIGameState>(this.possibleTargets).ToList<TIGameState>();
				if (list2.Any<TIGameState>())
				{
					return list2[0];
				}
				list2 = this.councilor.ref_hab.CouncilorsPresentAndKnownToFaction(this.councilor.faction, false, null).Intersect<TIGameState>(this.possibleTargets).ToList<TIGameState>();
				if (list2.Any<TIGameState>())
				{
					return list2[0];
				}
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

	// Token: 0x060008F0 RID: 2288 RVA: 0x00029E25 File Offset: 0x00028025
	private void SectorSelectedForTargeting(SectorSelectedEvent e)
	{
		base.SetTarget(e.sector);
	}

	// Token: 0x060008F1 RID: 2289 RVA: 0x00029E33 File Offset: 0x00028033
	private void CouncilorSelectedForTargeting(CouncilorMapItemSelected e)
	{
		if (this.actingCouncilor == e.councilor)
		{
			GameControl.eventManager.ClearPendingEvents(e, null, Array.Empty<object>());
		}
		base.SetTarget(e.councilor);
	}

	// Token: 0x04000642 RID: 1602
	private TICouncilorState actingCouncilor;
}

using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200028F RID: 655
public class TIMissionTargeting_Sector : TIMissionTargeting
{
	// Token: 0x060008E6 RID: 2278 RVA: 0x00029A90 File Offset: 0x00027C90
	public override List<Type> TargetedGameStates()
	{
		return new List<Type> { typeof(TISectorState) };
	}

	// Token: 0x060008E7 RID: 2279 RVA: 0x00029AA8 File Offset: 0x00027CA8
	public override void Activate()
	{
		if (!base.activated)
		{
			base.SetActivation(this);
			this.possibleTargets = base.missionTemplate.target.GetValidTargets(base.missionTemplate, this.councilor);
			base.SetDefaultTarget();
			GameControl.eventManager.AddListener<SectorSelectedEvent>(new EventManager.EventDelegate<SectorSelectedEvent>(this.SectorSelectedForTargeting), null, null, true, false);
		}
	}

	// Token: 0x060008E8 RID: 2280 RVA: 0x00029B06 File Offset: 0x00027D06
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
			GameControl.eventManager.RemoveListener<SectorSelectedEvent>(new EventManager.EventDelegate<SectorSelectedEvent>(this.SectorSelectedForTargeting), null);
		}
	}

	// Token: 0x060008E9 RID: 2281 RVA: 0x00029B30 File Offset: 0x00027D30
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
			return base.GetDefaultTarget();
		}
		TIMissionState activeMission2 = this.councilor.activeMission;
		if (activeMission2 == null)
		{
			return null;
		}
		return activeMission2.target;
	}

	// Token: 0x060008EA RID: 2282 RVA: 0x00029BC2 File Offset: 0x00027DC2
	private void SectorSelectedForTargeting(SectorSelectedEvent e)
	{
		base.SetTarget(e.sector);
	}
}

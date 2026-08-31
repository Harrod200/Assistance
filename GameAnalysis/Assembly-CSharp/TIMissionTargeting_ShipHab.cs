using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200028C RID: 652
public class TIMissionTargeting_ShipHab : TIMissionTargeting
{
	// Token: 0x060008CF RID: 2255 RVA: 0x000293A0 File Offset: 0x000275A0
	public override List<Type> TargetedGameStates()
	{
		return new List<Type>
		{
			typeof(TISpaceShipState),
			typeof(TIHabState)
		};
	}

	// Token: 0x060008D0 RID: 2256 RVA: 0x000293C8 File Offset: 0x000275C8
	public override void Activate()
	{
		if (!base.activated)
		{
			base.SetActivation(this);
			this.possibleTargets = base.missionTemplate.target.GetValidTargets(base.missionTemplate, this.councilor);
			base.SetDefaultTarget();
			GameControl.eventManager.AddListener<HabSelectedEvent>(new EventManager.EventDelegate<HabSelectedEvent>(this.HabSelectedForTargeting), null, null, true, false);
			GameControl.eventManager.AddListener<FleetSelectedEvent>(new EventManager.EventDelegate<FleetSelectedEvent>(this.FleetSelectedForTargeting), null, null, true, false);
			GameControl.eventManager.AddListener<ShipSelectedEvent>(new EventManager.EventDelegate<ShipSelectedEvent>(this.ShipSelectedForTargeting), null, null, true, false);
		}
	}

	// Token: 0x060008D1 RID: 2257 RVA: 0x0002945A File Offset: 0x0002765A
	private void HabSelectedForTargeting(HabSelectedEvent e)
	{
		base.SetTarget(e.hab);
	}

	// Token: 0x060008D2 RID: 2258 RVA: 0x00029468 File Offset: 0x00027668
	public static TISpaceShipState GetBestShipForCouncilor(TISpaceFleetState fleet)
	{
		IEnumerable<TISpaceShipState> enumerable = fleet.ships.Where<TISpaceShipState>((TISpaceShipState x) => x.role == ShipRole.CouncilorTransport);
		if (enumerable.Any<TISpaceShipState>())
		{
			return enumerable.MaxBy<TISpaceShipState, float>((TISpaceShipState x) => x.noseArmorValue);
		}
		IEnumerable<TISpaceShipState> enumerable2 = fleet.ships.Where<TISpaceShipState>((TISpaceShipState x) => x.role == ShipRole.TroopCarrier);
		if (enumerable2.Any<TISpaceShipState>())
		{
			return enumerable2.MaxBy<TISpaceShipState, float>((TISpaceShipState x) => x.noseArmorValue);
		}
		return fleet.ships.MaxBy<TISpaceShipState, float>((TISpaceShipState x) => x.noseArmorValue);
	}

	// Token: 0x060008D3 RID: 2259 RVA: 0x00029551 File Offset: 0x00027751
	private void FleetSelectedForTargeting(FleetSelectedEvent e)
	{
		base.SetTarget(TIMissionTargeting_ShipHab.GetBestShipForCouncilor(e.fleet));
	}

	// Token: 0x060008D4 RID: 2260 RVA: 0x00029564 File Offset: 0x00027764
	private void ShipSelectedForTargeting(ShipSelectedEvent e)
	{
		base.SetTarget(e.ship);
	}

	// Token: 0x060008D5 RID: 2261 RVA: 0x00029574 File Offset: 0x00027774
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
			TISpaceShipState tispaceShipState = this.councilor.location as TISpaceShipState;
			if (tispaceShipState != null && this.possibleTargets.Contains(tispaceShipState))
			{
				return tispaceShipState;
			}
			TISpaceFleetState ref_fleet = this.councilor.location.ref_fleet;
			if (ref_fleet != null && (this.possibleTargets.Contains(ref_fleet) || this.possibleTargets.Contains(ref_fleet.ships[0])))
			{
				return ref_fleet.ships[0];
			}
			TIHabState ref_hab = this.councilor.location.ref_hab;
			if (ref_hab != null && this.possibleTargets.Contains(ref_hab))
			{
				return ref_hab;
			}
			return base.GetDefaultTarget();
		}
	}

	// Token: 0x060008D6 RID: 2262 RVA: 0x00029664 File Offset: 0x00027864
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
			GameControl.eventManager.RemoveListener<HabSelectedEvent>(new EventManager.EventDelegate<HabSelectedEvent>(this.HabSelectedForTargeting), null);
			GameControl.eventManager.RemoveListener<FleetSelectedEvent>(new EventManager.EventDelegate<FleetSelectedEvent>(this.FleetSelectedForTargeting), null);
			GameControl.eventManager.RemoveListener<ShipSelectedEvent>(new EventManager.EventDelegate<ShipSelectedEvent>(this.ShipSelectedForTargeting), null);
		}
	}
}

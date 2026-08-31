using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002F2 RID: 754
public class TIOperationTargeting_FleetDestination : TIOperationTargeting
{
	// Token: 0x06000B5E RID: 2910 RVA: 0x0003DAC7 File Offset: 0x0003BCC7
	public override List<Type> TargetedGameStates()
	{
		return new List<Type>
		{
			typeof(TIHabState),
			typeof(TISpaceFleetState),
			typeof(TIOrbitState)
		};
	}

	// Token: 0x06000B5F RID: 2911 RVA: 0x0003DAFE File Offset: 0x0003BCFE
	public override OperationTargetingUIType UIType()
	{
		return OperationTargetingUIType.Transfer;
	}

	// Token: 0x06000B60 RID: 2912 RVA: 0x0003DB01 File Offset: 0x0003BD01
	public override void Init(IOperation operationType, TIGameState actorState, TIGameState defaultTarget = null)
	{
		this.operationType = operationType;
		this.actorState = actorState;
		this.fleet = actorState as TISpaceFleetState;
		this.faction = this.fleet.faction;
		this.possibleTargets = operationType.GetPossibleTargets(actorState, defaultTarget);
	}

	// Token: 0x06000B61 RID: 2913 RVA: 0x0003DB3C File Offset: 0x0003BD3C
	public override void Activate(TIGameState forceTarget = null)
	{
		if (!base.activated)
		{
			base.SetActivation(this);
			base.SetDefaultTarget(forceTarget);
			GameControl.eventManager.TriggerEvent(new FleetTargetDestination(this.fleet), null, Array.Empty<object>());
			GameControl.eventManager.AddListener<FleetSelectedEvent>(new EventManager.EventDelegate<FleetSelectedEvent>(this.FleetSelectedForDestinationTargeting), null, null, true, false);
			GameControl.eventManager.AddListener<HabSelectedEvent>(new EventManager.EventDelegate<HabSelectedEvent>(this.HabSelectedForDestinationTargeting), null, null, true, false);
			GameControl.eventManager.AddListener<SpaceBodySelectedEvent>(new EventManager.EventDelegate<SpaceBodySelectedEvent>(this.SpaceBodySelectedForDestinationTargeting), null, null, true, false);
			GameControl.eventManager.AddListener<LagrangePointSelectedEvent>(new EventManager.EventDelegate<LagrangePointSelectedEvent>(this.LagrangePointSelectedForDestinationTargeting), null, null, true, false);
		}
	}

	// Token: 0x06000B62 RID: 2914 RVA: 0x0003DBE8 File Offset: 0x0003BDE8
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
			GameControl.eventManager.TriggerEvent(new DeTargetFleetDestinations(), null, Array.Empty<object>());
			GameControl.eventManager.TriggerEvent(new DeTargetOrbits(this.faction), null, Array.Empty<object>());
			GameControl.eventManager.RemoveListener<FleetSelectedEvent>(new EventManager.EventDelegate<FleetSelectedEvent>(this.FleetSelectedForDestinationTargeting), null);
			GameControl.eventManager.RemoveListener<HabSelectedEvent>(new EventManager.EventDelegate<HabSelectedEvent>(this.HabSelectedForDestinationTargeting), null);
			GameControl.eventManager.RemoveListener<SpaceBodySelectedEvent>(new EventManager.EventDelegate<SpaceBodySelectedEvent>(this.SpaceBodySelectedForDestinationTargeting), null);
			GameControl.eventManager.RemoveListener<LagrangePointSelectedEvent>(new EventManager.EventDelegate<LagrangePointSelectedEvent>(this.LagrangePointSelectedForDestinationTargeting), null);
			GameControl.eventManager.RemoveListener<OrbitSelectedEvent>(new EventManager.EventDelegate<OrbitSelectedEvent>(this.OrbitSelectedForTargeting), null);
		}
	}

	// Token: 0x06000B63 RID: 2915 RVA: 0x0003DCA9 File Offset: 0x0003BEA9
	private void FleetSelectedForDestinationTargeting(FleetSelectedEvent e)
	{
		base.AttemptSetTarget(e.fleet);
	}

	// Token: 0x06000B64 RID: 2916 RVA: 0x0003DCB7 File Offset: 0x0003BEB7
	private void HabSelectedForDestinationTargeting(HabSelectedEvent e)
	{
		base.AttemptSetTarget(e.hab);
	}

	// Token: 0x06000B65 RID: 2917 RVA: 0x0003DCC8 File Offset: 0x0003BEC8
	private void SpaceBodySelectedForDestinationTargeting(SpaceBodySelectedEvent e)
	{
		List<TIGameState> list = this.possibleTargets.Intersect<TIGameState>(e.spaceBody.orbits).ToList<TIGameState>();
		if (list.Count >= 1)
		{
			this.currentTarget = list[0];
			GameControl.eventManager.TriggerEvent(new OperationTargettedEvent(this.currentTarget, this.actorState), null, Array.Empty<object>());
			if (list.Count > 1)
			{
				GameControl.eventManager.TriggerEvent(new TargetOrbits(this.actorState, e.spaceBody), null, Array.Empty<object>());
				GameControl.eventManager.AddListener<OrbitSelectedEvent>(new EventManager.EventDelegate<OrbitSelectedEvent>(this.OrbitSelectedForTargeting), null, null, true, false);
			}
		}
	}

	// Token: 0x06000B66 RID: 2918 RVA: 0x0003DD6C File Offset: 0x0003BF6C
	private void LagrangePointSelectedForDestinationTargeting(LagrangePointSelectedEvent e)
	{
		List<TIGameState> list = this.possibleTargets.Intersect<TIGameState>(e.lagrangePoint.orbits).ToList<TIGameState>();
		if (list.Count >= 1)
		{
			this.currentTarget = list[0];
			GameControl.eventManager.TriggerEvent(new OperationTargettedEvent(this.currentTarget, this.actorState), null, Array.Empty<object>());
			if (list.Count > 1)
			{
				GameControl.eventManager.TriggerEvent(new TargetOrbits(this.actorState, e.lagrangePoint), null, Array.Empty<object>());
				GameControl.eventManager.AddListener<OrbitSelectedEvent>(new EventManager.EventDelegate<OrbitSelectedEvent>(this.OrbitSelectedForTargeting), null, null, true, false);
			}
		}
	}

	// Token: 0x06000B67 RID: 2919 RVA: 0x0003DE10 File Offset: 0x0003C010
	private void OrbitSelectedForTargeting(OrbitSelectedEvent e)
	{
		base.AttemptSetTarget(e.orbit);
	}

	// Token: 0x06000B68 RID: 2920 RVA: 0x0003DE20 File Offset: 0x0003C020
	public override TIGameState GetDefaultTarget()
	{
		if (this.possibleTargets.Contains(GeneralControlsController.UIOtherSelectedState))
		{
			return GeneralControlsController.UIOtherSelectedState;
		}
		TIGameState uiotherSelectedState = GeneralControlsController.UIOtherSelectedState;
		if (uiotherSelectedState != null && uiotherSelectedState.isNaturalSpaceObjectState && GeneralControlsController.UIOtherSelectedState.ref_naturalSpaceObject.orbits.Count > 0)
		{
			TIOrbitState tiorbitState = GeneralControlsController.UIOtherSelectedState.ref_naturalSpaceObject.orbits.FirstOrDefault<TIOrbitState>((TIOrbitState x) => x != this.fleet.orbitState);
			if (tiorbitState != null && this.possibleTargets.Contains(tiorbitState))
			{
				return tiorbitState;
			}
		}
		if (this.fleet.homeport != null && !this.fleet.homeport.deleted && this.fleet.dockedLocation != this.fleet.homeport)
		{
			if (this.fleet.homeport.IsBase)
			{
				TIOrbitState tiorbitState2 = this.fleet.homeport.ref_naturalSpaceObject.orbits.FirstOrDefault<TIOrbitState>((TIOrbitState x) => x.interfaceOrbit);
				if (tiorbitState2 != null && this.possibleTargets.Contains(tiorbitState2))
				{
					return tiorbitState2;
				}
			}
			else if (this.possibleTargets.Contains(this.fleet.homeport))
			{
				return this.fleet.homeport;
			}
		}
		TINaturalSpaceObjectState tinaturalSpaceObjectState = ((this.fleet.orbitState != null) ? this.fleet.orbitState.barycenter : this.fleet.trajectory.GetBarycenterAtTime(TITimeState.Now()));
		if (tinaturalSpaceObjectState.orbits.Count > 1)
		{
			using (List<TIOrbitState>.Enumerator enumerator = tinaturalSpaceObjectState.orbits.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIOrbitState tiorbitState3 = enumerator.Current;
					if (this.fleet.orbitState != tiorbitState3 && this.possibleTargets.Contains(tiorbitState3))
					{
						return tiorbitState3;
					}
				}
				goto IL_0280;
			}
		}
		TINaturalSpaceObjectState barycenter = tinaturalSpaceObjectState.barycenter;
		if (barycenter != null && barycenter.orbits.Count > 0)
		{
			using (List<TIOrbitState>.Enumerator enumerator = tinaturalSpaceObjectState.barycenter.orbits.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIOrbitState tiorbitState4 = enumerator.Current;
					if (this.possibleTargets.Contains(tiorbitState4))
					{
						return tiorbitState4;
					}
				}
				goto IL_0280;
			}
		}
		if (this.possibleTargets.Contains(GameStateManager.LEOStates().First<TIOrbitState>()))
		{
			return GameStateManager.LEOStates().First<TIOrbitState>();
		}
		IL_0280:
		return this.possibleTargets[0];
	}

	// Token: 0x04000E91 RID: 3729
	private TISpaceFleetState fleet;

	// Token: 0x04000E92 RID: 3730
	private TIFactionState faction;
}

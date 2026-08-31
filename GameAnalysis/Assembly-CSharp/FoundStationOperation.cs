using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000348 RID: 840
public abstract class FoundStationOperation : FoundHabOperation
{
	// Token: 0x06000E95 RID: 3733 RVA: 0x000490DC File Offset: 0x000472DC
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		TINaturalSpaceObjectState ref_naturalSpaceObject = targetState.ref_naturalSpaceObject;
		if (ref_naturalSpaceObject != null && ref_naturalSpaceObject.orbits != null && ref_naturalSpaceObject.orbits.Count > 0 && TIEffectsState.CheckForAnyEffectInContext(this.GetRequiredConstructionTechEffectContext(), actorState))
		{
			TIFactionState faction = actorState.ref_faction;
			return ref_naturalSpaceObject.orbits.Any<TIOrbitState>((TIOrbitState x) => x.NewStationAllowed(this.GetTier(), null) && faction.EligibleforColonization(x));
		}
		return false;
	}

	// Token: 0x06000E96 RID: 3734 RVA: 0x00049150 File Offset: 0x00047350
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget)
	{
		List<TIOrbitState> orbits = defaultTarget.ref_naturalSpaceObject.orbits;
		List<TIGameState> list;
		if (orbits == null)
		{
			list = null;
		}
		else
		{
			IEnumerable<TIOrbitState> enumerable = orbits.Where<TIOrbitState>((TIOrbitState x) => x.NewStationAllowed(this.GetTier(), null));
			if (enumerable == null)
			{
				list = null;
			}
			else
			{
				list = enumerable.ToList<TIOrbitState>().ConvertAll<TIGameState>((TIOrbitState x) => x);
			}
		}
		return list ?? new List<TIGameState>();
	}

	// Token: 0x06000E97 RID: 3735 RVA: 0x000491B9 File Offset: 0x000473B9
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_OrbitMany);
	}

	// Token: 0x06000E98 RID: 3736 RVA: 0x000491C5 File Offset: 0x000473C5
	public override bool OnOperationConfirm(TIGameState actorState, TIGameState target, TIResourcesCost resourcesCost, Trajectory trajectory = null)
	{
		if (resourcesCost == null)
		{
			Log.Error("Null resources cost passed to FoundStationOperation.OnOperationConfirm", Array.Empty<object>());
			return false;
		}
		this.deliveryDuration_days = resourcesCost.completionTime_days;
		if (base.OnOperationConfirm(actorState, target, resourcesCost, trajectory))
		{
			target.ref_orbit.MarkPendingHab();
			return true;
		}
		return false;
	}

	// Token: 0x06000E99 RID: 3737 RVA: 0x00049202 File Offset: 0x00047402
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		target.ref_orbit.FoundHab();
		base.FoundHab(actorState.ref_faction, target.ref_orbit, this.GetTier());
	}
}

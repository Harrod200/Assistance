using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Systems.GameTime;

// Token: 0x02000343 RID: 835
public class LaunchOverrideProbeOperation : LaunchProbeOperation
{
	// Token: 0x06000E73 RID: 3699 RVA: 0x00048700 File Offset: 0x00046900
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		TIFactionState ref_faction = actorState.ref_faction;
		return !ref_faction.IsAlienFaction && targetState.isSpaceBodyState && ref_faction.CanOvertakeProbeWithProbe(targetState.ref_spaceBody).Count > 0;
	}

	// Token: 0x06000E74 RID: 3700 RVA: 0x0004873C File Offset: 0x0004693C
	public override List<TIResourcesCost> ResourceCostOptions(TIFactionState faction, TIGameState target, TIGameState actor, bool checkCanAfford = true)
	{
		List<TIResourcesCost> list = faction.CanOvertakeProbeWithProbe(target.ref_spaceBody);
		if (checkCanAfford)
		{
			list = list.Where<TIResourcesCost>((TIResourcesCost x) => x.CanAfford(faction, 1f, null, float.PositiveInfinity)).ToList<TIResourcesCost>();
		}
		if (list.Count > 1)
		{
			list = (from x in list
				orderby x.completionTime_days, x.GetSingleCostValue(FactionResource.Boost)
				select x).ToList<TIResourcesCost>();
		}
		return list;
	}

	// Token: 0x06000E75 RID: 3701 RVA: 0x000487DD File Offset: 0x000469DD
	public override string GetDescription(TIGameState actorState = null, TIGameState target = null)
	{
		return new StringBuilder(Loc.T(new StringBuilder(base.GetType().Name).Append(".description").ToString())).ToString();
	}

	// Token: 0x06000E76 RID: 3702 RVA: 0x00048810 File Offset: 0x00046A10
	public override bool OnOperationConfirm(TIGameState actorState, TIGameState target, TIResourcesCost resourcesCost, Trajectory trajectory)
	{
		TIFactionState ref_faction = actorState.ref_faction;
		TISpaceBodyState ref_spaceBody = target.ref_spaceBody;
		GameTimeManager.Singleton.timeQueue.CancelEvent(ref_faction.factionOperationCompleteName, ref_faction, ref_spaceBody, OperationsManager.operationsLookup[typeof(LaunchProbeOperation)].GetTemplate().dataName, ref_faction.ProspectorArrival(ref_spaceBody));
		GameTimeManager.Singleton.timeQueue.CancelEvent(ref_faction.factionOperationCompleteName, ref_faction, ref_spaceBody, OperationsManager.operationsLookup[typeof(LaunchOverrideProbeOperation)].GetTemplate().dataName, ref_faction.ProspectorArrival(ref_spaceBody));
		base.OnOperationConfirm_Base(actorState, target, resourcesCost, trajectory);
		ref_faction.LaunchProspector(ref_spaceBody);
		GameControl.eventManager.TriggerEvent(new ProspectingBody(actorState.ref_faction, target.ref_spaceBody), null, new object[] { actorState.ref_faction, target.ref_spaceBody });
		TINotificationQueueState.LogProbeLaunched(ref_faction, ref_spaceBody);
		TINotificationQueueState.LogEnemyProbeLaunched(ref_faction, ref_spaceBody);
		return true;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001C0 RID: 448
public class TIMissionCondition_SeizeSpaceAssetTroopsPresent : TIMissionCondition
{
	// Token: 0x06000660 RID: 1632 RVA: 0x0001CEEC File Offset: 0x0001B0EC
	public static bool AvailableAssaultAssetsAtLocation(TISpaceAssetState asset, TICouncilorState actingCouncilor)
	{
		TIFactionState assaultingFaction = ((actingCouncilor != null) ? actingCouncilor.faction : asset.ref_faction);
		TIHabState ref_hab = asset.ref_hab;
		List<TISpaceFleetState> list = new List<TISpaceFleetState>();
		List<TICouncilorState> list2 = new List<TICouncilorState>();
		if (ref_hab != null)
		{
			list = ref_hab.dockedFleets.Where<TISpaceFleetState>((TISpaceFleetState x) => x.faction == assaultingFaction && x.AssaultCombatValue(false) > 0f).ToList<TISpaceFleetState>();
		}
		else if (asset.ref_fleet.AssaultCombatValue(false) > 0f)
		{
			list.Add(asset.ref_fleet);
		}
		bool flag = ref_hab != null && ref_hab.DropTroops(assaultingFaction);
		if (!flag && list.Count == 0)
		{
			return false;
		}
		if (ref_hab != null)
		{
			list2.AddRange(ref_hab.councilorsPresent(assaultingFaction));
		}
		Func<TICouncilorState, bool> <>9__2;
		foreach (TISpaceFleetState tispaceFleetState in list)
		{
			List<TICouncilorState> list3 = list2;
			IEnumerable<TICouncilorState> councilorPassengers = tispaceFleetState.councilorPassengers;
			Func<TICouncilorState, bool> func;
			if ((func = <>9__2) == null)
			{
				func = (<>9__2 = (TICouncilorState x) => x.faction == assaultingFaction);
			}
			list3.AddRange(councilorPassengers.Where<TICouncilorState>(func));
		}
		list2 = list2.Where<TICouncilorState>((TICouncilorState x) => x != null).Distinct<TICouncilorState>().ToList<TICouncilorState>();
		list2.Remove(actingCouncilor);
		foreach (TICouncilorState ticouncilorState in list2)
		{
			if (ticouncilorState.HasMission && (ticouncilorState.activeMission.missionTemplate == TIFactionState.seizeHabMission || ticouncilorState.activeMission.missionTemplate == TIFactionState.controlHabMission))
			{
				return false;
			}
		}
		foreach (TISpaceFleetState tispaceFleetState2 in list)
		{
			if (tispaceFleetState2.CurrentOperations().Any<OperationData>((OperationData x) => x.operation is AssaultHabOperation))
			{
				return false;
			}
		}
		return flag || list.Count > 0;
	}

	// Token: 0x06000661 RID: 1633 RVA: 0x0001D148 File Offset: 0x0001B348
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (councilor.OnAShip && councilor.ref_fleet.faction == councilor.faction && TIMissionCondition_SeizeSpaceAssetTroopsPresent.AvailableAssaultAssetsAtLocation(councilor.ref_fleet, councilor))
		{
			return "_Pass";
		}
		if (councilor.InAHab && TIMissionCondition_SeizeSpaceAssetTroopsPresent.AvailableAssaultAssetsAtLocation(councilor.ref_hab, councilor))
		{
			return "_Pass";
		}
		return "TIMissionCondition_SeizeSpaceAssetTroopsPresent";
	}
}

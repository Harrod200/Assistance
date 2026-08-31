using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001C2 RID: 450
public class TIMissionCondition_SeizeSpaceAssetTargetInRange_IncludeAdjacentStations : TIMissionCondition
{
	// Token: 0x06000665 RID: 1637 RVA: 0x0001D330 File Offset: 0x0001B530
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (possibleTarget.isHabState)
		{
			TIHabState ref_hab = possibleTarget.ref_hab;
			if (councilor.ref_hab == ref_hab)
			{
				return "_Pass";
			}
			TIHabState ref_hab2 = councilor.ref_hab;
			bool? flag;
			if (ref_hab2 == null)
			{
				flag = null;
			}
			else
			{
				TIHabSiteState habSite = ref_hab2.habSite;
				flag = ((habSite != null) ? new bool?(habSite.AdjacentSites().Contains(possibleTarget)) : null);
			}
			bool? flag2 = flag;
			if (flag2.GetValueOrDefault())
			{
				return "_Pass";
			}
			if (councilor.OnAShip)
			{
				TIOrbitState orbitState = councilor.ref_fleet.orbitState;
				if (orbitState != null && orbitState.interfaceOrbit)
				{
					TIGameState ref_spaceBody = councilor.ref_fleet.ref_spaceBody;
					TIHabSiteState habSite2 = ref_hab.habSite;
					if (ref_spaceBody == ((habSite2 != null) ? habSite2.parentBody : null))
					{
						return "_Pass";
					}
				}
			}
			TIHabState ref_hab3 = councilor.ref_hab;
			if (ref_hab3 != null && ref_hab3.IsStation && ref_hab.IsBase && councilor.ref_hab.DropTroops(councilor.faction))
			{
				TIOrbitState orbitState2 = councilor.ref_hab.orbitState;
				if (orbitState2 != null && orbitState2.interfaceOrbit && councilor.ref_hab.ref_spaceBody == ref_hab.habSite.parentBody)
				{
					return "_Pass";
				}
			}
			TIHabState ref_hab4 = councilor.ref_hab;
			if (ref_hab4 != null && ref_hab4.IsStation && ref_hab.IsStation && councilor.ref_hab.DropTroops(councilor.faction) && councilor.ref_hab.orbitState == ref_hab.orbitState)
			{
				return "_Pass";
			}
		}
		return "TIMissionCondition_SeizeSpaceAssetTargetInRange_IncludeAdjacentStations";
	}
}

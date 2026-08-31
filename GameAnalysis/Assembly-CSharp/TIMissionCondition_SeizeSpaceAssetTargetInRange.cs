using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001C1 RID: 449
public class TIMissionCondition_SeizeSpaceAssetTargetInRange : TIMissionCondition
{
	// Token: 0x06000663 RID: 1635 RVA: 0x0001D1B4 File Offset: 0x0001B3B4
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (possibleTarget.isHabState)
		{
			TIHabState ref_hab = possibleTarget.ref_hab;
			if (ref_hab.IsAlien() && ref_hab.IsBase && ref_hab == ref_hab.faction.primaryHab)
			{
				return "TIMissionCondition_GenericFail";
			}
			if (ref_hab.underAssault)
			{
				return "TIMissionCondition_SeizeSpaceAssetTargetInRange_2";
			}
			if (councilor.ref_hab == ref_hab)
			{
				return "_Pass";
			}
			if (ref_hab.IsBase)
			{
				TIHabState ref_hab2 = councilor.ref_hab;
				bool? flag;
				if (ref_hab2 == null)
				{
					flag = null;
				}
				else
				{
					TIHabSiteState habSite = ref_hab2.habSite;
					flag = ((habSite != null) ? new bool?(habSite.AdjacentSites().Contains(ref_hab.ref_habSite)) : null);
				}
				bool? flag2 = flag;
				if (flag2.GetValueOrDefault())
				{
					return "_Pass";
				}
				if (councilor.OnAShip)
				{
					TIOrbitState orbitState = councilor.ref_fleet.orbitState;
					if (orbitState != null && orbitState.interfaceOrbit && councilor.ref_fleet.ref_spaceBody == ref_hab.habSite.parentBody)
					{
						return "_Pass";
					}
				}
				TIHabState ref_hab3 = councilor.ref_hab;
				if (ref_hab3 != null && ref_hab3.IsStation && councilor.ref_hab.DropTroops(councilor.faction))
				{
					TIOrbitState orbitState2 = councilor.ref_hab.orbitState;
					if (orbitState2 != null && orbitState2.interfaceOrbit && councilor.ref_hab.ref_spaceBody == ref_hab.habSite.parentBody)
					{
						return "_Pass";
					}
				}
			}
		}
		return "TIMissionCondition_SeizeSpaceAssetTargetInRange";
	}
}

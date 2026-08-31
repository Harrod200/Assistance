using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001C6 RID: 454
public class TIMissionCondition_AlienPrimaryBaseInRange : TIMissionCondition
{
	// Token: 0x0600066D RID: 1645 RVA: 0x0001D634 File Offset: 0x0001B834
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (possibleTarget.isHabModuleState)
		{
			TIHabState ref_hab = possibleTarget.ref_hab;
			if (ref_hab.IsAlien() && ref_hab.IsBase && ref_hab == ref_hab.faction.primaryHab)
			{
				List<TIHabModuleState> list = ref_hab.OkayModules();
				if (list.None<TIHabModuleState>((TIHabModuleState x) => x.isCombatModule))
				{
					if (list.Count<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.Salamanders)) <= 2)
					{
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
						if (ref_hab3 != null && ref_hab3.IsStation)
						{
							TIOrbitState orbitState2 = councilor.ref_hab.orbitState;
							if (orbitState2 != null && orbitState2.interfaceOrbit && councilor.ref_hab.ref_spaceBody == ref_hab.habSite.parentBody)
							{
								return "_Pass";
							}
						}
					}
				}
			}
			return "TIMissionCondition_AlienPrimaryBaseInRange";
		}
		return "TIMissionCondition_GenericFail";
	}
}

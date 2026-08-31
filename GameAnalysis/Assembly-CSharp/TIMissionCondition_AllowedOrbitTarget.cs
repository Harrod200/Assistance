using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000199 RID: 409
public class TIMissionCondition_AllowedOrbitTarget : TIMissionCondition
{
	// Token: 0x0600060B RID: 1547 RVA: 0x0001BB88 File Offset: 0x00019D88
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		string text;
		if (councilor.ValidDestination(TIUtilities.ObjectToExactLocation(possibleTarget), out text))
		{
			if (councilor.OnEarth)
			{
				TIOrbitState ref_orbit = possibleTarget.ref_orbit;
				if (ref_orbit != null && ref_orbit.barycenter.isEarth)
				{
					return "_Pass";
				}
				TIOrbitState ref_orbit2 = possibleTarget.ref_orbit;
				bool? flag;
				if (ref_orbit2 == null)
				{
					flag = null;
				}
				else
				{
					TINaturalSpaceObjectState barycenter = ref_orbit2.barycenter.barycenter;
					flag = ((barycenter != null) ? new bool?(barycenter.isEarth) : null);
				}
				bool? flag2 = flag;
				if (flag2.GetValueOrDefault())
				{
					return "_Pass";
				}
			}
			else if (councilor.AtABase)
			{
				TIOrbitState ref_orbit3 = possibleTarget.ref_orbit;
				TIGameState tigameState = ((ref_orbit3 != null) ? ref_orbit3.barycenter : null);
				TIHabState ref_hab = councilor.location.ref_hab;
				TIGameState tigameState2;
				if (ref_hab == null)
				{
					tigameState2 = null;
				}
				else
				{
					TIHabSiteState ref_habSite = ref_hab.ref_habSite;
					tigameState2 = ((ref_habSite != null) ? ref_habSite.parentBody : null);
				}
				if (tigameState == tigameState2)
				{
					TIOrbitState ref_orbit4 = possibleTarget.ref_orbit;
					if (ref_orbit4 != null && ref_orbit4.interfaceOrbit)
					{
						return "_Pass";
					}
				}
			}
			return base.GetType().Name;
		}
		return text;
	}
}

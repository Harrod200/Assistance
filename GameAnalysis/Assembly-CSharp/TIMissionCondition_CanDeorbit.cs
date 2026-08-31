using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200019D RID: 413
public class TIMissionCondition_CanDeorbit : TIMissionCondition
{
	// Token: 0x06000613 RID: 1555 RVA: 0x0001BF34 File Offset: 0x0001A134
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		TIOrbitState ref_orbit = councilor.ref_orbit;
		if (ref_orbit != null && ref_orbit.interfaceOrbit)
		{
			if (!councilor.isAlien)
			{
				return "_Pass";
			}
			if (!councilor.ref_orbit.barycenter.GetSunOrbitingRelatedObject.isEarth)
			{
				return "_Pass";
			}
		}
		if (councilor.isHuman)
		{
			TINaturalSpaceObjectState ref_naturalSpaceObject = councilor.ref_naturalSpaceObject;
			bool? flag;
			if (ref_naturalSpaceObject == null)
			{
				flag = null;
			}
			else
			{
				TINaturalSpaceObjectState barycenter = ref_naturalSpaceObject.barycenter;
				flag = ((barycenter != null) ? new bool?(barycenter.isEarth) : null);
			}
			bool? flag2 = flag;
			if (flag2.GetValueOrDefault())
			{
				return "_Pass";
			}
			TIOrbitState ref_orbit2 = councilor.ref_orbit;
			bool? flag3;
			if (ref_orbit2 == null)
			{
				flag3 = null;
			}
			else
			{
				TISpaceBodyState ref_spaceBody = ref_orbit2.ref_spaceBody;
				flag3 = ((ref_spaceBody != null) ? new bool?(ref_spaceBody.isEarth) : null);
			}
			flag2 = flag3;
			if (flag2.GetValueOrDefault())
			{
				return "_Pass";
			}
			TIOrbitState ref_orbit3 = councilor.ref_orbit;
			bool? flag4;
			if (ref_orbit3 == null)
			{
				flag4 = null;
			}
			else
			{
				TINaturalSpaceObjectState ref_naturalSpaceObject2 = ref_orbit3.ref_naturalSpaceObject;
				if (ref_naturalSpaceObject2 == null)
				{
					flag4 = null;
				}
				else
				{
					TINaturalSpaceObjectState barycenter2 = ref_naturalSpaceObject2.barycenter;
					flag4 = ((barycenter2 != null) ? new bool?(barycenter2.isEarth) : null);
				}
			}
			flag2 = flag4;
			if (flag2.GetValueOrDefault())
			{
				return "_Pass";
			}
		}
		return base.GetType().Name;
	}
}

using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200019C RID: 412
public class TIMissionCondition_AllowedDeorbitTarget : TIMissionCondition
{
	// Token: 0x06000611 RID: 1553 RVA: 0x0001BE0C File Offset: 0x0001A00C
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		bool flag = false;
		TIOrbitState ref_orbit = councilor.ref_orbit;
		if (((ref_orbit != null) ? ref_orbit.barycenter : null) == possibleTarget.ref_spaceBody)
		{
			flag = true;
		}
		TIOrbitState ref_orbit2 = councilor.ref_orbit;
		if (ref_orbit2 != null && ref_orbit2.barycenter.isEarth)
		{
			TISpaceBodyState ref_spaceBody = possibleTarget.ref_spaceBody;
			if (ref_spaceBody != null && ref_spaceBody.isEarth)
			{
				flag = true;
			}
		}
		TIOrbitState ref_orbit3 = councilor.ref_orbit;
		bool? flag2;
		if (ref_orbit3 == null)
		{
			flag2 = null;
		}
		else
		{
			TINaturalSpaceObjectState barycenter = ref_orbit3.barycenter;
			if (barycenter == null)
			{
				flag2 = null;
			}
			else
			{
				TINaturalSpaceObjectState barycenter2 = barycenter.barycenter;
				flag2 = ((barycenter2 != null) ? new bool?(barycenter2.isEarth) : null);
			}
		}
		bool? flag3 = flag2;
		if (flag3.GetValueOrDefault())
		{
			TISpaceBodyState ref_spaceBody2 = possibleTarget.ref_spaceBody;
			if (ref_spaceBody2 != null && ref_spaceBody2.isEarth)
			{
				flag = true;
			}
		}
		if (councilor.ref_habSite != null && councilor.ref_habSite.ref_spaceBody.isLuna)
		{
			TISpaceBodyState ref_spaceBody3 = possibleTarget.ref_spaceBody;
			if (ref_spaceBody3 != null && ref_spaceBody3.isEarth)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			return base.GetType().Name;
		}
		string text;
		if (councilor.ValidDestination(possibleTarget, out text))
		{
			return "_Pass";
		}
		return text;
	}
}

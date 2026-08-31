using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200019B RID: 411
public class TIMissionCondition_AllowedTransferTarget : TIMissionCondition
{
	// Token: 0x0600060F RID: 1551 RVA: 0x0001BCF8 File Offset: 0x00019EF8
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		string text;
		if (!councilor.ValidDestination(TIUtilities.ObjectToExactLocation(possibleTarget), out text))
		{
			return text;
		}
		bool flag;
		if (councilor.ref_orbit != null)
		{
			TISpaceObjectState sunOrbitingRelatedObject_static = TISpaceObjectState.GetSunOrbitingRelatedObject_static(councilor.location.ref_naturalSpaceObject);
			if (sunOrbitingRelatedObject_static != null && sunOrbitingRelatedObject_static.isEarth)
			{
				flag = true;
				goto IL_0059;
			}
		}
		TIHabSiteState ref_habSite = councilor.ref_habSite;
		flag = ref_habSite != null && ref_habSite.parentBody.isLuna;
		IL_0059:
		bool flag2;
		if (possibleTarget.ref_orbit != null)
		{
			TISpaceObjectState sunOrbitingRelatedObject_static2 = TISpaceObjectState.GetSunOrbitingRelatedObject_static(possibleTarget.ref_naturalSpaceObject);
			flag2 = sunOrbitingRelatedObject_static2 != null && sunOrbitingRelatedObject_static2.isEarth;
		}
		else
		{
			flag2 = false;
		}
		bool flag3 = flag2;
		if (flag && flag3)
		{
			return "_Pass";
		}
		if (councilor.AtABase)
		{
			TIHabSiteState ref_habSite2 = possibleTarget.ref_habSite;
			TIGameState tigameState = ((ref_habSite2 != null) ? ref_habSite2.parentBody : null);
			TIHabSiteState ref_habSite3 = councilor.location.ref_habSite;
			if (tigameState == ((ref_habSite3 != null) ? ref_habSite3.parentBody : null))
			{
				return "_Pass";
			}
		}
		else if (councilor.ref_orbit != null && councilor.ref_orbit == possibleTarget.ref_orbit)
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}

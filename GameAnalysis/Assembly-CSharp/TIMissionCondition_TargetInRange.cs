using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000196 RID: 406
public class TIMissionCondition_TargetInRange : TIMissionCondition
{
	// Token: 0x06000605 RID: 1541 RVA: 0x0001B9A0 File Offset: 0x00019BA0
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (possibleTarget.isCouncilorState)
		{
			TIGameState tigameState = TIMissionPhaseState.CouncilorLastKnownLocation(councilor.faction, possibleTarget.ref_councilor);
			if ((!(councilor.ref_hab != null) || !(councilor.ref_hab == tigameState.ref_hab)) && (!(councilor.ref_fleet != null) || !(councilor.ref_fleet == tigameState.ref_fleet)) && (!(councilor.ref_habSite != null) || !(councilor.ref_habSite == tigameState.ref_habSite)))
			{
				if (!councilor.OnEarth || !(tigameState.ref_spaceAsset == null))
				{
					goto IL_0170;
				}
				TISpaceBodyState ref_spaceBody = tigameState.ref_spaceBody;
				if (ref_spaceBody == null || !ref_spaceBody.isEarth)
				{
					goto IL_0170;
				}
			}
			string text;
			if (councilor.ValidDestination(TIUtilities.ObjectToExactLocation(tigameState), out text))
			{
				return "_Pass";
			}
			return text;
		}
		else
		{
			if ((!(councilor.ref_hab != null) || !(councilor.ref_hab == possibleTarget.ref_hab)) && (!(councilor.ref_fleet != null) || !(councilor.ref_fleet == possibleTarget.ref_fleet)) && (!(councilor.ref_habSite != null) || !(councilor.ref_habSite == possibleTarget.ref_habSite)))
			{
				if (!councilor.OnEarth || !(possibleTarget.ref_spaceAsset == null))
				{
					goto IL_0170;
				}
				TISpaceBodyState ref_spaceBody2 = possibleTarget.ref_spaceBody;
				if (ref_spaceBody2 == null || !ref_spaceBody2.isEarth)
				{
					goto IL_0170;
				}
			}
			string text2;
			if (councilor.ValidDestination(TIUtilities.ObjectToExactLocation(possibleTarget), out text2))
			{
				return "_Pass";
			}
			return text2;
		}
		IL_0170:
		return base.GetType().Name;
	}
}

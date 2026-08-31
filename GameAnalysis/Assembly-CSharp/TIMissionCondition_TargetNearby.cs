using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000195 RID: 405
public class TIMissionCondition_TargetNearby : TIMissionCondition
{
	// Token: 0x06000603 RID: 1539 RVA: 0x0001B954 File Offset: 0x00019B54
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (!(TIUtilities.ObjectToSupraLocation(possibleTarget) == TIUtilities.ObjectToSupraLocation(councilor)))
		{
			return base.GetType().Name;
		}
		string text;
		if (councilor.ValidDestination(TIUtilities.ObjectToExactLocation(possibleTarget), out text))
		{
			return "_Pass";
		}
		return text;
	}
}

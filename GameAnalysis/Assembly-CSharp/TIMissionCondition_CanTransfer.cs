using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200019A RID: 410
public class TIMissionCondition_CanTransfer : TIMissionCondition
{
	// Token: 0x0600060D RID: 1549 RVA: 0x0001BC8C File Offset: 0x00019E8C
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		string text;
		if (!councilor.ValidDestination(TIUtilities.ObjectToExactLocation(possibleTarget), out text))
		{
			return text;
		}
		if (councilor.location != possibleTarget && (councilor.location.ref_habSite != null || councilor.location.ref_orbit != null))
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}

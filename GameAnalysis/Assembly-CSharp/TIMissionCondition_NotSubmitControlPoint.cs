using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001B3 RID: 435
public class TIMissionCondition_NotSubmitControlPoint : TIMissionCondition
{
	// Token: 0x06000643 RID: 1603 RVA: 0x0001C958 File Offset: 0x0001AB58
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (!possibleTarget.ref_controlPoint.faction.IsAlienProxy)
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}

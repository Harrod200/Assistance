using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001B4 RID: 436
public class TIMissionCondition_NotAppeaseControlPoint : TIMissionCondition
{
	// Token: 0x06000645 RID: 1605 RVA: 0x0001C985 File Offset: 0x0001AB85
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (!possibleTarget.ref_controlPoint.faction.isAlienAppeaser)
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}

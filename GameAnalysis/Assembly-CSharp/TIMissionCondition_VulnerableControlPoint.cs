using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200018D RID: 397
public class TIMissionCondition_VulnerableControlPoint : TIMissionCondition
{
	// Token: 0x060005F1 RID: 1521 RVA: 0x0001B6A0 File Offset: 0x000198A0
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (possibleTarget.ref_controlPoint.CanBeAttacked(councilor.faction))
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}

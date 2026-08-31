using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200018E RID: 398
public class TIMissionCondition_EnthrallableControlPoint : TIMissionCondition
{
	// Token: 0x060005F3 RID: 1523 RVA: 0x0001B6CE File Offset: 0x000198CE
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (possibleTarget.ref_controlPoint.CanBeEnthralled())
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}

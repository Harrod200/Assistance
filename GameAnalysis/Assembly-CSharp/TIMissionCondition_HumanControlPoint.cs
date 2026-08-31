using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200018F RID: 399
public class TIMissionCondition_HumanControlPoint : TIMissionCondition
{
	// Token: 0x060005F5 RID: 1525 RVA: 0x0001B6F6 File Offset: 0x000198F6
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (!possibleTarget.ref_controlPoint.faction.IsAlienFaction)
		{
			return "_Pass";
		}
		return "TIMissionCondition_HumanControlPoint";
	}
}

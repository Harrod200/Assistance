using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000192 RID: 402
public class TIMissionCondition_TerrorizeVulnerableControlPoint : TIMissionCondition
{
	// Token: 0x060005FB RID: 1531 RVA: 0x0001B772 File Offset: 0x00019972
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (possibleTarget.ref_nation.controlPoints.Any<TIControlPoint>((TIControlPoint x) => x.CanBeTerrorized()))
		{
			return "_Pass";
		}
		return "TIMissionCondition_TerrorizeVulnerableControlPoint";
	}
}

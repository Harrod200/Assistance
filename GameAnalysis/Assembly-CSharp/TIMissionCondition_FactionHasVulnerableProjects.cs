using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001AC RID: 428
public class TIMissionCondition_FactionHasVulnerableProjects : TIMissionCondition
{
	// Token: 0x06000635 RID: 1589 RVA: 0x0001C745 File Offset: 0x0001A945
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (possibleTarget.ref_faction.ProjectsVulnerableToSabotage(councilor.faction).Count > 0)
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}

using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001AB RID: 427
public class TIMissionCondition_FactionHasStealableProjects : TIMissionCondition
{
	// Token: 0x06000633 RID: 1587 RVA: 0x0001C711 File Offset: 0x0001A911
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (possibleTarget.ref_faction.StealableProjects(councilor.faction).Count > 0)
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}

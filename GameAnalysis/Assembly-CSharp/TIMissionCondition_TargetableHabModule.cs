using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001C4 RID: 452
public class TIMissionCondition_TargetableHabModule : TIMissionCondition
{
	// Token: 0x06000669 RID: 1641 RVA: 0x0001D534 File Offset: 0x0001B734
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (possibleTarget.isHabModuleState && !possibleTarget.ref_hab.IsAlien())
		{
			TIHabModuleState ref_habModule = possibleTarget.ref_habModule;
			if (((ref_habModule != null) ? ref_habModule.ref_faction : null) != councilor.faction && ref_habModule.okay && !ref_habModule.moduleTemplate.coreModule)
			{
				return "_Pass";
			}
		}
		return "TIMissionCondition_TargetableHabModule";
	}
}

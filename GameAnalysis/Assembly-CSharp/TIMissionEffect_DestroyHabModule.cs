using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001F3 RID: 499
public class TIMissionEffect_DestroyHabModule : TIMissionEffect
{
	// Token: 0x060006DB RID: 1755 RVA: 0x00021890 File Offset: 0x0001FA90
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		TIHabModuleState tihabModuleState = target as TIHabModuleState;
		string displayName = tihabModuleState.displayName;
		switch (outcome)
		{
		case TIMissionOutcome.CriticalFailure:
			mission.councilor.DetainCouncilor(tihabModuleState.ref_faction, 3f, 2f, true);
			break;
		case TIMissionOutcome.Success:
		case TIMissionOutcome.CriticalSuccess:
			tihabModuleState.hab.DestroyModule(mission.ref_faction, tihabModuleState, false, true, true, mission.missionTemplate.hate[(int)outcome], false, true);
			break;
		}
		return displayName;
	}
}

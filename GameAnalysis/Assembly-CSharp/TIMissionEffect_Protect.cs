using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001E2 RID: 482
public class TIMissionEffect_Protect : TIMissionEffect
{
	// Token: 0x060006B5 RID: 1717 RVA: 0x000201A1 File Offset: 0x0001E3A1
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		mission.councilor.ProtectTarget(target);
		return string.Empty;
	}
}

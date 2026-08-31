using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001ED RID: 493
public class TIMissionEffect_BuildFacility : TIMissionEffect
{
	// Token: 0x060006CB RID: 1739 RVA: 0x00020EE6 File Offset: 0x0001F0E6
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		target.ref_region.alienFacility.BuildFacility();
		return string.Empty;
	}
}

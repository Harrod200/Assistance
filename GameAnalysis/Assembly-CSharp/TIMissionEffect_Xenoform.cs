using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001EB RID: 491
public class TIMissionEffect_Xenoform : TIMissionEffect
{
	// Token: 0x060006C7 RID: 1735 RVA: 0x00020DC0 File Offset: 0x0001EFC0
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		if (base.MissionSuccess(outcome))
		{
			float num = 1f + TIUtilities.RandomFloatValue() * 9f + (float)((outcome == TIMissionOutcome.CriticalSuccess) ? 10 : 0);
			target.ref_region.xenoforming.ChangeXenoformingLevel(num);
			return num.ToString();
		}
		return string.Empty;
	}
}

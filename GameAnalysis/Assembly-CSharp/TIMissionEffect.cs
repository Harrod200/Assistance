using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001CC RID: 460
public abstract class TIMissionEffect
{
	// Token: 0x0600067A RID: 1658
	public abstract string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success);

	// Token: 0x0600067B RID: 1659 RVA: 0x0001D8CB File Offset: 0x0001BACB
	public virtual bool HasDelayedEffect()
	{
		return false;
	}

	// Token: 0x0600067C RID: 1660 RVA: 0x0001D8CE File Offset: 0x0001BACE
	public virtual void ApplyDelayedEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success, string dataName = "")
	{
	}

	// Token: 0x0600067D RID: 1661 RVA: 0x0001D8D0 File Offset: 0x0001BAD0
	protected bool MissionSuccess(TIMissionOutcome outcome)
	{
		return outcome == TIMissionOutcome.Success || outcome == TIMissionOutcome.CriticalSuccess;
	}

	// Token: 0x0600067E RID: 1662 RVA: 0x0001D8DC File Offset: 0x0001BADC
	protected bool MissionFailure(TIMissionOutcome outcome)
	{
		return outcome == TIMissionOutcome.Failure || outcome == TIMissionOutcome.CriticalFailure;
	}
}

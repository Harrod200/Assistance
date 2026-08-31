using System;

// Token: 0x020000D7 RID: 215
public abstract class TICouncilorCondition : TICondition
{
	// Token: 0x060003C1 RID: 961 RVA: 0x0001372A File Offset: 0x0001192A
	public override ConditionTargetType ConditionTarget()
	{
		return ConditionTargetType.councilor;
	}
}

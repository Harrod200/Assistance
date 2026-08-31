using System;

// Token: 0x02000036 RID: 54
public abstract class TINationCondition : TICondition
{
	// Token: 0x0600020D RID: 525 RVA: 0x00010052 File Offset: 0x0000E252
	public override ConditionTargetType ConditionTarget()
	{
		return ConditionTargetType.nation;
	}
}

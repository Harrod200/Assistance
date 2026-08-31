using System;

// Token: 0x020000F3 RID: 243
public abstract class TIHabCondition : TICondition
{
	// Token: 0x06000413 RID: 1043 RVA: 0x000144A2 File Offset: 0x000126A2
	public override ConditionTargetType ConditionTarget()
	{
		return ConditionTargetType.hab;
	}
}

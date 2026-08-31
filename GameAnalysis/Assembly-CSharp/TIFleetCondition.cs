using System;

// Token: 0x02000122 RID: 290
public abstract class TIFleetCondition : TICondition
{
	// Token: 0x0600047E RID: 1150 RVA: 0x000153A8 File Offset: 0x000135A8
	public override ConditionTargetType ConditionTarget()
	{
		return ConditionTargetType.fleet;
	}
}

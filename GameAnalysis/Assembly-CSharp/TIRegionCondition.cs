using System;

// Token: 0x0200006D RID: 109
public abstract class TIRegionCondition : TICondition
{
	// Token: 0x0600029C RID: 668 RVA: 0x000111F1 File Offset: 0x0000F3F1
	public override ConditionTargetType ConditionTarget()
	{
		return ConditionTargetType.region;
	}
}

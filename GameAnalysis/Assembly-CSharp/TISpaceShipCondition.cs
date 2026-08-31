using System;

// Token: 0x02000127 RID: 295
public abstract class TISpaceShipCondition : TICondition
{
	// Token: 0x06000488 RID: 1160 RVA: 0x00015483 File Offset: 0x00013683
	public override ConditionTargetType ConditionTarget()
	{
		return ConditionTargetType.ship;
	}
}

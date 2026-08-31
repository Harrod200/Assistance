using System;

// Token: 0x0200013D RID: 317
public abstract class TIArmyCondition : TICondition
{
	// Token: 0x060004C3 RID: 1219 RVA: 0x00015B0F File Offset: 0x00013D0F
	public override ConditionTargetType ConditionTarget()
	{
		return ConditionTargetType.army;
	}
}

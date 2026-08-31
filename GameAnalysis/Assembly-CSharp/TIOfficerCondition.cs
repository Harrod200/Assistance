using System;

// Token: 0x02000134 RID: 308
public abstract class TIOfficerCondition : TICondition
{
	// Token: 0x060004AC RID: 1196 RVA: 0x000158EB File Offset: 0x00013AEB
	public override ConditionTargetType ConditionTarget()
	{
		return ConditionTargetType.officer;
	}
}

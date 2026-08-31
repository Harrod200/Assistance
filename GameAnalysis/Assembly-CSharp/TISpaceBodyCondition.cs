using System;

// Token: 0x0200011F RID: 287
public abstract class TISpaceBodyCondition : TICondition
{
	// Token: 0x06000478 RID: 1144 RVA: 0x000152EE File Offset: 0x000134EE
	public override ConditionTargetType ConditionTarget()
	{
		return ConditionTargetType.spaceBody;
	}
}

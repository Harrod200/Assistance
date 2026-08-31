using System;

// Token: 0x020000BF RID: 191
public abstract class TIGlobalCondition : TICondition
{
	// Token: 0x0600038C RID: 908 RVA: 0x0001301E File Offset: 0x0001121E
	public override ConditionTargetType ConditionTarget()
	{
		return ConditionTargetType.global;
	}
}

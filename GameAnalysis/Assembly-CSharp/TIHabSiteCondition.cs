using System;

// Token: 0x0200011C RID: 284
public abstract class TIHabSiteCondition : TICondition
{
	// Token: 0x06000470 RID: 1136 RVA: 0x000151C7 File Offset: 0x000133C7
	public override ConditionTargetType ConditionTarget()
	{
		return ConditionTargetType.habSite;
	}
}

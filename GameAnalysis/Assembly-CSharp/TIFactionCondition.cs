using System;

// Token: 0x02000090 RID: 144
public abstract class TIFactionCondition : TICondition
{
	// Token: 0x060002F6 RID: 758 RVA: 0x00011C18 File Offset: 0x0000FE18
	public override ConditionTargetType ConditionTarget()
	{
		return ConditionTargetType.faction;
	}
}

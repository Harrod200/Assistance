using System;

// Token: 0x020002F0 RID: 752
public class TIOperationTargeting_OrbitMany : TIOperationTargeting_Orbit
{
	// Token: 0x06000B54 RID: 2900 RVA: 0x0003D93B File Offset: 0x0003BB3B
	public override OperationTargetingUIType UIType()
	{
		return OperationTargetingUIType.TwoStage;
	}
}

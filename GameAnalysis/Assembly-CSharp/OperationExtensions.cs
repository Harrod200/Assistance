using System;

// Token: 0x02000357 RID: 855
public static class OperationExtensions
{
	// Token: 0x06000EE4 RID: 3812 RVA: 0x000495CE File Offset: 0x000477CE
	public static TIOperationTargeting GetOperationTargeting(this IOperation operation)
	{
		return Activator.CreateInstance(operation.GetTargetingMethod()) as TIOperationTargeting;
	}
}

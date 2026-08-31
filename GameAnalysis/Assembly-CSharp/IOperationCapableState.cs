using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002F8 RID: 760
public interface IOperationCapableState
{
	// Token: 0x06000B8C RID: 2956
	void OnTimedOperationComplete(TimeEventStart e);

	// Token: 0x06000B8D RID: 2957
	List<IOperation> VisibleOperationList(TINaturalSpaceObjectState naturalSpaceObject = null);

	// Token: 0x06000B8E RID: 2958
	List<IOperation> AvailableOperationList(TINaturalSpaceObjectState naturalSpaceObject = null);

	// Token: 0x06000B8F RID: 2959
	List<OperationData> CurrentOperations();
}

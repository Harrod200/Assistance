using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000355 RID: 853
public class OperationData
{
	// Token: 0x17000185 RID: 389
	// (get) Token: 0x06000ED6 RID: 3798 RVA: 0x00049500 File Offset: 0x00047700
	public IOperation operation
	{
		get
		{
			return this._operation ?? TemplateManager.Find<TIOperationTemplate>(this.operationDataName, false);
		}
	}

	// Token: 0x17000186 RID: 390
	// (get) Token: 0x06000ED7 RID: 3799 RVA: 0x00049518 File Offset: 0x00047718
	// (set) Token: 0x06000ED8 RID: 3800 RVA: 0x00049520 File Offset: 0x00047720
	public string operationDataName { get; private set; }

	// Token: 0x17000187 RID: 391
	// (get) Token: 0x06000ED9 RID: 3801 RVA: 0x00049529 File Offset: 0x00047729
	// (set) Token: 0x06000EDA RID: 3802 RVA: 0x00049531 File Offset: 0x00047731
	public TIGameState target { get; private set; }

	// Token: 0x17000188 RID: 392
	// (get) Token: 0x06000EDB RID: 3803 RVA: 0x0004953A File Offset: 0x0004773A
	// (set) Token: 0x06000EDC RID: 3804 RVA: 0x00049542 File Offset: 0x00047742
	public TIDateTime startDate { get; private set; }

	// Token: 0x17000189 RID: 393
	// (get) Token: 0x06000EDD RID: 3805 RVA: 0x0004954B File Offset: 0x0004774B
	// (set) Token: 0x06000EDE RID: 3806 RVA: 0x00049553 File Offset: 0x00047753
	public TIDateTime completionDate { get; private set; }

	// Token: 0x06000EDF RID: 3807 RVA: 0x0004955C File Offset: 0x0004775C
	public OperationData(IOperation operation, TIGameState target, TIDateTime startDate, TIDateTime completionDate)
	{
		this._operation = operation;
		this.operationDataName = (operation as TIOperationTemplate).dataName;
		this.target = target;
		this.startDate = startDate;
		this.completionDate = completionDate;
	}

	// Token: 0x06000EE0 RID: 3808 RVA: 0x00049592 File Offset: 0x00047792
	public void OnOperationCancel(TIGameState actorState)
	{
		this.operation.OnOperationCancel(actorState, this.target, this.completionDate);
	}

	// Token: 0x06000EE1 RID: 3809 RVA: 0x000495AC File Offset: 0x000477AC
	public void Reschedule(TIDateTime newTime)
	{
		this.completionDate = newTime;
	}

	// Token: 0x06000EE2 RID: 3810 RVA: 0x000495B5 File Offset: 0x000477B5
	public void ChangeTarget(TIGameState newTarget)
	{
		this.target = newTarget;
	}

	// Token: 0x06000EE3 RID: 3811 RVA: 0x000495BE File Offset: 0x000477BE
	public void RepairOperation(string operationDataName)
	{
		this.operationDataName = operationDataName;
		this._operation = null;
	}

	// Token: 0x04000EC1 RID: 3777
	private IOperation _operation;
}

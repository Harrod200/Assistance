using System;
using FullSerializer;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000152 RID: 338
public class ShipConstructionQueueItem
{
	// Token: 0x1700009F RID: 159
	// (get) Token: 0x06000527 RID: 1319 RVA: 0x000167BA File Offset: 0x000149BA
	// (set) Token: 0x06000528 RID: 1320 RVA: 0x000167DC File Offset: 0x000149DC
	[fsIgnore]
	public TISpaceShipTemplate shipDesign
	{
		get
		{
			if (this._shipDesign == null)
			{
				this._shipDesign = TemplateManager.Find<TISpaceShipTemplate>(this.shipDesignTemplateName, false);
			}
			return this._shipDesign;
		}
		private set
		{
		}
	}

	// Token: 0x170000A0 RID: 160
	// (get) Token: 0x06000529 RID: 1321 RVA: 0x000167DE File Offset: 0x000149DE
	public TISpaceShipTemplate refit_originalShipDesign
	{
		get
		{
			if (this._originalShipDesign == null)
			{
				this._originalShipDesign = TemplateManager.Find<TISpaceShipTemplate>(this.refit_originalShipDesignTemplateName, false);
			}
			return this._originalShipDesign;
		}
	}

	// Token: 0x170000A1 RID: 161
	// (get) Token: 0x0600052A RID: 1322 RVA: 0x00016800 File Offset: 0x00014A00
	public float durationInDays
	{
		get
		{
			return this.resourcesCost.completionTime_days;
		}
	}

	// Token: 0x170000A2 RID: 162
	// (get) Token: 0x0600052B RID: 1323 RVA: 0x0001680D File Offset: 0x00014A0D
	public float progressFraction
	{
		get
		{
			return Mathf.Clamp((this.durationInDays > 0f) ? ((this.durationInDays - this.daysToCompletion) / this.durationInDays) : 0f, 0f, 1f);
		}
	}

	// Token: 0x0600052C RID: 1324 RVA: 0x00016848 File Offset: 0x00014A48
	public ShipConstructionQueueItem(TISpaceShipTemplate shipDesign, TIHabModuleState shipyard, TIDateTime startDate, TIResourcesCost resourcesCost, FactionGoal_Fleet goal, bool isRefit = false, TISpaceShipTemplate originalShipDesign = null, TISpaceShipState originalSpaceShipState = null, TIResourcesCost refundCost = null)
	{
		this.shipDesign = shipDesign;
		this.shipDesignTemplateName = shipDesign.dataName;
		this.shipyard = shipyard;
		this.startDate = startDate;
		this.daysToCompletion = resourcesCost.completionTime_days;
		this.resourcesCost = new TIResourcesCost(resourcesCost);
		this.AIFactionGoal = goal;
		this.isRefit = isRefit;
		this.refit_originalShipDesignTemplateName = ((originalShipDesign != null) ? originalShipDesign.dataName : null);
		this.originalSpaceShipState = originalSpaceShipState;
		this.resourcesRefund = refundCost;
		this.costPaid = false;
	}

	// Token: 0x0600052D RID: 1325 RVA: 0x000168D1 File Offset: 0x00014AD1
	public void UpdateResourcesCost(TIResourcesCost cost)
	{
		this.resourcesCost = cost;
		this.daysToCompletion = this.resourcesCost.completionTime_days;
	}

	// Token: 0x0400023F RID: 575
	public string shipDesignTemplateName;

	// Token: 0x04000240 RID: 576
	public TIDateTime startDate;

	// Token: 0x04000241 RID: 577
	public TIHabModuleState shipyard;

	// Token: 0x04000242 RID: 578
	public float daysToCompletion;

	// Token: 0x04000243 RID: 579
	public TIResourcesCost resourcesCost;

	// Token: 0x04000244 RID: 580
	public TIResourcesCost resourcesRefund;

	// Token: 0x04000245 RID: 581
	public bool costPaid;

	// Token: 0x04000246 RID: 582
	private TISpaceShipTemplate _shipDesign;

	// Token: 0x04000247 RID: 583
	public FactionGoal_Fleet AIFactionGoal;

	// Token: 0x04000248 RID: 584
	public bool isRefit;

	// Token: 0x04000249 RID: 585
	public string refit_originalShipDesignTemplateName;

	// Token: 0x0400024A RID: 586
	private TISpaceShipTemplate _originalShipDesign;

	// Token: 0x0400024B RID: 587
	public TISpaceShipState originalSpaceShipState;

	// Token: 0x0400024C RID: 588
	[Obsolete]
	public TISpaceShipTemplate originalShipDesign;
}

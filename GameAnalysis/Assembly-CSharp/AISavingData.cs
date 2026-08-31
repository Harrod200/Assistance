using System;
using System.Collections.Generic;
using System.Linq;
using FullSerializer;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000158 RID: 344
public class AISavingData
{
	// Token: 0x170000A4 RID: 164
	// (get) Token: 0x06000543 RID: 1347 RVA: 0x000170EC File Offset: 0x000152EC
	public int importance
	{
		get
		{
			return this.relatedGoal.importance;
		}
	}

	// Token: 0x170000A5 RID: 165
	// (get) Token: 0x06000544 RID: 1348 RVA: 0x000170F9 File Offset: 0x000152F9
	public TIDataTemplate desiredPurchase
	{
		get
		{
			if (this._desiredPurchase == null)
			{
				this._desiredPurchase = TemplateManager.Find<TIDataTemplate>(this.desiredPurchaseDataName, true);
			}
			return this._desiredPurchase;
		}
	}

	// Token: 0x06000545 RID: 1349 RVA: 0x0001711B File Offset: 0x0001531B
	public void ClearPurchaseData()
	{
		this._desiredPurchase = null;
		this.desiredPurchaseDataName = string.Empty;
		this.bankingPercentage = 0f;
		this.active = false;
		this.relatedGoal = null;
		this.location = null;
	}

	// Token: 0x06000546 RID: 1350 RVA: 0x00017150 File Offset: 0x00015350
	public AISavingData(TIFactionState faction, TIDataTemplate desiredPurchase, TIGameState location, TIFactionGoalState relatedGoal, float bankingPercentage)
	{
		this.faction = faction;
		this._desiredPurchase = desiredPurchase;
		this.desiredPurchaseDataName = ((desiredPurchase != null) ? desiredPurchase.dataName : null) ?? string.Empty;
		this.location = location;
		this.relatedGoal = relatedGoal;
		this.bankingPercentage = bankingPercentage;
		this.bankedResources = new Dictionary<FactionResource, float>();
		this.yesterdaysResources = new Dictionary<FactionResource, float>();
		this.active = true;
		foreach (ResourceValue resourceValue in this.GetResourcesToSave().resourceCosts)
		{
			if (resourceValue.value > 0f)
			{
				this.bankedResources.Add(resourceValue.resource, 0f);
				this.yesterdaysResources.Add(resourceValue.resource, faction.GetCurrentResourceAmount(resourceValue.resource));
				this.bankedResources[resourceValue.resource] = Mathf.Min(resourceValue.value, this.yesterdaysResources[resourceValue.resource] * bankingPercentage);
			}
		}
	}

	// Token: 0x06000547 RID: 1351 RVA: 0x00017278 File Offset: 0x00015478
	public static float GetBankingPercentage(TIFactionGoalState goal)
	{
		return 0.5f + (float)goal.importance / 100f;
	}

	// Token: 0x06000548 RID: 1352 RVA: 0x00017290 File Offset: 0x00015490
	public void DailySavingUpdate()
	{
		TIResourcesCost resourcesToSave = this.GetResourcesToSave();
		this.bankingPercentage = AISavingData.GetBankingPercentage(this.relatedGoal);
		foreach (ResourceValue resourceValue in resourcesToSave.resourceCosts)
		{
			if (resourceValue.value > 0f)
			{
				if (!this.bankedResources.ContainsKey(resourceValue.resource))
				{
					this.bankedResources.Add(resourceValue.resource, 0f);
					this.yesterdaysResources.Add(resourceValue.resource, 0f);
				}
				float num = Mathf.Max(this.faction.GetCurrentResourceAmount(resourceValue.resource) - this.yesterdaysResources[resourceValue.resource], 0f);
				this.bankedResources[resourceValue.resource] = Mathf.Min(resourceValue.value, this.bankedResources[resourceValue.resource] + num * this.bankingPercentage);
				this.yesterdaysResources[resourceValue.resource] = this.faction.GetCurrentResourceAmount(resourceValue.resource);
			}
		}
	}

	// Token: 0x06000549 RID: 1353 RVA: 0x000173D0 File Offset: 0x000155D0
	public float GetBankedQuantity(FactionResource resource)
	{
		if (!this.active)
		{
			return 0f;
		}
		float num;
		if (this.bankedResources.TryGetValue(resource, out num))
		{
			float currentResourceAmount = this.faction.GetCurrentResourceAmount(resource);
			if (currentResourceAmount < num)
			{
				num = (this.bankedResources[resource] = currentResourceAmount);
			}
			return num;
		}
		return 0f;
	}

	// Token: 0x0600054A RID: 1354 RVA: 0x00017424 File Offset: 0x00015624
	public TIResourcesCost GetResourcesToSave()
	{
		TIResourcesCost tiresourcesCost = new TIResourcesCost();
		if (this.desiredPurchase != null)
		{
			TISpaceShipTemplate tispaceShipTemplate = this.desiredPurchase as TISpaceShipTemplate;
			if (tispaceShipTemplate != null)
			{
				tiresourcesCost = tispaceShipTemplate.spaceResourceConstructionCost(false, this.location.ref_habModule, true, false, false);
			}
			else
			{
				TIHabModuleTemplate tihabModuleTemplate = this.desiredPurchase as TIHabModuleTemplate;
				if (tihabModuleTemplate != null)
				{
					bool flag = this.location.ref_hab.ModuleUpgradePrereqModuleAlreadyOnHab(tihabModuleTemplate);
					tiresourcesCost = tihabModuleTemplate.CostFromSpace(this.faction, this.location, flag, false, 0, false);
				}
			}
		}
		return tiresourcesCost;
	}

	// Token: 0x170000A6 RID: 166
	// (get) Token: 0x0600054B RID: 1355 RVA: 0x0001749F File Offset: 0x0001569F
	public bool CanSaveFor
	{
		get
		{
			return this.GetResourcesToSave().resourceCosts.All<ResourceValue>((ResourceValue x) => this.faction.GetDailyIncome(x.resource, false, false) > 0f);
		}
	}

	// Token: 0x0600054C RID: 1356 RVA: 0x000174C0 File Offset: 0x000156C0
	public void LogSavingData()
	{
		if (this.desiredPurchase != null)
		{
			string text = TITimeState.Now().ToCustomDateString();
			TISpaceShipTemplate tispaceShipTemplate = this.desiredPurchase as TISpaceShipTemplate;
			if (tispaceShipTemplate != null)
			{
				text = string.Concat(new string[]
				{
					": Saving Log: ",
					this.faction.displayName,
					" ",
					tispaceShipTemplate.fullClassName,
					" ",
					this.location.ref_hab.displayName,
					" ",
					this.relatedGoal.description
				});
			}
			else
			{
				text = string.Concat(new string[]
				{
					": Saving Log: ",
					this.faction.displayName,
					" ",
					this.desiredPurchase.displayName,
					" ",
					this.location.ref_hab.displayName,
					" ",
					this.relatedGoal.description
				});
			}
			string[] array = new string[5];
			array[0] = text;
			array[1] = "\n     Cost: ";
			array[2] = this.GetResourcesToSave().resourceCosts.ToDictionary<ResourceValue, FactionResource, float>((ResourceValue x) => x.resource, (ResourceValue x) => x.value).ToDetailedString<FactionResource, float>();
			array[3] = "\n   Banked: ";
			array[4] = this.bankedResources.ToDetailedString<FactionResource, float>();
			text = string.Concat(array);
			TIFactionState.LogAI(text, false);
		}
	}

	// Token: 0x04000268 RID: 616
	public TIFactionState faction;

	// Token: 0x04000269 RID: 617
	public string desiredPurchaseDataName;

	// Token: 0x0400026A RID: 618
	public TIGameState location;

	// Token: 0x0400026B RID: 619
	public TIFactionGoalState relatedGoal;

	// Token: 0x0400026C RID: 620
	public float bankingPercentage;

	// Token: 0x0400026D RID: 621
	public Dictionary<FactionResource, float> bankedResources;

	// Token: 0x0400026E RID: 622
	public Dictionary<FactionResource, float> yesterdaysResources;

	// Token: 0x0400026F RID: 623
	public bool active;

	// Token: 0x04000270 RID: 624
	[fsIgnore]
	private TIDataTemplate _desiredPurchase;
}

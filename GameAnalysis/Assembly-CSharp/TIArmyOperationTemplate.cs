using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002FB RID: 763
public abstract class TIArmyOperationTemplate : TIOperationTemplate
{
	// Token: 0x06000BC2 RID: 3010 RVA: 0x0003F080 File Offset: 0x0003D280
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return true;
	}

	// Token: 0x06000BC3 RID: 3011 RVA: 0x0003F083 File Offset: 0x0003D283
	public override bool IsBlockingOperation()
	{
		return true;
	}

	// Token: 0x06000BC4 RID: 3012 RVA: 0x0003F086 File Offset: 0x0003D286
	public virtual bool IsCombatOperation()
	{
		return true;
	}

	// Token: 0x06000BC5 RID: 3013 RVA: 0x0003F089 File Offset: 0x0003D289
	public virtual string GetSuccessHeadline(TIArmyState army, TIGameState target)
	{
		return string.Empty;
	}

	// Token: 0x06000BC6 RID: 3014 RVA: 0x0003F090 File Offset: 0x0003D290
	public virtual string GetFailureHeadline(TIArmyState army, TIGameState target)
	{
		return string.Empty;
	}

	// Token: 0x06000BC7 RID: 3015 RVA: 0x0003F097 File Offset: 0x0003D297
	public virtual string GetSuccessSummary(TIArmyState army, TIGameState target)
	{
		return string.Empty;
	}

	// Token: 0x06000BC8 RID: 3016 RVA: 0x0003F09E File Offset: 0x0003D29E
	public virtual string GetSuccessDetail(TIArmyState army, TIGameState target)
	{
		return string.Empty;
	}

	// Token: 0x06000BC9 RID: 3017 RVA: 0x0003F0A5 File Offset: 0x0003D2A5
	public virtual string GetFailureSummary(TIArmyState army, TIGameState target)
	{
		return string.Empty;
	}

	// Token: 0x06000BCA RID: 3018 RVA: 0x0003F0AC File Offset: 0x0003D2AC
	public virtual string GetFailureDetail(TIArmyState army, TIGameState target)
	{
		return string.Empty;
	}

	// Token: 0x1700017D RID: 381
	// (get) Token: 0x06000BCB RID: 3019 RVA: 0x0003F0B3 File Offset: 0x0003D2B3
	public virtual bool isConvenienceOperation
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000BCC RID: 3020 RVA: 0x0003F0B8 File Offset: 0x0003D2B8
	public override bool OperationConfirmed(TIGameState actor, TIGameState target, TIDateTime opCompleteDate)
	{
		if (actor.isArmyState)
		{
			TIArmyState ref_army = actor.ref_army;
			if (ref_army.currentOperations.Count > 0)
			{
				ref_army.ClearOperations();
			}
			if (this is DeployArmyOperation)
			{
				if (target == ref_army.currentRegion)
				{
					ref_army.SetArmyDataDirty();
					return true;
				}
				bool flag = ref_army.homeNation.wars.Contains(target.ref_region.ref_nation);
				if (!ref_army.currentRegion.AdjacentRegions(flag).Contains(target.ref_region))
				{
					ref_army.SetSeaTransitStages(TITimeState.Now(), opCompleteDate, target.ref_region);
				}
			}
			ref_army.currentOperations.Add(new OperationData(this, target, TITimeState.Now(), opCompleteDate));
			string armyOperationCompleteEventName = ref_army.armyOperationCompleteEventName;
			TITimeEvent.CreateNewTimeEvent(opCompleteDate, ref_army, target, this, armyOperationCompleteEventName, true, false, TITimeQueueRepeatType.None, 1, true, false);
			GameControl.eventManager.TriggerEvent(new StartArmyOperation(actor, this, target), null, (from x in new object[] { actor, target }.Distinct<object>()
				where x != null
				select x).ToArray<object>());
			ref_army.SetArmyDataDirty();
			return true;
		}
		return false;
	}
}
